using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using GraphVirtualizationTool.Model;
using System.Windows;

namespace GraphVirtualizationTool
{
    public partial class GraphController : UserControl
    {
        Graph graph;
        GraphGlobalVariables globals;
        Algorithms algorithms;
        GraphTypes type;

        int nodes_count;
        int[] color_array;
        int[] connected_comps;

        public GraphController()
        {
            InitializeComponent();
            DataContext = this;
            globals = GraphGlobalVariables.getInstance();
            fileName.DataContext = globals;
        }

        private void onOpenGraphFileClickButton(object sender, System.Windows.RoutedEventArgs e)
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
                    AdjacencyMatrix am = new AdjacencyMatrix();
                    List<List<bool>> data;
                    try
                    {
                        data = am.ParseFile<bool>(globals.Filepath);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }

                    graph.setData(data);

                    nodes_count = graph.getData<bool>().Count;

                    //number of vertices to be "colored"
                    color_array = new int[nodes_count];
                    //number of vertices which each of vertex represented by the list index and the value is the component class number
                    connected_comps = new int[nodes_count];

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
                        rb_random.IsChecked = true;
                        rb_squared.IsChecked = false;
                        rb_controller.Visibility = Visibility.Visible;
                        GraphRealization.GeneralDrawType = GraphRealization.GeneralDraw.Random;
                    }
                    GraphRealization.draw<bool>(graph, color_array, connected_comps);
                    #endregion
                }

                else
                {
                    #region Sparse
                    graph = new SparseGraph();
                    AdjacencyList al = new AdjacencyList();
                    List<List<bool>> data;
                    try
                    {
                        data = al.ParseFile<bool>(globals.Filepath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }

                    graph.setData(data);

                    nodes_count = graph.getData<int>().Count;

                    //number of vertices to be colored
                    color_array = new int[nodes_count];
                    //number of vertices which each of vertex represented by the list index and the value is the component class number
                    connected_comps = new int[nodes_count];

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
                        rb_random.IsChecked = true;
                        rb_squared.IsChecked = false;
                        rb_controller.Visibility = Visibility.Visible;
                        GraphRealization.GeneralDrawType = GraphRealization.GeneralDraw.Random;
                    }
                    GraphRealization.draw<int>(graph, color_array, connected_comps);
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
            GraphGlobalVariables.getInstance().ExportToPng(MainViewModel.getInstance().MainCanvas);
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
                GraphRealization.draw<bool>(graph, color_array, connected_comps);
            }
            else
            {
                GraphRealization.draw<int>(graph, color_array, connected_comps);
            }
        }

        private void rb_random_Checked(object sender, RoutedEventArgs e)
        {
            GraphRealization.GeneralDrawType = GraphRealization.GeneralDraw.Random;
            if (type == GraphTypes.Dense)
            {
                GraphRealization.draw<bool>(graph, color_array, connected_comps);
            }
            else
            {
                GraphRealization.draw<int>(graph, color_array, connected_comps);
            }
        }
    }
}