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
using System.Collections.Generic;

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
        private void OnKeyDownHandler_search(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                search_Click(sender, e);
                moveFocuss(e);
            }
        }
        private void OnKeyDownHandler_browser(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                wbMaps.Visibility = Visibility.Hidden;
            }
        }
        

        private void parseNumberOfStations()
        {
            if(number_textBox.Text != "how many stations do you want to find? (eg.3)")
            try
            {

                numberOfStations = int.Parse(number_textBox.Text);
                    if (numberOfStations > 29000)
                    {
                        number_textBox.Text = "we have in Poland 29849 stations ;)";
                        numberOfStations = 29849;
                    }
            }
            catch (Exception)
            {
                number_textBox.Text = "enter an integer";
            }
        }

        List<stationInfo> stations = new List<stationInfo>(); 
        private void search_Click(object sender, RoutedEventArgs e)
        {
            Lista.Items.Clear();
            Lista2.Items.Clear();
            stations.Clear();
            parseNumberOfStations();
            string address_ = $"{address_street.Text} {address_house_number.Text}, {address_city.Text}";
            positionCord p1 = GetPositionCord(address_);
            if(p1 == null)
            {
                Lista2.Items.Add("WRONG ADDRESS");
                goto Found;
            }
            Lista2.Items.Add($"{p1.Name}\n{p1.PostalCode}\n{p1.Locality}\n{p1.Region}\n{p1.Country}" +
                $"\nLongitude: {p1.Longitude}\nLatitude: {p1.Latitude}");
            


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

                    String sql = $"select top {numberOfStations} * from znajdzNajblizszeStacje({p1.Longitude}, {p1.Latitude}) order by Odległość";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                                Lista.Items.Add($"{Lista.Items.Count+1}\t{reader.GetString(0)}\t{reader.GetString(1)}\t{reader.GetString(2)}" +
                                    $"\t{reader.GetString(3)}\n\tDistance: {Math.Round(reader.GetDouble(6))}m");
                                stations.Add(new stationInfo(Lista.Items.Count + 1, reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3),
                                    reader.GetDouble(4).ToString().Replace(',', '.'), reader.GetDouble(5).ToString().Replace(',', '.'), reader.GetDouble(6)));
                            }
                        }
                    }
                }
            }
            catch (SqlException y)
            {
                Lista.Items.Add(y.Message);
            }
        Found:
            ;

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
            try { 
                PositionFromApi cordInfoDeselialize = JsonConvert.DeserializeObject<PositionFromApi>(cordInfo);
                positionCord coords = new positionCord(cordInfoDeselialize.data[0].Longitude, cordInfoDeselialize.data[0].Latitude, cordInfoDeselialize.data[0].Name
                    , cordInfoDeselialize.data[0].Postal_Code, cordInfoDeselialize.data[0].Region, cordInfoDeselialize.data[0].Locality, cordInfoDeselialize.data[0].Country);
            return coords;
            }catch (Exception e)
            {
                return null;
            }
        }


        private void address_city_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        }
        private void address_street_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        }

        private void address_house_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        }

        private void city_GotFocuss(object sender, RoutedEventArgs e)
        {
            var txtControl = sender as System.Windows.Controls.TextBox;
            txtControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtControl.SelectAll();
            }));
        }
        private void addres_GotFocuss(object sender, RoutedEventArgs e)
        {
            var txtControl = sender as System.Windows.Controls.TextBox;
            txtControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtControl.SelectAll();
            }));
        }
        private void address_house_GotFocuss(object sender, RoutedEventArgs e)
        {
            var txtControl = sender as System.Windows.Controls.TextBox;
            txtControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtControl.SelectAll();
            }));
        }
        private void number_textbox_GotFocuss(object sender, RoutedEventArgs e)
        {
            var txtControl = sender as System.Windows.Controls.TextBox;
            txtControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtControl.SelectAll();
            }));
        }
        private void SelectedItem_Lista(object sender, RoutedEventArgs e)
        {
            Lista.Items.Add(Lista.SelectedItem);
            for (int i = 0; i < stations.Count; i++)
            {
                if (Lista.SelectedItem.ToString().Contains(stations[i].Id))
                { 
                    wbMaps.Visibility = Visibility.Visible;
                    wbMaps.Navigate($"https://www.google.com/maps/search/?api=1&query={stations[i].Longitude}%2C{stations[i].Latitude}" +
                        $"&query_place_id={stations[0].Id}");
                }
                
            }
            
        }
    }
}
