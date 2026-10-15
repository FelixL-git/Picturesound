using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Picturesound.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }
        private Picturesound.ViewModels.MainViewModel? VM => DataContext as Picturesound.ViewModels.MainViewModel;

        private bool _isDragging = false;

        private void OverlayCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            OverlayCanvas.CaptureMouse();
            UpdateProgressFromPoint(e.GetPosition(OverlayCanvas));
        }

        private void OverlayCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            OverlayCanvas.ReleaseMouseCapture();
        }

        private void OverlayCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(OverlayCanvas);
            // Change cursor when over hit area
            var left = Canvas.GetLeft(HitArea);
            if (!double.IsNaN(left) && pos.X >= left && pos.X <= left + HitArea.Width)
            {
                OverlayCanvas.Cursor = Cursors.Hand;
            }
            else
            {
                OverlayCanvas.Cursor = Cursors.Arrow;
            }

            if (_isDragging && VM != null)
            {
                UpdateProgressFromPoint(pos);
            }
        }

        private void OverlayCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            OverlayCanvas.Cursor = Cursors.Hand;
        }

        private void OverlayCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isDragging) OverlayCanvas.Cursor = Cursors.Arrow;
        }

        private void UpdateProgressFromPoint(Point p)
        {
            if (VM == null) return;
            var ratio = 0.0;
            if (OverlayCanvas.ActualWidth > 0)
            {
                ratio = p.X / OverlayCanvas.ActualWidth;
            }
            VM.SetProgress(ratio);
        }
    }
}