using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TtcMonitor
{
    public partial class Form1 : Form
    {
        ScheduleSetup config;
        Schedule s1;

        public Form1()
        {
            InitializeComponent();
            config = new ScheduleSetup(6132, 2, 20);
            s1 = new Schedule(config);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
           await s1.Tick();
           Text = s1.Items.Count.ToString();
//            var res = await PageDownloader.GetDataForItem(6132, 1);
        }
    }
}
