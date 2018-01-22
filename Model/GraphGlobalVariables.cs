using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.ComponentModel;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GraphVirtualizationTool
{
    public class GraphGlobalVariables : INotifyPropertyChanged
    {
        private GraphGlobalVariables() { }
        private static GraphGlobalVariables instance = null;
        public static GraphGlobalVariables getInstance()
        {
            if (instance == null)
            {
                instance = new GraphGlobalVariables();
            }
            return instance;
        }
        private string _filename { get; set; }
        public string Filepath { get; set; }
        public string Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                if (value != null)
                    _filename = value;
                OnPropertyChanged("Filename");
            }
        }
        public int TryParseInt32(string text, out int value)
        {
            int tmp;
            if (int.TryParse(text, out tmp))
            {
                value = tmp;
                return value;
            }
            else
            {
                value = -1;
                return value;
            }
        }
        public void ExportToPng(Canvas surface)
        {
            string dir;
            string filename = $"{DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")}.bmp";
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                dir = dialog.FileName;
            }
            else
            {
                return;
            }
            // System.IO.Directory.CreateDirectory("C:/GraphVirtualizationSaves/");
            Uri path;
            path = new Uri($"{dir}/{filename}");
            // Get the size of canvas
            Size size = new Size(surface.Width, surface.Height);
            // Measure and arrange the surface
            // VERY IMPORTANT
            Canvas printCanvas = new Canvas();
            printCanvas.Background = new VisualBrush(surface);
            printCanvas.Measure(size);
            printCanvas.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(printCanvas);

            // Create a file stream for saving image
            using (FileStream outStream = new FileStream(path.LocalPath, FileMode.Create))
            {
                // Use png encoder for our data
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outStream);

                MessageBox.Show($"File: {filename}\nSuccessfully saved to:\nDirectory: {dir}");
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
