using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
        /*orginalny obraz dodany przez użytkownika*/
        Bitmap orginalBitmap;
        /*obraz który będzie analizowany i modyfikowany np. przez filtrowanie w wazie wstępnekj*/
        Bitmap workingImage;

        Bitmap temporary;

        Fingerprint fingerprint;
        String newImageSource;
        MinutiasDatabase database;
        ImageCell randomCel;
        Bitmap threeToThree;
        Bitmap afterFiltering;
        bool testFlag=true;
        //Bitmap processImage;

        string databasePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\database\\ databse.json";
        MinutiaWektor temporaryMinutiasMap;
        #region Constructors
        public MainWindow()
        {

            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.Drop += new DragEventHandler(Form1_DragDrop);
            SetDefault();
            database = new MinutiasDatabase(databasePath);
            database.Load();


            //EqualFingerprintList.MouseDoubleClick += new EventHandler(ListBox_DoubleClick);
        }
        #endregion

        #region Methods
        #region DragAndDropMethods
        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;
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
            originalImage.Source = logo;

            Bitmap tmp = ImageSupporter.ColorToGrayscale(ImageSupporter.BitmapImage2Bitmap(logo));
           Bitmap or = Filtrator.segmentation(100,tmp);
            //originalImage.Source = ImageSupporter.Bitmap2BitmapImage(ImageSupporter.ColorToGrayscale(ImageSupporter.BitmapImage2Bitmap(logo)));
            originalImage.Source = ImageSupporter.Bitmap2BitmapImage(or);
             newImageSource = files[0];
            

            fingerprint = new Fingerprint((BitmapImage)originalImage.Source);
            this.orginalBitmap = fingerprint.orginalImage;
            this.workingImage = (Bitmap)orginalBitmap.Clone();

            //actualFingerprint = new Fingerprint(logo);
        }
        #endregion

        public void zapisz(object sender, EventArgs e)
        {


            if (newImageSource != null)
            {
                Boolean flaga = true;

                while (flaga)
                {
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
                
               // fingerprint.startRecognition(fingerprint.localOrientationMap);
                fingerprint.startRecognition((Bitmap)workingImage.Clone());
                fingerprint.toDirectionMask();

                mapakierunkow.Source = ImageSupporter.Bitmap2BitmapImage(fingerprint.localOrientationMap);

            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine("Nie podałeś odcisku palca do analizy");
            }


        }

        public void losowyObszar(object sender, EventArgs e)
        {
            fingerprint = new Fingerprint((BitmapImage)originalImage.Source);
            fingerprint.startRecognition(fingerprint.localOrientationMap);
            randomCel = fingerprint.getRandomCell(mapakierunkow);
            ImageSupporter.WriteBitmap(randomCel.bitmap);
            QUATRE.Source = ImageSupporter.Bitmap2BitmapImage(randomCel.bitmap);
            fingerprint.merge();
            threetothreeImage.Source = ImageSupporter.ToBitmapSource(ImageSupporter.Scale(1, fingerprint.localOrientationMap));
        }

        private void Q(object sender, RoutedEventArgs e)
        {
            try
            {
                //randomCel.toDirectionMap();
                //pojedynczyKierunek.Source = ImageSupporter.Bitmap2BitmapImage(randomCel.bitmap);
                //  pojedynczyKierunek.Source = ImageSupporter.Bitmap2BitmapImage(randomCel.toDirectionMapTest());
                pojedynczyKierunek.Source = ImageSupporter.ToBitmapSource(ImageSupporter.Scale(100, randomCel.toDirectionMapTest()));
            }
            catch (System.NullReferenceException en)
            {
                Console.WriteLine("Nie masz wylosowanej komórki");
            }

        }


        private void threetothree(object sender, EventArgs e)
        {
            try
            {
                Bitmap bb = new Bitmap(3, 3);
                bb.SetPixel(0, 0, Color.FromArgb(0, 0, 0));
                bb.SetPixel(1, 0, Color.FromArgb(50, 50, 50));
                bb.SetPixel(2, 0, Color.FromArgb(100, 100, 100));
                bb.SetPixel(0, 1, Color.FromArgb(50, 50, 50));
                bb.SetPixel(1, 1, Color.FromArgb(100, 100, 100));
                bb.SetPixel(2, 1, Color.FromArgb(255, 255, 255));
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

        private void gradient(object sender, EventArgs e)
        {
            try
            {
                // Console.WriteLine(randomCel.wektorToAngle(randomCel.getWektor(threeToThree)));
                int x = Int32.Parse(Console.ReadLine());
                int y = Int32.Parse(Console.ReadLine());

                fingerprint = new Fingerprint((BitmapImage)originalImage.Source);
                fingerprint.startRecognition(fingerprint.localOrientationMap);
                randomCel = fingerprint.getCell(mapakierunkow, x, y);
                ImageSupporter.WriteBitmap(randomCel.bitmap);
                QUATRE.Source = ImageSupporter.Bitmap2BitmapImage(randomCel.bitmap);
                fingerprint.merge();
                threetothreeImage.Source = ImageSupporter.ToBitmapSource(ImageSupporter.Scale(1, fingerprint.localOrientationMap));







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
            // Console.WriteLine(Color.Blue.ToArgb());
        }


        #endregion

        private void choosenArea_Click(object sender, RoutedEventArgs e)
        {
            int x = Int32.Parse(X.Text);
            int y = Int32.Parse(Y.Text);
            fingerprint = new Fingerprint((BitmapImage)originalImage.Source);
            fingerprint.startRecognition(fingerprint.localOrientationMap);
            randomCel = fingerprint.getCell(mapakierunkow, x, y);
            ImageSupporter.WriteBitmap(randomCel.bitmap);
            QUATRE.Source = ImageSupporter.Bitmap2BitmapImage(randomCel.bitmap);
            fingerprint.merge();
            threetothreeImage.Source = ImageSupporter.ToBitmapSource(ImageSupporter.Scale(1, fingerprint.localOrientationMap));
        }

        private void MinutaesDetection(object sender, RoutedEventArgs e)
        {
            //     try
            //     {
           // punktySekcjiImage.Source = ImageSupporter.Bitmap2BitmapImage(fingerprint.getSectionPoints(afterFiltering));
            punktySekcjiImage.Source = ImageSupporter.Bitmap2BitmapImage(fingerprint.getSectionPoints((Bitmap)workingImage.Clone()));
            prezentacjasekcjiImage.Source = ImageSupporter.Bitmap2BitmapImage(fingerprint.getAlreadyPassed());

            temporaryMinutiasMap = new MinutiaWektor( fingerprint.GetTemporaryMinutiasMap());
            MinuteaWektorInformator.Text = "Wykryto " + temporaryMinutiasMap.m.Count() + " minucji \n" +
                                           +temporaryMinutiasMap.GetEndCount() + " zakończeń \n" +
                                           +temporaryMinutiasMap.GetForkCount() + " rozwidleń";


            //    }
            //  catch (Exception ex)
            //  {
           // Console.WriteLine("Nie masz mapy kierunków");
            // }
        }
        List<Tuple<DatabaseElement, int, ModyficationElement>> equals;
        private void CheckWithDatabase(object sender, RoutedEventArgs e)
        {
            //temporaryMinutiasMap = new MinutiaWektor();
              equals = database.CheckWithDatabase(temporaryMinutiasMap);
            foreach(var element in equals)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = element.Item1.FingerprntName+" "+element.Item2+" ("+element.Item3.ToString()+")" ;
                EqualFingerprintList.Items.Add(item);
                
            }


        }

        private void ListBox_DoubleClick(object sender, RoutedEventArgs e)
        {

          
            int index = EqualFingerprintList.Items.IndexOf(sender);
            DatabaseElement chosen = equals[index].Item1;
            ModyficationElement przesuniecie = equals[index].Item3;
            ModyficationElement inversPrzesunięcie = new ModyficationElement(przesuniecie.x * (-1), przesuniecie.y * (-1), przesuniecie.angle * (-1));
            MinutiaWektor wektor = temporaryMinutiasMap;

            MinutiaWektor przesunietyWektor = new MinutiaWektorComperer().MapMinutiaWektor(temporaryMinutiasMap, przesuniecie);
            przesunietyWektor=new MinutiaWektorComperer().MapMinutiaWektor(przesunietyWektor,inversPrzesunięcie);
            // Bitmap  b=ImageSupporter.BitmapImage2Bitmap( new BitmapImage(new Uri(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\database\\" + chosen.FingerprntName + ".png")));
            Bitmap b = orginalBitmap;
            b = MatchMinuties(b, temporaryMinutiasMap);
            QUATRE.Source = ImageSupporter.Bitmap2BitmapImage(b);

        }
        private Bitmap MatchMinuties(Bitmap bitmap, MinutiaWektor wektor)
        {
            Bitmap final = (Bitmap)bitmap.Clone();
            foreach (var p in wektor.m)
            {
                //  final.SetPixel(item.Item1.X, item.Item1.Y,Color.Orange);

                for (int i = p.p.X - 1; i < p.p.X + 1; i++)
                {
                    for (int j = p.p.Y - 1; j < p.p.Y + 1; j++)
                    {
                        if (i < final.Width && i > 0 && j < final.Height && j > 0)
                        {
                            if (p.kind.Equals(KindOfMinutia.ZAKONCZENIE))
                                final.SetPixel(i, j, Color.Orange);

                            else final.SetPixel(i, j, Color.Purple);
                        }
                    }
                }
            }
            return final;
        }


        private void CleanDatabase(object sender, RoutedEventArgs e)
        {
            database.Clear();
        }

            private void AddToBase(object sender, RoutedEventArgs e)
        {
            database.Add(temporaryMinutiasMap,FingerprintName.Text);
            AddImageToDatabase();

        }

            private void AddImageToDatabase()
        {
           

                // if (!File.Exists(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\images\\" + i + ".png"))
                //   {
                String name = FingerprintName.Text;
                      //  System.IO.File.Copy(newImageSource, Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\database\\" + name + ".png");
                        

            ImageSupporter.Save((BitmapImage)originalImage.Source, Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\database\\" + name + ".png");
                  //  }
                
            
        }

        private void Filtracja(object sender, RoutedEventArgs e)
        {

            Bitmap b = fingerprint.filtruj(

                      /* float.Parse(Gamma.Text, CultureInfo.InvariantCulture.NumberFormat),
                       float.Parse(Lambda.Text, CultureInfo.InvariantCulture.NumberFormat),
                       float.Parse(Psi.Text, CultureInfo.InvariantCulture.NumberFormat),
                       float.Parse(Sigma.Text, CultureInfo.InvariantCulture.NumberFormat),
                       float.Parse(Theta.Text, CultureInfo.InvariantCulture.NumberFormat)*/
                      1, 1, 1, 1, 0.1f
                    );
            afterFiltering = b;
            odszumionyObraz.Source = ImageSupporter.Bitmap2BitmapImage(b);


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            /* Bitmap x = ImageSupporter.BitmapImage2Bitmap((BitmapImage)randomCell.Source);
             Bitmap b = actualFingerprint.filtruj(
                 x,
                      /* float.Parse(Gamma.Text, CultureInfo.InvariantCulture.NumberFormat),
                       float.Parse(Lambda.Text, CultureInfo.InvariantCulture.NumberFormat),
                       float.Parse(Psi.Text, CultureInfo.InvariantCulture.NumberFormat),
                       float.Parse(Sigma.Text, CultureInfo.InvariantCulture.NumberFormat),
                       float.Parse(Theta.Text, CultureInfo.InvariantCulture.NumberFormat)*/
            /*  1, 1, 1, 1, 0.1f
            );
     prezentacjasekcjiImage.Source=ImageSupporter.Bitmap2BitmapImage(b);*/
            prezentacjasekcjiImage.Source = ImageSupporter.Bitmap2BitmapImage(fingerprint.GetAfterFiltration());

        }

        private void Start(object sender, RoutedEventArgs e)
        {
            /*-----------------------Przefiltrowanie orginalnego filtru filtrem medianowym------------------------*/
            workingImage = fingerprint.medianFilter(orginalBitmap);
            /*--------------------Tworzenie mapy kierunków--------------------------*/
            fingerprint.startRecognition((Bitmap)workingImage.Clone());
            fingerprint.toDirectionMask();
            //wyświetlenie mapy kierunków
            mapakierunkow.Source = ImageSupporter.Bitmap2BitmapImage(/*workingImage*/fingerprint.localOrientationMap);
            fingerprint.ReuseImageCells(workingImage);
            /*----------------------- Przefiltrowanie obrazu filtrem Gabora----------------------------------*/
             HistogramEqualization he = new HistogramEqualization();

            workingImage = fingerprint.GetAfterFiltration();








            ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(workingImage));
            iw.Show();

             workingImage = Filtrator.ToBinarImage(workingImage, 127);
            NiblackThreshold nt = new NiblackThreshold();
            Random random = new Random();
            /* for (int i = 0; i < 20; i++)
             {
                 nt.C = 20;// i * 2.5; random.Next(255);
                 nt.K = i*0.2;//0.2*(float)random.Next(10);
                 nt.Radius =5 ;// random.Next(20);

                 ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(nt.Apply(workingImage)));
                 iw.Title = nt.C + " " + nt.K + " " + nt.Radius;
                 iw.Show();

             }*/
            nt.C = 20;// i * 2.5; random.Next(255);
            nt.K = 0.4;//0.2*(float)random.Next(10);
            nt.Radius = 5;// random.Next(20);
            workingImage = nt.Apply(workingImage);
            QUATRE.Source = ImageSupporter.Bitmap2BitmapImage(workingImage);
            /*-----------------------------Szukanie minucji----------------------------------*/

            punktySekcjiImage.Source = ImageSupporter.Bitmap2BitmapImage(fingerprint.getSectionPoints(workingImage));
            prezentacjasekcjiImage.Source = ImageSupporter.Bitmap2BitmapImage(fingerprint.getAlreadyPassed());

            /*---------------------------Sprawdzanie podobieństwa z odciskami z bazy danych-------------------*/

        }
        private void Start1(object sender, RoutedEventArgs e)
        {
            /*GaborFilter gb = new GaborFilter();
            Random random = new Random();
            for (int i = 0; i < 1; i++)
            {
                gb.Gamma = random.NextDouble();
                gb.Lambda = random.Next(1,10);
                gb.Psi = 1;
                gb.Sigma = random.Next(1,10);
                
                gb.Theta =1;


                ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(Filtrator.gaborFilter((Bitmap)orginalBitmap.Clone(),gb)));
                iw.Title = gb.Gamma + " " + gb.Lambda + " " + gb.Psi + " " + gb.Sigma + " " + gb.Theta;
                iw.Show();
            }*/
            //Filtrator.gaborFilterRandom(orginalBitmap);
            workingImage = Filtrator.Normalization(orginalBitmap);
            fingerprint.startRecognition((Bitmap)workingImage.Clone());
            fingerprint.toDirectionMask();
            //wyświetlenie mapy kierunków



            mapakierunkow.Source = ImageSupporter.Bitmap2BitmapImage(/*workingImage*/fingerprint.localOrientationMap);
            
            fingerprint.ReuseImageCells(workingImage);

            //QUATRE.Source = ImageSupporter.Bitmap2BitmapImage(fingerprint.Merge());
           // for (int i = 0; i < 10; i++)
           // {
                //List<Bitmap> bank = Filtrator.gaborFilterBank(orginalBitmap, 12);



            List<Bitmap> bank = Filtrator.gaborFilterMyBank(orginalBitmap);
                Bitmap b = fingerprint.GetGaborFilteredImage(bank);
              //  ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(b));
              //  iw.Show();
                QUATRE.Source = ImageSupporter.Bitmap2BitmapImage(b);
           // }
        }
        private void Start2(object sender, RoutedEventArgs e)
        {
            Filtrator.gaborFilterRandom(orginalBitmap);
        }


        #region Dobre Metody
        private void Normalization(object sender, RoutedEventArgs e)
        {
            temporary = Filtrator.Normalization(workingImage);
            ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(temporary));
            iw.Show();
        }

        private void OK(object sender, RoutedEventArgs e)
        {
            this.workingImage = (Bitmap)temporary.Clone();
            odszumionyObraz.Source = ImageSupporter.Bitmap2BitmapImage(workingImage);
        }

        private void MedianFilter(object sender, RoutedEventArgs e)
        {
            temporary = Filtrator.MedianFilter((Bitmap)workingImage.Clone());
            ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(temporary));
            iw.Show();
        }

        private void Histogram(object sender, RoutedEventArgs e)
        {
            temporary = Filtrator.Histogram((Bitmap)workingImage.Clone());
            ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(temporary));
            iw.Show();
        }
        private void Segmentation(object sender, RoutedEventArgs e)
        {
            temporary = Filtrator.segmentation(30,(Bitmap)workingImage.Clone());
            ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(temporary));
            iw.Show();
        }

        private void Binaryzation(object sender, RoutedEventArgs e)
        {
            temporary = Filtrator.Binaryzation((Bitmap)workingImage.Clone());
            ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(temporary));
            iw.Show();
        }
        private void Gornoprzepustowy(object sender, RoutedEventArgs e)
        {
            temporary = Filtrator.HighPassFilter(EMask.Prewitt,(Bitmap)workingImage.Clone());
            ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(temporary));
            iw.Show();
        }
        private void GridedHistogram(object sender, RoutedEventArgs e)
        {
            temporary = Filtrator.HistorgramGridedFilter((Bitmap)workingImage.Clone(),32);
            ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(temporary));
            iw.Show();
        }

        private void Invers(object sender, RoutedEventArgs e)
        {
            temporary = Filtrator.Invers((Bitmap)workingImage.Clone());
            ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(temporary));
            iw.Show();
        }
        private void Canny(object sender, RoutedEventArgs e)
        {
          //  CannyEdgeDetector cannyEdgeDetector = new CannyEdgeDetector();
          //  temporary=cannyEdgeDetector.Apply((Bitmap)workingImage.Clone());
            temporary = Filtrator.Canny((Bitmap)workingImage.Clone());
            ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(temporary));
            iw.Show();
        }

        private void LowPassFilter(object sender, RoutedEventArgs e)
        {
            temporary = Filtrator.LowPassFilter(JMask.GAUSS,(Bitmap)workingImage.Clone());
            ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(temporary));
            iw.Show();
        }
        private void Laplasjan(object sender, RoutedEventArgs e)
        {
            temporary = Filtrator.LaplasjanFilter( (Bitmap)workingImage.Clone());
            ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(temporary));
            iw.Show();
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            temporary = (Bitmap)orginalBitmap.Clone();
            workingImage = (Bitmap)orginalBitmap.Clone();
            odszumionyObraz.Source = ImageSupporter.Bitmap2BitmapImage(workingImage);
        }
        public void PrzetwarzanieWstepne()
        {
            this.workingImage =  Filtrator.Normalization(workingImage);
            this.workingImage =  Filtrator.MedianFilter((Bitmap)workingImage.Clone());
            this.workingImage =  Filtrator.Histogram((Bitmap)workingImage.Clone());


        }

        private void ZastosujGaborFilter(object sender, RoutedEventArgs e)
        {
            StartChosenGabor();
        }




            #endregion

            #region Gabor Filter Specyfikaction Click Listeners 

            private void GaborFilterClick(object sender, RoutedEventArgs e)
        {
            workingImage = Filtrator.Normalization(orginalBitmap);
            fingerprint.startRecognition((Bitmap)workingImage.Clone());
            fingerprint.toDirectionMask();
            //wyświetlenie mapy kierunków



            mapakierunkow.Source = ImageSupporter.Bitmap2BitmapImage(/*workingImage*/fingerprint.localOrientationMap);

            fingerprint.ReuseImageCells(workingImage);

            //QUATRE.Source = ImageSupporter.Bitmap2BitmapImage(fingerprint.Merge());

            List<Bitmap> bank = Filtrator.gaborFilterBank(orginalBitmap, 12, float.Parse(GammaInsert.Text),
                float.Parse(LambdaInsert.Text),
                float.Parse(PsiInsert.Text),
                float.Parse(SigmaInsert.Text)
             );
            Filtrator.gaborFilterMyBank(orginalBitmap);
            Bitmap b = fingerprint.GetGaborFilteredImage(bank);
            QUATRE.Source = ImageSupporter.Bitmap2BitmapImage(b);
            
        }
        private void SetDefault()
        {
            GammaInsert.Text = 0.3+"";
            LambdaInsert.Text = 4.0+"";
            PsiInsert.Text = 1.0 + "";
            SigmaInsert.Text = 2.0 + " ";
            ThetaInsert.Text = 0 + " ";

        }
        private void GammaMinusClick(object sender, RoutedEventArgs e)
        {
           
            GammaInsert.Text = (float.Parse(GammaInsert.Text) - 0.1)+"";
            if (testFlag==true)StartChosenGabor();
        }
        private void GammaPlusClick(object sender, RoutedEventArgs e)
        {
            GammaInsert.Text = (float.Parse(GammaInsert.Text) + 0.1)+"";
            if (testFlag==true)StartChosenGabor();
        }

        private void LambdaMinusClick(object sender, RoutedEventArgs e)
        {
            LambdaInsert.Text = (float.Parse(LambdaInsert.Text) - 0.1) + "";
            if (testFlag==true)StartChosenGabor();
        }
        private void LambdaPlusClick(object sender, RoutedEventArgs e)
        {
           LambdaInsert.Text = (float.Parse(LambdaInsert.Text) + 0.1) + "";
            if (testFlag==true)StartChosenGabor();
        }

        private void PsiMinusClick(object sender, RoutedEventArgs e)
        {
           PsiInsert.Text = (float.Parse(PsiInsert.Text) - 0.1) + "";
            if (testFlag==true)StartChosenGabor();
        }
        private void PsiPlusClick(object sender, RoutedEventArgs e)
        {
            PsiInsert.Text = (float.Parse(PsiInsert.Text) + 0.1) + "";
            if (testFlag==true)StartChosenGabor();
        }


        private void SigmaMinusClick(object sender, RoutedEventArgs e)
        {
            SigmaInsert.Text = (float.Parse(SigmaInsert.Text) - 0.1) + "";
           if (testFlag==true)StartChosenGabor();
        }
        private void SigmaPlusClick(object sender, RoutedEventArgs e)
        {
            SigmaInsert.Text = (float.Parse(SigmaInsert.Text) + 0.1) + "";
            if (testFlag==true)StartChosenGabor();
        }



        private void ThetaMinusClick(object sender, RoutedEventArgs e)
        {
            ThetaInsert.Text = (float.Parse(ThetaInsert.Text) - 15) + "";
            if (testFlag==true)StartChosenGabor();
        }
        private void ThetaPlusClick(object sender, RoutedEventArgs e)
        {
           ThetaInsert.Text = (float.Parse(ThetaInsert.Text) + 15) + "";
            if (testFlag==true)StartChosenGabor();
        }

        private void StartChosenGabor()
        {
             Filtrator.GaborFilter((Bitmap)orginalBitmap.Clone(),
                float.Parse(GammaInsert.Text),
                float.Parse(LambdaInsert.Text),
                float.Parse(PsiInsert.Text),
                float.Parse(SigmaInsert.Text),
                (float)ImageSupporter.DegreeToRadian(float.Parse(ThetaInsert.Text)));


        }
        private void Random(object sender, RoutedEventArgs e)
        {
            float x = 20;
            float y = 80;
            for (float i = float.Parse(LambdaInsert.Text) - (x * 0.1f); i < float.Parse(LambdaInsert.Text) + (x * 0.1f); i += 0.1f)
            {
                for (float j = float.Parse(SigmaInsert.Text) - (y * 0.01f); j < float.Parse(SigmaInsert.Text) + (y * 0.1f); j += 0.1f)
                {
                    Filtrator.GaborFilter(orginalBitmap,
                    float.Parse(GammaInsert.Text),
                    i,
                    float.Parse(PsiInsert.Text),
                    j,
                    (float)ImageSupporter.DegreeToRadian(float.Parse(ThetaInsert.Text)));

                }
            }

            /* for (float j = float.Parse(SigmaInsert.Text) - (x * 0.1f); j < float.Parse(SigmaInsert.Text); j += 0.1f)
             {
                 Filtrator.GaborFilter(orginalBitmap,
                 float.Parse(GammaInsert.Text),
                 float.Parse(LambdaInsert.Text),
                 float.Parse(PsiInsert.Text),
                 j,
                 (float)ImageSupporter.DegreeToRadian(float.Parse(ThetaInsert.Text)));

             }*/

        }

        #endregion

        

    }
}
