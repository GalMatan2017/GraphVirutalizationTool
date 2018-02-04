namespace GraphVisualisationTool
{
    public class Edge: CanvasObject
    {
        //The Connector's X and Y properties are always 0 because the line coordinates are actually determined
        //by the Start.X, Start.Y and End.X, End.Y Nodes' properties.
        public override double X
        {
            get { return 0; }
            set { }
        }
        public override double Y
        {
            get { return 0; }
            set { }
        }
        public override double Z
        {
            get { return 0; }
            set { }
        }
        private Vertex _start;
        public Vertex Start
        {
            get { return _start; }
            set
            {
                if (value != null) {

                    _start = value;
                     OnPropertyChanged("Start");
                }
            }
        }

        private Vertex _end;
        public Vertex End
        {
            get { return _end; }
            set
            {
                if (value != null)
                {

                    _end = value;
                    OnPropertyChanged("Start");
                }
            }
        }
    }
}