using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FM.UltraLab.Api.Model.CampusModel
{
    /// <summary>
    /// 
    /// </summary> 
    public class CampusCmdParam
    {
        public string Cno { get; set; }
        public string token { get; set; }
        public string apiBaseUrl { get; set; }
        public string clientKey { get; set; }
        public string useVersion { get; set; }
        public string expToken { get; set; }
        public string courseId { get; set; }
        public string courseName { get; set; }
        public string expParentName { get; set; }
        public string expParentId { get; set; }
        public string expId { get; set; }
        public string expName { get; set; }
    }
}
