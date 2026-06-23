using AnonymizationService.Permissions;
using AnonymizationService.Services.DicomAnonymizerService;
using AnonymizationService.StateMachines.Dicom;
using FellowOakDicom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using S_RACE.Anonymizer.CustomProcessor;
using S_RACE.Dicom.Anonymizer.Core;
using S_RACE.Dicom.Anonymizer.Core.Processors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;

namespace AnonymizationService.Services.DicomAnonymizerService
{
    [Authorize(AnonymizationServicePermissions.PatientManagementPermission)]
    public class DicomAppService : AnonymizationServiceAppService
    {

        private readonly ILogger<DicomAppService> _logger;

        public DicomAppService(ILogger<DicomAppService> logger)
        {
            _logger = logger;
        }

        

        [AllowAnonymous]
        public async Task AnonymizeDicomImagesInFolderAsync(string inputFolder, string outputFolder)
        {
            var anonymizationParameters = new AnonymizationParameters()
            {
                CloudPatientId = new Guid()
            };

            var customAnonymizerProcessorFactory = new CustomProcessorFactory();
            customAnonymizerProcessorFactory.RegisterProcessors(typeof(SubstituteWithParameterProcessor));
            var engine = new AnonymizerEngine("dicom-anonymizer-configuration.json", processorFactory: customAnonymizerProcessorFactory);
            var anonymizationDictionary = new Dictionary<string, string>()
            {
                { "CloudPatientId", anonymizationParameters.CloudPatientId.ToString() },
                { "DefaultDate", "19700101"},
                { "DefaultTime", "080000.000000"}
            };

            foreach (string file in Directory.EnumerateFiles(inputFolder, "*.dcm", SearchOption.AllDirectories))
            {

                _logger.LogDebug($"DicomAnonymizerService - inputFolder: {inputFolder}");
                _logger.LogDebug($"DicomAnonymizerService - outputFolder: {outputFolder}");
                _logger.LogDebug($"DicomAnonymizerService - inputFile: {file}");
                _logger.LogDebug($"DicomAnonymizerService - outputFile: {Path.Join(outputFolder, Path.GetRelativePath(inputFolder, file))}");
                await AnonymizeOneFileAsync(file, Path.Join(outputFolder, Path.GetRelativePath(inputFolder, file)), engine, anonymizationDictionary);

            }
        }

        internal async Task AnonymizeOneFileAsync(string inputFile, string outputFile, AnonymizerEngine engine, Dictionary<string, string> anonymizationParameters)
        {
            DicomFile dicomFile = await DicomFile.OpenAsync(inputFile).ConfigureAwait(false);

            List<DicomItem> itemsWithPrivateTag = new List<DicomItem>();
            List<DicomUniqueIdentifier> itemToUpdate = new List<DicomUniqueIdentifier>();

            foreach (var item in dicomFile.Dataset)
            {
                if (item.Tag.IsPrivate)
                {
                    itemsWithPrivateTag.Add(item);
                }

                if (item.Tag.DictionaryEntry.Keyword == "FrameOfReferenceUID" && ((DicomElement)item).Get<string>() is null)
                {
                    DicomUID newUID = DicomUIDGenerator.GenerateDerivedFromUUID();
                    var newItem = new DicomUniqueIdentifier(item.Tag, newUID);
                    itemToUpdate.Add(newItem);
                }
            }

            foreach (var item in itemsWithPrivateTag)
            {
                dicomFile.Dataset.Remove(item.Tag);
            }

            foreach (var item in itemToUpdate)
            {
                dicomFile.Dataset.AddOrUpdate(item);
            }

            //engine.AnonymizeDataset(dicomFile.Dataset, anonymizationParameters);

            engine.AnonymizeRecursively(dicomFile.Dataset, anonymizationParameters);



            if (!string.IsNullOrEmpty(Path.GetDirectoryName(outputFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
            }

            await dicomFile.SaveAsync(outputFile);
        }
    }


    public static class DicomRecursiveAnonymizer
    {
        public static void AnonymizeRecursively(
            this AnonymizerEngine engine,
            DicomDataset dataset,
            Dictionary<string, string> parameters)
        {
            if (dataset == null) { return; }


            // Anonimizza il dataset corrente
            engine.AnonymizeDataset(dataset, parameters);

            // Scansiona tutti gli item del dataset
            foreach (var item in dataset)
            {
                if (item is DicomSequence sequence)
                {
                    // Per ogni item della sequence → ricorsione
                    foreach (var seqItem in sequence.Items)
                    {
                        engine.AnonymizeRecursively(seqItem, parameters);
                    }
                }
            }
        }
    }
}
