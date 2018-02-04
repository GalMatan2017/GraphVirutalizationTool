using System.Collections.Generic;

namespace GraphVisualisationTool.Model
{
    interface Graph
    {
        GraphTypes GraphType { get; set; }
        int ConnectedComps { get; set; }

        int VerticesAmount { get; set; }

        int EdgesAmount { get; set; }
        List<List<T>> getData<T>();
        void setData<T>(T data);
        List<int> getNeighbours(int node);
        bool IsBipartite { get; set; }
    }
}
