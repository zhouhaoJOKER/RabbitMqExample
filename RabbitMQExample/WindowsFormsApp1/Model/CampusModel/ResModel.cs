using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM.UltraLab.Api.Model.CampusModel
{
    public class WsMesModel : BaseMesModel
    {
        public bool status { get; set; }
    }

    public class BaseMesModel
    {
        public int type { get; set; }
    }  
}
