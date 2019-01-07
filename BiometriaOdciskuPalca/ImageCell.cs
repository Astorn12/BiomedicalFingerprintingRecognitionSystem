using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{


    class ImageCell
    {
        #region Maski             
        //w tym do metody kompasowej
        double[,] maskV = new double[3, 3];// maska dla x ale stransponowana wię maska dla y maska 90 stopni
        double[,] maskH = new double[3, 3]; // maska dla y ale stransponowana woiec maska dla x maska 0 stopni
        double[,] mask45 = new double[3, 3]; // maska 45 stopni
        double[,] mask135 = new double[3, 3]; // maska 135 stopni
    #endregion
        #region Atributes
        int size;
        public Bitmap bitmap;
        private Bitmap bitmap2;//WERSJA2
        double angle { get; set; }
        #endregion

        #region Construktors
        public ImageCell(int size)
        {
            this.size = size;
            this.bitmap = new Bitmap(size, size);
            maskInitialization();
        }

        public ImageCell(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            this.size = bitmap.Width;
            maskInitialization();
            bitmap2 = new Bitmap(size, size);
        }
        #endregion

        #region Methods
        private void maskInitialization()
        {
            maskV[0, 0] = -1;
            maskV[0, 1] = 0;
            maskV[0, 2] = 1;
            maskV[1, 0] = -Math.Sqrt(2);
            maskV[1, 1] = 0;
            maskV[1, 2] = Math.Sqrt(2);
            maskV[2, 0] = -1;
            maskV[2, 1] = 0;
            maskV[2, 2] = 1;

            maskH[0, 0] = -1;
            maskH[0, 1] = -Math.Sqrt(2);
            maskH[0, 2] = -1;
            maskH[1, 0] = 0;
            maskH[1, 1] = 0;
            maskH[1, 2] = 0;
            maskH[2, 0] = 1;
            maskH[2, 1] = Math.Sqrt(2);
            maskH[2, 2] = 1;

            /*mask45[0, 0] = 0;
            mask45[1, 0] = 1;
            mask45[2, 0] = Math.Sqrt(2);
            mask45[0, 1] = -1;
            mask45[1, 1] = 0;
            mask45[2, 1] = 1;
            mask45[0, 2] = -Math.Sqrt(2);
            mask45[1, 2] = -1;
            mask45[2, 2] = 0;

            mask135[0, 0] = Math.Sqrt(2);
            mask135[1, 0] = 1;
            mask135[2, 0] = 0;
            mask135[0, 1] = 1;
            mask135[1, 1] = 0;
            mask135[2, 1] = -1;
            mask135[0, 2] = 0;
            mask135[1, 2] = -1;
            mask135[2, 2] = -Math.Sqrt(2);*/
        }
       

        public double getAngle()
        {
            return angle;
        }

        public void setBorder(Color color)
        {
            for (int i = 0; i < size; i++)
            {
                bitmap.SetPixel(i, 0, color);
                bitmap.SetPixel(0, i, color);
                bitmap.SetPixel(i, size - 1, color);
                bitmap.SetPixel(size - 1, i, color);
            }

        }
        public void maska()
        {
            //Maski Frei'a-Chen'a
            Wektor[,] angle1 = new Wektor[bitmap.Width - 2, bitmap.Height - 2];

            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    double[,] otoczenie = new double[3, 3] { { bitmap.GetPixel(i - 1, j - 1).R, bitmap.GetPixel(i-1, j ).R, bitmap.GetPixel(i -1, j + 1).R }, { bitmap.GetPixel(i , j-1).R, bitmap.GetPixel(i, j).R, bitmap.GetPixel(i, j+1).R }, { bitmap.GetPixel(i  +1, j-1).R, bitmap.GetPixel(i+1, j).R, bitmap.GetPixel(i + 1, j + 1).R } };
                    Wektor wektor = getWektor(otoczenie);
                    angle1[i-1, j-1] = wektor;
                }
            }
            double A = 0;
            double B = 0;
            double C = 0;

            for(int i=0;i<Math.Sqrt(angle1.Length);i++)
            {
                for (int j = 0; j < Math.Sqrt(angle1.Length); j++)
                {
                    A += angle1[i, j].x*angle1[i,j].y;
                    B += Math.Pow(angle1[i, j].x, 2);
                    C += Math.Pow(angle1[i, j].y, 2);
                }
            }
            this.angle = DegreeToRadian(90) + Math.Atan2(2 * A, B - C)/2;
            this.angle += DegreeToRadian(90);
            this.angle=restrictAngle(angle);
            /*Wektor D = new Wektor(0, 0);
            for (int i = 0; i < Math.Sqrt(angle.Length)-1; i++)
            {
                for (int j = 0; j < Math.Sqrt(angle.Length)-1; j++)
                {
                    D.x += angle[i, j].x;
                    D.y += angle[i, j].y;
                }
            }
            D.x = D.x / angle.Length;
            D.y= D.y / angle.Length;*/


            //double angleD = wektorToAngle(D);

           // Console.WriteLine("Kąt obszarowy= "+ RadianToDegree(this.angle));
          //  this.angle = DegreeToRadian( angleD);
            //Console.WriteLine(this.angle);

        }
        public Wektor getWektor(double[,] otoczenie)
        {
            return getWektorP(otoczenie);
            /*double deltaX = 0;
            double deltaY = 0;
            if ((otoczenie.Length == maskV.Length) && (maskV.Length == maskH.Length))
            {
                for (int i = 0; i < Math.Sqrt(otoczenie.Length); i++)
                {
                    for (int j = 0; j < Math.Sqrt(otoczenie.Length); j++)
                    {
                        deltaX += otoczenie[i, j] * maskV[i, j];
                        deltaY += otoczenie[i, j] * maskH[i, j];
                    }
                }
            }

            double Q = Math.Atan(deltaY / deltaX);
            Q = RadianToDegree(Q);
            Q += 90;
            if (Q < 0) Q += 360;
            if (Q > 180) Q -= 180;
            Q = DegreeToRadian(Q);
            // Q += 22.5;
            double r = Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2);
            Wektor d = new Wektor(r * Math.Cos( Q), r * Math.Sin( Q));
            Console.WriteLine("Kąt: "+d.ToString()+" i moc: " +r);
            return d;*/

        }

        public Wektor getWektorP1(double[,] otoczenie)
        {
            double deltaX = 0;
            double deltaY = 0;
            if ((otoczenie.Length == maskV.Length) && (maskV.Length == maskH.Length))
            {
                for (int i = 0; i < Math.Sqrt(otoczenie.Length); i++)
                {
                    for (int j = 0; j < Math.Sqrt(otoczenie.Length); j++)
                    {
                        deltaX += otoczenie[i, j] * maskV[i, j];
                        deltaY += otoczenie[i, j] * maskH[i, j];
                    }
                }
            }

            double Q = Math.Atan(deltaY / deltaX);
            Q = RadianToDegree(Q);
            Q += 90;
            if (Q < 0) Q += 180;
            if (Q > 180) Q -= 180;
            Q = DegreeToRadian(Q);
            // Q += 22.5;
            double r = Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2);
            Wektor d = new Wektor(r * Math.Cos(Q), r * Math.Sin(Q));
            Console.WriteLine("Kąt: " + d.ToString() + " i moc: " + r);
            return d;
        }
        public Wektor getWektorP(double[,] otoczenie)
        {
            double deltaX = 0;
            double deltaY = 0;

           // if ((otoczenie.Length == maskV.Length) && (maskV.Length == maskH.Length))
           // {
                for (int i = 0; i < Math.Sqrt(otoczenie.Length); i++)
                {
                    for (int j = 0; j < Math.Sqrt(otoczenie.Length); j++)
                    {
                        deltaX += otoczenie[i, j] * maskV[i, j];
                        deltaY += otoczenie[i, j] * maskH[i, j];
                    }
                }
        //    }
            return new Wektor(deltaX, deltaY);
            /*double Q = Math.Atan(deltaY / deltaX);
            Q = RadianToDegree(Q);
            Q += 90;
            if (Q < 0) Q += 180;
            if (Q > 180) Q -= 180;
            Q = DegreeToRadian(Q);
            // Q += 22.5;
            double r = Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2);
            Wektor d = new Wektor(r * Math.Cos(Q), r * Math.Sin(Q));
            Console.WriteLine("Kąt: " + d.ToString() + " i moc: " + r);
            return d;*/
        }

        /*Szukanie wektoru kierunkowego metodą kompasową*/
       
        
        
        

        public double wektorToAngle(Wektor wektor)//zmienia wektor na kąt
        {
            double angle = Math.Atan(wektor.y / wektor.x);

            angle = RadianToDegree(angle);
           //if (angle < 0) angle += 180;
          // if (angle > 180) angle -= 180;
            return angle ;

        }
        public void toDirectionMap()
        {
            maska();
            //if (this.angle.Equals(null)) maska();
           // this.angle = 45;
           // Console.WriteLine(angle);
           //this.angle = DegreeToRadian(0);
            /////////////////////double y = bitmap.Width / 2 * Math.Tan(angle);
            //double a = y / bitmap.Width;

            double y = 0;
            //if (angle == DegreeToRadian(90)) angle = DegreeToRadian(88);

            y = bitmap.Width / 2 * Math.Tan(angle);
            double a = Math.Tan(angle);
           // Console.WriteLine("Kąt obszarowy: "+RadianToDegree(angle));
            if (RadianToDegree(angle) > 45 && RadianToDegree(angle) <=90)
            {
                 a = Math.Tan(DegreeToRadian(90)- angle);
               
                for (int j = 0; j < bitmap.Height; j++)
                {
                    for (int i = 0; i < bitmap.Width; i++)
                    {
                       
                      
                        if ((int)((j - (bitmap.Width + 1) / 2) + 0.5) == (int)((-a * (i - (bitmap.Width) / 2) + 0.5)))
                        {
                            bitmap.SetPixel(j, i, Color.Yellow);
                           // bitmap2.SetPixel(j, i, Color.Black);
                        }
                       // else bitmap.SetPixel(j, i, Color.White);
                    }
                }
            }
           else if (RadianToDegree(angle) <135 && RadianToDegree(angle) > 90)
            { 
                a = Math.Tan(DegreeToRadian(270) - angle);
                for (int j = 0; j < bitmap.Height; j++)
                {
                    for (int i = 0; i < bitmap.Width; i++)
                    {


                        if ((int)((j - (bitmap.Width + 1) / 2) + 0.5) == (int)((-a * (i - (bitmap.Width) / 2) + 0.5)))
                        {//
                            bitmap.SetPixel(j, i, Color.Yellow);
                           // bitmap2.SetPixel(j, i, Color.Black);

                        }
                        //else bitmap.SetPixel(j, i, Color.White);
                    }
                }
            }


            else
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    for (int i = 0; i < bitmap.Width; i++)
                    {
                        if ((int)((j - (bitmap.Width + 1) / 2) + 0.5) == (int)((-a * (i - (bitmap.Width) / 2) + 0.5)))
                        {
                            bitmap.SetPixel(i, j, Color.Yellow);
                           // bitmap2.SetPixel(i, j, Color.Black);
                        }
                        //else bitmap.SetPixel(i, j, Color.White);
                    }
                }
            }

         //   bitmap = bitmap2;//WERJSJA2
        }//zmienia bitmapę na jeden kwadracik z ustalonum kierunkiem

        public double restrictAngle(double an)
        {
            if (an > DegreeToRadian(180)) an -= DegreeToRadian(180);
            if(an>DegreeToRadian(45)&&an<DegreeToRadian(90))
            { Console.Write(RadianToDegree(an));
               // an = DegreeToRadian(135) - an;
                Console.WriteLine(" " + RadianToDegree(an));
              //  an = 0;
            }
           
            return an;
        }
        public Bitmap toDirectionMapTest()
        {
            maska();
           
            Bitmap bitmap1 = (Bitmap)bitmap.Clone();
            double y = 0;
             y = bitmap1.Width / 2 * Math.Tan(angle);
            double a = Math.Tan(angle);
            if (RadianToDegree(angle) > 45 && RadianToDegree(angle) <= 90)
            {
                a = Math.Tan(DegreeToRadian(90) - angle);

                for (int j = 0; j < bitmap1.Height; j++)
                {
                    for (int i = 0; i < bitmap1.Width; i++)
                    {


                        if ((int)((j - (bitmap1.Width + 1) / 2) + 0.5) == (int)((-a * (i - (bitmap1.Width) / 2) + 0.5)))
                        {
                            bitmap1.SetPixel(j, i, Color.Black);
                        }
                        else bitmap1.SetPixel(j, i, Color.White);
                    }
                }
            }
            else if (RadianToDegree(angle) < 135 && RadianToDegree(angle) > 90)
            {
                a = Math.Tan(DegreeToRadian(270) - angle);
                for (int j = 0; j < bitmap1.Height; j++)
                {
                    for (int i = 0; i < bitmap1.Width; i++)
                    {


                        if ((int)((j - (bitmap1.Width + 1) / 2) + 0.5) == (int)((-a * (i - (bitmap1.Width) / 2) + 0.5)))
                        {
                            bitmap1.SetPixel(j, i, Color.Black);
                        }
                        else bitmap1.SetPixel(j, i, Color.White);
                    }
                }
            }

            else
            {
                for (int j = 0; j < bitmap1.Width; j++)
                {
                    for (int i = 0; i < bitmap1.Width; i++)
                    {
                        if ((int)((j - (bitmap1.Width + 1) / 2) + 0.5) == (int)((-a * (i - (bitmap1.Width) / 2) + 0.5)))
                        {
                            bitmap1.SetPixel(i, j, Color.Black);
                        }
                        else bitmap1.SetPixel(i, j, Color.White);
                    }
                }
            }
            return bitmap1;
        }
        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        
        #endregion
        #region InnerClasses
        public class Wektor
        {
            public double x { get; set; }
            public double y { get; set; }
            public Wektor()
            {
            }

            public Wektor(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
            public double getLength()
            {
                return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            }
            public override string ToString()
            {
                return this.x + "  " + this.y;
            }
        }
        #endregion

        #region MetodyTestowe
        public Bitmap getRandomThreeToThree()
        {
            Random random = new Random();
            int ii = random.Next(bitmap.Width - 2) + 1;
            int a = 0;
            int b = 0;

            Bitmap threetothree = new Bitmap(3, 3);
            for (int i = ii-1; i < ii+2;i++)
            {
                
                for (int j = ii-1; j < ii+2; j++)
                {
                     threetothree.SetPixel(a, b, bitmap.GetPixel(i, j));
                    b++;
                }
                a++;
                b = 0;

            }
           /* int wariancja = 0;
            for (int i =0; i < threetothree.Width; i++)
            {
                for (int j = 0; j < threetothree.Width; j++)
                {

                    wariancja += threetothree.GetPixel(i, j).R;
                }
            }*/

            return threetothree;
        }

        public Wektor getWektor(Bitmap kwadracik)
        {
            double[,] otoczenie = new double[3, 3];
            otoczenie[0,0]= kwadracik.GetPixel(0, 0).R;
            otoczenie[1,0]= kwadracik.GetPixel(1, 0).R;
            otoczenie[2,0]= kwadracik.GetPixel(2, 0).R;
            otoczenie[0,1]= kwadracik.GetPixel(0, 1).R;
            otoczenie[1,1]= kwadracik.GetPixel(1, 1).R;
            otoczenie[2,1]= kwadracik.GetPixel(2, 1).R;
            otoczenie[0,2]= kwadracik.GetPixel(0, 2).R;
            otoczenie[1,2]= kwadracik.GetPixel(1, 2).R;
            otoczenie[2,2]= kwadracik.GetPixel(2, 2).R;

            return getWektorP(otoczenie);
            /*double deltaX = 0;
            double deltaY = 0;
            if ((otoczenie.Length == maskV.Length) && (maskV.Length == maskH.Length))
            {
                for (int i = 0; i < Math.Sqrt(otoczenie.Length); i++)
                {
                    for (int j = 0; j < Math.Sqrt(otoczenie.Length); j++)
                    {
                        deltaX += otoczenie[i, j] * maskH[i, j];
                        deltaY += otoczenie[i, j] * maskV[i, j];
                    }
                }
            }

            double Q =/* (180 / Math.PI) **/ /*Math.Atan(deltaY / deltaX);

            Q = RadianToDegree(Q);
            Q += 90;
            if ( Q< 0) Q += 360;
            if (Q > 180) Q -= 180;
            Q = DegreeToRadian(Q);
            //Q += 22.5;
            double r = Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2);
            Wektor d = new Wektor(r * Math.Cos( Q), r * Math.Sin(Q));
               Console.WriteLine("Kąt: "+wektorToAngle( d)+" i moc: " +r);
            return d;*/

        }
    
        public Bitmap getRandomPixel()
        {
            Random random = new Random();
            Bitmap alone = new Bitmap(1,1);
            int i = random.Next(bitmap.Width);
            int j = random.Next(bitmap.Height);
            alone.SetPixel(0, 0, bitmap.GetPixel(i,j));
            return alone;
        }
        public Bitmap getRandomTwo()
        {
            Random random = new Random();
            Bitmap alone = new Bitmap(1, 2);
            int i = random.Next(bitmap.Width-1);
            int j = random.Next(bitmap.Height);
           // alone.SetPixel(0, 0, bitmap.GetPixel(i, j));
            alone.SetPixel(0, 0, Color.White);

            //alone.SetPixel(0, 1, bitmap.GetPixel(i+1, j));
            alone.SetPixel(0, 1,Color.Black);

            return alone;
        }
        public Wektor getWektorCompas(double[,] otoczenie)
        {
            double deltaX = sprezenie(otoczenie, maskV);
            double deltaY = sprezenie(otoczenie, maskH);

            double r = Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2);

            double delta45 = sprezenie(otoczenie, mask45);
            double delta135 = sprezenie(otoczenie, mask135);
            this.angle = chooseOrtogonalAngleCompas(deltaX, deltaY, delta45, delta135);
            return new Wektor(r * Math.Cos(angle), r * Math.Sin(angle));

        }

        public double chooseOrtogonalAngleCompas(double deltaX, double deltaY, double delta45, double delta135)
        {
            double absoluteX = Math.Abs(deltaX);
            double absoluteY = Math.Abs(deltaY);
            double absolute45 = Math.Abs(delta45);
            double absolute135 = Math.Abs(delta135);
            if (absoluteX > absoluteY && absoluteX > absolute45 && absoluteX > absolute135)
            {
                //if (absoluteX == deltaX)
                // return DegreeToRadian(0);
                //  else return DegreeToRadian(0);
                if (absoluteX == deltaX)
                    return DegreeToRadian(90);
                else return DegreeToRadian(0);
            }


            else if (absoluteY > absoluteX && absoluteY > absolute45 && absoluteY > absolute135)
            {
                // if (absoluteY == deltaY)
                //return DegreeToRadian(90);
                //else return DegreeToRadian(90);
                if (absoluteY == deltaY)
                    return DegreeToRadian(0);
                else return DegreeToRadian(90);
            }

            else if (absolute135 > absoluteX && absolute135 > absolute45 && absolute135 > absoluteY)
            {
                //if (absolute135 == delta135)
                //   return DegreeToRadian(135);
                // else return DegreeToRadian(45);
                if (absolute135 == delta135)
                    return DegreeToRadian(45);
                else return DegreeToRadian(45);
            }

            else //if (absolute45 > deltaX && absolute45 > delta135 && absolute45 > deltaY)
            {
                // if (absolute45 == delta45)
                //  return DegreeToRadian(45);
                //  else return DegreeToRadian(135);
                if (absolute45 == delta45)
                    return DegreeToRadian(135);
                else return DegreeToRadian(135);

            }
        }



        public double sprezenie(double[,] otoczenie, double[,] maska)
        {
            double sigmaSprezenia = 0;
            for (int i = 0; i < Math.Sqrt(otoczenie.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(otoczenie.Length); j++)
                {
                    sigmaSprezenia += otoczenie[i, j] * maska[i, j];

                }
            }
            return sigmaSprezenia;
        }

        
        public void GaborFilter()
        {


             for (int i = 0; i < 4; i++)
              {
            //bitmap = Filtrator.gaborFilter(bitmap, 0.1f, 4, 3, 0.01f, (float)(this.angle));
            HistogramEqualization he = new HistogramEqualization();//DZIAŁA


             bitmap = he.Apply(bitmap);
            //this.bitmap = Filtrator.GaborFilter(bitmap,(float)this.angle);
                    
            }
                //ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(tmp));

        }


       
        #endregion


    }
}
