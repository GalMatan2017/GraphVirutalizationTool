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
        public enum GeneralDraw { Random, Squared };
        public static GeneralDraw GeneralDrawType { get; set; }
        public void draw<T>(Graph graph,int[] colorArr,int[] conn_comps, int vertex_size, int marginX, int marginY)
        {

            List<Vertex> vertices = new List<Vertex>();
            List<Edge> edges = new List<Edge>();
            Random random = new Random();
            int rows = graph.getData<T>().Count;

            int comps = 0;
            int total_vertices = graph.getData<T>().Count;

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

                for (int i = 0; i < total_vertices; i++)
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
                yFactor[0] = DEFAULT_CONSTANT;
                yFactor[1] = DEFAULT_CONSTANT;

                //place pair y factors foreach component's color
                for (int i = 1; i < comps; i++)
                {
                    yFactor[i * 2] += yFactor[(i - 1) * 2] + (marginY + vertex_size) * (comps_colors_zeros[i / 2] > comps_colors_ones[i / 2] ? comps_colors_zeros[i / 2] : comps_colors_ones[i / 2]);
                    yFactor[i * 2 + 1] = yFactor[i * 2];
                }

                //assign coordinates and increase yfactor
                for (int i = 0; i < total_vertices; i++)
                {
                    coordinates.Add(new Point(marginX * (colorArr[i]) + DEFAULT_CONSTANT + (colorArr[i] * vertex_size), yFactor[2 * conn_comps[i] - colorArr[i] - 1]));
                    yFactor[2 * conn_comps[i] - colorArr[i] - 1] += marginY + vertex_size;
                }

                //adjust canvas
                MainViewModel.getInstance().CanvasHeight = (zeros > ones ? zeros * vertex_size : ones * vertex_size) + (marginY * (zeros > ones ? zeros - 1 : ones - 1)) + 2*DEFAULT_CONSTANT;
                MainViewModel.getInstance().CanvasWidth = 2*DEFAULT_CONSTANT + 2 * vertex_size + (marginX);

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

                for (int i = 0; i < total_vertices; i++)
                    ++conn_comps_sum[conn_comps[i] - 1];

                for (int i = 0; i < comps; i++)
                    conn_comps_sum[i] = (int)Math.Ceiling(Math.Sqrt(conn_comps_sum[i]));

                for (int i = 0; i < comps; i++)
                {
                    sumComp += conn_comps_sum[i];
                    if (conn_comps_sum[i] > maxComp)
                        maxComp = conn_comps_sum[i];
                }

                xFactor[0] = DEFAULT_CONSTANT;
                yFactor[0] = DEFAULT_CONSTANT;

                for (int i = 1; i < comps; i++)
                {
                    xFactor[i] = DEFAULT_CONSTANT;
                    yFactor[i] += yFactor[i - 1] + marginY * conn_comps_sum[i - 1] + vertex_size * conn_comps_sum[i - 1];
                }

                for (int i = 0; i < total_vertices; i++)
                {

                    coordinates.Add(new Point(xFactor[conn_comps[i] - 1], yFactor[conn_comps[i] - 1]));
                    xFactor[conn_comps[i] - 1] += marginX + vertex_size;
                    onCanvas[conn_comps[i] - 1]++;
                    if (onCanvas[conn_comps[i] - 1] - 1 != 0 && onCanvas[conn_comps[i] - 1] % conn_comps_sum[conn_comps[i] - 1] == 0)
                    {
                        xFactor[conn_comps[i] - 1] = DEFAULT_CONSTANT;
                        yFactor[conn_comps[i] - 1] += marginY + vertex_size;
                    }
                }

                //adjust canvas
                MainViewModel.getInstance().CanvasHeight = sumComp * vertex_size + (sumComp - 1) * marginY + 2*DEFAULT_CONSTANT;
                MainViewModel.getInstance().CanvasWidth = maxComp * vertex_size + (maxComp - 1) * marginX + 2 * DEFAULT_CONSTANT;
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

                for (int i = 0; i < total_vertices; i++)
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
                    xStart[i] = marginX;
                    yStart[i] = marginY;
                    if (i > 0)
                        yStart[i] = yStart[i - 1] + vertex_size * conn_comps_sum[i - 1] + (vertex_size - 1) * conn_comps_sum[i - 1];
                    yEnd[i] = yStart[i] + vertex_size * conn_comps_sum[i] + marginY * (conn_comps_sum[i] - 1);
                    xEnd[i] = xStart[i] + vertex_size * conn_comps_sum[i] + marginX * (conn_comps_sum[i] - 1);
                }

                for (int i = 0; i < total_vertices; i++)
                {
                    int r1 = new Random(Guid.NewGuid().GetHashCode()).Next(xStart[conn_comps[i] - 1], xEnd[conn_comps[i] - 1]);
                    int r2 = new Random(Guid.NewGuid().GetHashCode()).Next(yStart[conn_comps[i] - 1], yEnd[conn_comps[i] - 1]);
                    coordinates.Add(new Point(r1, r2));
                }

                //adjust canvas
                MainViewModel.getInstance().CanvasHeight = sumComp * vertex_size + (sumComp - 1) * marginY + 2 * DEFAULT_CONSTANT;
                MainViewModel.getInstance().CanvasWidth = maxComp * vertex_size + (maxComp - 1) * marginX + 2 * DEFAULT_CONSTANT;

            }

            //Matrix case
            if (typeof(T) == typeof(bool))
            {
                for (int row = 0; row < rows; row++)
                {
                    vertices.Add(
                        new Vertex()
                        {
                            Name = $"{row + 1}",
                            X = coordinates[row].X,
                            Y = coordinates[row].Y
                        });
                }
                graph.EdgesAmount = 0;
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
                                Start = vertices.Single(x => x.Name.Equals($"{row + 1}")),
                                End = vertices.Single(x => x.Name.Equals($"{col + 1}"))
                            });
                            graph.EdgesAmount++;
                        }
                    }
                }
            }

            //List case
            else
            {
                for (int row = 0; row < rows; row++)
                {
                    vertices.Add(
                        new Vertex()
                        {
                            Name = $"{graph.getData<T>().ElementAt(row).ElementAt(0)}",
                            X = coordinates[row].X,
                            Y = coordinates[row].Y
                        });
                }
                graph.EdgesAmount = 0;
                for (int row = 0; row < graph.getData<T>().Count; row++)
                {
                    for (int col = 1; col < graph.getData<T>().ElementAt(row).Count; col++)
                    {
                        if (edges.Find(x => x.Start.Name.Equals($"{graph.getData<T>().ElementAt(row).ElementAt(col)}")
                                            && x.End.Name.Equals(($"{graph.getData<T>().ElementAt(row).ElementAt(0)}"))) != null)
                            continue;
                        else
                        {
                            edges.Add(new Edge()
                            {
                                Name = $"edge {new Random().Next(999)}",
                                Start = vertices.Single(x => x.Name.Equals($"{graph.getData<T>().ElementAt(row).ElementAt(0)}")),
                                End = vertices.Single(x => x.Name.Equals($"{graph.getData<T>().ElementAt(row).ElementAt(col)}"))
                            });
                            graph.EdgesAmount++;
                        }
                    }
                }
            }
            //draw
            MainViewModel.getInstance().Vertices = new System.Collections.ObjectModel.ObservableCollection<Vertex>(vertices);
            MainViewModel.getInstance().Edges = new System.Collections.ObjectModel.ObservableCollection<Edge>(edges);

            Application.Current.Dispatcher.Invoke(new Action(() => {

                if (graph.IsBipartite)
                {
                    for (int i = 0; i < vertices.Count; i++)
                        vertices[i].VertexColor = colorArr[i] == 0 ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Orange);
                }
                else
                {
                    for (int i = 0; i < vertices.Count; i++)
                        vertices[i].VertexColor = new SolidColorBrush(Colors.Orange);
                }

            }));
        }
    }
}
