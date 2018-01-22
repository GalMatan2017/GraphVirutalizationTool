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

        const int DEFAULT_NODE_SIZE = 20;
        const int DEFAULT_SPACING = DEFAULT_NODE_SIZE * 2;
        int spacing = DEFAULT_SPACING;
        int[] color_array;
        int[] connected_comps;

        public GraphController()
        {
            InitializeComponent();
            DataContext = this;
            globals = GraphGlobalVariables.getInstance();
            fileName.DataContext = globals;
            graphInfo.DataContext = null;
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

                    graph.setData(am.ParseFile<bool>(globals.Filepath));

                    int nodes_count = graph.getData<bool>().Count;
                    //number of vertices to be "colored"
                    color_array = new int[nodes_count];
                    //number of vertices which each of vertex represented by the list index and the value is the component class number
                    connected_comps = new int[nodes_count];

                    if (algorithms.isBipartite<bool>(graph, nodes_count, color_array, GraphTypes.Dense, connected_comps))
                    {
                        graph.IsBipartite = true;
                    }

                    GraphRealization.draw<bool>(graph, color_array, connected_comps, spacing, spacing);
                    #endregion
                }

                else
                {
                    #region Sparse
                    graph = new SparseGraph();
                    AdjacencyList am = new AdjacencyList();

                    graph.setData(am.ParseFile<int>(globals.Filepath));

                    int node_count = graph.getData<int>().Count;
                    //number of vertices to be colored
                    color_array = new int[node_count];
                    //number of vertices which each of vertex represented by the list index and the value is the component class number
                    connected_comps = new int[node_count];

                    if (algorithms.isBipartite<int>(graph, node_count, color_array, GraphTypes.Sparse, connected_comps))
                    {
                        graph.IsBipartite = true;
                    }

                    GraphRealization.draw<int>(graph, color_array, connected_comps, spacing, spacing);
                    #endregion
                }
                graphInfo.DataContext = graph;
            }
        }




        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            MainViewModel.getInstance().CanvasHeight = 300;
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

        private void space_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (graph != null)
            {
                if (graph.GraphType == GraphTypes.Dense)
                    GraphRealization.draw<bool>(graph, color_array, connected_comps, spacing * (int)spaceX.Value, spacing * (int)spaceY.Value);
                else
                    GraphRealization.draw<int>(graph, color_array, connected_comps, spacing * (int)spaceX.Value, spacing * (int)spaceY.Value);
            }

        }
        private void zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            foreach (var node in MainViewModel.getInstance().Nodes)
            {
                if ((int)zoom.Value < 3)
                    node.NodeSize = DEFAULT_NODE_SIZE * ((int)zoom.Value - 3);
                node.NodeSize = DEFAULT_NODE_SIZE * ((int)zoom.Value - 2);
            }

            spacing = DEFAULT_SPACING * (int)zoom.Value - 2;
            space_ValueChanged(sender, e);
        }
    }
}