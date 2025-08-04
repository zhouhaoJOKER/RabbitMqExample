using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatsonTcpClientDemo
{
    public class MessageResp
    {
        public string message { get; set; }
        public bool bSucess { get; set; }
        public string data { get; set; }
        public string error { get; set; }

        public MessageResp() 
        {
            
        }
        public MessageResp(string _message, bool _bSucess, string _data) 
        {
            message = _message;
            bSucess = _bSucess;
            data = _data;
        }
    }
}
