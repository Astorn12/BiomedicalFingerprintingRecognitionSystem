using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using System.Windows.Media.Imaging;



namespace BiometriaOdciskuPalca
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        String newImageSource;
        Fingerprint actualFingerprint;
        ImageCell randomCel;
        Bitmap threeToThree;
        #region Constructors
        public MainWindow()
        {
     
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.Drop += new DragEventHandler(Form1_DragDrop);
            
        }
        #endregion

        #region Methods
        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;
          //  DragDrop.DoDragDrop(this.TXT, this.TXT.Text, DragDropEffects.Copy);
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
           String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
            // foreach (string file in files) Console.WriteLine(file);
            //this.TXT.Text = files[0];
            Bitmap bitmap = new Bitmap(files[0]);
            
           
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(files[0]);
            logo.EndInit(); // Getting the exception here
            originalImage.Source =logo;
            newImageSource = files[0];

            //actualFingerprint = new Fingerprint(logo);
        }

        public void zapisz(object sender, EventArgs e)
        {


            if (newImageSource != null)
            {
                Boolean flaga = true;

                while (flaga) { 
                Random random = new Random();
                int i = random.Next();
                    if (!File.Exists(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\images\\" + i + ".png"))
                    {
                        System.IO.File.Copy(newImageSource, Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\images\\" + i + ".png");
                        flaga = false;
                    }
                }   
            }

        }

        public void mapaKierunkow(object sender, EventArgs e)
        {
            // mapakierunkow.Source = ImageSupporter.Bitmap2BitmapImage(actualFingerprint.returnGrey());
            try
            {
                actualFingerprint = new Fingerprint((BitmapImage)originalImage.Source);
                actualFingerprint.startRecognition();
                //actualFingerprint.test1();
                actualFingerprint.toDirectionMask();

               // actualFingerprint.merge();
                // mapakierunkow.Source = ImageSupporter.Bitmap2BitmapImage(actualFingerprint.toGrey());
                mapakierunkow.Source = ImageSupporter.Bitmap2BitmapImage(actualFingerprint.localOrientationMap);
            }
            catch(InvalidCastException ex)
            {
                Console.WriteLine("Nie podałeś odcisku palca do analizy");
            }


        }

       public void losowyObszar(object sender, EventArgs e)
        {
            actualFingerprint = new Fingerprint((BitmapImage)originalImage.Source);
            actualFingerprint.startRecognition();
            randomCel = actualFingerprint.getRandomCell(mapakierunkow);
            ImageSupporter.WriteBitmap(randomCel.bitmap);
            randomCell.Source = ImageSupporter.Bitmap2BitmapImage(randomCel.bitmap);
            actualFingerprint.merge();
            threetothreeImage.Source =ImageSupporter.ToBitmapSource(ImageSupporter.Scale(1,actualFingerprint.localOrientationMap));
        }

        private void Q(object sender, RoutedEventArgs e)
        {
            try
            {
                //randomCel.toDirectionMap();
                 //pojedynczyKierunek.Source = ImageSupporter.Bitmap2BitmapImage(randomCel.bitmap);
              //  pojedynczyKierunek.Source = ImageSupporter.Bitmap2BitmapImage(randomCel.toDirectionMapTest());
                pojedynczyKierunek.Source = ImageSupporter.ToBitmapSource(ImageSupporter.Scale(100,randomCel.toDirectionMapTest()));
            }
            catch(System.NullReferenceException en)
            {
                Console.WriteLine("Nie masz wylosowanej komórki");
            }
           
        }


       private void threetothree(object sender,EventArgs e)
        {
            try
            {
                Bitmap bb = new Bitmap(3, 3);
               bb.SetPixel(0, 0, Color.FromArgb(0, 0, 0));
                bb.SetPixel(1, 0, Color.FromArgb(50,50 , 50));
                bb.SetPixel(2, 0, Color.FromArgb(100, 100, 100));
                bb.SetPixel(0, 1, Color.FromArgb(50,50,50));
                bb.SetPixel(1, 1, Color.FromArgb(100,100,100 ));
                bb.SetPixel(2, 1, Color.FromArgb(255,255, 255));
                bb.SetPixel(0, 2, Color.FromArgb(100, 100, 100));
                bb.SetPixel(1, 2, Color.FromArgb(255, 255, 255));
                bb.SetPixel(2, 2, Color.FromArgb(255, 255, 255));

               /* bb.SetPixel(0, 0, Color.FromArgb(0, 0, 0));
                bb.SetPixel(1, 0, Color.FromArgb(0, 0, 0));
                bb.SetPixel(2, 0, Color.FromArgb(0, 0, 0));
                bb.SetPixel(0, 1, Color.FromArgb(0, 0, 0));
                bb.SetPixel(1, 1, Color.FromArgb(0, 0,0));
                bb.SetPixel(2, 1, Color.FromArgb(0, 0, 0));
                bb.SetPixel(0, 2, Color.FromArgb(255, 255, 255));
                bb.SetPixel(1, 2, Color.FromArgb(255, 255, 255));
                bb.SetPixel(2, 2, Color.FromArgb(255, 255, 255));*/

                /*bb.SetPixel(0, 0, Color.FromArgb(255, 255, 255));
                bb.SetPixel(1, 0, Color.FromArgb(200, 200,200));
                bb.SetPixel(2, 0, Color.FromArgb(100, 100, 100));
                bb.SetPixel(0, 1, Color.FromArgb(200, 200, 200));
                bb.SetPixel(1, 1, Color.FromArgb(100, 100, 100));
                bb.SetPixel(2, 1, Color.FromArgb(50, 50, 50));
                bb.SetPixel(0, 2, Color.FromArgb(100, 100, 100));
                bb.SetPixel(1, 2, Color.FromArgb(50, 50, 50));
                bb.SetPixel(2, 2, Color.FromArgb(0, 0, 0));*/


              /*  bb.SetPixel(0, 0, Color.FromArgb(255, 255, 255));
                bb.SetPixel(1, 0, Color.FromArgb(200, 200, 200));
                bb.SetPixel(2, 0, Color.FromArgb(100, 100, 100));
                bb.SetPixel(0, 1, Color.FromArgb(200, 200, 200));
                bb.SetPixel(1, 1, Color.FromArgb(100, 100, 100));
                bb.SetPixel(2, 1, Color.FromArgb(50, 50, 50));
                bb.SetPixel(0, 2, Color.FromArgb(100, 100, 100));
                bb.SetPixel(1, 2, Color.FromArgb(50, 50, 50));
                bb.SetPixel(2, 2, Color.FromArgb(0, 0, 0));*/
                this.threeToThree = randomCel.getRandomThreeToThree();
                //this.threeToThree = bb;
                // var bitmapSrc = MyImagingHelper.Scale(100, twoPixelBitmap)//this will get a Bitmap size 200 * 100,change the ratio if it is not enough
                //.ToBitmapSource();
                threetothreeImage.Source = ImageSupporter.ToBitmapSource(ImageSupporter.Scale(100, threeToThree));
                /* Bitmap bitmapTwo = randomCel.getRandomTwo();
                 Console.WriteLine(bitmapTwo.Size);
                 threetothreeImage.Source = ImageSupporter.bitmapToBitmapimage2(bitmapTwo);
                 Console.WriteLine("Wielkość: " + threetothreeImage.Width + "  " + threetothreeImage.Source.Height);*/

                //Console.WriteLine("Color na pozycji 1 0= " +threeToThree.GetPixel(0, 1).R);
                ImageSupporter.WriteBitmap(threeToThree);
               
               
            }
            catch (System.NullReferenceException en)
            {
                Console.WriteLine("Nie masz wylosowanej komórki");
            }


        }
       
        private void gradient(object sender,EventArgs e)
        {
            try
            {
                // Console.WriteLine(randomCel.wektorToAngle(randomCel.getWektor(threeToThree)));
                int x =Int32.Parse( Console.ReadLine());
                int y =Int32.Parse( Console.ReadLine());

                actualFingerprint = new Fingerprint((BitmapImage)originalImage.Source);
                actualFingerprint.startRecognition();
                randomCel = actualFingerprint.getCell(mapakierunkow,x,y);
                ImageSupporter.WriteBitmap(randomCel.bitmap);
                randomCell.Source = ImageSupporter.Bitmap2BitmapImage(randomCel.bitmap);
                actualFingerprint.merge();
                threetothreeImage.Source = ImageSupporter.ToBitmapSource(ImageSupporter.Scale(1, actualFingerprint.localOrientationMap));







            }
            catch (System.NullReferenceException en)
            {
                Console.WriteLine("Nie masz wylosowanego obszaru 3 na 3");
            }

            //Console.WriteLine(imageCell.)

        }
        

        
        public void ExpandImage(object sender, EventArgs e)
        {
            var source = ((sender as Button).Content as System.Windows.Controls.Image).Source;
            var okno = new ImageWindow(source);
            okno.Show();
        }


        #endregion

        private void choosenArea_Click(object sender, RoutedEventArgs e)
        {
            int x =Int32.Parse( X.Text);
            int y = Int32.Parse(Y.Text);
            actualFingerprint = new Fingerprint((BitmapImage)originalImage.Source);
            actualFingerprint.startRecognition();
            randomCel = actualFingerprint.getCell(mapakierunkow, x, y);
            ImageSupporter.WriteBitmap(randomCel.bitmap);
            randomCell.Source = ImageSupporter.Bitmap2BitmapImage(randomCel.bitmap);
            actualFingerprint.merge();
            threetothreeImage.Source = ImageSupporter.ToBitmapSource(ImageSupporter.Scale(1, actualFingerprint.localOrientationMap));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            punktySekcjiImage.Source = ImageSupporter.Bitmap2BitmapImage(actualFingerprint.getSectionPoints());
            prezentacjasekcjiImage.Source = ImageSupporter.Bitmap2BitmapImage(actualFingerprint.getAlreadyPassed());
        }
    }

}
