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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TEXT文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            dlg.FilterIndex = 1;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = dlg.FileName;
            }
        }

        //最陡坡度值
        private int maxmid(double[] a)
        {
            double max = a[0];
            int mi = 0;
            int num = 0;
            for (int i = 0; i < 8; i++)
            {
                if (max < a[i])
                {
                    max = a[i];
                    mi = i;
                }
            }
            switch (mi)
            {
                case 0:
                    num = 32;
                    break;
                case 1:
                    num = 64;
                    break;
                case 2:
                    num = 128;
                    break;
                case 3:
                    num = 16;
                    break;
                case 4:
                    num = 1;
                    break;
                case 5:
                    num = 8;
                    break;
                case 6:
                    num = 4;
                    break;
                case 7:
                    num = 2;
                    break;
                default:
                    break;
            }
            return num;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read);
            StreamReader Reader = new StreamReader(fs);
            Reader.BaseStream.Seek(0, SeekOrigin.Begin);
            string[] hand = new string[6];
            string[,] arr = new string[39, 47];//原始数据字符串
            string strline;
            int count = 0;
            while (count < 6)
            {
                strline = Reader.ReadLine();
                hand[count] += strline;
                count++;
            }
            strline = Reader.ReadLine();
            int k = 0;
            while (strline != null)
            {

                string[] substr = strline.Split(' ');

                for (int j = 0; j < 47; j++)
                {
                    arr[k, j] = substr[j];
                }
                
                strline = Reader.ReadLine();
                k++;
            }
            Reader.Close();
            /**/
            double[,] ayy = new double[39, 47];//原始数据
            for (int i = 0; i < 39; i++)
            {
                for (int j = 0; j < 47; j++)
                    ayy[i, j] = double.Parse(arr[i, j]);
            }
            //richTextBox1.Text += ayy[1, 0] + "\n";
            double NODATA_value = -9999;
            int[,] byy = new int[39, 47];//输出结果
            double[] d8 = new double[8] { NODATA_value, NODATA_value, NODATA_value, NODATA_value, NODATA_value, NODATA_value, NODATA_value, NODATA_value };
            byy[0, 0] = 32;
            byy[0, 46] = 128;
            byy[38, 0] = 8;
            byy[38, 46] = 2;
            for (int i = 0; i < 39; i++)
            {
                for (int j = 0; j < 47; j++)
                {
                    if (i > 0 && i < 38 && j > 0 && j < 46)
                    {
                        d8[0] = (ayy[i, j] - ayy[i - 1, j - 1]) / Math.Sqrt(2);
                        d8[1] = ayy[i, j] - ayy[i - 1, j];
                        d8[2] = (ayy[i, j] - ayy[i - 1, j + 1]) / Math.Sqrt(2);
                        d8[3] = ayy[i, j] - ayy[i, j - 1];
                        d8[4] = ayy[i, j] - ayy[i, j + 1];
                        d8[5] = (ayy[i, j] - ayy[i + 1, j - 1]) / Math.Sqrt(2);
                        d8[6] = ayy[i, j] - ayy[i + 1, j];
                        d8[7] = (ayy[i, j] - ayy[i + 1, j + 1]) / Math.Sqrt(2);
                        byy[i, j] = maxmid(d8);
                    }
                    if (i == 0 && j > 0 && j < 46)
                    {
                        d8[0] = NODATA_value;
                        d8[1] = NODATA_value;
                        d8[2] = NODATA_value;
                        d8[3] = ayy[i, j] - ayy[i, j - 1];
                        d8[4] = ayy[i, j] - ayy[i, j + 1];
                        d8[5] = (ayy[i, j] - ayy[i + 1, j - 1]) / Math.Sqrt(2);
                        d8[6] = ayy[i, j] - ayy[i + 1, j];
                        d8[7] = (ayy[i, j] - ayy[i + 1, j + 1]) / Math.Sqrt(2);
                        byy[i, j] = maxmid(d8);
                    }
                    if (i == 38 && j > 0 && j < 46)
                    {
                        d8[0] = (ayy[i, j] - ayy[i - 1, j - 1]) / Math.Sqrt(2);
                        d8[1] = ayy[i, j] - ayy[i - 1, j];
                        d8[2] = (ayy[i, j] - ayy[i - 1, j + 1]) / Math.Sqrt(2);
                        d8[3] = ayy[i, j] - ayy[i, j - 1];
                        d8[4] = ayy[i, j] - ayy[i, j + 1];
                        d8[5] = NODATA_value;
                        d8[6] = NODATA_value;
                        d8[7] = NODATA_value;
                        byy[i, j] = maxmid(d8);
                    }
                    if (j == 0 && i > 0 && i < 38)
                    {
                        d8[0] = NODATA_value;
                        d8[1] = ayy[i, j] - ayy[i - 1, j];
                        d8[2] = (ayy[i, j] - ayy[i - 1, j + 1]) / Math.Sqrt(2);
                        d8[3] = NODATA_value;
                        d8[4] = ayy[i, j] - ayy[i, j + 1];
                        d8[5] = NODATA_value;
                        d8[6] = ayy[i, j] - ayy[i + 1, j];
                        d8[7] = (ayy[i, j] - ayy[i + 1, j + 1]) / Math.Sqrt(2);
                        byy[i, j] = maxmid(d8);
                    }
                    if (j == 46 && i > 0 && i < 38)
                    {
                        d8[0] = (ayy[i, j] - ayy[i - 1, j - 1]) / Math.Sqrt(2);
                        d8[1] = ayy[i, j] - ayy[i - 1, j];
                        d8[2] = NODATA_value;
                        d8[3] = ayy[i, j] - ayy[i, j - 1];
                        d8[4] = NODATA_value;
                        d8[5] = (ayy[i, j] - ayy[i + 1, j - 1]) / Math.Sqrt(2);
                        d8[6] = ayy[i, j] - ayy[i + 1, j];
                        d8[7] = NODATA_value;
                        byy[i, j] = maxmid(d8);
                    }

                }
            }
            SaveFileDialog sdg = new SaveFileDialog();
            sdg.Filter = "TEXT文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            sdg.FilterIndex = 1;
            string path="";
            if (sdg.ShowDialog() == DialogResult.OK)
            {
                path = sdg.FileName;
            }
            FileStream fw = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fw);

            //使用StreamWriter往文件写入内容
            string temp = "";
            sw.BaseStream.Seek(0, SeekOrigin.End);
            for (int i = 0; i < 6; i++)
                sw.WriteLine(hand[i]);
            for (int i = 0; i < 39; i++)
            {
                for (int j = 0; j < 47; j++)
                {
                    temp += byy[i, j] + " ";
                }
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(temp);
                temp = "";
            }

            //关闭此文件
            sw.Close();
            MessageBox.Show("成功保存文件！");
        }
    }
}
