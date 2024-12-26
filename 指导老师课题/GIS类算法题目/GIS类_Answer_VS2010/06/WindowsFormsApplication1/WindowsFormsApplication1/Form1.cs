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
        int[] Morton = new int[64];
        int[] Deep = new int[64];
        int[] Imorton = new int[64];

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
            strline = sr.ReadLine();
            string data = "";
            while (strline != null)
            {
                data += strline;
                strline = sr.ReadLine();
            }
            sr.Close();
            string[] arr = data.Split(',');
            int[] Arry = new int[64];
            for (int i = 0; i < 64; i++)
            {
                Arry[i] = int.Parse(arr[i]);
            }
            for (int i = 0; i < 64; i++)
            {
                Morton[mortonS(i)] = Arry[i];
            }
            for (int i = 0; i < 64; i++)
            {
                Imorton[mortonS(i)] = i;
            }

            deepth();
            //for (int i = 0; i < 64; i++) { label1.Text += "(" + Arry[i] + "," + row(i) + "," + line(i) + ")" + "\n"; }
            //创建文本
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
            string answer1 = "求出每个数据对应的二进制编码的行列号：" + "\r\n" + "（数据，行号，列号）" + "\r\n";
            string answer2 = "\r\n" + "求出每个数据所在位置的原始四进制和十进制编码:" + "\r\n"
                            + "（数据，四进制，十进制）" + "\r\n";
            string answer3 = "\r\n" + "输出最终每个数值对应的Morton码(四叉树四进制编码)和Morton码(四叉树十进制编码)"
                            + "\r\n" + "（四进制十进制，深度，数值）" + "\r\n";
            
            for (int i = 0; i < 64; i++) { answer1 += "(" + Arry[i] + "," + row(i) + "," + line(i) + ")" + "\r\n"; }
            for (int i = 0; i < 64; i++) { answer2 += "(" + Arry[i] + "," + morton(i) + "," + mortonS(i) + ")" + "\r\n"; }
            for (int i = 0; i < 64; i++)
            {
                answer3 += "(" + Sts((int)(i / (Math.Pow(4, 3 - Deep[i])))) + "," + i / (Math.Pow(4, 3 - Deep[i])) + "," + Deep[i] + "," + Morton[i] + ")" + "\r\n";
                i += (int)Math.Pow(4, 3 - Deep[i]) - 1;
            }
            sw.Write(answer1);
            sw.Write(answer2);
            sw.Write(answer3);
            sw.Close();
            MessageBox.Show("文件保存成功！"); 
        }
        private string row(int a)
        {
            string x = Convert.ToString(a / 8, 2);
            if (x.Length == 2) { x = "0" + x; }
            if (x.Length == 1) { x = "00" + x; }
            return x;
        }
        private string line(int a)
        {
            string x = Convert.ToString(a % 8, 2);
            if (x.Length == 2) { x = "0" + x; }
            if (x.Length == 1) { x = "00" + x; }
            return x;
        }

        //morton码
        private int morton(int a)
        {
            return 2 * int.Parse(row(a)) + int.Parse(line(a));
        }

        //morton码的十进制
        private int mortonS(int a)
        {
            int x = morton(a);
            int sum = 0, k = 0;
            while (x != 0)
            {
                int temp = x;
                x = x / 10;
                temp = temp % 10;
                sum += temp * (int)Math.Pow(4, k);
                k = k + 1;
            }
            return sum;

        }

        //各点的深度
        private void deepth()
        {
            for (int i = 0; i < 16; i++)
            {
                if (Morton[i * 4] == Morton[i * 4 + 1] && Morton[i * 4 + 1] == Morton[i * 4 + 2] && Morton[i * 4 + 2] == Morton[i * 4 + 3])
                    Deep[i * 4] = Deep[i * 4 + 1] = Deep[i * 4 + 2] = Deep[i * 4 + 3] = 2;
                else
                    Deep[i * 4] = Deep[i * 4 + 1] = Deep[i * 4 + 2] = Deep[i * 4 + 3] = 3;
            }

            for (int i = 0; i < 4; i++)
            {
                if (Deep[i * 16] == 2 && Deep[i * 16] == Deep[i * 16 + 4] && Deep[i * 16 + 4] == Deep[i * 16 + 8] && Deep[i * 16 + 8] == Deep[i * 16 + 12])
                    if (Morton[i * 16] == Morton[i * 16 + 4] && Morton[i * 16 + 4] == Morton[i * 16 + 8] && Morton[i * 16 + 8] == Morton[i * 16 + 12])
                        for (int j = 0; j < 16; j++)
                            Deep[i * 16 + j] = 1;
            }
        }

        //十进制转为四进制
        private int Sts(int s)
        {
            int sum = 0, k = 0;
            while (s != 0)
            {
                int temp = s;
                s = s / 4;
                temp = temp % 4;
                sum += temp * (int)Math.Pow(10, k);
                k = k + 1;
            }
            return sum;
        }
    }
}
