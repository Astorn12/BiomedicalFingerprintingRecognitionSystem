using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{
     class FiltratorMedianowy
    {
      
       

       public  Bitmap  filtrowanie(Bitmap bitmapaOrginalna )
        {
            int wielkoscObszaru  = 5;
            Bitmap b = (Bitmap)bitmapaOrginalna.Clone() ;
            
                for (int i = wielkoscObszaru / 2; i < b.Width - wielkoscObszaru / 2; i++)
                {
                    for (int j = wielkoscObszaru / 2; j < b.Height - wielkoscObszaru / 2; j++)
                    {
                        byte byt = medianaObszaru(getObszar(new Point(i, j), wielkoscObszaru, bitmapaOrginalna));
                        //Console.WriteLine(byt + " " + bitmapaOrginalna.GetPixel(i, j).R);
                        b.SetPixel(i, j, Color.FromArgb(byt, byt, byt));
                    }
                }
                //bitmapaOrginalna = (Bitmap)b.Clone();
            
            return b;
        }
        private  int[,] getObszar(Point srodek,int wielkoscObszaru,Bitmap bitmapaOrginalna)
        {
           
            int[,] area = new int[wielkoscObszaru, wielkoscObszaru];
            int a = 0;
            int b = 0;
            for(int i=srodek.X-wielkoscObszaru/2;i<=srodek.X+wielkoscObszaru/2;i++)
            {
                for (int j = srodek.Y - wielkoscObszaru / 2; j <= srodek.Y + wielkoscObszaru / 2; j++)
                {
                   
                    area[a, b] = bitmapaOrginalna.GetPixel(i, j).R;
                    a++;
                }
                b ++;
                a = 0;
            }
            
            return area;
        }


        private byte medianaObszaru(int[,] area)
        {
            List<int> l = new List<int>(); 
            for(int i=0;i<area.GetLength(0);i++)
            {
                for (int j = 0; j < area.GetLength(1); j++)
                {
                    
                    
                    int tmp = area[i, j];
                   
                    
                    int tmpx = Math.Abs(area.GetLength(0) / 2 - i);
                    int tmpy=Math.Abs(area.GetLength(0) / 2 - j);
                    if (tmpx==0&&tmpy==0)
                    {
                        l.Add(tmp);l.Add(tmp);l.Add(tmp);
                    }
                    else if(tmpx<2&&tmpy<2)
                    {
                         l.Add(tmp);l.Add(tmp);
                    }
                    else  l.Add(tmp);
                }
               
            }
             
             l.Sort();
            return (byte)l[(int)l.Count/2];
        }
    }
}
