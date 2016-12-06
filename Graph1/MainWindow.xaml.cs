using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

        public MainWindow()
        {
            InitializeComponent();

            List<string> data = new List<string>();
            data.Add("Line graph");//0
            data.Add("Bar graph");            
            data.Add("Column graph");
            data.Add("Pie graph");//3
            data.Add("Bubble graph");
            data.Add("Scatter graph");
            data.Add("Area graph");//6

            comboBox.ItemsSource = data;
            comboBox.SelectedIndex = 0;
            currentGraphType = - 1;

            //create datagrid with #, X, Y columns
            DataColumn fNameColumn = new DataColumn();
            fNameColumn.ColumnName = "#";
            fNameColumn.DataType = System.Type.GetType("System.Int32");
            fNameColumn.AutoIncrement = true;
            fNameColumn.AutoIncrementSeed = 1;
            fNameColumn.AutoIncrementStep = 1;
            fNameColumn.ReadOnly = true;            
            table.Columns.Add(fNameColumn);

            fNameColumn = new DataColumn();
            fNameColumn.ColumnName = "X";
            fNameColumn.DataType = System.Type.GetType("System.Int32");
            table.Columns.Add(fNameColumn);

            fNameColumn = new DataColumn();
            fNameColumn.ColumnName = "Y";
            fNameColumn.DataType = System.Type.GetType("System.Int32");
            table.Columns.Add(fNameColumn);
                        
            dataGrid.AutoGenerateColumns = true;
            dataGrid.ItemsSource = table.DefaultView;
            dataGrid.ColumnWidth = 70;            

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.Items.Count == 1)
            {
                MessageBox.Show("Table is empty");
                return;
            }

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

            //convert each row into KeyValuePair
            for (int i = 0; i < dataGrid.Items.Count - 1; i++)
            {
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                TextBlock cellContentX = dataGrid.Columns[1].GetCellContent(row) as TextBlock;
                TextBlock cellContentY = dataGrid.Columns[2].GetCellContent(row) as TextBlock;

                testList.Add(new KeyValuePair<int, int>(int.Parse(cellContentX.Text), int.Parse(cellContentY.Text)));
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
                    mySeriesPie.ItemsSource = testList;                    
                    mcChart.Series.Add(mySeriesPie);
                    break;
                case 4:
                    //  mySeries = new BubbleSeries();
                    BubbleSeries mySeriesBubble = new BubbleSeries();
                    mySeriesBubble.Title = title;
                    mySeriesBubble.IndependentValueBinding = new Binding("Key");
                    mySeriesBubble.DependentValueBinding = new Binding("Value");
                    mySeriesBubble.ItemsSource = testList;
                    mcChart.Series.Add(mySeriesBubble);
                    break;
                case 5:
                    //  mySeries = new ScatterSeries();
                    ScatterSeries mySeriesScatter = new ScatterSeries();
                    mySeriesScatter.Title = title;
                    mySeriesScatter.IndependentValueBinding = new Binding("Key");
                    mySeriesScatter.DependentValueBinding = new Binding("Value");
                    mySeriesScatter.ItemsSource = testList;
                    mcChart.Series.Add(mySeriesScatter);
                    break;
                case 6:
                    //  mySeries = new AreaSeries();
                    AreaSeries mySeriesArea = new AreaSeries();
                    mySeriesArea.Title = title;
                    mySeriesArea.IndependentValueBinding = new Binding("Key");
                    mySeriesArea.DependentValueBinding = new Binding("Value");
                    mySeriesArea.ItemsSource = testList;
                    mcChart.Series.Add(mySeriesArea);
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
            mcChart.Axes.Add(axisX);
            
            //add Y axis
            LinearAxis axisY = new LinearAxis();
            axisY.Orientation = AxisOrientation.Y;
            axisY.Title = axesY;
            axisY.ShowGridLines = true;
            mcChart.Axes.Add(axisY);            
        }
    }
}
