using Accord.Imaging.Filters;
using Accord.Statistics.Visualizations;
//using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometriaOdciskuPalca
{
    class Filtrator
    {



        public static Bitmap HighPassFilter(EMask name, Bitmap bitmap)
        {
            Bitmap b = (Bitmap)bitmap.Clone();
            double[,] maskx = MaskBox.GetMaskX(name);
            double[,] masky = MaskBox.GetMaskY(name);

            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    double sigma = Math.Sqrt(Math.Pow(Convolution(GetArea(new Point(i, j), 3, bitmap), maskx), 2)
                        +
                        Math.Pow(Convolution(GetArea(new Point(i, j), 3, bitmap), masky), 2));


                    byte byt = (byte)(sigma + 0.5);

                    b.SetPixel(i, j, Color.FromArgb(byt, byt, byt));
                }
            }
            return b;

        }

        public static Bitmap LaplasjanFilter( Bitmap bitmap)
        {
            Bitmap b = (Bitmap)bitmap.Clone();
            double[,] mask = MaskBox.GetLaplasjanMask();
            

            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    double sigma = Convolution(GetArea(new Point(i, j), 3, bitmap), mask);


                    byte byt = (byte)(sigma + 0.5);

                    b.SetPixel(i, j, Color.FromArgb(byt, byt, byt));
                }
            }
            return b;

        }

        public static Bitmap LowPassFilter(JMask name, Bitmap bitmap)
        {
            Bitmap b = (Bitmap)bitmap.Clone();
            double[,] mask = MaskBox.GetMask(name);
            

            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    double sigma = Convolution(GetArea(new Point(i, j), 3, bitmap), mask)/(Math.Pow(mask.GetLength(0),2));
                     


                    byte byt = (byte)(sigma + 0.5);

                    b.SetPixel(i, j, Color.FromArgb(byt, byt, byt));
                }
            }
            return b;

        }

        public static int[,] BackgroundSegmentation(Bitmap bitmap)
        {
            int[,] segTab = new int[bitmap.Width, bitmap.Height];

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    segTab[i, j] = 1;
                }
            }



            for (int j = 0; j < bitmap.Height; j++)
            {
                for (int i = 0; i < bitmap.Width; i++)
                {

                    if (bitmap.GetPixel(i, j).R > 150)
                    {
                        segTab[i, j] = 0;
                    }
                    else
                    {
                        break;
                    }
                }
            }


            for (int j = bitmap.Height - 1; j > 0; j--)
            {
                for (int i = bitmap.Width - 1; i > 0; i--)
                {

                    if (bitmap.GetPixel(i, j).R > 150)
                    {
                        segTab[i, j] = 0;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return segTab;
        }

        public static Bitmap HistorgramGridedFilter(Bitmap bitmap,int size)
        {
            Bitmap b = (Bitmap)bitmap.Clone();
            GridedImage grid = new GridedImage(b,size);


            return grid.GaborFilter();
        }
        public static Bitmap Invers(Bitmap b)
        {
            Bitmap bi = (Bitmap)b.Clone();
            for(int i=0;i<bi.Width;i++)
            {
                for(int j=0;j<bi.Height;j++)
                {
                    byte newColor =(byte) (255 - bi.GetPixel(i, j).R);

                    bi.SetPixel(i, j, Color.FromArgb(newColor, newColor, newColor));

                }
            }
            return bi;
        }

        #region Private Methods

        private static double[,] GetArea(Point middlePoint, int len, Bitmap bitmap)
        {


            double[,] area = new double[len, len];
            int a = 0;
            int b = 0;
            for (int i = middlePoint.X - len / 2; i <= middlePoint.X + len / 2; i++)
            {
                for (int j = middlePoint.Y - len / 2; j <= middlePoint.Y + len / 2; j++)
                {
                    area[a, b] = bitmap.GetPixel(i, j).R;
                    a++;
                }
                b++;
                a = 0;
            }

            return area;
        }

        private static double Convolution(double[,] area, double[,] mask)
        {
            double result = 0;

            for (int i = 0; i < area.GetLength(0); i++)
            {
                for (int j = 0; j < area.GetLength(1); j++)
                {
                    result += area[i, j] * mask[i, j];
                }
            }
            return result;
        }

        public static Bitmap gaborFilter(Bitmap b, float gamma, float lambda, float psi, float sigma, float theta)
        {
            // Grayscale g = new Grayscale(0.2125, 0.7154, 0.0721);

            GaborFilter filter = new GaborFilter();
            Random random = new Random();
            Boolean flaga = true;




            filter.Gamma = gamma;
            filter.Lambda = lambda;
            filter.Psi = psi;
            filter.Sigma = sigma;
            filter.Theta = theta;


            /*filter.Gamma = random.Next(1,5);
            filter.Lambda=random.Next(1,5);
            filter.Psi=random.Next(1,5);
            filter.Sigma=random.Next(1,5);
            filter.Theta=random.Next(1,5);*/

            Bitmap bx = ImageSupporter.ColorToGrayscale(b);
            filter.Apply(ImageSupporter.ColorToGrayscale(bx));
            //  var okno = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(filter.Apply(ImageSupporter.ColorToGrayscale(bx))));
            // okno.Title = filter.Gamma + " " + filter.Lambda + " " + filter.Psi + " " + filter.Sigma + " " + filter.Theta;
            // okno.Show();



            GrayscaleToRGB grayscaleToRGB = new GrayscaleToRGB();
            return grayscaleToRGB.Apply(filter.Apply(ImageSupporter.ColorToGrayscale(b)));
        }
        public static Bitmap gaborFilter(Bitmap b, GaborFilter gf)
        {
            // Grayscale g = new Grayscale(0.2125, 0.7154, 0.0721);




            /*filter.Gamma = random.Next(1,5);
            filter.Lambda=random.Next(1,5);
            filter.Psi=random.Next(1,5);
            filter.Sigma=random.Next(1,5);
            filter.Theta=random.Next(1,5);*/

            Bitmap bx = ImageSupporter.ColorToGrayscale(b);
            gf.Apply(ImageSupporter.ColorToGrayscale(bx));
            //  var okno = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(filter.Apply(ImageSupporter.ColorToGrayscale(bx))));
            // okno.Title = filter.Gamma + " " + filter.Lambda + " " + filter.Psi + " " + filter.Sigma + " " + filter.Theta;
            // okno.Show();



            GrayscaleToRGB grayscaleToRGB = new GrayscaleToRGB();
            return grayscaleToRGB.Apply(gf.Apply(ImageSupporter.ColorToGrayscale(b)));
        }

        public static void GaborFilter(Bitmap b, float gamma, float lambda, float psi, float sigma, float theta)
        {
            GaborFilter gb = new GaborFilter();
            Random random = new Random();
            Bitmap bx;





            gb.Gamma = gamma;
            gb.Lambda = lambda;
            gb.Psi = psi;
            gb.Sigma = sigma;

            gb.Theta = theta;
            bx = ImageSupporter.ColorToGrayscale((Bitmap)b.Clone());

            // try
            //{
           Bitmap tmp = bx;
            //for (int i = 0; i < 3; i++)
           // {
                tmp = gb.Apply(bx);
          //  }
                // if (WhiteningLevel(tmp, 0.1f))
                //  {
               // tmp = FFT((Bitmap)b.Clone());
                    ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(tmp));

                    iw.Title = gb.Gamma + " " + gb.Lambda + " " + gb.Psi + " " + gb.Sigma + " " + gb.Theta;
                    iw.Show();
              //  }
          /*  }
            catch (Exception e)
            {
                e.GetBaseException();
            }*/

        }

        public static Bitmap FFT(Bitmap b)
        {
            /*Bitmap tmp=ImageSupporter.ColorToGrayscale((Bitmap)b.Clone());
            ComplexImage ci= ComplexImage.FromBitmap(tmp);
             ci.BackwardFourierTransform();
             return ci.ToBitmap();*/
            return null;
        }







        public static Bitmap GaborFilter(Bitmap b,float angle)
        {
            Bitmap bz = (Bitmap)b.Clone();
            GaborBank gb= new GaborBank();
            GaborFilter gf = gb.GetGaborFilterFromBank(angle);
            Bitmap bx =ImageSupporter.ColorToGrayscale((Bitmap)bz.Clone());
            Bitmap filtered = gf.Apply(bx);


            return ImageSupporter.GrayScaleToColor(filtered);
            
        }



        public static void gaborFilterRandom(Bitmap b)
        {
            GaborFilter gb = new GaborFilter();
            Random random = new Random();
            Bitmap bx;




            for (int i = 1; i < 100; i++)
            {
                gb.Gamma = random.NextDouble() * random.Next(1, 10); ;
                gb.Lambda = random.Next(1, 10);
                gb.Psi = random.NextDouble();
                gb.Sigma = random.Next(1, 10);

                gb.Theta = 0;
                bx = ImageSupporter.ColorToGrayscale(b);
                //gb.Apply(ImageSupporter.ColorToGrayscale(bx));
                if (gb.Gamma == 0) gb.Gamma = 0.01;
                Console.WriteLine(gb.Gamma + " " + gb.Lambda + " " + gb.Psi + " " + gb.Sigma + " " + gb.Theta);
                try
                {
                    ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(gb.Apply(bx)));
                    iw.Title = gb.Gamma + " " + gb.Lambda + " " + gb.Psi + " " + gb.Sigma + " " + gb.Theta;
                    iw.Show();
                }
                catch (Exception e)
                {
                    e.GetBaseException();
                }

            }
        }

        public static List<Bitmap> gaborFilterBank(Bitmap b, int numberOfFilters)
        {
            GaborFilter gb = new GaborFilter();
            Random random = new Random();
            Bitmap bx;

            List<Bitmap> gaborBank = new List<Bitmap>();

            float jump = (3.14f) / numberOfFilters;


            gb.Gamma = 1; //random.NextDouble() * random.Next(1,10)+0.1 ;
            gb.Lambda = 9;// random.Next(1, 10);//7;
            gb.Psi = 0.1;
            gb.Sigma = 20;// 3;// random.Next(1, 10);

            //gb.Theta = 1;
            bx = ImageSupporter.ColorToGrayscale(b);
            Console.WriteLine(gb.Gamma + " " + gb.Lambda + " " + gb.Psi + " " + gb.Sigma);
            for (int i = 0; i < numberOfFilters + 1; i++)
            {
                gb.Theta = (float)i * jump;
                gaborBank.Add(gb.Apply(bx));
                //ImageWindow im=new ImageWindow(ImageSupporter.Bitmap2BitmapImage( gaborBank[i]));
                // im.Title = ImageSupporter.RadianToDegree(gb.Theta)+"";
                //  im.Show();
            }

            return gaborBank;



        }
        

        


        public static List<Bitmap> gaborFilterMyBank(Bitmap bs)
        {
           

        List<SetValuesForGabor> bank = new List<SetValuesForGabor>();

            bank.Add(new SetValuesForGabor(4,8,0.5f,9.2f,0));
            bank.Add(new SetValuesForGabor(4,8,0.5f,4.8f,(float)ImageSupporter.DegreeToRadian(15)));
            bank.Add(new SetValuesForGabor(4,6.5f,0.5f,4.3f,(float)ImageSupporter.DegreeToRadian(30)));
            bank.Add(new SetValuesForGabor(4,7,0.5f,4.3f,(float)ImageSupporter.DegreeToRadian(45)));
            bank.Add(new SetValuesForGabor(4,6.5f,0.5f,4.3f,(float)ImageSupporter.DegreeToRadian(60)));
            bank.Add(new SetValuesForGabor(4,8,0.5f,4.8f,(float)ImageSupporter.DegreeToRadian(75)));
            bank.Add(new SetValuesForGabor(4,7,0.5f,4.6f,(float)ImageSupporter.DegreeToRadian(90)));
            bank.Add(new SetValuesForGabor(4,7.3f,0.5f,4.4f,(float)ImageSupporter.DegreeToRadian(105)));
            bank.Add(new SetValuesForGabor(4,6.2f,0.5f,3.7f,(float)ImageSupporter.DegreeToRadian(120)));
            bank.Add(new SetValuesForGabor(4,7,0.5f,4.3f,(float)ImageSupporter.DegreeToRadian(135)));
            bank.Add(new SetValuesForGabor(4,6.5f,0.5f,4.3f,(float)ImageSupporter.DegreeToRadian(150)));
            bank.Add(new SetValuesForGabor(4,8,0.5f,4.8f,(float)ImageSupporter.DegreeToRadian(165)));




       
            GaborFilter gb = new GaborFilter();
            Random random = new Random();
            Bitmap bx;
            Bitmap b = (Bitmap)bs.Clone();

            List<Bitmap> gaborBank = new List<Bitmap>();

            bx = ImageSupporter.ColorToGrayscale(b);
          
            for (int i = 0; i < bank.Count; i++)
            {
                bank[i].SetGaborFilter(gb);
                gaborBank.Add(gb.Apply((Bitmap)bx.Clone()));
                
                ImageWindow im=new ImageWindow(ImageSupporter.Bitmap2BitmapImage( gaborBank[i]));
                
                im.Title = gb.Gamma + " " + gb.Lambda + " " + gb.Psi + " " + gb.Sigma + " " + gb.Theta;
                  im.Show();
            }

            return gaborBank;



        }






       public static Bitmap Canny(Bitmap bitmap)
        {
            Bitmap temporary= ImageSupporter.ColorToGrayscale(bitmap);;
            CannyEdgeDetector cannyEdgeDetector = new CannyEdgeDetector();
            temporary = cannyEdgeDetector.Apply((Bitmap)temporary.Clone());

            return ImageSupporter.GrayScaleToColor(temporary);
        }









        public static List<Bitmap> gaborFilterBank(Bitmap b, int numberOfFilters,float gamma,float lambda,float psi,float sigma)
        {
            GaborFilter gb = new GaborFilter();
            Random random = new Random();
            Bitmap bx;

            List<Bitmap> gaborBank = new List<Bitmap>();

            float jump = (3.14f) / numberOfFilters;


            gb.Gamma = gamma;
            gb.Lambda = lambda;
            gb.Psi = psi;
            gb.Sigma = sigma;

            //gb.Theta = 1;
            bx = ImageSupporter.ColorToGrayscale(b);
            Console.WriteLine(gb.Gamma + " " + gb.Lambda + " " + gb.Psi + " " + gb.Sigma);
            for (int i = 0; i < numberOfFilters + 1; i++)
            {
                gb.Theta = (float)i * jump;
                gaborBank.Add(gb.Apply(bx));
                ImageWindow im=new ImageWindow(ImageSupporter.Bitmap2BitmapImage( gaborBank[i]));
               im.Title = ImageSupporter.RadianToDegree(gb.Theta)+"";
                  im.Show();
            }

            return gaborBank;



        }

        public static Bitmap ToBinarImage(Bitmap bitmap,int limit)
        {
            Bitmap b =(Bitmap) bitmap.Clone();
            for(int i=0;i<b.Width;i++)
            {
                for(int j=0;j<b.Height;j++)
                {
                    if (b.GetPixel(i, j).R < limit)
                    {
                        b.SetPixel(i, j, Color.Black);
                    }
                    else b.SetPixel(i, j, Color.White);
                }
            }
            return b;
        }

        public static Bitmap Normalization(Bitmap bitmap)
        {
            Bitmap b = (Bitmap)bitmap.Clone();
            int max = 0;
            int min = 255;

            for(int i=0;i<b.Width;i++)
            {
                for(int j=0;j<b.Height;j++)
                {
                    if (b.GetPixel(i, j).R > max) max = b.GetPixel(i, j).R;
                     if(b.GetPixel(i, j).R < min) min = b.GetPixel(i, j).R;
                }
            }

            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {

                    byte by =(byte)( ((b.GetPixel(i, j).R - min) * 255)/(max - min));
                    b.SetPixel(i, j, Color.FromArgb(by, by, by));
                }
            }
            return b;

        }
        
        public static bool WhiteningLevel(Bitmap b,float level)
        {
            int counter = 0;
            for(int i=0; i<b.Height;i++)
            {
                for(int j=0;j<b.Width;j++)
                {
                    if(b.GetPixel(i,j).R>200)
                    {
                        counter++;
                    }
                }
            }
            float f = ((float)counter) / (b.Height * b.Width);
            if (f> level) return true;
            else return false;
        }

        public static Bitmap MedianFilter(Bitmap b)
        {
            FiltratorMedianowy fm = new FiltratorMedianowy();
            return fm.filtrowanie(b);
        }

        public static Bitmap Histogram(Bitmap bitmap)
        {
            HistogramEqualization he = new HistogramEqualization();
            return he.Apply((Bitmap)bitmap.Clone());
        }

        public static Bitmap Binaryzation(Bitmap b)
        {
            Threshold th = new Threshold();

            Bitmap bx= ImageSupporter.ColorToGrayscale((Bitmap)b.Clone());

            GrayscaleToRGB grayscaleToRGB = new GrayscaleToRGB();
            return grayscaleToRGB.Apply(th.Apply(ImageSupporter.ColorToGrayscale(bx)));
        }

        //metoda segmentuje zdjęcia zwracając prostokątny obraz o wartościach maksymalnej szerokości i wysokości odcisku palca, innymi słowy iusuwa poczne tło, marginesy
        public static Bitmap segmentation(int blackBound,Bitmap orginal)
            //blackBound oznacza od jakiego poziomu czerni określamy wystąpienie krawędzi,
            // w przypadku obrazów czarnobiałych limit można ustawić na 0, gdy skaner wytwarza zdjęcia z całej gamy sszarości, tzn. tło zdjęcia jest szare to należy 
           //odpowiednio zdefiniować granicę tła
        {
            int left = orginal.Width, right =0, bottom = 0, top =orginal.Width;
            Bitmap segmented;

            for(int i=0;i<orginal.Width;i++)
            {
                for(int j=0;j<orginal.Height;j++)
                {
                    if(orginal.GetPixel(i,j).R<=blackBound)// użycie <= podyktowane jest tym, aby w przypadku obrazów czarnobiałych można było ustaiwć limit na 0
                    {
                        if (i < left) left = i;
                        break;
                    }
                }
            }

            for (int i =  orginal.Width-1; i >0; i--)
            {
                for (int j =  orginal.Height-1; j >0; j--)
                {
                    if (orginal.GetPixel(i, j).R <= blackBound)
                    {
                        if (i > right) right = i;
                        break;
                    }
                }
            }

            
          for (int j = 0; j < orginal.Height; j++)
          {
                for (int i = 0; i < orginal.Width; i++)
                {
                    if (orginal.GetPixel(i, j).R <= blackBound)
                    {
                        if (j <top) top = j;
                        break;
                    }
                }
            }

            for (int j = orginal.Height-1; j > 0; j--)
            {
                for (int i = orginal.Width-1; i > 0; i--)
                {
               
                    if (orginal.GetPixel(i, j).R <= blackBound)
                    {
                        if (j > bottom) bottom = j;
                        break;
                    }
                }
            }

            segmented = new Bitmap(right - left, bottom - top);

            for (int i = left; i < right; i++)
            {
                for (int j = top; j < bottom; j++)
                {
                    segmented.SetPixel(i - left, j - top,orginal.GetPixel(i,j));
                }
            }




            return segmented;
        }

      
        #endregion
    }
}
