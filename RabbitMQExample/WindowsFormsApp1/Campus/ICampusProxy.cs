using FM.UltraLab.Api.Model.CampusModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FM.UltraLab.Api.Campus
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICampusProxy 
    { 
        //void EndTask();
        //void HeartBeat();
        void LineUp();
        void Login(string userName, string password);
        //void StartTask(long taskId);
    }
}
