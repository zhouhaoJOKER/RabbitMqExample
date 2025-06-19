using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Model
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
