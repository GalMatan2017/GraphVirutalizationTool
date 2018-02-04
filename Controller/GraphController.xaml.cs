using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows;
using System.Threading.Tasks;
using GraphVisualisationTool.Model;

namespace GraphVisualisationTool
{
    public partial class GraphController : UserControl
    {
        Graph graph;
        FileGlobalVars globals;
        Algorithms algorithms;
        GraphTypes type;

        int nodes_count;
        int[] color_array;
        int[] connected_comps;

        public GraphController()
        {
            InitializeComponent();
            DataContext = this;
            globals = FileGlobalVars.getInstance();
            fileName.DataContext = globals;
        }

        private async void onOpenGraphFileClickButton(object sender, System.Windows.RoutedEventArgs e)
        {
            algorithms = new Algorithms();

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                globals.Filepath = openFileDialog.FileName;
                globals.Filename = Path.GetFileName(globals.Filepath);

                #region File Open
                StreamReader reader = File.OpenText(globals.Filepath);
                string line;
                if ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(":"))
                        type = GraphTypes.Sparse;
                    else
                        type = GraphTypes.Dense;
                    reader.Close();
                }
                #endregion

                if (type == GraphTypes.Dense)
                {
                    #region Dense
                    graph = new DenseGraph();
                    List<List<bool>> data = null;
                    try
                    {
                        await Task.Factory.StartNew(() => setMatrix(data));
                        await Task.Factory.StartNew(() => new GraphRealization().draw<bool>(graph, color_array, connected_comps, 30, 0, 0));
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }

                    //graph.setData(data);

                    //nodes_count = graph.getData<bool>().Count;

                    ////number of vertices to be "colored"
                    //color_array = new int[nodes_count];
                    ////number of vertices which each of vertex represented by the list index and the value is the component class number
                    //connected_comps = new int[nodes_count];

                    //if (algorithms.isBipartite<bool>(graph, nodes_count, color_array, GraphTypes.Dense, connected_comps))
                    //{
                    //    graph.IsBipartite = true;
                    //    isBip_cb.IsChecked = true;
                    //    rb_controller.IsEnabled = false;
                    //    rb_random.IsChecked = false;
                    //    rb_squared.IsChecked = false;
                    //    rb_controller.Visibility = Visibility.Hidden;
                    //}
                    //else
                    //{
                    //    graph.IsBipartite = false;
                    //    isBip_cb.IsChecked = false;
                    //    rb_controller.IsEnabled = true;
                    //    rb_random.IsChecked = true;
                    //    rb_squared.IsChecked = false;
                    //    rb_controller.Visibility = Visibility.Visible;
                    //    GraphRealization.GeneralDrawType = GraphRealization.GeneralDraw.Random;
                    //}
                    //GraphRealization.draw<bool>(graph, color_array, connected_comps);
                    #endregion
                }

                else
                {
                    #region Sparse
                    graph = new SparseGraph();
                    List<List<int>> data = null;
                    try
                    {
                        await Task.Factory.StartNew(() => setList(data));
                        await Task.Factory.StartNew(() => new GraphRealization().draw<int>(graph, color_array, connected_comps, 30, 0, 0));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }

                    //graph.setData(data);

                    //nodes_count = graph.getData<int>().Count;

                    ////number of vertices to be colored
                    //color_array = new int[nodes_count];
                    ////number of vertices which each of vertex represented by the list index and the value is the component class number
                    //connected_comps = new int[nodes_count];

                    //if (algorithms.isBipartite<int>(graph, nodes_count, color_array, GraphTypes.Sparse, connected_comps))
                    //{
                    //    graph.IsBipartite = true;
                    //    isBip_cb.IsChecked = true;
                    //    rb_controller.IsEnabled = false;
                    //    rb_random.IsChecked = false;
                    //    rb_squared.IsChecked = false;
                    //    rb_controller.Visibility = Visibility.Hidden;
                    //}
                    //else
                    //{
                    //    graph.IsBipartite = false;
                    //    isBip_cb.IsChecked = false;
                    //    rb_controller.IsEnabled = true;
                    //    rb_random.IsChecked = true;
                    //    rb_squared.IsChecked = false;
                    //    rb_controller.Visibility = Visibility.Visible;
                    //    GraphRealization.GeneralDrawType = GraphRealization.GeneralDraw.Random;
                    //}
                    //new GraphRealization().draw<int>(graph, color_array, connected_comps,30,0,0);
                    #endregion
                }

