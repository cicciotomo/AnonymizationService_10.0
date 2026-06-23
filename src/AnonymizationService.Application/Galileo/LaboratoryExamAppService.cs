using AnonymizationService.ClinicalDocuments;
using AnonymizationService.ContainerImages;
using AnonymizationService.Enums;
using AnonymizationService.LaboratoryExams;
using AnonymizationService.Localization;
using AnonymizationService.Permissions;
using AnonymizationService.PodDefinitions;
using AnonymizationService.Services.Galileo;
using AnonymizationService.Services.LaboratoryExamService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using SimpleSOAPClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
namespace AnonymizationService.LaboratoryExams
{


    public class LaboratoryExamAppService : ApplicationService
    {
        private readonly ILaboratoryExamsService _laboratoryExamService;

        public LaboratoryExamAppService(ILaboratoryExamsService laboratoryExamService)
        {
            _laboratoryExamService = laboratoryExamService;
        }

        

        //[AllowAnonymous]
        //public async Task<List<LaboratoryExam>> GetPatientLaboratoryExamResultsAsync(string masterPatientIndex, DateTime startDate, DateTime? endDate)
        //{
        //    //var examResults = await _laboratoryExamService.GetPatientLaboratoryExamResultsAsync(masterPatientIndex, startDate);
        //    var examResults = await _laboratoryExamService.GetPatientLaboratoryExamResultsFromDateToDateAsync(masterPatientIndex, startDate, endDate);

        //    var laboratoryExams = examResults
        //        .GroupBy(e => (e.ID_VISIT_HOSP))
        //        .Select(e => new LaboratoryExam(
        //            id: e.Key,
        //            examStartDate: e.MinBy(exam => exam.DAY).DAY,
        //            examEndDate: e.MaxBy(exam => exam.DAY).DAY,
        //            cloudPatientId: new Guid(),
        //            valueResults: e.Select(exam => new LaboratoryExamResult(
        //                exam.ID_LAB_RESULT.ToString(),
        //                exam.RESULT_VALUE,
        //                exam.METHOD_ID,
        //                exam.REFERENCE_LOW,
        //                exam.REFERENCE_HIGH,
        //                exam.REFERENCE_RANGE,
        //                exam.LAB_COMMENT,
        //                exam.UNIT,
        //                exam.METHOD_TEXT,
        //                exam.DAY,
        //                exam.ID_EXAM_FILLER,
        //                exam.ID_ORDER_FILLER
        //                )).ToList()
        //            )).ToList();

        //    return laboratoryExams;
        //}

        //[AllowAnonymous]
        //public async Task<List<LaboratoryExam>> GetPatientLaboratoryExamResultsByXmlAsync(string xml)
        //{
        //    List<LabResult> examResults = new List<LabResult>();

        //    XmlSerializer serializer = new XmlSerializer(typeof(GetLabResultResponse));

        //    using (StringReader reader = new StringReader(xml))
        //    {
        //        GetLabResultResponse res_1 = (GetLabResultResponse)serializer.Deserialize(reader);
        //        examResults = res_1.LabResult.ToList();
        //    }

        //    var laboratoryExams = examResults
        //        .GroupBy(e => (e.ID_VISIT_HOSP))
        //        .Select(e => new LaboratoryExam(
        //            id: e.Key,
        //            examStartDate: e.MinBy(exam => exam.DAY).DAY,
        //            examEndDate: e.MaxBy(exam => exam.DAY).DAY,
        //            cloudPatientId: new Guid(),
        //            valueResults: e.Select(exam => new LaboratoryExamResult(
        //                exam.ID_LAB_RESULT.ToString(),
        //                exam.RESULT_VALUE,
        //                exam.METHOD_ID,
        //                exam.REFERENCE_LOW,
        //                exam.REFERENCE_HIGH,
        //                exam.REFERENCE_RANGE,
        //                exam.LAB_COMMENT,
        //                exam.UNIT,
        //                exam.METHOD_TEXT,
        //                exam.DAY,
        //                exam.ID_EXAM_FILLER,
        //                exam.ID_ORDER_FILLER
        //                )).ToList()
        //            )).ToList();

        //    return laboratoryExams;
        //}
    }
}
