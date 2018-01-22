namespace GraphVirtualizationTool
{
    public class Edge: DiagramObject
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
        private Node _start;
        public Node Start
        {
            get { return _start; }
            set
            {
                _start = value;
                OnPropertyChanged("Start");
            }
        }

        private Node _end;
        public Node End
        {
            get { return _end; }
            set
            {
                _end = value;
                OnPropertyChanged("End");
            }
        }
    }
}