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

        int vertices_count;
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
                        await Task.Factory.StartNew(() => new GraphRealization().draw<bool>(graph, color_array, connected_comps, 30, 30, 30));
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }

                    #region synchronous

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
                        await Task.Factory.StartNew(() => new GraphRealization().draw<int>(graph, color_array, connected_comps, 30, 30, 30));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }


                    #region synchronous
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

                    #endregion
                }

                zoom.Value = 6;

                zoom.ValueChanged += zoom_ValueChanged;
                spaceX.ValueChanged += spaceX_ValueChanged;
                spaceY.ValueChanged += spaceY_ValueChanged;

                connComps.DataContext = graph;
                verticesAmount.DataContext = graph;
                edgesAmount.DataContext = graph;

                graph.ConnectedComps = 0;
                for (int i = 0; i < vertices_count; i++)
                {
                    if (connected_comps[i] > graph.ConnectedComps)
                        graph.ConnectedComps = connected_comps[i];
                }
                if (graph.ConnectedComps == 1)
                    isCon_cb.IsChecked = true;
                else
                    isCon_cb.IsChecked = false;
            }
        }


        private void setMatrix(List<List<bool>> data)
        {
            AdjacencyMatrix am = new AdjacencyMatrix();

            data = am.ParseFile<bool>(globals.Filepath);

            graph.setData(data);

            vertices_count = graph.getData<bool>().Count;

            graph.VerticesAmount = vertices_count;

            //number of vertices to be "colored"
            color_array = new int[vertices_count];
            //number of vertices which each of vertex represented by the list index and the value is the component class number
            connected_comps = new int[vertices_count];

            Dispatcher.Invoke(new Action(() =>
            {
                if (algorithms.isBipartite<bool>(graph, vertices_count, color_array, GraphTypes.Dense, connected_comps))
                {
                    setViewBI();
                }
                else
                {
                    setViewNotBi();
                }
            }));
        }

        private void setList(List<List<int>> data)
        {

            AdjacencyList al = new AdjacencyList();

            data = al.ParseFile<int>(globals.Filepath);

            graph.setData(data);

            vertices_count = graph.getData<int>().Count;

            graph.VerticesAmount = vertices_count;

            //number of vertices to be colored
            color_array = new int[vertices_count];
            //number of vertices which each of vertex represented by the list index and the value is the component class number
            connected_comps = new int[vertices_count];

            Dispatcher.Invoke(new Action(() =>
            {
                if (algorithms.isBipartite<int>(graph, vertices_count, color_array, GraphTypes.Sparse, connected_comps))
                {
                    setViewBI();
                }
                else
                {
                    setViewNotBi();
                }

            }));
        }

        private async Task redrawAsync() {
            int vertexSize = Vertex.vertexSize;
            int space_Y = (int)spaceY.Value;
            int space_X = (int)spaceX.Value;

            await Task.Factory.StartNew(() =>
            redraw(Vertex.vertexSize, space_X, space_Y)
            );

        }
        private void redraw(int DEFAULT_SIZE,int space_X, int space_Y)
        {
            if (type == GraphTypes.Dense)
            {
                new GraphRealization().draw<bool>(graph, color_array, connected_comps, Vertex.vertexSize, DEFAULT_SIZE * space_X, DEFAULT_SIZE * space_Y);
            }
            else
            {
                new GraphRealization().draw<int>(graph, color_array, connected_comps, Vertex.vertexSize, DEFAULT_SIZE * space_X, DEFAULT_SIZE * space_Y);
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
            FileGlobalVars.getInstance().ExportToPng(MainViewModel.getInstance().MainCanvas);
        }

        private void setViewBI()
        {
            graph.IsBipartite = true;
            isBip_cb.IsChecked = true;
            rb_controller.IsEnabled = false;
            rb_random.IsChecked = false;
            rb_squared.IsChecked = false;
            rb_circular.IsChecked = false;
            rb_controller.Visibility = Visibility.Hidden;

        }

        private void setViewNotBi()
        {
            graph.IsBipartite = false;
            isBip_cb.IsChecked = false;
            rb_controller.IsEnabled = true;
            rb_random.Checked -= rb_random_Checked;
            rb_random.IsChecked = true;
            rb_random.Checked += rb_random_Checked;
            rb_squared.IsChecked = false;
            rb_circular.IsChecked = false;
            rb_controller.Visibility = Visibility.Visible;
            GraphRealization.GeneralDrawType = GraphRealization.GeneralDraw.Random;;
        }
        private async void zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            foreach (var vertex in MainViewModel.getInstance().Vertices)
            {
                vertex.VertexSize = GraphRealization.DEFAULT_CONSTANT * (int)zoom.Value/6;
            }
            await redrawAsync();
        }

        private async void rb_squared_Checked(object sender, RoutedEventArgs e)
        {
            GraphRealization.GeneralDrawType = GraphRealization.GeneralDraw.Squared;
            await redrawAsync();
        }

        private async void rb_random_Checked(object sender, RoutedEventArgs e)
        {
            GraphRealization.GeneralDrawType = GraphRealization.GeneralDraw.Random;
            await redrawAsync();
        }
        private async void rb_circular_Checked(object sender, RoutedEventArgs e)
        {
            GraphRealization.GeneralDrawType = GraphRealization.GeneralDraw.Circular;
            await redrawAsync();
        }
        private async void spaceX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            await redrawAsync();
        }

        private async void spaceY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            await redrawAsync();
        }
    }
}