using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace GraphVisualisationTool.Model
{
    class GraphRealization
    {
        public static readonly int DEFAULT_CONSTANT = 30;
        public static int MARGIN_X = DEFAULT_CONSTANT;
        public static int MARGIN_Y = MARGIN_X;
        public enum GeneralDraw { Random, Squared };
        public static GeneralDraw GeneralDrawType { get; set; }
        public void draw<T>(Graph graph,int[] colorArr,int[] conn_comps, int node_size, int marginX, int marginY)
        {

            List<Vertex> nodes = new List<Vertex>();
            List<Edge> edges = new List<Edge>();
            Random random = new Random();
            int rows = graph.getData<T>().Count;

            int comps = 0;
            int total_nodes = graph.getData<T>().Count;

            List<Point> coordinates = new List<Point>();

            foreach (var comp in conn_comps)
            {
                if (comp > comps)
                    comps = comp;
            }

            //Bipartite draw algorithm
            if (graph.IsBipartite)
            {
                //foreach component - 2 colors
                int[] yFactor = new int[comps * 2];

                //count 2 colors (zero,one) foreach component
                int[] comps_colors_zeros = new int[comps];
                int[] comps_colors_ones = new int[comps];

                //count total of each color
                int ones = 0, zeros = 0;

                for (int i = 0; i < total_nodes; i++)
                {
                    if (colorArr[i] == 0)
                    {
                        zeros++;
                        ++comps_colors_zeros[conn_comps[i] - 1];
                    }
                    else
                    {
                        ones++;
                        ++comps_colors_ones[conn_comps[i] - 1];
                    }
                }

                //margin top
                yFactor[0] = MARGIN_Y;
                yFactor[1] = MARGIN_Y;

                //place pair y factors foreach component's color
                for (int i = 1; i < comps; i++)
                {
                    yFactor[i * 2] += yFactor[(i - 1) * 2] + (MARGIN_Y + node_size) * (comps_colors_zeros[i / 2] > comps_colors_ones[i / 2] ? comps_colors_zeros[i / 2] : comps_colors_ones[i / 2]);
                    yFactor[i * 2 + 1] = yFactor[i * 2];
                }

                //assign coordinates and increase yfactor
                for (int i = 0; i < total_nodes; i++)
                {
                    coordinates.Add(new Point(MARGIN_X * (colorArr[i] + 1) + (colorArr[i] * node_size), yFactor[2 * conn_comps[i] - colorArr[i] - 1]));
                    yFactor[2 * conn_comps[i] - colorArr[i] - 1] += MARGIN_Y + node_size;
                }

                //adjust canvas
                MainViewModel.getInstance().CanvasHeight = (zeros > ones ? zeros * node_size : ones * node_size) + (MARGIN_Y * (zeros > ones ? zeros + 1 : ones + 1));
                MainViewModel.getInstance().CanvasWidth = 2 * node_size + (MARGIN_X) * 3;

            }
            //general draw algorithm squared
            else if (GeneralDrawType == GeneralDraw.Squared)
            {
                int[] yFactor = new int[comps];
                int[] xFactor = new int[comps];
                int[] onCanvas = new int[comps];
                int maxComp = 0;
                int sumComp = 0;

                int[] conn_comps_sum = new int[comps];

                for (int i = 0; i < total_nodes; i++)
                    ++conn_comps_sum[conn_comps[i] - 1];

                for (int i = 0; i < comps; i++)
                    conn_comps_sum[i] = (int)Math.Ceiling(Math.Sqrt(conn_comps_sum[i]));

                for (int i = 0; i < comps; i++)
                {
                    sumComp += conn_comps_sum[i];
                    if (conn_comps_sum[i] > maxComp)
                        maxComp = conn_comps_sum[i];
                }

                xFactor[0] = MARGIN_X;
                yFactor[0] = MARGIN_Y;

                for (int i = 1; i < comps; i++)
                {
                    xFactor[i] = MARGIN_X;
                    yFactor[i] += yFactor[i - 1] + MARGIN_Y * conn_comps_sum[i - 1] + node_size * conn_comps_sum[i - 1];
                }

                for (int i = 0; i < total_nodes; i++)
                {

                    coordinates.Add(new Point(xFactor[conn_comps[i] - 1], yFactor[conn_comps[i] - 1]));
                    xFactor[conn_comps[i] - 1] += MARGIN_X + node_size;
                    onCanvas[conn_comps[i] - 1]++;
                    if (onCanvas[conn_comps[i] - 1] - 1 != 0 && onCanvas[conn_comps[i] - 1] % conn_comps_sum[conn_comps[i] - 1] == 0)
                    {
                        xFactor[conn_comps[i] - 1] = MARGIN_X;
                        yFactor[conn_comps[i] - 1] += MARGIN_Y + node_size;
                    }
                }

                //adjust canvas
                MainViewModel.getInstance().CanvasHeight = sumComp * node_size + (sumComp + 1) * MARGIN_Y;
                MainViewModel.getInstance().CanvasWidth = maxComp * node_size + (maxComp + 1) * MARGIN_X;
            }
            //general draw algorithm random
            else
            {
                int[] yStart = new int[comps];
                int[] xStart = new int[comps];
                int[] yEnd = new int[comps];
                int[] xEnd = new int[comps];
                int maxComp = 0;
                int sumComp = 0;

                int[] conn_comps_sum = new int[comps];

                for (int i = 0; i < total_nodes; i++)
                    ++conn_comps_sum[conn_comps[i] - 1];

                for (int i = 0; i < comps; i++)
                    conn_comps_sum[i] = (int)Math.Ceiling(Math.Sqrt(conn_comps_sum[i]));

                for (int i = 0; i < comps; i++)
                {
                    sumComp += conn_comps_sum[i];
                    if (conn_comps_sum[i] > maxComp)
                        maxComp = conn_comps_sum[i];
                }

                for (int i = 0; i < comps; i++)
                {
                    xStart[i] = MARGIN_X;
                    yStart[i] = MARGIN_Y;
                    if (i > 0)
                        yStart[i] = yStart[i - 1] + node_size * conn_comps_sum[i - 1] + (node_size - 1) * conn_comps_sum[i - 1];
                    yEnd[i] = yStart[i] + node_size * conn_comps_sum[i] + MARGIN_Y * (conn_comps_sum[i] - 1);
                    xEnd[i] = xStart[i] + node_size * conn_comps_sum[i] + MARGIN_X * (conn_comps_sum[i] - 1);
                }

                for (int i = 0; i < total_nodes; i++)
                {
                    int r1 = new Random(Guid.NewGuid().GetHashCode()).Next(xStart[conn_comps[i] - 1], xEnd[conn_comps[i] - 1]);
                    int r2 = new Random(Guid.NewGuid().GetHashCode()).Next(yStart[conn_comps[i] - 1], yEnd[conn_comps[i] - 1]);
                    coordinates.Add(new Point(r1, r2));
                }

                //adjust canvas
                MainViewModel.getInstance().CanvasHeight = sumComp * node_size + (sumComp + 1) * MARGIN_Y;
                MainViewModel.getInstance().CanvasWidth = maxComp * node_size + (sumComp + 1) * MARGIN_X;

            }

            //Matrix case
            if (typeof(T) == typeof(bool))
            {
                for (int row = 0; row < rows; row++)
                {
                    nodes.Add(
                        new Vertex()
                        {
                            Name = $"node {row + 1}",
                            X = coordinates[row].X,
                            Y = coordinates[row].Y
                        });
                }

                for (int row = 0; row < graph.getData<T>().Count; row++)
                {
                    for (int col = graph.getData<T>().Count - 1; col > row - 1; col--)
                    {
                        if (col == row)
                            continue;
                        if ((bool)Convert.ChangeType(graph.getData<T>().ElementAt(row).ElementAt(col), typeof(bool)) == true)
                        {
                            edges.Add(new Edge()
                            {
                                Name = $"connector {new Random().Next(999)}",
                                Start = nodes.Single(x => x.Name.Equals($"node {row + 1}")),
                                End = nodes.Single(x => x.Name.Equals($"node {col + 1}"))
                            });
                        }
                    }
                }
            }

            //List case
            else
            {
                for (int row = 0; row < rows; row++)
                {
                    nodes.Add(
                        new Vertex()
                        {
                            Name = $"node {graph.getData<T>().ElementAt(row).ElementAt(0)}",
                            X = coordinates[row].X,
                            Y = coordinates[row].Y
                        });
                }
                for (int row = 0; row < graph.getData<T>().Count; row++)
                {
                    for (int col = 1; col < graph.getData<T>().ElementAt(row).Count; col++)
                    {
                        if (edges.Find(x => x.Start.Name.Equals($"node {graph.getData<T>().ElementAt(row).ElementAt(col)}")
                                            && x.End.Name.Equals(($"node {graph.getData<T>().ElementAt(row).ElementAt(0)}"))) != null)
                            continue;
                        else
                        {
                            edges.Add(new Edge()
                            {
                                Name = $"edge {new Random().Next(999)}",
                                Start = nodes.Single(x => x.Name.Equals($"node {graph.getData<T>().ElementAt(row).ElementAt(0)}")),
                                End = nodes.Single(x => x.Name.Equals($"node {graph.getData<T>().ElementAt(row).ElementAt(col)}"))
                            });
                        }
                    }
                }
            }
            //draw
            MainViewModel.getInstance().Nodes = new System.Collections.ObjectModel.ObservableCollection<Vertex>(nodes);
            MainViewModel.getInstance().Edges = new System.Collections.ObjectModel.ObservableCollection<Edge>(edges);

            Application.Current.Dispatcher.Invoke(new Action(() => {

                if (graph.IsBipartite)
                {
                    for (int i = 0; i < nodes.Count; i++)
                        nodes[i].NodeColor = colorArr[i] == 0 ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Orange);
                }
                else
                {
                    for (int i = 0; i < nodes.Count; i++)
                        nodes[i].NodeColor = new SolidColorBrush(Colors.Orange);
                }

            }));
        }
    }
}
