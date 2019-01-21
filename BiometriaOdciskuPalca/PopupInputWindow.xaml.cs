using System;
using System.Collections.Generic;
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
    /// Interaction logic for PopupInputWindow.xaml
    /// </summary>
    public partial class PopupInputWindow : Window
    {
        public delegate void InputReader(String text);
        InputReader ir;
       // String messageText;
        public PopupInputWindow(InputReader inputReader,String messageText,String title)
        {
            InitializeComponent();
            ir = inputReader;
           // this.messageText = messageText;
            textBox.Content = messageText;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.Title = title;
        }

            private void AddCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TextBox.Text))
            e.CanExecute = true;
            else   e.CanExecute = false;

        }

        private void AddCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ir(TextBox.Text);
            this.Close();
        }

    }








}