                connComps.DataContext = graph;
                graph.ConnectedComps = 0;
                for (int i = 0; i < nodes_count; i++)
                {
                    if (connected_comps[i] > graph.ConnectedComps)
                        graph.ConnectedComps = connected_comps[i];
                }
            }
        }


        private void setMatrix(List<List<bool>> data)
        {
            AdjacencyMatrix am = new AdjacencyMatrix();

            data = am.ParseFile<bool>(globals.Filepath);

            graph.setData(data);

            nodes_count = graph.getData<bool>().Count;

            //number of vertices to be "colored"
            color_array = new int[nodes_count];
            //number of vertices which each of vertex represented by the list index and the value is the component class number
            connected_comps = new int[nodes_count];

            Dispatcher.Invoke(new Action(() =>
            {
                if (algorithms.isBipartite<bool>(graph, nodes_count, color_array, GraphTypes.Dense, connected_comps))
                {
                    graph.IsBipartite = true;
                    isBip_cb.IsChecked = true;
                    rb_controller.IsEnabled = false;
                    rb_random.IsChecked = false;
                    rb_squared.IsChecked = false;
                    rb_controller.Visibility = Visibility.Hidden;
                }
                else
                {
                    graph.IsBipartite = false;
                    isBip_cb.IsChecked = false;
                    rb_controller.IsEnabled = true;
                    rb_random.Checked -= rb_random_Checked;
                    rb_random.IsChecked = true;
                    rb_random.Checked += rb_random_Checked;
                    rb_squared.IsChecked = false;
                    rb_controller.Visibility = Visibility.Visible;
                    GraphRealization.GeneralDrawType = GraphRealization.GeneralDraw.Random;
                }

            }));
        }

        private void setList(List<List<int>> data)
        {

            AdjacencyList al = new AdjacencyList();

            data = al.ParseFile<int>(globals.Filepath);

            graph.setData(data);

            nodes_count = graph.getData<int>().Count;

            //number of vertices to be colored
            color_array = new int[nodes_count];
            //number of vertices which each of vertex represented by the list index and the value is the component class number
            connected_comps = new int[nodes_count];

            Dispatcher.Invoke(new Action(() =>
            {
                if (algorithms.isBipartite<int>(graph, nodes_count, color_array, GraphTypes.Sparse, connected_comps))
                {
                    graph.IsBipartite = true;
                    isBip_cb.IsChecked = true;
                    rb_controller.IsEnabled = false;
                    rb_random.IsChecked = false;
                    rb_squared.IsChecked = false;
                    rb_controller.Visibility = Visibility.Hidden;
                }
                else
                {
                    graph.IsBipartite = false;
                    isBip_cb.IsChecked = false;
                    rb_controller.IsEnabled = true;
                    rb_random.Checked -= rb_random_Checked;
                    rb_random.IsChecked = true;
                    rb_random.Checked += rb_random_Checked;
                    rb_squared.IsChecked = false;
                    rb_controller.Visibility = Visibility.Visible;
                    GraphRealization.GeneralDrawType = GraphRealization.GeneralDraw.Random;
                }

            }));
        }


        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.Name == "showNamesBox") MainViewModel.getInstance().ShowNames = true;
        }
        private void HandleUnchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            if (cb.Name == "showNamesBox") MainViewModel.getInstance().ShowNames = false;
        }
        private void SaveGraph(object sender, RoutedEventArgs e)
        {
            FileGlobalVars.getInstance().ExportToPng(MainViewModel.getInstance().MainCanvas);
        }

        private void zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            foreach (var node in MainViewModel.getInstance().Nodes)
            {
                GraphRealization.MARGIN_X = node.NodeSize;
                GraphRealization.MARGIN_Y = GraphRealization.MARGIN_X;
                node.NodeSize = GraphRealization.DEFAULT_CONSTANT * (int)zoom.Value/5;

            }
        }

        private void rb_squared_Checked(object sender, RoutedEventArgs e)
        {
            GraphRealization.GeneralDrawType = GraphRealization.GeneralDraw.Squared;
            if (type == GraphTypes.Dense) {
                new GraphRealization().draw<bool>(graph, color_array, connected_comps,30,0,0);
            }
            else
            {
                new GraphRealization().draw<int>(graph, color_array, connected_comps,30,0,0);
            }
        }

        private void rb_random_Checked(object sender, RoutedEventArgs e)
        {
            GraphRealization.GeneralDrawType = GraphRealization.GeneralDraw.Random;
            if (type == GraphTypes.Dense)
            {
                new GraphRealization().draw<bool>(graph, color_array, connected_comps,30,0,0);
            }
            else
            {
                new GraphRealization().draw<int>(graph, color_array, connected_comps,30,0,0);
            }
        }
    }
}