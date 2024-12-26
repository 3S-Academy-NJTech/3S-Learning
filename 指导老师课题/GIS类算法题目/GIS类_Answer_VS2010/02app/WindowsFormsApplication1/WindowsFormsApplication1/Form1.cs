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
        public Form1()
        {
            InitializeComponent();
        }
        const int maxValue = 10000;
        private void button1_Click(object sender, EventArgs e)
        {
            int [,]arr=new int [6,6]{{maxValue,6,3,maxValue,maxValue,maxValue},{6,maxValue,2,5,maxValue,maxValue},
					{3,2,maxValue,3,4,maxValue},{maxValue,5,3,maxValue,2,3},
					{maxValue,maxValue,4,2,maxValue,5},{maxValue,maxValue,maxValue,3,5,maxValue}};
            int[]dist=new int[6];
            int[]path=new int[6];   
                           

	int start=0;             
    label1.Text += "起点  终点    最短路径    距离";
	
	while(true)           
	{
		
		if(start>=0&&start<6)
			break;
		else 
		{
            label1.Text +="输入的起始点超出范围！请重新输入：";break;
			
		}
	}
    label1.Text +="\n";
	ShortestPath(arr,0,dist,path);      
    printPath(arr, 0, dist, path);         
    label1.Text += "\n";
    ShortestPath(arr, 1, dist, path); 
    printPath(arr, 1, dist, path);
    label1.Text += "\n";
    ShortestPath(arr, 2, dist, path);
    printPath(arr, 2, dist, path);
    label1.Text += "\n";
    ShortestPath(arr, 2, dist, path);
    printPath(arr, 2, dist, path);
    label1.Text += "\n";
    ShortestPath(arr, 3, dist, path);
    printPath(arr, 3, dist, path);
    label1.Text += "\n";
    ShortestPath(arr, 4, dist, path);
    printPath(arr, 4, dist, path);
    label1.Text += "\n";
    ShortestPath(arr, 5, dist, path);
    printPath(arr, 5, dist, path); 
        }
        void ShortestPath(int [,]arr,int v,int []dist,int []path)
{
	int n=6;        
	bool []S=new bool[n];       
	int i,j,k;
	int w,min;
	for(i=0;i<n;i++)
	{
		dist[i]=arr[v,i];            
		S[i]=false;
		if(i!=v&&dist[i]<maxValue)
			path[i]=v;
		else path[i]=-1;
	}

	S[v]=true;        
	dist[v]=0;

	for(i=0;i<n-1;i++)
	{
		min=maxValue;
		int u=v;         
		for(j=0;j<n;j++)
			if(S[j]==false&&dist[j]<min)
			{
				u=j;
				min=dist[j];
			}
		S[u]=true;            


		for(k=0;k<n;k++)    
		{
			w=arr[u,k];
			if(S[k]==false&&w<maxValue&&dist[u]+w<dist[k])
			{                              
				dist[k]=dist[u]+w;
				path[k]=u;           
			}
		}
	}
}


void printPath(int [,]arr,int v,int []dist,int []path)
{
	int i,j,k;
    const int n=6;                
	int []d=new int[n];
	for(i=0;i<n;i++)
		if(i!=v)
		{
			j=i;
			k=0;
			while(j!=v)
			{
				d[k++]=j;
				j=path[j];
			}
            label1.Text +=v.ToString()+"    "+i.ToString()+"    "+v.ToString()+"--->";
			while(k>1)
                label1.Text +=d[--k].ToString()+"--->";
            label1.Text +=d[--k].ToString();
            label1.Text += "       " + dist[i].ToString() + "\n";
		}
}
    }
}
