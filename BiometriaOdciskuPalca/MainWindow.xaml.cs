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

        string databasePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\database\\databse.json";
        MinutiaWektor temporaryMinutiasMap;
        #region Constructors
        public MainWindow()
        {

            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.Drop += new DragEventHandler(Form1_DragDrop);
            //SetDefault();
            database = new MinutiasDatabase(databasePath);
            database.Load();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            FiltracjaCheckedBox.IsChecked = true;
          
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
            helpText.Visibility=Visibility.Hidden;
        }
        #endregion
        private void LoadImage(String path)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(path);
            logo.EndInit(); 
            //originalImage.Source = logo;

            Bitmap tmp = ImageSupporter.ColorToGrayscale(ImageSupporter.BitmapImage2Bitmap(logo));
            Bitmap or = Filtrator.segmentation(100, tmp);
            //originalImage.Source = ImageSupporter.Bitmap2BitmapImage(ImageSupporter.ColorToGrayscale(ImageSupporter.BitmapImage2Bitmap(logo)));
            //originalImage.Source = ImageSupporter.Bitmap2BitmapImage(or);
            newImageSource = path;


            fingerprint = new Fingerprint(ImageSupporter.Bitmap2BitmapImage(or));
            this.orginalBitmap = fingerprint.orginalImage;
            this.workingImage = (Bitmap)orginalBitmap.Clone();
        }


        public void AutomaticalStart(object sender,RoutedEventArgs e)
        {
            //filtrowanie
           Normalization();
             BackgroundOK();
            MedianFilter();
             BackgroundOK();
            GridedHistogram();
            ShowOK(sender,e);
            //tworzenie mapy kierunków
            MapaKierunkow(sender, e);
            //znajdowanie wektora minucji
            MinutaesDetection(sender, e);

            //zprawdzanie z baza danych
            CheckWithDatabase(sender, e);
        }


        private void WorkingInBackground(String name)
        {
            if (FiltracjaCheckedBox.IsChecked==true)
            { 
            Normalization();
            BackgroundOK();
            MedianFilter();
            BackgroundOK();
            GridedHistogram();
        }
            BackgroundOK();

            MapaKierunkow();

            MinutaesDetection();
            CheckWithDatabase();

            AddToBase(name);

            
        }
        private void AutomaticalRejestration(object sender,RoutedEventArgs e)
        {

            PreProcessing();

            ProperAlgorithm();

           // AddToBase(name);
            PopupInputWindow popup = new PopupInputWindow(AddToBase, "Wpisz nazwę dla odcisku palca", "Dodawanie odcisku palca do bazy danych");
            popup.Show();


        }

        private void ControlledRejestration(object sender,RoutedEventArgs e)
        {


            ProperAlgorithm();
        PopupInputWindow popup = new PopupInputWindow(AddToBase, "Wpisz nazwę dla odcisku palca", "Dodawanie odcisku palca do bazy danych");
        popup.Show();
        }

        public void PreProcessing()
        {
            Normalization();
            BackgroundOK();
            MedianFilter();
            BackgroundOK();
            GridedHistogram();

            BackgroundOK();
        }

        public void ProperAlgorithm()
        {
            BackgroundOK();

            MapaKierunkow();

            MinutaesDetection();
            

            
        }



        public void ControlledStart(object sender,RoutedEventArgs e)
        {
            //tworzenie mapy kierunków
            MapaKierunkow(sender, e);
            //znajdowanie wektora minucji
            MinutaesDetection(sender, e);

            //zprawdzanie z baza danych
            CheckWithDatabase(sender, e);
        }


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

        public void MapaKierunkow(object sender, RoutedEventArgs e)
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

        public void MapaKierunkow()
        {
            try
            {
                fingerprint.startRecognition((Bitmap)workingImage.Clone());
                fingerprint.toDirectionMask();
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine("Nie podałeś odcisku palca do analizy");
            }
        }

     



        public void ExpandImageSpecial(object sender, EventArgs e)
        {
            var source = (((sender as Button).Content as Grid));// as System.Windows.Controls.Image).Source;
            var s=(source.Children[0]  as System.Windows.Controls.Image).Source;
            var okno = new ImageWindow(s);
            okno.Show();
            // Console.WriteLine(Color.Blue.ToArgb());
        }
        public void ExpandImage(object sender, EventArgs e)
        {
            var source = ((sender as Button).Content as  System.Windows.Controls.Image).Source;
           
            var okno = new ImageWindow(source);
            okno.Show();
            // Console.WriteLine(Color.Blue.ToArgb());
        }

        public void ExpandImage2(object sender, EventArgs e)
        {
            var source = ((sender as Button).Content as System.Windows.Controls.Image).Source;

            var okno = new ImageWindow(source,MinutiaFromPixcel);
            okno.Show();
            // Console.WriteLine(Color.Blue.ToArgb());
        }

        public void MinutiaFromPixcel(int x,int y)
        {
            MinutiaWektor mw = database.mBase[chossenDatabaseElement].MinutiaesWektor;
            foreach(var item in mw.m)
            {
                
                    if (Math.Abs(item.p.X - x) < 3 && Math.Abs(item.p.Y - y) < 3)
                    {
                    int index = mw.m.FindIndex(a=>a==item);
                        Console.WriteLine("Minutia: " + item.p.X + " " + item.p.Y + " " + item.direction+" "+index);
                    }
                
            }

        }

        #endregion

        private void choosenArea_Click(object sender, RoutedEventArgs e)
        {
          //  int x = Int32.Parse(X.Text);
          //  int y = Int32.Parse(Y.Text);
            fingerprint = new Fingerprint((BitmapImage)originalImage.Source);
            fingerprint.startRecognition(fingerprint.localOrientationMap);
           /// randomCel = fingerprint.getCell(mapakierunkow, x, y);
            ImageSupporter.WriteBitmap(randomCel.bitmap);
            QUATRE.Source = ImageSupporter.Bitmap2BitmapImage(randomCel.bitmap);
            fingerprint.merge();
            threetothreeImage.Source = ImageSupporter.ToBitmapSource(ImageSupporter.Scale(1, fingerprint.localOrientationMap));
        }

        private void MinutaesDetection(object sender, RoutedEventArgs e)
        {
         
            QUATRE.Source = ImageSupporter.Bitmap2BitmapImage(fingerprint.getSectionPoints((Bitmap)workingImage.Clone()));

            Tuple<Bitmap, Bitmap> alreadyPassedImages = fingerprint.getAlreadyPassed();
            pojedynczyKierunek.Source = ImageSupporter.Bitmap2BitmapImage(alreadyPassedImages.Item2);
           threetothreeImage.Source=ImageSupporter.Bitmap2BitmapImage(alreadyPassedImages.Item1);
            temporaryMinutiasMap = new MinutiaWektor( fingerprint.GetTemporaryMinutiasMap());
            MinuteaWektorInformator.Text = "Wykryto " + temporaryMinutiasMap.m.Count() + " minucji \n" +
                                           +temporaryMinutiasMap.GetEndCount() + " zakończeń \n" +
                                           +temporaryMinutiasMap.GetForkCount() + " rozwidleń";
        }
        private void MinutaesDetection()
        {

           QUATRE.Source = ImageSupporter.Bitmap2BitmapImage(fingerprint.getSectionPoints((Bitmap)workingImage.Clone()));
           // fingerprint.getAlreadyPassed();

            temporaryMinutiasMap = new MinutiaWektor(fingerprint.GetTemporaryMinutiasMap());
            //MinuteaWektorInformator.Text = "Wykryto " + temporaryMinutiasMap.m.Count() + " minucji \n" +
                                         //  +temporaryMinutiasMap.GetEndCount() + " zakończeń \n" +
                                          // +temporaryMinutiasMap.GetForkCount() + " rozwidleń";
        }
        List<DatabaseElement> databaseList;
        private void ShowDatabase(object sender, RoutedEventArgs e)
        {
            databaseList= database.mBase;
            
            DatabaseList.Items.Clear();
            if (databaseList != null)
            {
                foreach (var element in databaseList)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = element.FingerprntName;
                    DatabaseList.Items.Add(item);
                }
            }

        }

        

        List<Tuple<DatabaseElement, int, ModyficationElement,int>> equals;
        private void CheckWithDatabase(object sender, RoutedEventArgs e)
        {
            EqualFingerprintList.Items.Clear();
            equals = database.CheckWithDatabase(temporaryMinutiasMap);
      
            foreach(var element in equals)
            {
                ListBoxItem item = new ListBoxItem();
                double d = (double)element.Item2 / (double)element.Item1.MinutiaesWektor.m.Count;
                item.Content = d+" "+ element.Item1.FingerprntName+" "+element.Item2+" ("+element.Item3.ToString()+") voting:"+element.Item4 ;
                if (d >= 0.4)
                {
                    item.Background = System.Windows.Media.Brushes.Red;
                }
                EqualFingerprintList.Items.Add(item);
             
                
            }
        }

        private void CheckWithDatabase()
        {
            EqualFingerprintList.Items.Clear();
            equals = database.CheckWithDatabase(temporaryMinutiasMap);
         /*   foreach (var element in equals)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = element.Item1.FingerprntName + " " + element.Item2 + " (" + element.Item3.ToString() + ") voting:" + element.Item4;
                EqualFingerprintList.Items.Add(item);

            }*/
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            MinutiaWektor wektor = database.mBase[1].MinutiaesWektor;
            ModyficationElement przesuniecie = new ModyficationElement(0,0, 10);
           

            MinutiaWektor przesunietyWektor = new MinutiaWektorComperer().MapMinutiaWektor(wektor, przesuniecie);
            Bitmap b = (Bitmap)orginalBitmap.Clone();
            b = MatchMinuties(b, przesunietyWektor);
            QUATRE.Source = ImageSupporter.Bitmap2BitmapImage(b);
        }



        int chossenDatabaseElement;
        private void ShowChosenFingerprint(object sender,RoutedEventArgs e)
        {
           
            int index = DatabaseList.Items.IndexOf(sender);
            chossenDatabaseElement = index;
            DatabaseElement chosen = databaseList[index];
            
            Bitmap b = ImageSupporter.BitmapImage2Bitmap(new BitmapImage(new Uri(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\database\\" + chosen.FingerprntName + ".png")));
            
            MinutiaWektor zbazy = database.mBase[index].MinutiaesWektor;

            b = MatchMinuties2(b, zbazy, Color.Blue, Color.Orange);
            QUATRE.Source = ImageSupporter.Bitmap2BitmapImage(b);
        }
        private void CheckSimillar(object sender, RoutedEventArgs e)
        {

            int index = DatabaseList.Items.IndexOf(sender);
            DatabaseElement chosen = databaseList[index];


            MinutiaWektor zbazy = database.mBase[index].MinutiaesWektor;
            temporaryMinutiasMap = zbazy;
            CheckWithDatabase(sender, e);

        }

        private void ListBox_DoubleClick(object sender, RoutedEventArgs e)
        {

          
            int index = EqualFingerprintList.Items.IndexOf(sender);
            DatabaseElement chosen = equals[index].Item1;
            ModyficationElement przesuniecie = equals[index].Item3;
            //ModyficationElement przesuniecie = new ModyficationElement(20, -20, -10);
            MinutiaWektor wektor = temporaryMinutiasMap;
           
            MinutiaWektor przesunietyWektor = new MinutiaWektorComperer().MapMinutiaWektor(temporaryMinutiasMap, przesuniecie);
             
           // przesunietyWektor=new MinutiaWektorComperer().MapMinutiaWektor(przesunietyWektor,inversPrzesunięcie);
            Bitmap  b=ImageSupporter.BitmapImage2Bitmap( new BitmapImage(new Uri(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\database\\" + chosen.FingerprntName + ".png")));
            //Bitmap b = orginalBitmap;
            //b = MatchMinuties(b, przesunietyWektor);
            //ModyficationElement tmpp = new ModyficationElement(0, 0, 1);
            /*for(int i=0;i<30;i++)
            {tmpp = new ModyficationElement(0, 0, i);
                MinutiaWektor wektorx=new MinutiaWektorComperer().MapMinutiaWektor(temporaryMinutiasMap, tmpp);
                Bitmap x= MatchMinuties2(b,wektorx,Color.Aqua,Color.Bisque );
                ImageWindow image = new ImageWindow(ImageSupporter.Bitmap2BitmapImage( x));
                image.Show();
            }*/
            b = MatchMinuties2(b,przesunietyWektor,Color.Green,Color.Green );
            MinutiaWektor zbazy = database.mBase[index].MinutiaesWektor;
            b = MatchMinuties2(b,zbazy,Color.Blue,Color.Blue );
            b = MatchMinuties2(b,temporaryMinutiasMap,Color.Orange,Color.Orange );

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
        private Bitmap MatchMinuties2(Bitmap bitmap, MinutiaWektor wektor, Color zakocznczenia,Color rozwidlenia)
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
                           ImageSupporter.MatchMinutia2(final, zakocznczenia, rozwidlenia, p);                      
                           // if (p.kind.Equals(KindOfMinutia.ZAKONCZENIE))
                           //     final.SetPixel(i, j, zakocznczenia);

                           // else final.SetPixel(i, j, rozwidlenia);
                        }
                    }
                }
            }

           // final.SetPixel(wektor.m[0].p.X, wektor.m[0].p.Y,Color.Red);
           // final.SetPixel(wektor.m[1].p.X, wektor.m[1].p.Y,Color.Pink);
           // final.SetPixel(wektor.m[2].p.X, wektor.m[2].p.Y,Color.Green);
           // final.SetPixel(wektor.m[3].p.X, wektor.m[3].p.Y,Color.HotPink);
           // final.SetPixel(wektor.m[4].p.X, wektor.m[4].p.Y,Color.Lavender);
          
                return final;
        }


        private void CleanDatabase(object sender, RoutedEventArgs e)
        {
            database.Clear();
        }

            public void AddToBase(String text)
        {
            database.Add(temporaryMinutiasMap,text);
            AddImageToDatabase(text);

        }
         delegate void  AddToBaseDelegate(String str);

       

        private void ShowAddToBaseWindow(object sender, RoutedEventArgs e)
        {
            PopupInputWindow popup = new PopupInputWindow(AddToBase,"Wpisz nazwę dla odcisku palca","Dodawanie odcisku palca do bazy danych");
            popup.Show();
        }
        private void ShowLoadDatabaseFromFolder(object sender, RoutedEventArgs e)
        {
            PopupInputWindow popup = new PopupInputWindow(LoadDatabaseFromFolder, "Wpisz ścieżkę do folderu","Ładowanie bazy danych odcisków palców");
            popup.Show();
        }
        public void LoadDatabaseFromFolder(String path)
        {
            string[] filePaths = Directory.GetFiles(path);
            foreach (string filePath in filePaths)
            {
                int x= 0;
                for(int i=filePath.Length-1;i>0;i--)
                {
                    if(filePath[i].Equals('\\'))
                        {
                        break;
                    }
                    x++;
                }

                string name = filePath.Remove(0, filePath.Length - x);
                LoadImage(filePath);

                WorkingInBackground(name);

            }





        }
        


        private void AddImageToDatabase(String text)
        {
           

                // if (!File.Exists(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\images\\" + i + ".png"))
                //   {
                String name = text;
            //  System.IO.File.Copy(newImageSource, Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\database\\" + name + ".png");
            try
            {

                ImageSupporter.Save(ImageSupporter.Bitmap2BitmapImage(orginalBitmap), Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\database\\" + name + ".png");
                //  }
            }
            catch(Exception ex)
            {

            }
            
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


        private void Start(object sender, RoutedEventArgs e)
        {
            /*-----------------------Przefiltrowanie orginalnego filtru filtrem medianowym------------------------*/
            workingImage = fingerprint.medianFilter(orginalBitmap);
             /*-----------------------Wykorzystanie normalizacji------------------------*/
             /*-----------------------Wyrównanie histogramu na obszarach------------------------*/
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

            pojedynczyKierunek.Source = ImageSupporter.Bitmap2BitmapImage(fingerprint.getSectionPoints(workingImage));
           // prezentacjasekcjiImage.Source = ImageSupporter.Bitmap2BitmapImage(fingerprint.getAlreadyPassed());

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
        private void StartImageWindow(Bitmap bitmap)
        {
            ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(bitmap),OK);
            iw.Show();
        }
        private void ShowOK(object sender, RoutedEventArgs e)
        {
            try
            {
                this.workingImage = (Bitmap)temporary.Clone();
                odszumionyObraz.Source = ImageSupporter.Bitmap2BitmapImage(workingImage);
            }
            catch(Exception ex)
            {

            }
        }

        public void OK()
        {
              this.workingImage = (Bitmap)temporary.Clone();
            odszumionyObraz.Source = ImageSupporter.Bitmap2BitmapImage(workingImage);
        }
        public void BackgroundOK()
        {
            try
            {
                this.workingImage = (Bitmap)temporary.Clone();
            }
            catch(Exception ex)
            {
                
            }
        }


        private void NormalizationBinder(object sender, RoutedEventArgs e)
        {
            try
            {
                Normalization();
                StartImageWindow(temporary);
            }
            catch(Exception ex)
            {

            }
            }

        private void Normalization()
        {
            try { 
            temporary = Filtrator.Normalization(workingImage);
            }
            catch (Exception ex)
            {

            }
        }
    


        

        

        private void MedianFilterBinder(object sender, RoutedEventArgs e)
        {
            MedianFilter();
            StartImageWindow(temporary);
        }
        private void MedianFilter()
        {
             temporary = Filtrator.MedianFilter((Bitmap)workingImage.Clone());
        }

        private void Histogram(object sender, RoutedEventArgs e)
        {
            Histogram();
            StartImageWindow(temporary);
        }

        private void Histogram()
        {
            temporary = Filtrator.Histogram((Bitmap)workingImage.Clone());
        }

        private void Segmentation(object sender, RoutedEventArgs e)
        {
            temporary = Filtrator.segmentation(30,(Bitmap)workingImage.Clone());
            ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(temporary));
            iw.Show();
        }

        private void Binaryzation(object sender, RoutedEventArgs e)
        {
            Binaryzation();
            temporary = Filtrator.Binaryzation((Bitmap)workingImage.Clone(),Int32.Parse(BinaryzationValue.Text));
            StartImageWindow(temporary);
        }

        private void BinaryzationIncrease(object sender, RoutedEventArgs e)
        {

            int value = Int32.Parse(BinaryzationValue.Text);
            BinaryzationValue.Text = (value + 3) + "";

        }
        private void BinaryzationDecrease(object sender, RoutedEventArgs e)
        {

            int value = Int32.Parse(BinaryzationValue.Text);

            int result = value - 3;
            if (result < 0) result = 0;
            BinaryzationValue.Text = (result) + "";

        }
        private void Binaryzation()
        {
            temporary = Filtrator.Binaryzation((Bitmap)workingImage.Clone(),Int32.Parse(BinaryzationValue.Text));
        }

        private void Gornoprzepustowy(object sender, RoutedEventArgs e)
        {
            temporary = Filtrator.HighPassFilter(EMask.Prewitt,(Bitmap)workingImage.Clone());
            ImageWindow iw = new ImageWindow(ImageSupporter.Bitmap2BitmapImage(temporary));
            iw.Show();
        }


        private void GridedHistogram(object sender, RoutedEventArgs e)
        {
            GridedHistogram();
             StartImageWindow(temporary);
        }

        private void GridedHistogram()
        {
            temporary = Filtrator.HistorgramGridedFilter((Bitmap)workingImage.Clone(), 32);
           
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
            try
            {
                temporary = (Bitmap)orginalBitmap.Clone();
                workingImage = (Bitmap)orginalBitmap.Clone();
                odszumionyObraz.Source = ImageSupporter.Bitmap2BitmapImage(workingImage);
            }
            catch(Exception ex)
            {

            }
                
            }
        public void PrzetwarzanieWstepne()
        {
            this.workingImage =  Filtrator.Normalization(workingImage);
            this.workingImage =  Filtrator.MedianFilter((Bitmap)workingImage.Clone());
            this.workingImage =  Filtrator.Histogram((Bitmap)workingImage.Clone());


        }

        private void ZastosujGaborFilter(object sender, RoutedEventArgs e)
        {
            //StartChosenGabor();
        }






        #endregion
        #region Metody Testowe
        //metody nieużywane do działąnia programu, ale istotne w trakcie testów i przygotowania pracy dyplomowej
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Gabor Filter Specyfikaction Click Listeners 

        /*  private void GaborFilterClick(object sender, RoutedEventArgs e)
       {
           workingImage = Filtrator.Normalization(orginalBitmap);
           fingerprint.startRecognition((Bitmap)workingImage.Clone());
           fingerprint.toDirectionMask();
           //wyświetlenie mapy kierunków



           mapakierunkow.Source = ImageSupporter.Bitmap2BitmapImage(workingImagefingerprint.localOrientationMap);

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
           }*/

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
//}
