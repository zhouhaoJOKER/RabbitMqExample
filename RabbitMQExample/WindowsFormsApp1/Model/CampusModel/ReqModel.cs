using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM.UltraLab.Api.Model.CampusModel
{
    public class StartTaskModel 
    {
        public string type { get; set; } = "business";

        public string signalling { get; set; } = "startTask";

        public int learnType { get; set; } = 0;

        public long taskId { get; set; } = 0L;
    }

    /// <summary>
    /// 
    /// </summary>
    public class EndTaskModel
    {
        public string type { get; set; } = "business";

        public string signalling { get; set; } = "endTask";
    }

    public class HeartBeatModel
    {
        public string type { get; set; } = "business";

        public string signalling { get; set; } = "refreshSessn";
    }
}
