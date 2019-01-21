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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BiometriaOdciskuPalca
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    
    public partial class ImageWindow : Window
    {
        public delegate void OK();
        private OK ok;
        public ImageWindow(ImageSource source, OK ok)
        {
            InitializeComponent();

            obrazek.Source = source;
            this.ok = ok;
        }
        public ImageWindow(ImageSource source)
        {
            InitializeComponent();

            obrazek.Source = source;
           
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Boolean flaga = true;
            while (flaga)
            {
                Random random = new Random();
                int i = random.Next();

                if (!File.Exists(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\screens\\" + i + ".png"))
                {
                    ImageSupporter.Save((BitmapImage)obrazek.Source, Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\screens\\" + i + ".png");
                    flaga = false;
                }
            }
        }

        public void changeImage(Bitmap bitmap)
        {
            obrazek.Source =ImageSupporter.Bitmap2BitmapImage(bitmap);
        }

        private void ScrollViewer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (ok.Method != null)
                {
                    ok.Invoke();
                    this.Close();
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}
