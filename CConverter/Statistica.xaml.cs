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
using LiveCharts;
using System.ComponentModel;
using LiveCharts.Wpf.Charts.Base;
using LiveCharts.Wpf;
using System.Data;
using System.Collections;

namespace CConverter
{
    /// <summary>
    /// Логика взаимодействия для Statistica.xaml
    /// </summary>
    public partial class Statistica : Window
    {
        DataTable dt;
        public Statistica(DataTable dtable)
        {
            
            InitializeComponent();
            dt = dtable;
            ShowTable(dt);
            SeriesCollection = new SeriesCollection();
        }

        private void val_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Chart.Series != null) { Chart.Series.Clear();}
            
            string k = Convert.ToString(cb.SelectedValue);
            Graf(k);
        }
        public void Graf(string k)
        {
            Labels = dt.AsEnumerable().Select(row => row.Field<string>("Дата")).ToArray();
            YFormatter = value => value.ToString("C");
            SeriesCollection.Add(new LineSeries
            {
                Title = k,
                Values = new ChartValues<double> (dt.AsEnumerable().Select(x => x.Field<double>(k)).ToArray()),
                LineSmoothness = 0,
                PointGeometrySize = 5, PointForeground = Brushes.White
            });
            DataContext = this;
        }
        private void ShowTable(DataTable dTable)
        {
            DataGrid1.ItemsSource = dt.DefaultView;
            for (int i = 1; i < dTable.Columns.Count; i++)
            {
                cb.Items.Add(Convert.ToString(dTable.Columns[i]));
            }
        }
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
    }
}
