using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using InoueLab;

namespace SoundMaker
{
    public partial class Form1 : Form
    {
        TrackBar[] trackBars = new TrackBar[7];

        double[] f0 = { 261.63, 293.66, 329.63, 349.23, 391.1, 440.0, 493.88 };
        double[] freqs = new double[7];
        double[] freq_power = new double[7];
        double[] real_freqs;
        double[] wavdata;

        int chord_flag = 0;
        int fs;
        int fft_length;

        public Form1()
        {
            InitializeComponent();
            trackBars[0] = trackBar1;
            trackBars[1] = trackBar2;
            trackBars[2] = trackBar3;
            trackBars[3] = trackBar4;
            trackBars[4] = trackBar5;
            trackBars[5] = trackBar6;
            trackBars[6] = trackBar7;
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
            string output_foldername = "";
            FolderBrowserDialog fdb = new FolderBrowserDialog();
            fdb.Description = "出力フォルダを選択してください";
            fdb.RootFolder = Environment.SpecialFolder.Desktop;
            fdb.ShowNewFolderButton = true;
            if (fdb.ShowDialog(this) == DialogResult.OK)
                output_foldername = fdb.SelectedPath;

            if (textBox3.Text == "")
                MessageBox.Show("出力ファイル名を入力してね", "注意", MessageBoxButtons.YesNo);
            else
                WaveFile.Save(output_foldername + "\\" + textBox3.Text + ".wav", makeStereo(wavdata), fs);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            checkParameter();
            //ifft
            Complex[] spect = new Complex[fft_length+1];
            for (int n = 0; n < fft_length; n++)
                spect[n] = new Complex(real_freqs[n], 0);
            spect[fft_length]=new Complex(0,0); //直流成分

            MySignalProcessing.SP.Real_IFFT(spect, out wavdata);

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
                fs = int.Parse(textBox1.Text);
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
                fft_length = int.Parse(textBox2.Text);
        }
        //---------------------------------------------------------------------------
        private void setFreqs()
        {
            double f = f0[chord_flag];
            foreach (int n in freqs)
                freqs[n] = f * (n + 1);
        }
        //---------------------------------------------------------------------------
        private void setFreqPower()
        {
            foreach (int n in freq_power)
                freq_power[n] = trackBars[n].Value; //調整必要
        }
        //---------------------------------------------------------------------------
        private void checkParameter()
        {
            if (fft_length == 0 || fs == 0)
                MessageBox.Show("値を入力してください");
            if (Math.Log(fft_length, 2) != Math.Floor(Math.Log(fft_length, 2)))
                MessageBox.Show("fft_lengthは2の冪乗にしてください");
        }
        //---------------------------------------------------------------------------
        private void drawChart()
        {
            double delta_f = fs / 2 / fft_length;
            double[] x = new double[fft_length]; //freq
            double[] y = new double[fft_length]; //power

            foreach (int n in x)
                x[n] = n * delta_f;
        }
        //---------------------------------------------------------------------------
        private double[][] makeStereo(double[] data)
        {
            double[][] output = new double[2][];
            output[0] = (double[])data.Clone();
            output[1] = (double[])data.Clone();
            return output;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
