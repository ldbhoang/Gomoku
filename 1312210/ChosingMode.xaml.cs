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

namespace _1312210
{
    /// <summary>
    /// Interaction logic for ChosingMode.xaml
    /// </summary>
    public partial class ChosingMode : Window
    {
        public static int mode = 0;
        public ChosingMode()
        {
            InitializeComponent();
        }

        private void cm_btn_vsPC_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            mode = 1;
            MainWindow mw = new MainWindow();
            mw.ShowDialog();
            this.Show();
        }

        private void cm_btn_1vs1_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            mode = 2;
            MainWindow mw = new MainWindow();
            mw.ShowDialog();
            this.Show();
        }

        private void cm_btn_onl_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            mode = 3;
            MainWindow mw = new MainWindow();
            mw.ShowDialog();
            this.Show();
        }

        private void cm_btn_bot_onl_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            mode = 4;
            MainWindow mw = new MainWindow();
            mw.ShowDialog();
            this.Show();
        }

    }
}
