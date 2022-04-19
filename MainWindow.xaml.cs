using NearestOperator.PositionApi;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Input;

namespace NearestOperator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        int numberOfStations = 3;
        private void moveFocuss(KeyEventArgs e)
        {
            var uie = e.OriginalSource as UIElement;
            if (e.Key == Key.Return)
            {
                e.Handled = true;
                uie.MoveFocus(
                new TraversalRequest(
                FocusNavigationDirection.Next));
            }
        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            moveFocuss(e);
        }
        private void OnKeyDownHandler_number(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                parseNumberOfStations();
                moveFocuss(e);
            }
        }

        private void parseNumberOfStations()
        {
            try
            {
                numberOfStations = int.Parse(number_textBox.Text);
            }
            catch (Exception)
            {
                number_textBox.Text = "enter an integer";
            }
        }

        string address_;
        private void search_Click(object sender, RoutedEventArgs e)
        {
            Lista.Items.Clear();
            parseNumberOfStations();
            string address_ = $"{address_street.Text} {address_house_number.Text}, {address_city.Text}";
            positionCord p1 = GetPositionCord(address_);
            //address_city.Text = $"{p1.Longitude.ToString().Replace(',','.')}, {p1.Latitude.ToString().Replace(',', '.')}";
            


            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = "warszawka-serwer.database.windows.net";
                builder.UserID = "sqlserver";
                builder.Password = "Dupa1234";
                builder.InitialCatalog = "lte1800";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    //Console.WriteLine("\nQuery data example:");
                    //Console.WriteLine("=========================================\n");

                    connection.Open();

                    String sql = $"select top {numberOfStations} * from znajdzNajblizszeStacje({p1.Longitude.ToString().Replace(',', '.')}, {p1.Latitude.ToString().Replace(',', '.')}) order by Odległość";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                                Lista.Items.Add($"{Lista.Items.Count+1}\t{reader.GetString(0)}\t{reader.GetString(1)}\t{reader.GetString(2)}" +
                                    $"\t{reader.GetString(3)}\n\tDistance: {Math.Round(reader.GetDouble(6))}m");
                            }
                        }
                    }
                }
            }
            catch (SqlException y)
            {
                Console.WriteLine(y.ToString());
            }
            Console.WriteLine("\nDone. Press enter.");
            Console.ReadLine();


        }
        static string GetCoordFromAPI(string address_)
        {
            string responseFromServer;
            WebRequest request = WebRequest.Create(
                $"http://api.positionstack.com/v1/forward?access_key=fc2fb33c25ff120b0f7343ef1cb34a3b&query=" + address_);
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        responseFromServer = reader.ReadToEnd();

                    }
                }
                return responseFromServer;
            }
            catch (WebException)
            {
                return null;
            }
        }
        static positionCord GetPositionCord(string address_)
        {
            string cordInfo = GetCoordFromAPI(address_);
            if (cordInfo != null)
            {
                return createPositionCoordObject(cordInfo);
            }
            else
                return null;
        }
        private static positionCord createPositionCoordObject(string cordInfo)
        {
            PositionFromApi cordInfoDeselialize = JsonConvert.DeserializeObject<PositionFromApi>(cordInfo);
            positionCord coords = new positionCord(cordInfoDeselialize.data[0].Longitude,cordInfoDeselialize.data[0].Latitude);
            return coords;
        }


        private void address_city_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void address_street_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void address_house_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void city_GotFocuss(object sender, RoutedEventArgs e)
        {
            var txtControl = sender as TextBox;
            txtControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtControl.SelectAll();
            }));
        }
        private void addres_GotFocuss(object sender, RoutedEventArgs e)
        {
            var txtControl = sender as TextBox;
            txtControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtControl.SelectAll();
            }));
        }
        private void address_house_GotFocuss(object sender, RoutedEventArgs e)
        {
            var txtControl = sender as TextBox;
            txtControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtControl.SelectAll();
            }));
        }
        private void number_textbox_GotFocuss(object sender, RoutedEventArgs e)
        {
            var txtControl = sender as TextBox;
            txtControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtControl.SelectAll();
            }));
        }
        
    }
}
