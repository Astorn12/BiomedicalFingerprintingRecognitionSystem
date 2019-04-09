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
        #region Attributes
        public Bitmap orginalImage { get; set; }
       

        public Bitmap localOrientationMap{ get; set; }//orginalne zdjęcie,(przynajmniej na początku)
       // public FiltratorMedianowy filtratorMedianowy;
        GridedImage mapaKierunkow;
        int squareLen=16;// ustalona wielkość kwadratów przy tworzeniu mapy kierunków

    //    int sectionStartingPointDistance=32;//ustalanie odległości między poszczególnymi węzłami dyskretnej siatki

        List<System.Drawing.Point> minucje = new List<System.Drawing.Point>();
        MinutiaeDetector minutiaeDetector;
        #endregion
        #region Methods
        public Fingerprint(BitmapImage bitmapImage)
        {
           localOrientationMap =ImageSupporter.convertToModuloBitmap(ImageSupporter.BitmapImage2Bitmap(bitmapImage),squareLen);
           orginalImage= ImageSupporter.convertToModuloBitmap(ImageSupporter.BitmapImage2Bitmap(bitmapImage),squareLen);
            
            
            
        }
        
        public void startRecognition(Bitmap b)
        {
           
            mapaKierunkow = new GridedImage(b, squareLen);


        }

        public void ReuseImageCells(Bitmap b)
        {
            mapaKierunkow.bitmap = b;
            mapaKierunkow.regridIt();
        }
       

       


        public Bitmap filtruj(float gamma, float lambda,float psi,float sigma,float theta)
        {
            Bitmap b = medianFilter(orginalImage);
           
           b= gaborFilter(b, gamma,  lambda, psi, sigma, theta);
          
            return b;
        }
       
        public Bitmap medianFilter(Bitmap b)
        {
            FiltratorMedianowy f = new FiltratorMedianowy();

            return f.filtrowanie(b);
        }

        public Bitmap gaborFilter(Bitmap b,float gamma, float lambda,float psi,float sigma,float theta )
        {
          
            GaborFilter filter = new GaborFilter();
            Random random = new Random();
            Boolean flaga = true;




            filter.Gamma = gamma;
            filter.Lambda = lambda;
            filter.Psi = psi;
            filter.Sigma = sigma;
            filter.Theta = theta;

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
#endregion
        #region Tests


        public Bitmap toDirectionMask()
        {
            mapaKierunkow.convertToDirectionMap();
            mapaKierunkow.mergeIt();
            this.localOrientationMap = mapaKierunkow.bitmap;
            return this.localOrientationMap;
        }
        public void merge()
        {
           
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
           
            return mapaKierunkow.GaborFilter();
        }

        public Bitmap GetGaborFilteredImage(List<Bitmap> bank)
        {
            return mapaKierunkow.GetGaborFIlteredImage(bank);
        }
        #endregion

    }
}
