using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GraphVisualisationTool.Model
{
    class DenseGraph : Graph, INotifyPropertyChanged
    {
        private List<List<bool>> data;
        public List<List<T>> getData<T>()
        {
            return (List<List<T>>)Convert.ChangeType(data, typeof(List<List<T>>));
        }
        public void setData<T>(T data)
        {
            this.data = new List<List<bool>>();
            this.data = (List<List<bool>>)Convert.ChangeType(data, typeof(List<List<bool>>));
        }
        public List<int> getNeighbours(int node)
        {
            List<int> neighbours = new List<int>();
            for(int i = 0; i < data.Count; i++)
            {
                if(data[node-1][i] == true)
                    neighbours.Add(i+1);
            }
            return neighbours;
        }
        public GraphTypes GraphType { get; set; } = GraphTypes.Dense;
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
