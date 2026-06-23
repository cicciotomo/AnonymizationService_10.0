using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Services.Pathox
{
	public class PathoxSettings
	{
		public const string SettingsName = "Settings:PathoxSettings";
		public string GetExamListUrl { get; set; }
		public string GetExamDetailUrl { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
