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
        private Bitmap orginalBitmap { get; set; }// orginalna mapa bitowa
        private Bitmap alreadyPassed { get; set; }// mapa bitowa służąca do zapisywania które łuki zostały już pokonane
        private Bitmap training { get; set; }
        private Bitmap minutiasMatched { get; set; }
        int sectionLen;//długość sekcji przy czym jest to połowa tej długości
        int startingSectionLen;
        int startingPointsDistance;//odreślenie odległości międzi punktami dyskretnej siatki służącej do upewnienia się że każdy łuk został sprawdzony
        int dlugoscSkoku;
       // List<Tuple<Point,double>> minucje = new List<Tuple<Point,double>>();//to będzie sbiór ostatecznie znalezionych minucji
        List<Minutia> minucje = new List<Minutia>();
        GridedImage localDirectionMap;//mapa kiery=unków lokalnych przechowujące kierunek w jakik skierowany jest łuk, a dokładnie kąt i ką ten jest od 0 do 180 stopni
        ImageWindow okno;
        Color standard = Color.FromArgb(0, 0, 128);

        int limit = 200;//limit czarności
        bool inverted=false;
        
        public MinutiaeDetector(Bitmap orginalBitmap,int sectionLen,int startingPointsDistance,GridedImage localDirectionMap,int dlugoscSkoku)
        {
            this.sectionLen = sectionLen;
            this.startingPointsDistance = startingPointsDistance;
            this.orginalBitmap =  orginalBitmap;

            this.alreadyPassed =(Bitmap) orginalBitmap.Clone();
            this.training=(Bitmap) orginalBitmap.Clone();
            this.minutiasMatched=(Bitmap) orginalBitmap.Clone();
            this.localDirectionMap = localDirectionMap;
            this.dlugoscSkoku = dlugoscSkoku;
            this.startingSectionLen = 5;
           
        }
        public  List<Minutia> getMinutionsMap()
        {
            //tutaj trzeba zmienić na usuwanie tylko jendej z nich
            // getSectionPointsBitmap();
           // List<Tuple<Point, double>> tmpRemove = new List<Tuple<Point, double>>();
            List<Minutia> tmpRemove = new List<Minutia>();
           foreach(var p in minucje)
            {
                foreach (var t in minucje)
                {
                    if(p!=t&&pointsDistance(p.p,t.p)<5)
                    {
                        if(!tmpRemove.Contains(p)&&!tmpRemove.Contains(t))
                        tmpRemove.Add(p);
                       //tmpRemove.Add(t);
                    }
                }
            }
           foreach(var p in tmpRemove)
            {
                if(minucje.Contains(p))
                {
                    minucje.Remove(p);
                }
            }

            MinutiaeVeryfication(minucje,orginalBitmap);
            foreach(var p in minucje)
            {
               for(int i=p.p.X-2;i<p.p.X+2;i++)
                {
                    for (int j = p.p.Y - 2; j < p.p.Y + 2; j++)
                    {if(i<training.Width&&i>0&&j<training.Height&&j>0)
                        training.SetPixel(i, j, Color.Orange);
                    }
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
            
            angle = ImageSupporter.restrictAngle(angle);
            
            Point p = new Point((int)(c.X - Math.Sin(angle) * sectionLeng + 0.5), (int)(c.Y - Math.Cos(angle) * sectionLeng + 0.5));
            Point k = new Point((int)(c.X + Math.Sin(angle) * sectionLeng + 0.5), (int)(c.Y + Math.Cos(angle) * sectionLeng + 0.5));
            if (ImageSupporter.RadianToDegree(angle) > 80 && ImageSupporter.RadianToDegree(angle) < 100)
            {
                double ck = ImageSupporter.RadianToDegree(angle);
            }
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
         

            return section;
        }
        
        public void sortSection(Dictionary<Point,int> section)
        {
            var sortowanko = section.OrderBy(x => x.Key.X);// nie wiem czy to już posortowało czy section czy tylko do skoiowało do sortowanko
            
        }

        public void followingUp(Point m)
        {
            double angle = localDirectionMap.getDirectionForPixel(m.X, m.Y);
           
            Point n = new Point((int)(m.X + Math.Cos(angle) * dlugoscSkoku + 0.5), (int)(m.Y - Math.Sin(angle) * dlugoscSkoku + 0.5));

           

            if (n.X < training.Width && n.X > 0 && n.Y > 0 && n.Y < training.Height)
            {

                
              //  if (orginalBitmap.GetPixel(n.X, n.Y).R < limit && alreadyPassed.GetPixel(n.X, n.Y).B!=128 )
             //   {
                  
                    Dictionary<Point, int> sectionLine = getSectionPixels(n, orginalBitmap, startingSectionLen);

                    foreach (var item in sectionLine)
                    {
                        training.SetPixel(item.Key.X, item.Key.Y, Color.Red);

                    }
                    Point p = sectionMinimum(sectionLine, orginalBitmap);

                List<Point> points = ImageSupporter.GetLine(m, p);
                bool flag = true;
                foreach(var item in points)
                {
                    if(orginalBitmap.GetPixel(item.X, item.Y).R>limit)
                    {
                        flag = false;
                    }
                }
                


                if (/*orginalBitmap.GetPixel(p.X, p.Y).R< limit*/flag  && alreadyPassed.GetPixel(p.X, p.Y).B != 128)
                {
                    for (int a = p.X - 1; a < p.X + 2; a++)
                    {
                        for (int b = p.Y - 1; b < p.Y + 2; b++)
                        {
                           // if(a<training.Width&&a>0&&b<training.Height&&b>0)
                          //  training.SetPixel(a, b, Color.Blue);
                        }
                    }
                    ImageSupporter.matchLine2(alreadyPassed, m, n,standard , 5);
                    ImageSupporter.matchLine2(training, m, n, Color.Green, 1);
                    if (ImageSupporter.RadianToDegree(
                        Math.Abs(
                            localDirectionMap
                            .getDirectionForPixel(p.X, p.Y) - localDirectionMap.getDirectionForPixel(m.X, m.Y)))>100)
                    {
                         followingDown(p);
                    }
                    else
                       
                    followingUp(p);

                    
                }
                else if (alreadyPassed.GetPixel(p.X, p.Y).B == 128&&flag)
                {
                    List<Point> list = ImageSupporter.GetLine(m, n);


                    Point wsk = m;

                    foreach (Point pi in list)
                    {
                        wsk = pi;
                        int i = orginalBitmap.GetPixel(pi.X, pi.Y).R;
                        if (orginalBitmap.GetPixel(pi.X, pi.Y).B != 128)
                        {

                        }
                        else
                        {
                            break;
                        }
                    }

                 // minucje.Add(new Tuple<Point,double>(wsk, angle));
                    try
                    {
                        for (int a = wsk.X - 1; a < wsk.X + 2; a++)
                        {
                            for (int b = wsk.Y - 1; b < wsk.Y + 2; b++)
                            {
                                training.SetPixel(a, b, Color.Yellow);
                            }
                        }
                        //int i = orginalBitmap.GetPixel(wsk.X, wsk.Y).R;
                        //training.SetPixel(wsk.X, wsk.Y, Color.Orange);
                    }
                    catch (Exception ec)
                    {

                    }
                }
               // else if(orginalBitmap.GetPixel(p.X, p.Y).R >= limit)
               else// if( !flag&& alreadyPassed.GetPixel(p.X, p.Y).B != 128)
                {
                    // training.SetPixel(n.X, n.Y, Color.Orange);

                    List<Point> list = ImageSupporter.GetLine(m, n);


                    Point wsk = n;

                    foreach (Point pi in list)
                    {
                       
                       // int i = orginalBitmap.GetPixel(pi.X, pi.Y).R;
                        if (orginalBitmap.GetPixel(pi.X, pi.Y).R <limit )
                        {
                            //wsk = pi;
                        }
                        else
                        {
                            wsk = pi;
                            break;
                        }
                    }
                    Minutia newMinutia;
                    if(!inverted)
                    {
                        newMinutia = new Minutia(wsk, angle, KindOfMinutia.ZAKONCZENIE); 
                    }
                    else
                    {
                         newMinutia = new Minutia(wsk, angle, KindOfMinutia.ROZWIDLENIE); 
                    }
                    minucje.Add(newMinutia);
                    try
                    {
                        for (int a = wsk.X - 1; a < wsk.X + 2; a++)
                        {
                            for (int b = wsk.Y - 1; b < wsk.Y + 2; b++)
                            {
                               // training.SetPixel(a, b, Color.Orange);
                            }
                        }
                        
                    }
                    catch (Exception ec)
                    {

                    }
                }
               
               



            }


        }
        private void followingDown(Point m)
        {
            double angle = localDirectionMap.getDirectionForPixel(m.X, m.Y);
            angle = ImageSupporter.angletoIIIandIV(angle);
           
            Point n = new Point((int)(m.X - Math.Cos(angle) * dlugoscSkoku + 0.5), (int)(m.Y + Math.Sin(angle) * dlugoscSkoku + 0.5));

            
           
            if (n.X < training.Width && n.X > 0 && n.Y > 0 && n.Y < training.Height)
            {
             
                //if (orginalBitmap.GetPixel(n.X, n.Y).R < limit && alreadyPassed.GetPixel(n.X, n.Y).B!=128)
                //{
                    Dictionary<Point, int> sectionLine = getSectionPixels(n, orginalBitmap, startingSectionLen);

                    foreach (var item in sectionLine)
                    {
                        training.SetPixel(item.Key.X, item.Key.Y, Color.Red);

                    }
                    Point p = sectionMinimum(sectionLine, alreadyPassed);
                List<Point> points = ImageSupporter.GetLine(m, p);
                bool flag = true;
                foreach (var item in points)
                {
                    if (orginalBitmap.GetPixel(item.X, item.Y).R > limit)
                    {
                        flag = false;
                    }
                }



                if (/*orginalBitmap.GetPixel(p.X, p.Y).R< limit*/flag && alreadyPassed.GetPixel(p.X, p.Y).B != 128)
                
                    //if (orginalBitmap.GetPixel(p.X, p.Y).R < limit && alreadyPassed.GetPixel(p.X, p.Y).B != 128)
                    {
                    for (int a = p.X - 1; a < p.X + 2; a++)
                    {
                        for (int b = p.Y - 1; b < p.Y + 2; b++)
                        {
                            try
                            {
                              //   if(a<training.Width&&a>0&&b<training.Height&&b>0)
                               // training.SetPixel(a, b, Color.Blue);
                            }
                            catch(Exception e)
                            {

                            }
                        }
                    }
                    ImageSupporter.matchLine2(alreadyPassed, m, n, standard, 5);
                    ImageSupporter.matchLine2(training, m, n, Color.Pink, 1);
                    if (ImageSupporter.RadianToDegree(
                         Math.Abs(
                             localDirectionMap
                             .getDirectionForPixel(p.X, p.Y) - localDirectionMap.getDirectionForPixel(m.X, m.Y))) > 100)
                    {
                        followingUp(p);
                    }
                    else
                    followingDown(p);
                }
                else if(alreadyPassed.GetPixel(p.X, p.Y).B==128&&flag)
                {
                    List<Point> list = ImageSupporter.GetLine(m, n);


                    Point wsk = p;

                    foreach (Point pi in list)
                    {
                        wsk = pi;
                        int i = orginalBitmap.GetPixel(pi.X, pi.Y).R;
                        if (orginalBitmap.GetPixel(pi.X, pi.Y).B != 128)
                        {

                        }
                        else
                        {
                            break;
                        }
                    }
                   // minucje.Add(new Tuple<Point,double>(wsk, angle));
                    try
                    {
                        for (int a = wsk.X - 1; a < wsk.X + 2; a++)
                        {
                            for (int b = wsk.Y - 1; b < wsk.Y + 2; b++)
                            {
                                training.SetPixel(a, b, Color.Yellow);
                            }
                        }
                        //int i = orginalBitmap.GetPixel(wsk.X, wsk.Y).R;
                        //training.SetPixel(wsk.X, wsk.Y, Color.Orange);
                    }
                    catch (Exception ec)
                    {

                    }
                }
                //   else if(orginalBitmap.GetPixel(p.X, p.Y).R >= limit)
                else //if( !flag&& alreadyPassed.GetPixel(p.X, p.Y).B != 128)
                {
                    // training.SetPixel(n.X, n.Y, Color.Orange);

                    List<Point> list= ImageSupporter.GetLine(m,n);
                   
                  
                    Point wsk = n;
                    
                    foreach(Point pi in list)
                    {
                        
                       // int i = orginalBitmap.GetPixel(pi.X, pi.Y).R;
                        if(orginalBitmap.GetPixel(pi.X,pi.Y).R<limit)
                        {
                            
                        }
                        else
                        {
                            wsk = pi;
                            break;
                        }
                    }
                    Minutia newMinutia;
                    if (!inverted)
                    {
                        newMinutia = new Minutia(wsk, angle, KindOfMinutia.ZAKONCZENIE);
                    }
                    else
                    {
                        newMinutia = new Minutia(wsk, angle, KindOfMinutia.ROZWIDLENIE);
                    }
                    minucje.Add(newMinutia);
                    try
                    {
                        for (int a = wsk.X - 1; a < wsk.X + 2; a++)
                        {
                            for (int b = wsk.Y - 1; b < wsk.Y + 2; b++)
                            {
                               // training.SetPixel(a, b, Color.Orange);
                            }
                        }
                          //int i = orginalBitmap.GetPixel(wsk.X, wsk.Y).R;
                        //training.SetPixel(wsk.X, wsk.Y, Color.Orange);
                    }
                    catch (Exception ec)
                    {

                    }
                }

               



            }

        }
        public void startFollowing(Point m)
        {
            followingUp(m);
            followingDown(m);


    
        }

        public Bitmap GetImageWithMatchedMinutias()
        {
            getSectionPointsBitmap(orginalBitmap);
           
            this.alreadyPassed = (Bitmap)orginalBitmap.Clone();
           this.orginalBitmap = ImageSupporter.ReverseBitmap(orginalBitmap);
           this.training =(Bitmap) orginalBitmap.Clone();
            this.inverted = true;
             getSectionPointsBitmap(orginalBitmap);
            this.orginalBitmap=ImageSupporter.ReverseBitmap(orginalBitmap);
            getMinutionsMap();//tutaj jest sprawdzanie i usuwanie niewłaściwych minucji
            this.inverted = false;
            return MatchMunities();
          
        }
        private Bitmap MatchMunities()
        {
        
           Bitmap final = (Bitmap)orginalBitmap.Clone() ;
          // Bitmap final = (Bitmap)training.Clone() ;
     
            foreach (var p in minucje)
            {
                //  final.SetPixel(item.Item1.X, item.Item1.Y,Color.Orange);

                for (int i = p.p.X - 1; i < p.p.X + 1; i++)
                {
                    for (int j = p.p.Y - 1; j < p.p.Y + 1; j++)
                    {
                        if (i < training.Width && i > 0 && j < training.Height && j > 0)
                        {
                            if(p.kind.Equals(KindOfMinutia.ZAKONCZENIE))
                            final.SetPixel(i, j, Color.Orange);

                            else final.SetPixel(i, j, Color.Purple);
                        }
                    }
                }
            }
            return final;
        }
       
        public Bitmap GetImageWithFullMinutiesDetection()
        {


            return getSectionPointsBitmap(orginalBitmap);
        }

        public Bitmap getSectionPointsBitmap(Bitmap bit)
        {
            //Bitmap bitmapWithSectionPoints =(Bitmap) orginalBitmap.Clone();
            Bitmap bitmapWithSectionPoints =(Bitmap) bit.Clone();
           
            int x = 0;
            int y = 0;
          
            for (int j = bitmapWithSectionPoints.Height / startingPointsDistance; j > 0; j--)// tutaj trzeba uważać bo iterujemy po j,i ale też po x,y co prowadzi do tego że nie zawsze wszystko się ziteruje w zależności czy j czy i jest większe, większe musi być iterowane pierwsze
            {
                for (int i=0;i<bitmapWithSectionPoints.Width/startingPointsDistance;i++)
            {
                
                    x += startingPointsDistance;
                    if (x > 0 && x < bitmapWithSectionPoints.Width && y > 0 && y < bitmapWithSectionPoints.Height)
                    {
                       
                            Dictionary<Point, int> sectionLine = getSectionPixels(new Point(x, y), bit,sectionLen);
                    
                        
                        Point p = sectionMinimum(sectionLine,bitmapWithSectionPoints);
                     
                      
                      
                            if ((!(alreadyPassed.GetPixel(p.X, p.Y).B==128))&& bit.GetPixel(p.X,p.Y).R<100){
                            bitmapWithSectionPoints.SetPixel(p.X, p.Y, Color.Red);
                            for (int a = p.X - 1; a < p.X + 2; a++)
                            {
                                for (int b = p.Y - 1; b < p.Y + 2; b++)
                                {
                                    bitmapWithSectionPoints.SetPixel(a, b, Color.Red);
                                }
                            }
                            startFollowing(p);
                       
                        }
                    }
                }
                y += startingPointsDistance;
                x = 0;
            }
            //getMinutionsMap();
            return training;
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

        private double pointsDistance(Point p1,Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private void MinutiaeVeryfication(List<Minutia> minutions,Bitmap bitmap)
        {
           List<Minutia> removeList = new List<Minutia>();
            int[,] segmentationImage = Filtrator.BackgroundSegmentation(bitmap);
            int w = 5;
            foreach (var item in minutions)
            {
                Boolean flag = true;
                
               for(int i=item.p.X-w/2;i<item.p.X+w/2;i++)
                {
                    for (int j = item.p.Y - w / 2; j < item.p.Y + w / 2; j++)
                    {
                        if(i<bitmap.Width&&i>0&&j<bitmap.Height&&j>0)
                        {
                            if(segmentationImage[i,j]==0)
                            {
                                flag = false;
                            }
                        }
                    }
                }
                if (flag == false) removeList.Add(item);
            }

            foreach(var item in removeList)
            {
                minutions.Remove(item);
            }

            removeList.Clear();
            
        }

    }
}
