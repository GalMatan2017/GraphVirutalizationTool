using System;
using System.Windows.Media;

namespace GraphVisualisationTool
{
    public class Vertex : CanvasObject
    {
        private double x;
        public override double X
        {
            get { return x; }
            set
            {
                if (value > MainViewModel.getInstance().CanvasWidth)
                    x = MainViewModel.getInstance().CanvasWidth;
                else if (value < 0)
                    x = 0;
                else
                    x = value;

                CenterX = x + Vertex.vertexSize / 2;
                OnPropertyChanged("X");
            }
        }

        private double y;       
        public override double Y
        {
            get { return y; }
            set
            {
                if (value > MainViewModel.getInstance().CanvasHeight)
                    y = MainViewModel.getInstance().CanvasHeight;
                else if (value < 0)
                    y = 0;
                else
                    y = value;
                CenterY = (int)y + Vertex.vertexSize / 2;
                OnPropertyChanged("Y");
            }
        }

        private double z;
        public override double Z
        {
            get { return z; }
            set
            {
                z = value;
                OnPropertyChanged("Z");
            }
        }

        private bool isHighlighted;
        public bool IsHighlighted
        {
            get
            {
                return isHighlighted;
            }
            set
            {
                isHighlighted = value;
                OnPropertyChanged("IsHighlighted");
            }
        }

        public static int vertexSize = 30;
        public int VertexSize
        {
            get { return vertexSize; }
            set
            {
                if (value > 0)
                {
                    vertexSize = value;
                    OnPropertyChanged("VertexSize");
                    Y = Y;
                    X = X;
                }
            }
        }

        private double center_x;
        public double CenterX {
            get { return center_x; }
            set
            {
                center_x = value;
                OnPropertyChanged("CenterX");
            }
        }

        private double center_y;
        public double CenterY
        {
            get { return center_y; }
            set
            {
                center_y = value;
                OnPropertyChanged("CenterY");
            }
        }

        private Brush vertexColor;
        public Brush VertexColor
        {
            get { return vertexColor; }
            set
            {
                vertexColor = value;
                OnPropertyChanged("VertexColor");
            }
        }
    }
}