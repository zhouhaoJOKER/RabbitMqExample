using FM.UltraLab.Api.Campus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ICampusProxy proxy = new CampusProxy();
            proxy.Login("zhouhao", "123456");
            proxy.LineUp();
             
            InitializeComponent();
        }
    }
}
