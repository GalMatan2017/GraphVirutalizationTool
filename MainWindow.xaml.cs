using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;

namespace GraphVirtualizationTool
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainViewModel.getInstance();
          //  SizeToContent = SizeToContent.WidthAndHeight;

        }
        private void Thumb_Drag(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb == null)
                return;
            var node = thumb.DataContext as Node;
            if (node == null)
                return;
            node.X += e.HorizontalChange;
            node.Y += e.VerticalChange;
        }
        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var listbox = sender as ListBox;

            if (listbox == null)
                return;
            var vm = listbox.DataContext as MainViewModel;
            if (vm == null)
                return;
            if (vm.SelectedObject != null && vm.SelectedObject is Node && vm.SelectedObject.IsNew)
            {
                vm.SelectedObject.X = e.GetPosition(listbox).X;
                vm.SelectedObject.Y = e.GetPosition(listbox).Y;
            }
            else if (vm.SelectedObject != null && vm.SelectedObject is Edge && vm.SelectedObject.IsNew)
            {
                var node = GetNodeUnderMouse();
                if (node == null)
                    return;
                var edge = vm.SelectedObject as Edge;
                if (edge.Start != null && node != edge.Start)
                    edge.End = node;

            }
        }
        private void ListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            if (vm != null)
            {
                var node = GetNodeUnderMouse();
                var connector = vm.SelectedObject as Edge;
                if (node != null && connector != null && connector.Start == null)
                {
                    connector.Start = node;
                    node.IsHighlighted = true;
                    e.Handled = true;
                    return;
                }
                if (vm.SelectedObject != null)
                    vm.SelectedObject.IsNew = false;
            }
        }
        private Node GetNodeUnderMouse()
        {
            var item = Mouse.DirectlyOver as ContentPresenter;
            if (item == null)
                return null;
            return item.DataContext as Node;
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            MainViewModel.getInstance().MainCanvas = canvas;
        }
    }
}