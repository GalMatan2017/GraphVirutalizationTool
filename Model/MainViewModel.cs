using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GraphVisualisationTool
{
    public class MainViewModel: INotifyPropertyChanged
    {
        #region Singleton
        private MainViewModel() {
            ShowNames = true;
        }
        private static MainViewModel instance = null;
        public static MainViewModel getInstance()
        {
                if (instance == null)
                {
                    instance = new MainViewModel();
                }
                return instance;
        }
        #endregion

        #region Collections
        private ObservableCollection<Vertex> vertices;
        public ObservableCollection<Vertex> Vertices
        {
            get { return vertices ?? (vertices = new ObservableCollection<Vertex>()); }
            set
            {
                if (value != null)
                {
                    vertices = value;
                    OnPropertyChanged("Vertices");
                }
            }
        }

        private ObservableCollection<Edge> edges;
        public ObservableCollection<Edge> Edges
        {
            get { return edges ?? (edges = new ObservableCollection<Edge>()); }
            set
            {
                if (value != null)
                {
                    edges = value;
                    OnPropertyChanged("Edges");
                }
            }
        }

        private CanvasObject selectedObject;
        public CanvasObject SelectedObject
        {
            get
            {
                return selectedObject;
            }
            set
            {
                Vertices.ToList().ForEach(x => x.IsHighlighted = false);
                selectedObject = value;
                OnPropertyChanged("SelectedObject");
                //DeleteCommand.IsEnabled = value != null;
                var connector = value as Edge;
                if (connector != null)
                {
                    if (connector.Start != null)
                        connector.Start.IsHighlighted = true;

                    if (connector.End != null)
                        connector.End.IsHighlighted = true;
                }
            }
        }
        #endregion

        #region Bool (Visibility) Names
        private bool _showNames;
        public bool ShowNames
        {
            get { return _showNames; }
            set
            {
                _showNames = value;
                OnPropertyChanged("ShowNames");
            }
        }
        #endregion

        #region Canvas
        private Canvas _mainCanvas;
        public Canvas MainCanvas
        {
            get { return _mainCanvas; }
            set { if (value != null) _mainCanvas = value; }
        }

        private int _canvasHeight;
        public int CanvasHeight
        {
            get { return _canvasHeight; }
            set
            {
                _canvasHeight = value;
                OnPropertyChanged("CanvasHeight");
            }
        }
        private int _canvasWidth;
        public int CanvasWidth
        {
            get { return _canvasWidth; }
            set
            {
                _canvasWidth = value;
                OnPropertyChanged("CanvasWidth");
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
