using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Graph1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int currentGraphType = 0;
        DataTable table = new DataTable("Table");
        string serverConn = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|ProcCodes.mdf; Integrated Security=True";
        SqlConnection connection;
        string selectProcCodes = "SELECT DISTINCT ProcCode FROM dbo.Codes ORDER BY ProcCode";
        string selectAreas = "SELECT DISTINCT Area FROM dbo.Codes ORDER BY Area";
        DataRow addedRow;

        public MainWindow()
        {
            InitializeComponent();

            //dinamically create columns in Table
            DataColumn fNameColumn = new DataColumn();
            fNameColumn.DataType = System.Type.GetType("System.Boolean");
            fNameColumn.ColumnName = "T/F";
            fNameColumn.DefaultValue = false;
            table.Columns.Add(fNameColumn);

            fNameColumn = new DataColumn();
            fNameColumn.DataType = System.Type.GetType("System.String");
            fNameColumn.ColumnName = "Code";
            table.Columns.Add(fNameColumn);

            List<string> procCodesList = new List<string>();

            //connect to DB and get list of all proc codes and all areas
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(serverConn);
            using (connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = selectProcCodes;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;

                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        addedRow = table.NewRow();
                        addedRow[1] = reader[0];
                        table.Rows.Add(addedRow);
                        procCodesList.Add(reader[0].ToString());
                    }
                    reader.Close();

                    dataGrid.AutoGenerateColumns = true;
                    dataGrid.ItemsSource = table.DefaultView;
                    dataGrid.ColumnWidth = 50;
                    dataGrid.CanUserDeleteRows = false;
                    dataGrid.CanUserAddRows = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            List<int> areasList = new List<int>();
            //list of all Areas            
            using (connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = selectAreas;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;

                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        areasList.Add(Int32.Parse(reader[0].ToString()));
                    }
                    reader.Close();

                    dataGrid.AutoGenerateColumns = true;
                    dataGrid.ItemsSource = table.DefaultView;
                    dataGrid.ColumnWidth = 50;
                    dataGrid.CanUserDeleteRows = false;
                    dataGrid.CanUserAddRows = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            comboBox1.ItemsSource = areasList;
            comboBox1.SelectedIndex = 0;

            comboBox.ItemsSource = areasList;
            comboBox.SelectedIndex = 0;

            comboBox2.ItemsSource = areasList;
            comboBox2.SelectedIndex = 16;

            comboBox3.ItemsSource = procCodesList;
            comboBox3.SelectedIndex = 0;

            currentGraphType = -1;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            //if the same graph then clear else add another one
            if (currentGraphType != comboBox.SelectedIndex)
            {
                mcChart.Series.Clear();
                mcChart.Axes.Clear();

                if (comboBox.SelectedIndex != 3)
                {

                    addAxes(textBox.Text, textBox1.Text);
                }
                currentGraphType = comboBox.SelectedIndex;
            }


            List<KeyValuePair<int, int>> testList = new List<KeyValuePair<int, int>>();
            List<KeyValuePair<string, int>> testListPie = new List<KeyValuePair<string, int>>();

            //convert each row into KeyValuePair
            for (int i = 0; i < dataGrid.Items.Count - 1; i++)
            {
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                TextBlock cellContentX = dataGrid.Columns[1].GetCellContent(row) as TextBlock;
                TextBlock cellContentY = dataGrid.Columns[2].GetCellContent(row) as TextBlock;

                if (comboBox.SelectedIndex == 3) //for pie chart
                    testListPie.Add(new KeyValuePair<string, int>(cellContentX.Text, int.Parse(cellContentY.Text)));
                else
                    try
                    {
                        testList.Add(new KeyValuePair<int, int>(int.Parse(cellContentX.Text), int.Parse(cellContentY.Text)));
                    }
                    catch
                    {
                        MessageBox.Show("Check table/Type of graph");
                        return;
                    }
            }

            string title = textBox2.Text;

            switch (comboBox.SelectedIndex)
            {
                case 0:
                    LineSeries mySeriesLine = new LineSeries();
                    mySeriesLine.Title = title;
                    mySeriesLine.IndependentValueBinding = new Binding("Key");
                    mySeriesLine.DependentValueBinding = new Binding("Value");
                    mySeriesLine.ItemsSource = testList;
                    mcChart.Series.Add(mySeriesLine);
                    break;
                case 1:
                    BarSeries mySeriesBar = new BarSeries();
                    mySeriesBar.Title = title;
                    mySeriesBar.IndependentValueBinding = new Binding("Key");
                    mySeriesBar.DependentValueBinding = new Binding("Value");
                    mySeriesBar.ItemsSource = testList;
                    mcChart.Series.Add(mySeriesBar);
                    break;
                case 2:
                    //mySeries = new ColumnSeries();  
                    ColumnSeries mySeriesLineColumn = new ColumnSeries();
                    mySeriesLineColumn.Title = title;
                    mySeriesLineColumn.IndependentValueBinding = new Binding("Key");
                    mySeriesLineColumn.DependentValueBinding = new Binding("Value");
                    mySeriesLineColumn.ItemsSource = testList;
                    mcChart.Series.Add(mySeriesLineColumn);
                    break;
                case 3:
                    //mySeries = new PieSeries();
                    PieSeries mySeriesPie = new PieSeries();
                    mySeriesPie.Title = title;
                    mySeriesPie.IndependentValueBinding = new Binding("Key");
                    mySeriesPie.DependentValueBinding = new Binding("Value");
                    mySeriesPie.ItemsSource = testListPie;
                    mcChart.Series.Add(mySeriesPie);
                    break;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            mcChart.Series.Clear();
            mcChart.Axes.Clear();
            currentGraphType = -1;
        }

        private void addAxes(string axesX, string axesY)
        {
            //add X axis
            LinearAxis axisX = new LinearAxis();
            axisX.Orientation = AxisOrientation.X;
            axisX.Title = axesX;
            axisX.ShowGridLines = true;
            if (comboBox.SelectedIndex == -1)
                axisX.Minimum = 0;
            else
                axisX.Minimum = int.Parse(comboBox.SelectedValue.ToString());

            if (comboBox2.SelectedIndex == -1)
                axisX.Maximum = 100;
            else
                axisX.Maximum = int.Parse(comboBox2.SelectedValue.ToString());

            mcChart.Axes.Add(axisX);

            //add Y axis
            LinearAxis axisY = new LinearAxis();
            axisY.Orientation = AxisOrientation.Y;
            axisY.Title = axesY;
            axisY.ShowGridLines = true;
            mcChart.Axes.Add(axisY);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {

            mcChart.Series.Clear();
            mcChart.Axes.Clear();

            //select proccode, ratio from dbo.Codes where Area = 10 and (ProcCode = 'CCC' OR ProcCode = 'CAA')
            //select all selected Codes with selected Area
            string selectCodes = "SELECT proccode, ratio FROM dbo.Codes WHERE Area = ";
            string whereString = "AND (";
            List<KeyValuePair<string, float>> testListPie = new List<KeyValuePair<string, float>>();
            float sum = 0;

            for (int i = 0; i < dataGrid.Items.Count - 1; i++)
            {
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                CheckBox cellContentX = dataGrid.Columns[0].GetCellContent(row) as CheckBox;
                TextBlock cellContentY = dataGrid.Columns[1].GetCellContent(row) as TextBlock;

                if (cellContentX.IsChecked == true)
                    whereString = whereString + "ProcCode = '" + cellContentY.Text + "' OR ";
            }

            whereString = whereString + "ProcCode = '')";

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(serverConn);
            using (connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = selectCodes + comboBox1.SelectedValue + whereString;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        testListPie.Add(new KeyValuePair<string, float>(reader[0].ToString(), float.Parse(reader[1].ToString())));
                        sum = sum + float.Parse(reader[1].ToString()) * int.Parse(comboBox1.SelectedValue.ToString());
                        //Console.WriteLine(reader[0].ToString() + "  " + reader[1].ToString());
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            PieSeries mySeriesPie = new PieSeries();
            mySeriesPie.Title = "Pie chart";
            mySeriesPie.IndependentValueBinding = new Binding("Key");
            mySeriesPie.DependentValueBinding = new Binding("Value");
            mySeriesPie.ItemsSource = testListPie;
            mcChart.Series.Add(mySeriesPie);

            textBox3.Text = sum.ToString();

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {

            mcChart.Series.Clear();
            mcChart.Axes.Clear();
            textBox.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";

            addAxes("Area [in2]", "Ratio [$/in2]");
            //select area, ratio from dbo.Codes where ProcCode = 'CCC'
            for (int i = 0; i < dataGrid.Items.Count - 1; i++)
            {

                string selectCodes = "SELECT area, ratio FROM dbo.Codes WHERE ProcCode = ";
                List<KeyValuePair<int, float>> testList = new List<KeyValuePair<int, float>>();
                string currentCode = "";

                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                CheckBox cellContentX = dataGrid.Columns[0].GetCellContent(row) as CheckBox;
                TextBlock cellContentY = dataGrid.Columns[1].GetCellContent(row) as TextBlock;

                if (cellContentX.IsChecked == true)
                {
                    currentCode = cellContentY.Text;
                    selectCodes = selectCodes + "'" + cellContentY.Text + "'";
                }
                else
                {
                    continue;
                }

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(serverConn);
                using (connection = new SqlConnection(builder.ConnectionString))
                {
                    try
                    {
                        connection.Open();

                        SqlCommand cmd = new SqlCommand();
                        SqlDataReader reader;

                        cmd.CommandText = selectCodes;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = connection;

                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            testList.Add(new KeyValuePair<int, float>(int.Parse(reader[0].ToString()), float.Parse(reader[1].ToString())));
                            //Console.WriteLine(reader[0].ToString() + "  " + reader[1].ToString());
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                LineSeries mySeriesLine = new LineSeries();
                mySeriesLine.Title = currentCode.Trim();
                mySeriesLine.IndependentValueBinding = new Binding("Key");
                mySeriesLine.DependentValueBinding = new Binding("Value");
                mySeriesLine.ItemsSource = testList;

                mcChart.Series.Add(mySeriesLine);
            }
        }

        private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var xAxis = this.mcChart.ActualAxes.OfType<LinearAxis>().FirstOrDefault(ax => ax.Orientation == AxisOrientation.X);
            if (xAxis != null)
                xAxis.Maximum = int.Parse(comboBox2.SelectedValue.ToString());
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var xAxis = this.mcChart.ActualAxes.OfType<LinearAxis>().FirstOrDefault(ax => ax.Orientation == AxisOrientation.X);
            if (xAxis != null)
                xAxis.Minimum = int.Parse(comboBox.SelectedValue.ToString());
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            if (textBox4.Text == "")
            {
                MessageBox.Show("Fill Area");
                return;
            }

            if (textBox5.Text == "")
            {
                MessageBox.Show("Fill Ratio");
                return;
            }

            try
            {
                var lineGraph = mcChart.Series.OfType<LineSeries>().FirstOrDefault(ax => ax.Title.ToString() == comboBox3.Text.Trim());
                List<KeyValuePair<int, float>> testListNew = (List<KeyValuePair<int, float>>)lineGraph.ItemsSource;
                testListNew.Add(new KeyValuePair<int, float>(int.Parse(textBox4.Text), float.Parse(textBox5.Text)));

                lineGraph.ItemsSource = testListNew;
                lineGraph.Refresh();
            }
            catch
            { }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (textBox4.Text == "")
            {
                MessageBox.Show("Fill Area");
                return;
            }

            if (textBox5.Text == "")
            {
                MessageBox.Show("Fill Ratio");
                return;
            }

            string SQLquery = "INSERT INTO dbo.Codes VALUES ('" + comboBox3.SelectedValue.ToString().Trim() + "', " + textBox4.Text + ", " + textBox5.Text.Replace(",", ".") + ")";
            runSQLquery(SQLquery);
        }

        private void runSQLquery(string SQLCommand)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(serverConn);
            using (connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = SQLCommand;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;

                    reader = cmd.ExecuteReader();
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        
        private void mcChart_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            /*    var series = this.mcChart.Series.FirstOrDefault();
                if (series == null) return;

                var position = e.GetPosition(series);
                var xAxis = this.mcChart.ActualAxes.OfType<LinearAxis>().FirstOrDefault(ax => ax.Orientation == AxisOrientation.X);

                Console.WriteLine(String.Format("X: {0}, Y: {1}", position.X, position.Y));
                Console.WriteLine(xAxis.GetPlotAreaCoordinate(position.X).Value);*/

            /*  IInputElement lInputElement = sender as IInputElement; // == Chart, LineSeries ...
              Chart lChart = sender as Chart;
              LineSeries lLineSeries = (LineSeries) mcChart.Series.FirstOrDefault(); //sender as LineSeries;

              Point lPoint = e.GetPosition(lInputElement);
              if (lChart != null)
              {
                  IInputElement lSelection = lChart.InputHitTest(lPoint);

                  //IInputElement lSelection = lChart.InputHitTest(lPoint);
                  if (lSelection == null) return;                
                  Console.WriteLine(lSelection.GetType().ToString());
                 // lSelection.DataContext.Key; //x
                  //lSelection.DataContext.Value; //y

              }
              else if (lLineSeries != null)
              {
                  IInputElement lSelection = lLineSeries.InputHitTest(lPoint);
                  if (lSelection == null) return;
                  Console.WriteLine(lSelection.GetType().ToString());
              }*/

        }
    }
}
