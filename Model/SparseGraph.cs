using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GraphVisualisationTool.Model
{
    class SparseGraph : Graph, INotifyPropertyChanged
    {
        private List<List<int>> data;
        public List<List<T>> getData<T>()
        {
            return (List<List<T>>)Convert.ChangeType(data, typeof(List<List<T>>));
        }
        public void setData<T>(T data)
        {
            this.data = new List<List<int>>();
            this.data = (List<List<int>>)Convert.ChangeType(data, typeof(List<List<int>>));
        }
        public List<int> getNeighbours(int node)
        { 
            return data[node-1]; 
        }
        public GraphTypes GraphType { get; set; } = GraphTypes.Sparse;
        private int verticesAmount { get; set; }
        public int VerticesAmount
        {
            get
            {
                return verticesAmount;
            }
            set
            {
                if (value != 0)
                    verticesAmount = value;
                OnPropertyChanged("VerticesAmount");
            }
        }
        private int edgesAmount { get; set; }
        public int EdgesAmount
        {
            get
            {
                return edgesAmount;
            }
            set
            {
                if (value != 0)
                    edgesAmount = value;
                OnPropertyChanged("EdgesAmount");
            }
        }
        private int connComps { get; set; }
        public int ConnectedComps
        {
            get
            {
                return connComps;
            }
            set
            {
                if (value != 0)
                    connComps = value;
                OnPropertyChanged("ConnectedComps");
            }
        }
        public bool IsBipartite { get; set; } = false;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
