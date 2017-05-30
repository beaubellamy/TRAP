using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TRAP
{
    public partial class TrainPerformance : Form
    {
        public static TrackGeometry track = new TrackGeometry();

        public TrainPerformance()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Train> trains = Algorithm.trainPerformance();
        }
    }
}
