using AnonymizationService.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.Patients
{
    public class PagedAndSortedResultRequestFilterDto : PagedAndSortedResultRequestDto
    {
     
        public DateTime CreationTime { get; set; }
       



    }
}
