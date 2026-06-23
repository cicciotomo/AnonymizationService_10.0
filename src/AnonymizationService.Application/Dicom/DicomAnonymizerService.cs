using FellowOakDicom;
using S_RACE.Anonymizer.CustomProcessor;
using S_RACE.Dicom.Anonymizer.Core;
using S_RACE.Dicom.Anonymizer.Core.Processors;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.DicomAnonymizerService
{
    internal class DicomAnonymizerService : IDicomAnonymizerService, ITransientDependency
    {
        public async Task AnonymizeDicomImagesInFolderAsync(string inputFolder, string outputFolder, AnonymizationParameters anonymizationParameters)
        {
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
                await AnonymizeOneFileAsync(file, Path.Join(outputFolder, Path.GetRelativePath(inputFolder, file)), engine, anonymizationDictionary);
            }
        }

        internal static async Task AnonymizeOneFileAsync(string inputFile, string outputFile, AnonymizerEngine engine, Dictionary<string, string> anonymizationParameters)
        {
            DicomFile dicomFile = await DicomFile.OpenAsync(inputFile).ConfigureAwait(false);

            engine.AnonymizeDataset(dicomFile.Dataset, anonymizationParameters);

            if (!string.IsNullOrEmpty(Path.GetDirectoryName(outputFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
            }

            await dicomFile.SaveAsync(outputFile);
        }
    }
}
