
using AnonymizationService.ClinicalDocuments;
using AnonymizationService.ClinicalDocumentTypes;
using AnonymizationService.ContainerImages;
using AnonymizationService.Containers;
using AnonymizationService.Converters;
using AnonymizationService.DefaultImportParameters;
using AnonymizationService.ExternalServiceRequestLimits;
using AnonymizationService.HospitalPatients;
using AnonymizationService.IntensiveCare;
using AnonymizationService.IntensiveCareData;
using AnonymizationService.Jobs.UpsertFhirLaboratoryData;
using AnonymizationService.LaboratoryExams;
using AnonymizationService.Patients;
using AnonymizationService.PatientUploadRequests;
using AnonymizationService.PodDefinitions;
using AnonymizationService.Redcap;
using AnonymizationService.ScheduledIncrementalUploadParameters;
using AnonymizationService.ScheduledUploadParameters;
using AnonymizationService.Services.Dicom;
using AnonymizationService.Services.PatientLoadManager;
using AnonymizationService.Services.PlatformManagementConsole;
using AnonymizationService.StateMachines.Clinical;
using AnonymizationService.StateMachines.Dicom;
using AnonymizationService.StateMachines.LoadPatientData;
using AnonymizationService.Studies;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnonymizationService;

public class AnonymizationServiceApplicationAutoMapperProfile : Profile
{
    public AnonymizationServiceApplicationAutoMapperProfile()
    {
        CreateMap<string, DicomParametersJson>().ConvertUsing<JsonToDtoConverter>();
        CreateMap<DicomParametersJson, string>().ConvertUsing<DtoToJsonConverter>();

        CreateMap<ClinicalDocumentTypeFilter[], string>().ConvertUsing<ClinicalDocumentTypeFilterArrayToStringTypeConverter>();
        CreateMap<string, ClinicalDocumentTypeFilter[]>().ConvertUsing<StringToClinicalDocumentTypeFilterArrayTypeConverter>();

        CreateMap<PatientDto, HospitalPatient>()
            .ReverseMap();

        CreateMap<BatchUploadRequestDto, PatientBatchUploadRequest>()
            .ReverseMap();

        CreateMap<UploadRequestDto, PatientUploadRequest>()
            .ForMember(u => u.StudyId, c => c.MapFrom(src => string.Join(',', src.StudyIds)))
            .ForMember(dest => dest.ClinicalStateMachineDocumentType, opt => opt.MapFrom(src => src.ClinicalStateMachineDocumentType))
            .ForMember(u => u.DicomPacsSource, c => c.MapFrom(src => src.PacsSource))
            .AfterMap((src, dest) =>
            {
                if (!string.IsNullOrEmpty(dest.DicomStateMachineParameterJson))
                {
                    var dto = JsonSerializer.Deserialize<DicomParametersJsonDto>(dest.DicomStateMachineParameterJson);

                    src.DicomStateMachineMoveToPacsRicerca = dto?.MoveToPacsRicerca ?? default;
                    src.DicomStateMachineStudyDescription = dto?.StudyDescription;
                }
            })
            .ReverseMap()
            .ForMember(u => u.StudyIds, c => c.MapFrom(src => src.StudyId != null ? src.StudyId.Split(',', StringSplitOptions.None).ToList() : null))
            .ForMember(dest => dest.ClinicalStateMachineDocumentType, opt => opt.MapFrom(src => src.ClinicalStateMachineDocumentType))
            .ForMember(u => u.PacsSource, c => c.MapFrom(src => src.DicomPacsSource))
            .AfterMap((src, dest) =>
            {
                if (!string.IsNullOrEmpty(src.DicomStateMachineParameterJson))
                {
                    var dto = JsonSerializer.Deserialize<DicomParametersJsonDto>(src.DicomStateMachineParameterJson);

                    dest.DicomStateMachineMoveToPacsRicerca = dto?.MoveToPacsRicerca ?? default;
                    dest.DicomStateMachineStudyDescription = dto?.StudyDescription;
                }
            })
            ;

        CreateMap<ClinicalDocumentTypeFilterDto, ClinicalDocumentTypeFilter>()
            .ReverseMap();

        CreateMap<BatchPatientDto, PatientUploadInput>()
            .ForMember(p => p.DicomStateMachineModalities, c => c.MapFrom(src => string.IsNullOrEmpty(src.DicomStateMachineModalities) ? null : src.DicomStateMachineModalities.Split(",", StringSplitOptions.None)))
            .ForMember(dest => dest.ClinicalStateMachineDocumentType, opt => opt.MapFrom(src => src.ClinicalStateMachineDocumentType))
            .ForMember(dest => dest.DicomParametersJson, opt => opt.MapFrom(src => src.DicomParametersJson))
            .ReverseMap()
            .ForMember(p => p.DicomStateMachineModalities, c => c.MapFrom(src => string.Join(",", src.DicomStateMachineModalities)))
            .ForMember(dest => dest.ClinicalStateMachineDocumentType, opt => opt.MapFrom(src => src.ClinicalStateMachineDocumentType))
            .ForMember(dest => dest.DicomParametersJson, opt => opt.MapFrom(src => src.DicomParametersJson))
            ;

        CreateMap<ContainerImage, ContainerImageDto>()
            .ReverseMap();

        CreateMap<UpsertContainerImageDto, ContainerImage>();

        CreateMap<ContainerImageVariable, ContainerImageVariableDto>()
            .ReverseMap();

        CreateMap<ContainerImageVolume, ContainerImageVolumeDto>()
            .ReverseMap();

        CreateMap<DefaultImportParameterDto, DefaultImportParameter>()
            .ReverseMap();

        CreateMap<ContainerInvocationHeader, ContainerInvocationHeaderDto>()
            .ReverseMap();

        CreateMap<ExternalServiceRequestLimit, ExternalServiceRequestLimitDto>()
           .ReverseMap();

        CreateMap<CreatePatientDto, PatientUploadInput>()
            .ForMember(p => p.StudyId, c => c.MapFrom(src => string.Join(',', src.StudyIds)))
            .ForMember(p => p.ClinicalStateMachineShouldStart, c => c.MapFrom(src => src.ClinicalParameters != null && src.ClinicalParameters.ShouldStart))
            .ForMember(p => p.ClinicalStateMachineStartTime, c => c.MapFrom(src => src.ClinicalParameters != null ? src.ClinicalParameters.StartTime : null))
            .ForMember(p => p.ClinicalStateMachineDocumentType, c => c.MapFrom(src => src.ClinicalParameters != null ? src.ClinicalParameters.DocumentType : null))
            .ForMember(p => p.DicomStateMachineShouldStart, c => c.MapFrom(src => src.DicomParameters != null && src.DicomParameters.ShouldStart))
            .ForMember(p => p.DicomStateMachineStartTime, c => c.MapFrom(src => src.DicomParameters != null ? src.DicomParameters.StartTime : null))
            .ForMember(p => p.DicomPacsSource, c => c.MapFrom(src => src.DicomParameters != null ? src.DicomParameters.DicomPacsSource : null))
            .ForMember(p => p.DicomStateMachineEndTime, c => c.MapFrom(src => src.DicomParameters != null ? src.DicomParameters.EndTime : null))
            .ForMember(p => p.DicomStateMachineModalities, c => c.MapFrom(src => src.DicomParameters != null ? src.DicomParameters.DicomModalities : null))
            .ForMember(p => p.DicomParametersJson, c => c.MapFrom(src => src.DicomParameters != null ? src.DicomParameters.DicomParametersJson : null))
            .ForMember(p => p.LaboratoryStateMachineShouldStart, c => c.MapFrom(src => src.LaboratoryParameters != null && src.LaboratoryParameters.ShouldStart))
            .ForMember(p => p.LaboratoryStateMachineStartTime, c => c.MapFrom(src => src.LaboratoryParameters != null ? src.LaboratoryParameters.StartTime : null))
            .ForMember(p => p.DbUriStateMachineShouldStart, c => c.MapFrom(src => src.DbUriParameters != null && src.DbUriParameters.ShouldStart))
            .ForMember(p => p.DbUriStateMachineStartTime, c => c.MapFrom(src => src.DbUriParameters != null ? src.DbUriParameters.StartTime : null))
            .ForMember(p => p.PathoxStateMachineShouldStart, c => c.MapFrom(src => src.PathoxParameters != null && src.PathoxParameters.ShouldStart))
            .ForMember(p => p.PathoxStateMachineStartTime, c => c.MapFrom(src => src.PathoxParameters != null ? src.PathoxParameters.StartTime : null))
            .ForMember(p => p.IntensiveCareStateMachineShouldStart, c => c.MapFrom(src => src.IntensiveCareParameters != null && src.IntensiveCareParameters.ShouldStart))
            .ForMember(p => p.IntensiveCareStateMachineStartTime, c => c.MapFrom(src => src.IntensiveCareParameters != null ? src.IntensiveCareParameters.StartTime : null))
            .ForMember(p => p.IntensiveCareStateMachineNosologicalCode, c => c.MapFrom(src => src.IntensiveCareParameters != null ? src.IntensiveCareParameters.NosologicalCode : null))
            .ForMember(p => p.RedcapStateMachineShouldStart, c => c.MapFrom(src => src.RedcapParameters != null && src.RedcapParameters.ShouldStart))
            .ForMember(p => p.RedcapStateMachineStartTime, c => c.MapFrom(src => src.RedcapParameters != null ? src.RedcapParameters.StartTime : null))
            .ForMember(p => p.RedcapStateMachineRedcapStudyConfigurationId, c => c.MapFrom(src => src.RedcapParameters != null ? src.RedcapParameters.RedcapStudyConfigurationId : null))
            .ForMember(p => p.RedcapStateMachineRecordId, c => c.MapFrom(src => src.RedcapParameters != null ? src.RedcapParameters.RecordId : null))
            .ReverseMap()
            ;

        CreateMap<LaboratoryExam, FhirLaboratoryDataDto>();

        CreateMap<LaboratoryExamResult, FhirLaboratoryExamResultDto>();

        CreateMap<PodDefinition, PodDefinitionDto>()
            .ReverseMap();

        CreateMap<PodDefinitionContainer, PodDefinitionContainerDto>()
            .ReverseMap();

        CreateMap<PodEnvironmentVariable, PodEnvironmentVariableDto>()
            .ReverseMap();

        CreateMap<UpsertPodDefinitionDto, PodDefinition>();

        CreateMap<ScheduledIncrementalUploadParameter, ScheduledIncrementalUploadParameterDto>()
            .ReverseMap();

        CreateMap<StudyMetadata, StudyMetadataDto>()
            .ReverseMap();

        CreateMap<ClinicalDocumentType, ClinicalDocumentTypeDto>()
            .ReverseMap();

        CreateMap<DicomParametersJson, DicomParametersJsonDto>()
            .ReverseMap();

        CreateMap<StudyMetadata, StudyMetadataDto>()
            .ReverseMap();

        CreateMap<FullStudyMetadata, FullStudyMetadataDto>()
            .ReverseMap();

        CreateMap<PatientDetail, PatientDetailDto>()
            .ReverseMap();

        CreateMap<RedcapStudyConfiguration, RedcapStudyDetailDto>();

        CreateMap<RedcapStudyConfiguration, RedcapStudyDto>();

        CreateMap<CreateRedcapStudyDto, RedcapStudyConfiguration>();

        CreateMap<UpdateRedcapStudyDto, RedcapStudyConfiguration>();

        CreateMap<IntensiveCareEncounter, IntensiveCareEncounterDto>()
            .ReverseMap();

        CreateMap<IntensiveCarePatientStringAttribute, IntensiveCarePatientStringAttributeDto>()
            .ReverseMap();

    }
}
