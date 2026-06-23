using System;

namespace AnonymizationService.StateMachines.Laboratory
{
    public class LaboratoryParametersDto
    {
        public bool ShouldStart { get; set; }
        public DateTime? StartTime { get; set; }
    }
}
