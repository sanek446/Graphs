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
using System.Windows.Controls.DataVisualization;
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
        float oldRatio = 0;

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
                        procCodesList.Add(reader[0].ToString().Trim());
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
            textBlock.Text = "";
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
                        
            //select all selected Codes with selected Area
            string selectCodes = "SELECT proccode, ratio FROM dbo.Codes WHERE Area = ";
            string whereString = "AND (";
            List<KeyValuePair<string, float>> testListPie = new List<KeyValuePair<string, float>>();
            float sum = 0;

            for (int i = 0; i < dataGrid.Items.Count; i++)
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
                        
            textBlock.Text = "Total cost: " + sum.ToString();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {

            mcChart.Series.Clear();
            mcChart.Axes.Clear();            

            addAxes("Area [in2]", "Ratio [$/in2]");
            //select area, ratio from dbo.Codes where ProcCode = 'CCC'
            for (int i = 0; i < dataGrid.Items.Count; i++)
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
                mySeriesLine.DataContext = testList;

                mcChart.Series.Add(mySeriesLine);
                mySeriesLine.MouseLeftButtonDown += new MouseButtonEventHandler(LineSeries_MouseLeftButtonUp);                                
            }
        }

        void LineSeries_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {        
            var element = Mouse.DirectlyOver as DependencyObject;
            while (element != null && !(element is LineDataPoint))
            {
                element = VisualTreeHelper.GetParent(element);
            }
            if (element != null)
            {
                var columnDataPoint = element as LineDataPoint;
                //X                
                textBox4.Text = columnDataPoint.FormattedIndependentValue;

                //Y                
                textBox5.Text = columnDataPoint.FormattedDependentValue;
                oldRatio = float.Parse(columnDataPoint.FormattedDependentValue);

                comboBox3.SelectedValue = ((LineSeries)sender).Title.ToString();

                double x = double.Parse(columnDataPoint.FormattedIndependentValue);
                double y = double.Parse(columnDataPoint.FormattedDependentValue);
                double z = x * y;
                
                textBlock.Text = " Proc code: " + ((LineSeries)sender).Title.ToString() + "\r\n Area: " + columnDataPoint.FormattedIndependentValue + " \r\n Ratio: " + columnDataPoint.FormattedDependentValue + " \r\n Cost: " + z.ToString();
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
                oldRatio = float.Parse(textBox5.Text);
                lineGraph.ItemsSource = testListNew;
                lineGraph.Refresh();
            }
            catch
            { }

            //also add Point into DB
            string SQLquery = "INSERT INTO dbo.Codes VALUES ('" + comboBox3.SelectedValue.ToString().Trim() + "', " + textBox4.Text + ", " + textBox5.Text.Replace(",", ".") + ")";
            if (runSQLquery(SQLquery) == true)
                Console.WriteLine("Point has been added");

        }

        private bool runSQLquery(string SQLCommand)
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
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
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

            try
            {
                var lineGraph = mcChart.Series.OfType<LineSeries>().FirstOrDefault(ax => ax.Title.ToString() == comboBox3.Text.Trim());
                List<KeyValuePair<int, float>> testListNew = (List<KeyValuePair<int, float>>)lineGraph.ItemsSource;
                testListNew.Remove(new KeyValuePair<int, float>(int.Parse(textBox4.Text), float.Parse(textBox5.Text)));
                oldRatio = 0;
                lineGraph.ItemsSource = testListNew;
                lineGraph.Refresh();
            }
            catch
            { }

            //also delete Point from DB
            string SQLquery = "DELETE FROM dbo.Codes WHERE ProcCode = '" + comboBox3.SelectedValue.ToString().Trim() + "' AND Area =  " + textBox4.Text + " AND Ratio = " + textBox5.Text.Replace(",", ".");
            if (runSQLquery(SQLquery) == true)
                Console.WriteLine("Point has been deleted");
        }

        private void button5_Click(object sender, RoutedEventArgs e)
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
                //remove old
                testListNew.Remove(new KeyValuePair<int, float>(int.Parse(textBox4.Text), oldRatio));
                oldRatio = float.Parse(textBox5.Text);
                //add new
                testListNew.Add(new KeyValuePair<int, float>(int.Parse(textBox4.Text), float.Parse(textBox5.Text)));
                
                lineGraph.ItemsSource = testListNew;
                lineGraph.Refresh();
            }
            catch
            { }

            //also update Point Ratio in DB
            string SQLquery = "UPDATE dbo.Codes SET Ratio = " + textBox5.Text.Replace(",", ".") + " WHERE ProcCode = '" + comboBox3.SelectedValue.ToString().Trim() + "' AND Area =  " + textBox4.Text;
            if (runSQLquery(SQLquery) == true)
                Console.WriteLine("Point has been updated");
        }
    }
}
