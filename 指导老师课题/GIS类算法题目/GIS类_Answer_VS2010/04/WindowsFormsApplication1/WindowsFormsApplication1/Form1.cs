using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        double x, y;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = ("交点坐标：" + "\n");

            int Xr = 60;
            int Yr = 40;
            int R = 24;
            Graphics gra = this.pictureBox1.CreateGraphics();
            Pen pen = new Pen(Color.Blue);//画笔颜色
            gra.DrawEllipse(pen, 36, 16, 48, 48);//画椭圆的方法，x坐标、y坐标、宽、高，如果是100，则半径为50 中心点的坐标是(x+width/2,y+height/2)
            
            int[,] p = new int[4, 2] { { 30, 35 }, { 40, 27 }, { 48, 40 }, { 45, 60 } };

            int i = 0;
            for (i = 0; i < 3; i++)
            {
                node(p[i, 0], p[i, 1], p[i + 1, 0], p[i + 1, 1], Xr, Yr, R);
                //node(p[i + 1, 0], p[i + 1, 1], p[i, 0], p[i, 1], Xr, Yr, R);
                Point point1 = new Point(p[i, 0], p[i, 1]);
                Point point2 = new Point(p[i + 1, 0], p[i + 1, 1]);
                gra.DrawLine(pen, point1, point2);
            }

            node(p[0, 0], p[0, 1], p[3, 0], p[3, 1], Xr, Yr, R);
            Point point111 = new Point(p[0, 0], p[0, 1]);
            Point point222 = new Point(p[3, 0], p[3, 1]);
            gra.DrawLine(pen, point111, point222);
        }

        //求交点
        private void node(double a1, double b1, double a2, double b2, double ar, double br, double r)
        {
            double x0 = a1;
            double y0 = b1;
            double x1 = a2;
            double y1 = b2;
            if (a1 > a2)
            {
                x0 = a2;
                y0 = b2;
                x1 = a1;
                y1 = b1;
            }
            double xr = ar;
            double yr = br;
            double R = r;
            double k = (y1 - y0) / (x1 - x0);
            double a = (k * k + 1);
            double b = 2 * k * ((y0 - y1) * x0 / (x1 - x0) + y0 - yr) - 2 * xr;
            double c = ((y0 - y1) * x0 / (x1 - x0) + y0 - yr) * ((y0 - y1) * x0 / (x1 - x0) + y0 - yr)
                + xr * xr - R * R;

            if (b * b - 4 * a * c >= 0)
            {
                double xx1 = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
                double xx2 = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);

                if ((xx1 > x0) && (xx1 < x1))
                {
                    x = xx1;
                    y = (y1 - y0) * (x - x0) / (x1 - x0) + y0;
                    x = Math.Round(x, 1);
                    y = Math.Round(y, 1);
                    label1.Text += ("(" + x.ToString() + "," + y.ToString() + ")" + "\n");
                    Graphics gra = this.pictureBox1.CreateGraphics();
                    Pen pen = new Pen(Color.Red);//画笔颜色
                    gra.DrawEllipse(pen, (int)(x - 1), (int)(y - 1), 2, 2);
                }

                if ((xx2 > x0) && (xx2 < x1))
                {
                    x = xx2;
                    y = (y1 - y0) * (x - x0) / (x1 - x0) + y0;
                    x = Math.Round(x, 1);
                    y = Math.Round(y, 1);
                    label1.Text += ("(" + x.ToString() + "," + y.ToString() + ")" + "\n");
                    Graphics gra = this.pictureBox1.CreateGraphics();
                    Pen pen = new Pen(Color.Red);//画笔颜色
                    gra.DrawEllipse(pen, (int)(x - 1), (int)(y - 1), 2, 2);//画椭圆的方法，x坐标、y坐标、宽、高，如果是100，则半径为50

                }
            }
        }
    }
}
