using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        double B, L;//定义两个全局变量
        const double K0 = 1.57048687472752E-7;
        const double K1 = 5.05250559291393E-3;
        const double K2 = 2.98473350966158E-5;
        const double K3 = 2.41627215981336E-7;
        const double K4 = 2.22241909461273E-9;
        const double en2 = 6.73950181947292E-3;
        const double a = 6378140.0;
        const double b = 6356755.29;
        const double PI = 3.14159265358979;
        const double e2 = 6.69438499958795E-3;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //导入文件
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TEXT文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            dlg.FilterIndex = 1;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = dlg.FileName;
            }
            else
                return;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //读文件
            FileStream fs = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            string data = "";
            string strline = sr.ReadLine();
            while (strline != null)
            {
                data += strline + "\n";
                strline = sr.ReadLine();
                data += strline + "\n";
                strline = sr.ReadLine();
                if (strline == "")
                    strline = sr.ReadLine();
            }
            sr.Close();

            //创建写入文件
            SaveFileDialog sdg = new SaveFileDialog();
            sdg.Filter = "TEXT文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            sdg.FilterIndex = 1;
            string path = "";
            if (sdg.ShowDialog() == DialogResult.OK)
            {
                path = sdg.FileName;
            }
            FileStream fw = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fw);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            
            //原始数据
            string[] arr = data.Replace("\n", ",").Split(',');
            
            int len = arr.GetLength(0) / 4;
            for (int i = 0; i < len; i++)
            {
                double x1 = double.Parse(arr[4 * i]);
                double y1 = double.Parse(arr[4 * i + 1]);
                double x2 = double.Parse(arr[4 * i + 2]);
                double y2 = double.Parse(arr[4 * i + 3]);

                double B1, B2, L1, L2;
                change(x1, y1);
                B1 = B;
                L1 = L;
                change(x2, y2);
                B2 = B;
                L2 = L;   

                double Lm = L2 - L1;
                double Bm = (B1 + B2) / 2;

                double As = 1 + 3 * e2 / 6 + 30 * e2 * e2 / 80 + 35 * e2 * e2 * e2 / 112 + 630 * Math.Pow(e2, 4) / 2304;
                double Bs = 1 * e2 / 6 + 15 * e2 * e2 / 80 + 21 * e2 * e2 * e2 / 112 + 420 * Math.Pow(e2, 4) / 2304;
                double Cs = 3 * e2 / 80 * e2 + 7 * e2 * e2 * e2 / 112 + 180 * Math.Pow(e2, 4) / 2304;
                double Ds = 1 * e2 * e2 * e2 / 112 + 45 * Math.Pow(e2, 4) / 2304;
                double Es = 5 * Math.Pow(e2, 4) / 2304;

                double S = 2 * b * b * Lm * (As * Math.Sin(0.5 * (B2 - B1)) * Math.Cos(Bm)
                    - Bs * Math.Sin(1.5 * (B2 - B1)) * Math.Cos(3 * Bm)
                    + Cs * Math.Sin(2.5 * (B2 - B1)) * Math.Cos(5 * Bm)
                    - Ds * Math.Sin(3.5 * (B2 - B1)) * Math.Cos(7 * Bm)
                    + Es * Math.Sin(4.5 * (B2 - B1)) * Math.Cos(9 * Bm));

                string answer = B1.ToString() + "," + L1.ToString()
                                + "\r\n" + B2.ToString() + "," + L2.ToString() + "\r\n"
                                + S.ToString() + "\r\n" + "\r\n";
                sw.Write(answer);
            }
            sw.Close();
            MessageBox.Show("成功保存！");
        }

        //高斯投影反解变换
        private void change(double i, double j)
        {
            double x, y;
            x = i;
            y = j;

            double yn = y - 500000 - 0 * 1000000;
            double E = K0 * x;
            double Bf = E + Math.Cos(E) * (K1 * Math.Sin(E) - K2 * Math.Pow(Math.Sin(E), 3)
                + K3 * Math.Pow(Math.Sin(E), 5) - K4 * Math.Pow(Math.Sin(E), 7));
            double t = Math.Tan(Bf);
            double n2 = en2 * Math.Cos(Bf) * Math.Cos(Bf);
            double C = a * a / b;
            double V = Math.Sqrt(1 + n2);
            double N = C / V;

            B = Bf - 0.5 * V * V * t * Math.Pow(yn / N, 2) + 1 * (5 + 3 * t * t + n2 - 9 * n2 * t * t) * V * V * t * Math.Pow(yn / N, 4) / 24
                - 1 * (61 + 90 * t * t + 45 * t * t * t * t) * V * V * t * Math.Pow(yn / N, 6) / 720;
            L = (1 / Math.Cos(Bf)) * (yn / N) - 1 * (1 + 2 * t * t + n2) * (1 / Math.Cos(Bf)) * Math.Pow((yn / N), 3) / 6 
                + 1 * (5 + 28 * t * t + 24 * t * t * t * t + 6 * n2 + 8 * n2 * t * t) * (1 / Math.Cos(Bf)) * Math.Pow((yn / N), 5) / 120 + PI * 2 / 3;
        }
    }
}
