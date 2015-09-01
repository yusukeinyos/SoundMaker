using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundMaker
{
    public partial class Form1 : Form
    {
        TrackBar[] trackBars=new TrackBar[7];

        double[] f0 = { 261.63, 293.66, 329.63, 349.23, 391.1, 440.0, 493.88 };
        double[] freqs = new double[7];
        double[] freq_power = new double[7];

        int chord_flag = 0;
        int fs;

        public Form1()
        {
            InitializeComponent();
            trackBars[0] = trackBar1;
            trackBars[1] = trackBar2;
            trackBars[2] = trackBar3;
            trackBars[3] = trackBar4;
            trackBars[4] = trackBar5;
            trackBars[5] = trackBar6;
            trackBars[1] = trackBar7;
        }
        //---------------------------------------------------------------------------
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < comboBox1.Items.Count; i++)
                if (comboBox1.SelectedIndex == i)
                {
                    chord_flag = i;
                    break;
                }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //folder dialog
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //ifft
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
                fs = int.Parse(textBox1.Text);
        }
        //---------------------------------------------------------------------------
        private void setFreqs()
        {
            double f = f0[chord_flag];
            foreach(int n in freqs)
                freqs[n] = f * (n + 1);
        }
        //---------------------------------------------------------------------------
        private void setFreqPower()
        {
            foreach (int n in freq_power)
                freq_power[n] = trackBars[n].Value; //調整必要
        }
        //---------------------------------------------------------------------------
        private void drawChart()
        {

        }

    }
}
