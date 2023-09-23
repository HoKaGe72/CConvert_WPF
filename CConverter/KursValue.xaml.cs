using System;
using System.Collections.Generic;
using System.Data;
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

namespace CConverter
{
    /// <summary>
    /// Логика взаимодействия для KursValue.xaml
    /// </summary>
    public partial class KursValue : Window
    {
        public KursValue(DataTable dt,string data)
        {
            InitializeComponent();
            txt.Text = txt.Text + " " + data;
            DataGrid.ItemsSource = dt.DefaultView;
        }
    }
}
