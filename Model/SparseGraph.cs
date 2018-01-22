using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GraphVirtualizationTool.Model
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
        private string _graphinfo { get; set; }
        public string GraphInfo
        {
            get
            {
                return _graphinfo;
            }
            set
            {
                if (value != null)
                    _graphinfo = value;
                OnPropertyChanged("GraphInfo");
            }
        }
        private bool _isBipartite { get; set; } = false;
        public bool IsBipartite
        {
            get
            {
                return _isBipartite;
            }
            set
            {
                _isBipartite = value;
                if (value == true)
                    GraphInfo = "A bipartite graph";
                else
                    GraphInfo = "A non-bipartite graph";
                OnPropertyChanged("IsBipartite");
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
