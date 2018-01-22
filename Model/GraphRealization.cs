using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace GraphVirtualizationTool.Model
{
    class GraphRealization
    {

        public static Tuple<IEnumerable<Node>, IEnumerable<Edge>> draw<T>(Graph graph,int[] colorArr,int[] conn_comps,int marginX, int marginY)
        {
            List<Node> nodes = new List<Node>();
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

            if (graph.IsBipartite)
            {
                int[] yFactor = new int[comps*2];

                int[] comps_colors_zeros = new int[comps];
                int[] comps_colors_ones = new int[comps];

                int ones=0, zeros=0;

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

                for (int i = 0; i < comps*2; i++)
                {
                    yFactor[i] += marginY;
                }

                for (int i = 1; i < comps; i++)
                {
                    yFactor[i*2] += (marginY + Node._nodeSize) * (comps_colors_zeros[i/2] > comps_colors_ones[i/2] ? comps_colors_zeros[i/2] : comps_colors_ones[i/2]);
                    yFactor[i * 2 + 1] = yFactor[i * 2];

                }


                for (int i = 0; i < total_nodes; i++)
                {
                    coordinates.Add(new Point(marginX*(colorArr[i]+1)+(colorArr[i]*Node._nodeSize),yFactor[2 * conn_comps[i] - colorArr[i] - 1]));
                    //coordinates.Add(new Point(2*conn_comps[i] * marginX - colorArr[i] * marginX,yFactor[2 * conn_comps[i] - colorArr[i] - 1]));
                    yFactor[2 * conn_comps[i] - colorArr[i] - 1] += marginY + Node._nodeSize;
                }

                MainViewModel.getInstance().CanvasHeight = (zeros > ones ? zeros:ones * Node._nodeSize) + (marginY * (zeros > ones ? zeros+1 : ones+1));
                MainViewModel.getInstance().CanvasWidth = 2 * Node._nodeSize + (marginX) * 3;
            }

            //else
            //{

            //}
            //Matrix case
            if (typeof(T) == typeof(bool))
            {
                for (int row = 0; row < rows; row++)
                {
                    SolidColorBrush color;
                    if (colorArr[row] == 0)
                        color = new SolidColorBrush(Colors.Blue);
                    else
                        color = new SolidColorBrush(Colors.Orange);
                    nodes.Add(
                        new Node()
                        {
                            Name = $"node {row + 1}",
                            X = coordinates[row].X,
                            Y = coordinates[row].Y,
                            NodeColor = (color)
                        });
                }

                for (int row = 0; row < graph.getData<T>().Count; row++)
                {
                    for (int col = graph.getData<T>().Count - 1; col > row - 1; col--)
                    {
                        if (col == row)
                            continue;
                        if ((bool)Convert.ChangeType(graph.getData<T>().ElementAt(row).ElementAt(col),typeof(bool)) == true)
                        {
                            edges.Add(new Edge()
                            {
                                Name = $"connector {new Random().Next(999)}",
                                Start = nodes.Single(x => x.Name.Equals($"node {row+1}")),
                                End = nodes.Single(x => x.Name.Equals($"node {col+1}"))
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

                    SolidColorBrush color;
                    if (colorArr[row] == 0)
                        color = new SolidColorBrush(Colors.Blue);
                    else
                        color = new SolidColorBrush(Colors.Orange);
                    nodes.Add(
                        new Node()
                        {
                            Name = $"node {graph.getData<T>().ElementAt(row).ElementAt(0)}",
                            X = coordinates[row].X,
                            Y = coordinates[row].Y,
                            NodeColor = (color)
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
            MainViewModel.getInstance().Nodes = new System.Collections.ObjectModel.ObservableCollection<Node>(nodes);
            MainViewModel.getInstance().Edges = new System.Collections.ObjectModel.ObservableCollection<Edge>(edges);

            return new Tuple<IEnumerable<Node>, IEnumerable<Edge>>(nodes, edges);

        }
    }
}
