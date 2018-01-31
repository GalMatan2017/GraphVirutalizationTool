using System.Collections.Generic;

namespace GraphVirtualizationTool.Model
{
    interface Graph
    {
        GraphTypes GraphType { get; set; }
        int ConnectedComps { get; set; }
        List<List<T>> getData<T>();
        void setData<T>(T data);
        List<int> getNeighbours(int node);
        bool IsBipartite { get; set; }
    }
}
