using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{
    class MinutiaeDetector
    {
        public Bitmap orginalBitmap; // orginalna mapa bitowa
        public Bitmap alreadyPassed { get; set; }// mapa bitowa służąca do zapisywania które łuki zostały już pokonane
        int sectionLen;//długość sekcji przy czym jest to połowa tej długości
        int startingSectionLen;
        int startingPointsDistance;//odreślenie odległości międzi punktami dyskretnej siatki służącej do upewnienia się że każdy łuk został sprawdzony
        int dlugoscSkoku;
        List<System.Drawing.Point> minucje = new List<System.Drawing.Point>();//to będzie sbiór ostatecznie znalezionych minucji
        GridedImage localDirectionMap;//mapa kiery=unków lokalnych przechowujące kierunek w jakik skierowany jest łuk, a dokładnie kąt i ką ten jest od 0 do 180 stopni
        public MinutiaeDetector(Bitmap orginalBitmap,int sectionLen,int startingPointsDistance,GridedImage localDirectionMap,int dlugoscSkoku)
        {
            this.sectionLen = sectionLen;
            this.startingPointsDistance = startingPointsDistance;
            this.orginalBitmap = orginalBitmap;
            //this.alreadyPassed = new Bitmap(orginalBitmap.Width, orginalBitmap.Height);
            this.alreadyPassed =(Bitmap) orginalBitmap.Clone();

            this.localDirectionMap = localDirectionMap;
            this.dlugoscSkoku = dlugoscSkoku;
            this.startingSectionLen = 5;
        }
        public  List<System.Drawing.Point> getMinutionsMap()
        {
            for(int i=0;i<orginalBitmap.Width/startingPointsDistance;i++)
            {
                for (int j = 0; j < orginalBitmap.Width / startingPointsDistance;j++)
                {
                    Point startingPoint = getMinumumPoint(new Point(i, j));
                }
            }

            return this.minucje;
        }

        public Point  getMinumumPoint(Point c)
        {
            double angle = localDirectionMap.getDirectionForPixel(c.X, c.Y);
            return new Point(0, 0);
        }


        public Dictionary<Point, int> getSectionPixels(Point c,Bitmap orginal,int sectionLeng)
        {
            double angle = localDirectionMap.getDirectionForPixel(c.X, c.Y);
            //angle += ImageSupporter.DegreeToRadian(90);
            angle = ImageSupporter.restrictAngle(angle);

            Point p = new Point((int)(c.X - Math.Cos(angle) * sectionLeng + 0.5), (int)(c.Y - Math.Sin(angle) * sectionLeng + 0.5));
            Point k = new Point((int)(c.X + Math.Cos(angle) * sectionLeng + 0.5), (int)(c.Y + Math.Sin(angle) * sectionLeng + 0.5));
            Dictionary<Point, int> section = new Dictionary<Point, int>();
            foreach(var item in ImageSupporter.EnumerateLineNoDiagonalSteps(p.X,p.Y,k.X,k.Y))
            {
                try
                {
                    section.Add(new Point(item.Item1, item.Item2), orginal.GetPixel(item.Item1, item.Item2).R);
                }
                catch (Exception e)
                {

                }
            }
           /* List<Point> sectionOne = new List<Point>();
            Bitmap bitmap1;//= new Bitmap(1, 1);
            if(ImageSupporter.RadianToDegree(angle)>80 && ImageSupporter.RadianToDegree(angle)<100)
            {
                int x = 0;
            }
            if (Math.Abs(p.Y - k.Y) > 0 && Math.Abs(p.X - k.X)>0)
            {
               bitmap1 = new Bitmap(Math.Abs(p.X - k.X), Math.Abs(p.Y - k.Y));
            }
            else if(Math.Abs(p.Y - k.Y) == 0 && Math.Abs(p.X - k.X)==0)
            {
                bitmap1 = new Bitmap(1, 1);
            }
            else if(Math.Abs(p.Y - k.Y) == 0 )
            {
                bitmap1 = new Bitmap(Math.Abs(p.X - k.X),1);
            }
            else
            {
                bitmap1 = new Bitmap(1, Math.Abs(p.Y - k.Y));
            }
            int xprzesuniecie = p.X < k.X ? p.X : k.X;
            int yprzesuniecie = p.Y < k.Y ? p.Y : k.Y;




            double a = Math.Tan(angle);
            if (ImageSupporter.RadianToDegree(angle) > 45 && ImageSupporter.RadianToDegree(angle) <= 90)
            {
                a = Math.Tan(ImageSupporter.DegreeToRadian(90) - angle);

                for (int j = 0; j < bitmap1.Width; j++)
                {
                    for (int i = 0; i < bitmap1.Height; i++)
                    {


                        if ((int)((j - (bitmap1.Width) / 2) + 0.5) == (int)((-a * (i - (bitmap1.Height) / 2) + 0.5)))
                        {
                           
                           sectionOne.Add(new Point(j, i));
                        }
                       
                    }
                }
            }
            else if (ImageSupporter.RadianToDegree(angle) < 135 && ImageSupporter.RadianToDegree(angle) > 90)
            {
                a = Math.Tan(ImageSupporter.DegreeToRadian(270) - angle);
                for (int j = 0; j < bitmap1.Width; j++)
                {
                    for (int i = 0; i < bitmap1.Height; i++)
                    {


                        if ((int)((j - (bitmap1.Width ) / 2) + 0.5) == (int)((-a * (i - (bitmap1.Height) / 2) + 0.5)))
                        {
                            sectionOne.Add(new Point(j, i));
                        }
                       
                    }
                }
            }

            else
            {
                for (int j = 0; j < bitmap1.Height; j++)
                {
                    for (int i = 0; i < bitmap1.Width; i++)
                    {
                        if ((int)((j - (bitmap1.Height ) / 2) + 0.5) == (int)((-a * (i - (bitmap1.Width) / 2) + 0.5)))
                        {
                             sectionOne.Add(new Point(i, j));
                        }
                       
                    }
                }
            }


            foreach(Point n in sectionOne)
            {
                if (n.Y + yprzesuniecie < orginal.Height&&n.Y + yprzesuniecie >0&&n.X + xprzesuniecie>0&&n.X + xprzesuniecie<orginal.Width)
                {
                    section.Add(new Point(n.X + xprzesuniecie, n.Y + yprzesuniecie ), orginal.GetPixel(n.X + xprzesuniecie, n.Y + yprzesuniecie ).R);
                }
            }




            bitmap1.Dispose();*/

            return section;
        }
        
        public void sortSection(Dictionary<Point,int> section)
        {
            var sortowanko = section.OrderBy(x => x.Key.X);// nie wiem czy to już posortowało czy section czy tylko do skoiowało do sortowanko
            
        }

        public void startFollowing(Point m, Bitmap training)
        {
            double angle = localDirectionMap.getDirectionForPixel(m.X, m.Y);
         
            Point n = new Point((int)(m.X + Math.Cos(angle) * dlugoscSkoku + 0.5), (int)(m.Y - Math.Sin(angle) * dlugoscSkoku + 0.5));

            ImageSupporter.matchLine2(alreadyPassed, m, n, Color.Blue, 4);
            
            if (n.X < training.Width && n.X > 0 && n.Y > 0 && n.Y < training.Height)
            {
                Console.WriteLine(alreadyPassed.GetPixel(n.X, n.Y).R);
                if (training.GetPixel(n.X, n.Y).R < 100 && alreadyPassed.GetPixel(n.X, n.Y) !=Color.Blue)
                {
                    Dictionary<Point, int> sectionLine = getSectionPixels(n, training, startingSectionLen);

                    foreach (var item in sectionLine)
                    {
                        training.SetPixel(item.Key.X, item.Key.Y, Color.Red);

                    }
                    Point p = sectionMinimum(sectionLine, alreadyPassed);
                    for (int a = p.X - 1; a < p.X + 2; a++)
                    {
                        for (int b = p.Y - 1; b < p.Y + 2; b++)
                        {
                            training.SetPixel(a, b, Color.Blue);
                        }
                    }
                    startFollowing(p, training);
                }
                else
                {
                   
                    try
                    {
                        for (int a = n.X - 1; a < n.X + 2; a++)
                        {
                            for (int b = n.Y - 1; b < n.Y + 2; b++)
                            {
                                training.SetPixel(a, b, Color.Orange);
                            }
                        }
                    }
                    catch (Exception ec)
                    {

                    }
                }


            }

     



        }

        public void followingUp(Point m,Bitmap training)
        {
            double angle = localDirectionMap.getDirectionForPixel(m.X, m.Y);
            //  if (angle > ImageSupporter.DegreeToRadian(110) && angle < ImageSupporter.DegreeToRadian(160)) { 
            Point n = new Point((int)(m.X + Math.Cos(angle) * dlugoscSkoku + 0.5), (int)(m.Y - Math.Sin(angle) * dlugoscSkoku + 0.5));

            ImageSupporter.matchLine2(alreadyPassed, m, n, Color.Blue, 4);
            //ImageSupporter.matchLine1(alreadyPassed, new Point(0,300), , Color.Blue, 1);
            if (n.X < training.Width && n.X > 0 && n.Y > 0 && n.Y < training.Height)
            {
                Console.WriteLine(alreadyPassed.GetPixel(n.X, n.Y).R);
                if (training.GetPixel(n.X, n.Y).R < 100 && alreadyPassed.GetPixel(n.X, n.Y) != Color.Blue)
                {
                    Dictionary<Point, int> sectionLine = getSectionPixels(n, training, startingSectionLen);

                    foreach (var item in sectionLine)
                    {
                        training.SetPixel(item.Key.X, item.Key.Y, Color.Red);

                    }
                    Point p = sectionMinimum(sectionLine, alreadyPassed);
                    for (int a = p.X - 1; a < p.X + 2; a++)
                    {
                        for (int b = p.Y - 1; b < p.Y + 2; b++)
                        {
                            training.SetPixel(a, b, Color.Blue);
                        }
                    }
                    startFollowing(p, training);
                }
                else
                {
                    // training.SetPixel(n.X, n.Y, Color.Orange);
                    try
                    {
                        for (int a = n.X - 1; a < n.X + 2; a++)
                        {
                            for (int b = n.Y - 1; b < n.Y + 2; b++)
                            {
                                training.SetPixel(a, b, Color.Orange);
                            }
                        }
                    }
                    catch (Exception ec)
                    {

                    }
                }


              
            }

           
        }

        public Bitmap getSectionPointsBitmap()
        {
            Bitmap bitmapWithSectionPoints =(Bitmap) orginalBitmap.Clone();
            int x = 0;
            int y = 0;
            for (int j = bitmapWithSectionPoints.Height / startingPointsDistance; j > 0; j--)// tutaj trzeba uważać bo iterujemy po j,i ale też po x,y co prowadzi do tego że nie zawsze wszystko się ziteruje w zależności czy j czy i jest większe, większe musi być iterowane pierwsze
            {
                for (int i=0;i<bitmapWithSectionPoints.Width/startingPointsDistance;i++)
            {
                
                    x += startingPointsDistance;
                    if (x > 0 && x < bitmapWithSectionPoints.Width && y > 0 && y < bitmapWithSectionPoints.Height)
                    {
                      // if (i==14 && j == 18)//14,18
                      // {
                           // bitmapWithSectionPoints.SetPixel(x, y, Color.Green);
                       
                        /*for(int a =x-1;a<x+2;a++)
                        {
                            for (int b = y - 1; b < y + 2; b++)
                            {
                                bitmapWithSectionPoints.SetPixel(a, b, Color.Red);
                            }
                        }*/
                        
                        Dictionary<Point, int> sectionLine = getSectionPixels(new Point(x, y), orginalBitmap,sectionLen);
                    
                        foreach (var item in sectionLine)
                        {
                          // bitmapWithSectionPoints.SetPixel(item.Key.X,item.Key.Y, Color.Red);
                           
                        }
                        Point p = sectionMinimum(sectionLine,bitmapWithSectionPoints);
                       // bitmapWithSectionPoints.SetPixel(p.X,p.Y, Color.Blue);
                        for (int a = p.X - 1; a < p.X + 2; a++)
                        {
                            for (int b = p.Y - 1; b < p.Y + 2; b++)
                            {
                                //bitmapWithSectionPoints.SetPixel(a, b, Color.Blue);
                            }
                        }
                        startFollowing(p,bitmapWithSectionPoints);
                    // }
                    }
                }
                y += startingPointsDistance;
                x = 0;
            }
            return bitmapWithSectionPoints;
        }

        public Point sectionMinimum(Dictionary<Point,int> section,Bitmap onBitmap)//metoda używana zarówno do znalezienia punktów startowych jad do podążania za łukami
        {
            double[] gaussianMask = { 1.0/24.0,2.0/24.0, 3.0 / 24.0, 4.0 / 24.0, 4.0 / 24.0, 4.0 / 24.0, 3.0 / 24.0, 2.0 / 24.0,1.0/24.0 };
            Point wsk =new Point(0,0);
            int color=255;
            List<ColorPoint> cpList = new List<ColorPoint>();
            List<ColorPoint> cpListAC = new List<ColorPoint>();//lista po splocie z maska
            
            foreach(var item in section)
            {
                ColorPoint cp= new ColorPoint(item.Value, item.Key);
                cpList.Add(cp);

            }
                for(int i=4;i<cpList.Count-4;i++)
                {
                    double nPV = 0;// nowa wartość pixela
                int it = 0;
                    for(int a=i-4;a<i+4;a++)
                    {
                    nPV += gaussianMask[it] * (float)cpList[a].color;
                        it++;
                    }
                cpListAC.Add(new ColorPoint((int)nPV, cpList[i].point));

                }
              
              foreach(var item in cpListAC)
            {
                if (color > item.color)
                {
                    color = item.color;
                    wsk = item.point;
                }
            }
         
            return wsk;
        }

        public Bitmap getAlreadyPassed()
        {
            return this.alreadyPassed;
        }

    }
}
