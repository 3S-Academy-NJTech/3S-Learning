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
            sr.Close();
            string[] result = data.Split(',');
            int num = (result.GetLength(0) - 1) / 3;

            //源数据矩阵
            double[] arr0 = new double[3 * num + 3];
            for (int i = 0; i < 3 * num + 2; i++)
            {
                arr0[i] = double.Parse(result[i]);
            }
            arr0[3 * num + 2] = 0;
            for (int i = 0; i < 3 * num - 1; i++)
            {
                label1.Text += arr0[i] + " ";
            }
            double[] brr0 = new double[num];//与未知点的距离向量
            for (int i = 0; i < num; i++)
                brr0[i] = Math.Sqrt(Math.Pow(arr0[3 * i + 0] - arr0[3 * num + 0], 2)
                         + Math.Pow(arr0[3 * i + 1] - arr0[3 * num + 1], 2));

            double[,] dist = new double[num, num];//距离矩阵

            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                    dist[i, j] = Math.Sqrt(Math.Pow(arr0[3 * i + 0] - arr0[3 * j + 0], 2)
                                + Math.Pow(arr0[3 * i + 1] - arr0[3 * j + 1], 2));
            }

            double[,] arr1 = new double[num + 1, num + 1];//距离矩阵经半球模型转换而得的半方差矩阵
            double[] brr1 = new double[num + 1];//与未知点的距离向量经半球模型转换而得的半方差向量
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                    arr1[i, j] = rh(dist[i, j]);

                arr1[i, num] = 1;
                arr1[num, i] = 1;
                brr1[num] = 1;
                brr1[i] = rh(brr0[i]);
            }

            double[,] arr2 = new double[num + 1, num + 1];//arr1的逆矩阵
            arr2 = ReverseMatrix(arr1, num + 1);

            double[] brr2 = new double[num + 1];//权重
            for (int i = 0; i < num + 1; i++)
            {
                for (int j = 0; j < num + 1; j++)
                {
                    brr2[i] += arr2[i, j] * brr1[j];
                }
            }
            double z = 0;//待求高值
            for (int i = 0; i < num; i++)
            {
                z += brr2[i] * arr0[3 * i + 2];
            }

            z = Math.Round(z, 4);//化为四精度

            label1.Text = z.ToString();
        }
        private double rh(double h)//半球体模型
        {
            return 2.048 + 1.154 * (1.5 * h / 8.535 - Math.Pow(h / 8.535, 3) * 0.5);
        }

        //求逆矩阵
        private double[,] ReverseMatrix(double[,] dMatrix, int Level)
        {
            double dMatrixValue = MatrixValue(dMatrix, Level);
            if (dMatrixValue == 0) return null;

            double[,] dReverseMatrix = new double[Level, 2 * Level];
            double x, c;
            // Init Reverse matrix
            for (int i = 0; i < Level; i++)
            {
                for (int j = 0; j < 2 * Level; j++)
                {
                    if (j < Level)
                        dReverseMatrix[i, j] = dMatrix[i, j];
                    else
                        dReverseMatrix[i, j] = 0;
                }

                dReverseMatrix[i, Level + i] = 1;
            }

            for (int i = 0, j = 0; i < Level && j < Level; i++, j++)
            {
                if (dReverseMatrix[i, j] == 0)
                {
                    int m = i;
                    for (; dMatrix[m, j] == 0; m++) ;
                    if (m == Level)
                        return null;
                    else
                    {
                        // Add i-row with m-row
                        for (int n = j; n < 2 * Level; n++)
                            dReverseMatrix[i, n] += dReverseMatrix[m, n];
                    }
                }

                // Format the i-row with "1" start
                x = dReverseMatrix[i, j];
                if (x != 1)
                {
                    for (int n = j; n < 2 * Level; n++)
                        if (dReverseMatrix[i, n] != 0)
                            dReverseMatrix[i, n] /= x;
                }

                // Set 0 to the current column in the rows after current row
                for (int s = Level - 1; s > i; s--)
                {
                    x = dReverseMatrix[s, j];
                    for (int t = j; t < 2 * Level; t++)
                        dReverseMatrix[s, t] -= (dReverseMatrix[i, t] * x);
                }
            }

            // Format the first matrix into unit-matrix
            for (int i = Level - 2; i >= 0; i--)
            {
                for (int j = i + 1; j < Level; j++)
                    if (dReverseMatrix[i, j] != 0)
                    {
                        c = dReverseMatrix[i, j];
                        for (int n = j; n < 2 * Level; n++)
                            dReverseMatrix[i, n] -= (c * dReverseMatrix[j, n]);
                    }
            }

            double[,] dReturn = new double[Level, Level];
            for (int i = 0; i < Level; i++)
                for (int j = 0; j < Level; j++)
                    dReturn[i, j] = dReverseMatrix[i, j + Level];
            return dReturn;
        }
        private double MatrixValue(double[,] MatrixList, int Level)
        {
            double[,] dMatrix = new double[Level, Level];
            for (int i = 0; i < Level; i++)
                for (int j = 0; j < Level; j++)
                    dMatrix[i, j] = MatrixList[i, j];
            double c, x;
            int k = 1;
            for (int i = 0, j = 0; i < Level && j < Level; i++, j++)
            {
                if (dMatrix[i, j] == 0)
                {
                    int m = i;
                    for (; dMatrix[m, j] == 0; m++) ;
                    if (m == Level)
                        return 0;
                    else
                    {
                        // Row change between i-row and m-row
                        for (int n = j; n < Level; n++)
                        {
                            c = dMatrix[i, n];
                            dMatrix[i, n] = dMatrix[m, n];
                            dMatrix[m, n] = c;
                        }

                        // Change value pre-value
                        k *= (-1);
                    }
                }

                // Set 0 to the current column in the rows after current row
                for (int s = Level - 1; s > i; s--)
                {
                    x = dMatrix[s, j];
                    for (int t = j; t < Level; t++)
                        dMatrix[s, t] -= dMatrix[i, t] * (x / dMatrix[i, j]);
                }
            }

            double sn = 1;
            for (int i = 0; i < Level; i++)
            {
                if (dMatrix[i, i] != 0)
                    sn *= dMatrix[i, i];
                else
                    return 0;
            }
            return k * sn;
        }
    }
}
