using System;
using System.Windows.Media;

namespace GraphVirtualizationTool
{
    public class Node : DiagramObject
    {

        private double _x;
        public override double X
        {
            get { return _x; }
            set
            {
                if (value > MainViewModel.getInstance().CanvasWidth)
                    _x = MainViewModel.getInstance().CanvasWidth;
                else if (value < 0)
                    _x = 0;
                else
                    _x = value;
                OnPropertyChanged("X");
            }
        }
        private double _y;       
        public override double Y
        {
            get { return _y; }
            set
            {
                if (value > MainViewModel.getInstance().CanvasHeight)
                    _y = MainViewModel.getInstance().CanvasHeight;
                else if (value < 0)
                    _y = 0;
                else
                    _y = value;
                OnPropertyChanged("Y");
            }
        }
        private double _z;
        public override double Z
        {
            get { return _z; }
            set
            {
                _z = value;
                OnPropertyChanged("Z");
            }
        }
        private bool _isHighlighted { get; set; }
        public bool IsHighlighted
        {
            get
            {
                return _isHighlighted;
            }
            set
            {
                _isHighlighted = value;
                OnPropertyChanged("IsHighlighted");
            }
        }
        //Node ellipse resizing
        public static int _nodeSize = 30;
        public int NodeSize
        {
            get { return _nodeSize; }
            set
            {
                if (value > 0)
                {
                    _nodeSize = value;
                    OnPropertyChanged("NodeSize");
                }
            }
        }

        private Brush _nodeColor;
        public Brush NodeColor
        {
            get { return _nodeColor; }
            set
            {
                _nodeColor = value;
                OnPropertyChanged("color");
            }
        }
    }
}