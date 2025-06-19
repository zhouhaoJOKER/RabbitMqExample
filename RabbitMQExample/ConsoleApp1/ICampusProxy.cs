using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICampusProxy : IDisposable
    {
        void EndTask();
        void HeartBeat();
        void LineUp();
        void Login(string userName, string password);
        void StartTask(long taskId);
    }
}
