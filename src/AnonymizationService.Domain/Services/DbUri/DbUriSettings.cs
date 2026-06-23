using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Services.DbUri
{
	public class DbUriSettings
	{
		public const string SettingsName = "Settings:DbUriSettings";
		public string GetDbUriDataByMasterPatientIndexUrl { get; set; }
		public string GetDbUriDataByTaxCodeIndexUrl { get; set; }
	}
}
