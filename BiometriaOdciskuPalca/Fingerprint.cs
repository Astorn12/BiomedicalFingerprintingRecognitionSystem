using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BiometriaOdciskuPalca
{
    class Fingerprint
    {
        public Bitmap orginalImage { get; set; }
       

        public Bitmap localOrientationMap{ get; set; }// to jest orginalne zdjęcie,(przynajmniej na początku)
        public FiltratorMedianowy filtratorMedianowy;
        GridedImage mapaKierunkow;
        int squareLen=16;// ustalona wielkość kwardarów przy tworzeniu mapy kierunków

        int sectionStartingPointDistance=32;//ustalanie odległości między poszczególnymi węzłami dyskretnej siatki

        List<System.Drawing.Point> minucje = new List<System.Drawing.Point>();
        MinutiaeDetector minutiaeDetector;
        
        public Fingerprint(BitmapImage bitmapImage)
        {
           localOrientationMap =ImageSupporter.convertToModuloBitmap(ImageSupporter.BitmapImage2Bitmap(bitmapImage),squareLen);
           orginalImage= ImageSupporter.convertToModuloBitmap(ImageSupporter.BitmapImage2Bitmap(bitmapImage),squareLen);
            //alreadyPassed = new Bitmap(bitmap.Width, bitmap.Height);
           
            
            
        }
        
        public void startRecognition(Bitmap b)
        {
            //mapaKierunkow = new GridedImage(localOrientationMap, squareLen);
            mapaKierunkow = new GridedImage(b, squareLen);


        }

        public void ReuseImageCells(Bitmap b)
        {
            mapaKierunkow.bitmap = b;
            mapaKierunkow.regridIt();
        }
        public Bitmap returnGrey2()
        {
            //Bitmap newBitamp =(Bitmap) bitmap.Clone();
            Bitmap newBitmap = new Bitmap(localOrientationMap.Width, localOrientationMap.Height);
            for(int i=0;i<newBitmap.Width;i++)
            {
                for(int j = 0;j<newBitmap.Height;j++)
                {
                    //Color pixelColor = c.GetPixel(x, y);
                    //Color newColor = Color.FromArgb(pixelColor.R, 0, 0);
                   // c.SetPixel(x, y, newColor); //


                    Color color = localOrientationMap.GetPixel(i, j);
                    Color newColor = Color.FromArgb(color.R, 0, 0);
                   // byte grey = (color.R + color.G + color.B) / 3;
                    Color greyColor = new Color();
                   // byte b = greyColor.G;
                    greyColor = Color.FromArgb(newColor.R,0,0);
                    newBitmap.SetPixel(i, j, newColor);
                  

                }
            }
            return newBitmap;
        }

        public Bitmap To8bit()
        {
           // using (var bitmap = image)
            using (var stream = new MemoryStream())
            {
                var parameters = new EncoderParameters(1);
                parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 8L);

                var info = GetEncoderInfo("image/tiff");
                localOrientationMap.Save(stream, info, parameters);

                return new Bitmap(Image.FromStream(stream));
            }
        }
        private  ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var imageEncoders = ImageCodecInfo.GetImageEncoders();
            return imageEncoders.FirstOrDefault(t => t.MimeType == mimeType);
        }

        public Bitmap toGrey()
        {
            
            Bitmap newBitmap = new Bitmap(localOrientationMap.Width, localOrientationMap.Height);
            for (int i = 0; i < newBitmap.Width; i++)
            {
                for (int j = 0; j < newBitmap.Height; j++)
                {
                    Color color = localOrientationMap.GetPixel(i, j);
                    int grey = (color.R + color.G + color.B) / 3;
                    byte b = (byte)grey;
                    Color greyColor = Color.FromArgb(100,b,b,b);
                    newBitmap.SetPixel(i, j, greyColor);
                }
            }
            return newBitmap;
        }

        public Bitmap filtruj(float gamma, float lambda,float psi,float sigma,float theta)
        {
            Bitmap b = medianFilter(orginalImage);
            //Bitmap b = orginalImage;
           b= gaborFilter(b, gamma,  lambda, psi, sigma, theta);
            //return areaFilter(b);
            return b;
        }
        public Bitmap filtruj(Bitmap o,float gamma, float lambda, float psi, float sigma, float theta)
        {
            Bitmap  b= medianFilter(o);
            //Bitmap b = orginalImage;
            b = gaborFilter(b, gamma, lambda, psi, sigma, theta);
            //return areaFilter(b);
            return b;
        }
        public Bitmap medianFilter(Bitmap b)
        {
            FiltratorMedianowy f = new FiltratorMedianowy();

            return f.filtrowanie(b);
        }

        public Bitmap gaborFilter(Bitmap b,float gamma, float lambda,float psi,float sigma,float theta )
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

                    var okno = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(filter.Apply(ImageSupporter.ColorToGrayscale(bx))));
                    okno.Title = filter.Gamma + " " + filter.Lambda + " " + filter.Psi + " " + filter.Sigma + " " + filter.Theta;
                    okno.Show();
                
            
            filter.Gamma = 3.0;
            filter.Theta = 0.0;
            GrayscaleToRGB grayscaleToRGB = new GrayscaleToRGB();
            return grayscaleToRGB.Apply( filter.Apply(ImageSupporter.ColorToGrayscale(b)));
        }

        
        private static Bitmap areaFilter(Bitmap b)
        {
            

            return Filtrator.HighPassFilter(EMask.Prewitt,b);
        }

        
        #region Tests
        public void test1()
        {
            mapaKierunkow.boundGrids();
            mapaKierunkow.mergeIt();
            this.localOrientationMap =mapaKierunkow.bitmap ;
            
        }

        public Bitmap toDirectionMask()
        {
            mapaKierunkow.convertToDirectionMap();
            mapaKierunkow.mergeIt();
            this.localOrientationMap = mapaKierunkow.bitmap;
            return this.localOrientationMap;
        }
        public void merge()
        {
           // mapaKierunkow.mergeIt();
            this.localOrientationMap = mapaKierunkow.bitmap;
        }
       

        public ImageCell getRandomCell(System.Windows.Controls.Image image)
        {
            return mapaKierunkow.getRandomCell(image);
        }

        public ImageCell getCell(System.Windows.Controls.Image image,int x,int y)
        {
            return mapaKierunkow.getCell(image,x,y);
        }

        public void startGoing()
        {
            
        }

        public void startMinutazDetection()
        {
            int sectionLen = 14;
            int startingPointsDistance = 16;
            int dlugoscSkoku =5;
            minutiaeDetector =new MinutiaeDetector(orginalImage, sectionLen, startingPointsDistance,mapaKierunkow,dlugoscSkoku);

        }
        public Bitmap getSectionPoints(Bitmap bitmap)
        {
            this.orginalImage = bitmap;
            startMinutazDetection();
            return minutiaeDetector.GetImageWithMatchedMinutias();
        }
        public Tuple<Bitmap,Bitmap> getAlreadyPassed()
        {
            return minutiaeDetector.getAlreadyPassed();
        }

        public List<Minutia> GetTemporaryMinutiasMap()
        {
            return minutiaeDetector.getMinutionsMap();
        }






        public Bitmap GetAfterFiltration()
        {
            //mapaKierunkow.convertToDirectionMap();
            //mapaKierunkow.GaborFilter();
            // mapaKierunkow.mergeIt();
            //  return mapaKierunkow.bitmap;
            return mapaKierunkow.GaborFilter();
        }

        public Bitmap GetGaborFilteredImage(List<Bitmap> bank)
        {
            return mapaKierunkow.GetGaborFIlteredImage(bank);
        }
       
        public Bitmap Merge()
        {
            this.mapaKierunkow.mergeIt();
            return mapaKierunkow.bitmap;
        }


        public void ShowAngles()
        {
            mapaKierunkow.ShowAngles();
        }
        
        #endregion

    }
}
