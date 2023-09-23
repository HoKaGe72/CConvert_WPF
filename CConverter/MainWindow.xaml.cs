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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Xml.Linq;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Reflection.Emit;
using System.Xml;
using System.Text.RegularExpressions;

namespace CConverter
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SQLiteConnection SQLiteConn;
        private DataTable dTable, dT;
        double kontrol = 0;
        int dt_kontrol = 0;
        string data;
        public MainWindow()
        {
            InitializeComponent();
            //comboBox1.KeyDown += (sender, e) => e.Handled = true;
            comboBox2.KeyDown += (sender, e) => e.Handled = true;
            kol.Text = "1";
            SQLiteConn = new SQLiteConnection();
            dTable = new DataTable();
            dT = new DataTable();
            ConnectDB(); // Подключение к БД
            ShowTable(SQL_AllTable()); // Чтение БД и Заполнение табл.
            SQLiteCommand SQLT = SQLiteConn.CreateCommand();
            SQLT.CommandText = "SELECT data FROM ДатаПоследногоОбновления WHERE id=1";
            SQLiteDataReader reader1 = SQLT.ExecuteReader();
            while (reader1.Read())
            {
                data = reader1[0].ToString();// Дата последного обновления валют
            }
            Obnov();// Обновление курса валют 
        }
        private void ConnectDB()
        {
            SQLiteConn = new SQLiteConnection("Data Source = Курс.sqlite");
            SQLiteConn.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = SQLiteConn;
        }
        private string SQL_AllTable()
        {
            return "SELECT * FROM [Курс];";
        }

        private void ShowTable(string SQLQuery)
        {
            dTable.Clear();
            SQLiteDataAdapter piu = new SQLiteDataAdapter(SQLQuery, SQLiteConn);
            piu.Fill(dTable);
            comboBox1.Items.Add("RUB");
            comboBox2.Items.Add("RUB");
            for (int j = 0; j < dTable.Rows.Count; j++)
            {
                comboBox1.Items.Add(Convert.ToString(dTable.Rows[j][1]));
                comboBox2.Items.Add(Convert.ToString(dTable.Rows[j][1]));
            }

        }

        private void rakirovka_Click(object sender, RoutedEventArgs e)
        {
            string a, b;
            a = comboBox1.Text;
            b = comboBox2.Text;
            comboBox1.Text = b;
            comboBox2.Text = a;
            if (kontrol == 1)
            {
                Perevod();
            }
        }
        private void Perevod()
        {
            string cb1, cb2, zn;
            if (comboBox1.Text == "" || comboBox2.Text == "")
            {
                MessageBox.Show("Перевод невозможен!Укажите валюту.", "Внимание!");
                return;
            }
            else
            {
                cb1 = comboBox1.SelectedItem.ToString();
                cb2 = comboBox2.SelectedItem.ToString();
            }
            if (kol.Text == "")
            {
                MessageBox.Show("Введите количество валюты для перевода!", "Внимание!");
                return;
            }
            else { zn = kol.SelectedText.ToString(); }

            if (comboBox1.Text == comboBox2.Text)
            {
                rez.Text = kol.Text + " " + cb1 + " = " + kol.Text + " " + cb2;
            }
            else
            {
                var rezul = Raschot(cb1, cb2);
                rez.Text = kol.Text + " " + cb1 + " = " + rezul + " " + cb2;
            }

            //var parsY = Parsing(Url: "https://cash.rbc.ru/converter.html?from=" + from + "&to=" + to + "&sum=" + textBox1.Text + "&date=&rate=cbrf");
            //label7.Text = Convert.ToString(parsY);
        }

        private void perevod_Click(object sender, RoutedEventArgs e)//кнопка "Перевод"
        {
            kontrol = 1;
            Perevod();
        }

        private object Raschot(string cb1, string cb2)
        {
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                if (cb1 == "RUB")
                {
                    if (cb2 == Convert.ToString(dTable.Rows[i][1]))
                    {
                        double rez = Math.Round(Convert.ToDouble(kol.Text) / (Convert.ToDouble(dTable.Rows[i][4]) / Convert.ToDouble(dTable.Rows[i][2])), 4);
                        return rez;
                    }
                }
                if (cb2 == "RUB")
                {
                    if (cb1 == Convert.ToString(dTable.Rows[i][1]))
                    {
                        double rez = Math.Round(Convert.ToDouble(kol.Text) * (Convert.ToDouble(dTable.Rows[i][4]) / Convert.ToDouble(dTable.Rows[i][2])), 4);
                        return rez;
                    }
                }
                if (cb1 != "RUB" && cb2 != "RUB")
                {
                    if (cb1 == Convert.ToString(dTable.Rows[i][1]))
                    {
                        double Vrub = Convert.ToDouble(kol.Text) * (Convert.ToDouble(dTable.Rows[i][4]) / Convert.ToDouble(dTable.Rows[i][2]));
                        if (Vrub != 0)
                        {
                            for (int j = 0; j < dTable.Rows.Count; j++)
                            {
                                if (cb2 == Convert.ToString(dTable.Rows[j][1]))
                                {
                                    double IZrub = Math.Round(Vrub / (Convert.ToDouble(dTable.Rows[j][4]) / Convert.ToDouble(dTable.Rows[j][2])), 4);
                                    return IZrub;
                                }
                            }
                        }
                        else { return "off"; }
                    }
                }
            }
            return "Ошибка перевода";
        }
        private void XmlLinq()
        {
            double[] stkurs = new double[dTable.Rows.Count];
            double[] kode = new double[dTable.Rows.Count];
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                kode[i] = Convert.ToDouble(dTable.Rows[i][0]);
                stkurs[i] = Convert.ToDouble(dTable.Rows[i][4]);
            }
            XDocument xdoc = XDocument.Load("https://www.cbr-xml-daily.ru/daily.xml");
            for (int i = 0; dTable.Rows.Count > i; i++)
            {
                var Val = xdoc.Element("ValCurs").Elements("Valute").Where(p => p.Element("CharCode").Value == "" + dTable.Rows[i][1] + "").Select(
                    p => new { value = p.Element("Value").Value });
                if (Val != null)
                {
                    foreach (var Valute in Val)
                    {
                        double V = Convert.ToDouble(Valute.value);
                        dTable.Rows[i][4] = V;
                    }
                }
            }
        }
        private void Obnov()
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load("https://www.cbr-xml-daily.ru/daily.xml");
                XmlElement xRoot = xDoc.DocumentElement;
                if (xRoot != null)
                {
                    var datatable = xRoot.Attributes.GetNamedItem("Date");
                    if (Convert.ToString(datatable.Value) != data)
                    {
                        data = Convert.ToString(datatable.Value);
                        SQLiteCommand _cmd = new SQLiteCommand();
                        _cmd.CommandText = @"UPDATE ДатаПоследногоОбновления SET data=@A WHERE id=1";
                        _cmd.Connection = SQLiteConn;
                        _cmd.Parameters.Add(new SQLiteParameter("@A", data.ToString().Replace(',', '.')));
                        _cmd.ExecuteNonQuery();
                        XmlLinq();
                        coxpaH();
                        Statice();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Отсутствует подключение к интернету.\nПроверти подключение к интернету и перезапустите программу, чтобы получить последние обновления по курсам валют.", "Внимание");
            }
        }
        private void coxpaH()
        {
            SQLiteCommand _cmd = new SQLiteCommand();
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                _cmd.CommandText = @"UPDATE Курс SET Курс=@A WHERE ЦифрКод='" + dTable.Rows[i][0].ToString() + "'";
                _cmd.Connection = SQLiteConn;
                _cmd.Parameters.Add(new SQLiteParameter("@A", dTable.Rows[i][4].ToString().Replace(',', '.')));
                _cmd.ExecuteNonQuery();
            }
        }

        private void kurs_Click(object sender, RoutedEventArgs e)//открыть окно для просмотра курса валют
        {
            KursValue kursValue = new KursValue(dTable,data);
            kursValue.Show();
        }

        private void kol_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (kol.Text != "" && comboBox1.Text != "" && comboBox2.Text != "")
            {
                Perevod();
            }
        }

        private void kol_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            double val;
            e.Handled = !double.TryParse(fullText, out val);
        }
        private string SQL_AllTable2()
        {
            return "SELECT * FROM [Статистика];";
        }
        private void ShowTable2(string SQLQuery)
        {
            dT.Clear();
            SQLiteDataAdapter piu = new SQLiteDataAdapter(SQLQuery, SQLiteConn);
            piu.Fill(dT);
            dt_kontrol++;
        }
        private void static_Click(object sender, RoutedEventArgs e)
        {
            if(dt_kontrol==0)
            {
                ShowTable2(SQL_AllTable2());
            }
            Statistica statistica = new Statistica(dT);
            statistica.Show();
        }
        private int ddd()
        {
            string del_data = "";
            SQLiteCommand _cmd = new SQLiteCommand();
            _cmd.Connection = SQLiteConn;
            _cmd.CommandText = "SELECT data FROM ДатаДляУдаления WHERE id=1";
            SQLiteDataReader reader1 = _cmd.ExecuteReader();
            while (reader1.Read())
            {
                del_data = reader1[0].ToString();
            }
            return Convert.ToInt32(del_data);
        }
        private void del_ROW()
        {
            int del_data = ddd();
            SQLiteCommand _cmd = new SQLiteCommand();
            _cmd.Connection = SQLiteConn;                      
            _cmd.CommandText = "DELETE FROM Статистика WHERE _rowid_ = " + del_data + " ;";
            _cmd.ExecuteNonQuery();
            del_data++;
            ob_ddd(del_data);
        }
        private void ob_ddd(int idrow)
        {
            SQLiteCommand _cmd = new SQLiteCommand();
            _cmd.CommandText = @"UPDATE ДатаДляУдаления SET data=@A WHERE id=1";
            _cmd.Connection = SQLiteConn;
            _cmd.Parameters.Add(new SQLiteParameter("@A", idrow));
            _cmd.ExecuteNonQuery();
        }
        private void Statice()
        {
            //del_ROW();
            SQLiteCommand _cmd = new SQLiteCommand();
            _cmd.Connection = SQLiteConn;
            _cmd.CommandText = "INSERT INTO Статистика (Дата) VALUES ('" + data.ToString() + "')";
            _cmd.ExecuteNonQuery();
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                _cmd.CommandText = @"UPDATE Статистика SET '" + dTable.Rows[i][1] + "'=@A WHERE Дата='" + data.ToString() + "'";
                _cmd.Connection = SQLiteConn;
                _cmd.Parameters.Add(new SQLiteParameter("@A", dTable.Rows[i][4].ToString().Replace(',', '.')));
                _cmd.ExecuteNonQuery();
            }
        }
    }
}
