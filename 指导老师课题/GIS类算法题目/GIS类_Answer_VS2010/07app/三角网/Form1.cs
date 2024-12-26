using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace 三角网
{
    public partial class Form1 : Form
    {
        Point point = new Point();
        Point point1 = new Point();
        Point point2 = new Point();
        Point point3 = new Point();

        int tPoints = 0;//记录集散点个数

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)//构建三角网
        {
            //读文件
            StreamReader objReader = new StreamReader(this.textBox1.Text, UnicodeEncoding.GetEncoding("GB2312"));
            string sLine = "";
            ArrayList LineList = new ArrayList();
            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null && !sLine.Equals(""))
                {
                    LineList.Add(sLine);
                }
            }
            //数据全部记录到LineList中。
            objReader.Close();
            int HowMany = 0;//一共多少个三角形
            function fu = new function();
            datatype dt = new datatype();//定义结构体对象
            ArrayList AnswerList = new ArrayList();

            for (int i = 0; i < LineList.Count; i++)
            {
                dt.Vertex[tPoints].x = Convert.ToInt32(LineList[i].ToString().Split(' ')[1].Split(',')[0]);
                dt.Vertex[tPoints].y = Convert.ToInt32(LineList[i].ToString().Split(' ')[1].Split(',')[1]);

                if (i > 1)//大于两个，起码3个点才能够成三角形
                {
                    HowMany = fu.Triangulate(tPoints, dt);
                }
                else
                {
                    point = new Point(Convert.ToInt32(LineList[i].ToString().Split(' ')[1].Split(',')[0]), Convert.ToInt32(LineList[i].ToString().Split(' ')[1].Split(',')[1]));
                    
                }
                tPoints++;
            }
            for (int j = 1; j <= HowMany; j++)
            {
                point1 = new Point(Convert.ToInt32(dt.Vertex[dt.Triangle[j].vv0].x), Convert.ToInt32(dt.Vertex[dt.Triangle[j].vv0].y));
                point2 = new Point(Convert.ToInt32(dt.Vertex[dt.Triangle[j].vv1].x), Convert.ToInt32(dt.Vertex[dt.Triangle[j].vv1].y));
                point3 = new Point(Convert.ToInt32(dt.Vertex[dt.Triangle[j].vv2].x), Convert.ToInt32(dt.Vertex[dt.Triangle[j].vv2].y));

                //查找对应点的点号
                int P1_Num = 0;
                int P2_Num = 0;
                int P3_Num = 0;
                
                for (int i = 0; i < LineList.Count; i++)
                {
                    if (Convert.ToInt32(LineList[i].ToString().Split(' ')[1].Split(',')[0]) == point1.X && Convert.ToInt32(LineList[i].ToString().Split(' ')[1].Split(',')[1]) == point1.Y)
                    {
                        P1_Num=Convert.ToInt32(LineList[i].ToString().Split(' ')[0]);
                    }
                    if (Convert.ToInt32(LineList[i].ToString().Split(' ')[1].Split(',')[0]) == point2.X && Convert.ToInt32(LineList[i].ToString().Split(' ')[1].Split(',')[1]) == point2.Y)
                    {
                        P2_Num = Convert.ToInt32(LineList[i].ToString().Split(' ')[0]);
                    }
                    if (Convert.ToInt32(LineList[i].ToString().Split(' ')[1].Split(',')[0]) == point3.X && Convert.ToInt32(LineList[i].ToString().Split(' ')[1].Split(',')[1]) == point3.Y)
                    {
                        P3_Num = Convert.ToInt32(LineList[i].ToString().Split(' ')[0]);
                    }
                }
                AnswerList.Add(P1_Num + "," + P2_Num);
                AnswerList.Add(P2_Num + "," + P3_Num);
                AnswerList.Add(P1_Num + "," + P3_Num);
            }
            FileStream fs1 = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Report.txt", FileMode.Create, FileAccess.Write);
            //创建写入文件 
            StreamWriter sw = new StreamWriter(fs1, UnicodeEncoding.GetEncoding("GB2312"));
            //写入数值 一般写入txt文件都是用“写入的文本”+"\r\n";来操作
            for (int i = 0; i < AnswerList.Count; i++)
            {
                for(int j=i+1; j<AnswerList.Count; j++)
                {
                    if (AnswerList[i].ToString() == AnswerList[j].ToString())
                    {
                        AnswerList.Remove(AnswerList[i]);
                        i = 0;
                    }
                }
            }
            for (int i = 0; i < AnswerList.Count; i++)
            {
                for (int j = i + 1; j < AnswerList.Count; j++)
                {
                    if (AnswerList[i].ToString().Split(',')[0] == AnswerList[j].ToString().Split(',')[1] && AnswerList[i].ToString().Split(',')[1] == AnswerList[j].ToString().Split(',')[0])
                    {
                        AnswerList.Remove(AnswerList[i]);
                        i = 0;
                    }
                }
            }
            for (int i = 0; i < AnswerList.Count; i++)
            {
                string Add = AnswerList[i].ToString() + "\r";
                sw.WriteLine(Add);
            }
                //sw.WriteLine(this.textBox3.Text.Trim() + "+" + this.textBox4.Text);//开始写入值
                sw.Close();
            fs1.Close();
            MessageBox.Show("三角形个数为："+HowMany+","+"离散点数为："+(tPoints-1).ToString());
        }

        private void button2_Click(object sender, EventArgs e)//导入文件
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TXT文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            dlg.FilterIndex = 1;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = dlg.FileName;
            }
            else
            {
                return;
            }
            
        }
        class datatype//定义数据结构体
        {
            // 点(Vertices)
            public struct dVertex
            {
                public long x;
                public long y;
                public int SUM;//判断点的比较次数
            }

            // 三角形, vv#代表点
            public struct dTriangle
            {
                public long vv0;
                public long vv1;
                public long vv2;
            }

            // 三角形外心结构
            public struct BaryCenter
            {
                public double a;
                public double b;
                public Boolean ID; //判断外心是否连接
                public int NUM;//外心累计连接次数
            }

            //Set these as applicable
            public static long MaxVertices = 300;
            public static long MaxTriangles = 900;
            //Our points
            public dVertex[] Vertex = new dVertex[MaxVertices];
            //Our Created Triangles
            public dTriangle[] Triangle = new dTriangle[MaxTriangles];
            public BaryCenter[] OutHert = new BaryCenter[MaxTriangles];
        }
        class function : Form1
        {
            public static long MaxVertices = 500;
            public static long MaxTriangles = 1000;

            // 三角划分
            public int Triangulate(int nvert, datatype dt)
            {
                //输入NVERT vertices in arrays Vertex()
                //'Returned is a list of NTRI triangular faces in the array
                //'Triangle(). These triangles are arranged in clockwise order.
                Boolean[] Complete = new Boolean[MaxVertices];
                long[,] Edges = new long[3, MaxTriangles * 3];
                long Nedge;
                //超级三角形
                long xmin, xmax, ymin, ymax;
                long xmid, ymid, dx, dy, dmax;
                //普通变量
                int i, j, k, ntri;
                double xc, yc, r;
                Boolean inc;
                //Find the maximum and minimum vertex bounds.
                //This is to allow calculation of the bounding triangle
                xmin = dt.Vertex[1].x; ymin = dt.Vertex[1].y;
                xmax = xmin; ymax = ymin;
                for (i = 2; i <= nvert; i++)
                {
                    if (dt.Vertex[i].x < xmin)
                        xmin = dt.Vertex[i].x;
                    if (dt.Vertex[i].x > xmax)
                        xmax = dt.Vertex[i].x;
                    if (dt.Vertex[i].y < ymin)
                        ymin = dt.Vertex[i].y;
                    if (dt.Vertex[i].x > ymax)
                        ymax = dt.Vertex[i].y;
                }
                dx = xmax - xmin; dy = ymax - ymin;
                if (dx > dy)
                    dmax = dx;
                else
                    dmax = dy;
                xmid = (xmax + xmin) / 2;
                ymid = (ymax + ymin) / 2;
                // 构建超级三角形
                //'This is a triangle which encompasses all the sample points.
                //'The supertriangle coordinates are added to the end of the
                //'vertex list. The supertriangle is the first triangle in
                //'the triangle list.
                dt.Vertex[nvert + 1].x = Convert.ToInt64(xmid - 2 * dmax);
                dt.Vertex[nvert + 1].y = Convert.ToInt64(ymid - dmax);
                dt.Vertex[nvert + 2].x = xmid;
                dt.Vertex[nvert + 2].y = Convert.ToInt64(ymid + 2 * dmax);
                dt.Vertex[nvert + 3].x = Convert.ToInt64(xmid + 2 * dmax);
                dt.Vertex[nvert + 3].y = Convert.ToInt64(ymid - dmax);
                dt.Triangle[1].vv0 = nvert + 1;
                dt.Triangle[1].vv1 = nvert + 2;
                dt.Triangle[1].vv2 = nvert + 3;
                Complete[1] = false;
                ntri = 1; xc = 0; yc = 0; r = 0;
                //Include each point one at a time into the existing mesh
                for (i = 1; i <= nvert; i++)
                {
                    Nedge = 0;
                    //Set up the edge buffer.
                    // If the point (Vertex(i).x,Vertex(i).y) lies inside the circumcircle then the
                    // 'three edges of that triangle are added to the edge buffer.
                    j = 0;
                    do
                    {
                        j = j + 1;
                        if (Complete[j] != true)
                        {
                            inc = InCircle(dt.Vertex[i].x, dt.Vertex[i].y, dt.Vertex[dt.Triangle[j].vv0].x, dt.Vertex[dt.Triangle[j].vv0].y, dt.Vertex[dt.Triangle[j].vv1].x, dt.Vertex[dt.Triangle[j].vv1].y, dt.Vertex[dt.Triangle[j].vv2].x, dt.Vertex[dt.Triangle[j].vv2].y, ref xc, ref yc, ref r);
                            if (inc)
                            {
                                Edges[1, Nedge + 1] = dt.Triangle[j].vv0;
                                Edges[2, Nedge + 1] = dt.Triangle[j].vv1;
                                Edges[1, Nedge + 2] = dt.Triangle[j].vv1;
                                Edges[2, Nedge + 2] = dt.Triangle[j].vv2;
                                Edges[1, Nedge + 3] = dt.Triangle[j].vv2;
                                Edges[2, Nedge + 3] = dt.Triangle[j].vv0;
                                Nedge = Nedge + 3;
                                dt.Triangle[j].vv0 = dt.Triangle[ntri].vv0;
                                dt.Triangle[j].vv1 = dt.Triangle[ntri].vv1;
                                dt.Triangle[j].vv2 = dt.Triangle[ntri].vv2;
                                Complete[j] = Complete[ntri];
                                j = j - 1;
                                ntri = ntri - 1;
                            }
                        }
                    }
                    while (j < ntri);
                    for (j = 1; j <= (Nedge - 1); j++)
                    {
                        if (!(Edges[1, j] == 0) && !(Edges[2, j] == 0))
                        {
                            for (k = j + 1; k <= Nedge; k++)
                            {
                                if (!((Edges[1, k] == 0)) && !((Edges[2, k] == 0)))
                                {
                                    if ((Edges[1, j] == Edges[2, k]))
                                    {
                                        if (Edges[2, j] == Edges[1, k])
                                        {
                                            Edges[1, j] = 0;
                                            Edges[2, j] = 0;
                                            Edges[1, k] = 0;
                                            Edges[2, k] = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //Form new triangles for the current point
                    for (j = 1; j <= Nedge; j++)
                    {
                        if (!(Edges[1, j] == 0) && !(Edges[2, j] == 0))
                        {
                            ntri = ntri + 1;
                            dt.Triangle[ntri].vv0 = Edges[1, j];
                            dt.Triangle[ntri].vv1 = Edges[2, j];
                            dt.Triangle[ntri].vv2 = i;
                            Complete[ntri] = false;
                        }
                    }
                }
                //Remove triangles with supertriangle vertices
                i = 0;
                do
                {
                    i = i + 1;
                    if (dt.Triangle[i].vv0 > nvert || dt.Triangle[i].vv1 > nvert || dt.Triangle[i].vv2 > nvert)
                    {
                        dt.Triangle[i].vv0 = dt.Triangle[ntri].vv0;
                        dt.Triangle[i].vv1 = dt.Triangle[ntri].vv1;
                        dt.Triangle[i].vv2 = dt.Triangle[ntri].vv2;
                        i = i - 1;
                        ntri = ntri - 1;
                    }
                }
                while (i < ntri);
                return ntri;
            }

            private Boolean InCircle(double xp, double yp, double x1, double y1, double x2, double y2, double x3, double y3, ref double xc, ref double yc, ref double r)
            {
                //'Return TRUE if the point (xp,yp) lies inside the circumcircle
                //made up by points (x1,y1) (x2,y2) (x3,y3)
                //'The circumcircle centre is returned in (xc,yc) and the radius r
                //'NOTE: A point on the edge is inside the circumcircle
                double eps, m1, m2, mx1, mx2, my1, my2, dx, dy, rsqr, drsqr;
                eps = 0.000001;
                if (System.Math.Abs(y1 - y2) < eps && System.Math.Abs(y2 - y3) < eps)
                    MessageBox.Show("there is some problems;");
                if (System.Math.Abs(y2 - y1) < eps)
                {
                    m2 = -(x3 - x2) / (y3 - y2);
                    mx2 = (x2 + x3) / 2;
                    my2 = (y2 + y3) / 2;
                    xc = (x2 + x1) / 2;
                    yc = m2 * (xc - mx2) + my2;
                }
                else if (System.Math.Abs(y3 - y2) < eps)
                {
                    m1 = -(x2 - x1) / (y2 - y1);
                    mx1 = (x1 + x2) / 2;
                    my1 = (y1 + y2) / 2;
                    xc = (x3 + x2) / 2;
                    yc = m1 * (xc - mx1) + my1;
                }
                else
                {
                    m1 = Convert.ToDouble(((x2 - x1) / (y2 - y1)) - 2 * ((x2 - x1) / (y2 - y1)));
                    m2 = Convert.ToDouble(((x3 - x2) / (y3 - y2)) - 2 * ((x3 - x2) / (y3 - y2)));
                    mx1 = (x1 + x2) / 2;
                    mx2 = (x2 + x3) / 2;
                    my1 = (y1 + y2) / 2;
                    my2 = (y2 + y3) / 2;
                    xc = (m1 * mx1 - m2 * mx2 + my2 - my1) / (m1 - m2);
                    yc = m1 * (xc - mx1) + my1;
                }
                dx = x2 - xc;
                dy = y2 - yc;
                rsqr = dx * dx + dy * dy;
                r = System.Math.Sqrt(rsqr);
                dx = xp - xc;
                dy = yp - yc;
                drsqr = dx * dx + dy * dy;
                if (drsqr <= rsqr)
                    return true;
                return false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
             tPoints = 1;
        }
    }
}
