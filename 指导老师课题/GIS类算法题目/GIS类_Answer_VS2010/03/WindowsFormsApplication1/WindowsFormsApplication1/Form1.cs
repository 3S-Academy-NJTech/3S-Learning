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

        private void button2_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            string strline = sr.ReadLine();
            string data = "";
            while (strline != null)
            {
                data += strline + ",";
                strline = sr.ReadLine();
            }
            string[] result = data.Split(',');
            int num = result.GetLength(0) / 3;
            double[] arr = new double[3 * num];//原始数据
            for (int i = 0; i < 3*num; i++)
            {
                arr[i] = double.Parse(result[i]);
            }
            if (textBox2.Text == null||textBox3.Text==null)
                return;
            double x = double.Parse(textBox2.Text);
            double y = double.Parse(textBox3.Text);
            //判断矩形框
            double x1=0, y1=0, x2=0, y2=0,q11=0,q12=0,q21=0,q22=0;
            for(int a=0;a<num;a++)
                for(int b=0;b<num;b++)
                    for(int c=0;c<num;c++)
                        for (int d = 0; d <num; d++)
                        {
                            if (arr[3 * a] == arr[3 * b] && arr[3 * c] == arr[3 * d] && arr[3 * a + 1] == arr[3 * c + 1] && arr[3 * b + 1] == arr[3 * d + 1] && arr[3 * a] != arr[3 * c] && arr[3 * a + 1] != arr[3 * b + 1])
                            {
                                x1 = arr[3 * a];
                                y1 = arr[3 * a + 1];
                                x2 = arr[3 * d];
                                y2 = arr[3 * d + 1];
                                q11 = arr[3 * a + 2];
                                q12 = arr[3 * b + 2];
                                q21 = arr[3 * c + 2];
                                q22 = arr[3 * d + 2];
                            }
                        }
            double testresult = q11 * (x2 - x) * (y2 - y) / (x2 - x1) / (y2 - y1) + q21 * (x - x1) * (y2 - y) / (x2 - x1) / (y2 - y1)
                             + q12 * (x2 - x) * (y - y1) / (x2 - x1) / (y2 - y1) + q22 * (x - x1) * (y - y1) / (x2 - x1) / (y2 - y1);
            label3.Text = testresult.ToString();
        }
    }
}
