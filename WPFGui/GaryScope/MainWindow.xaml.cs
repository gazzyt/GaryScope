using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace GaryScope
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;
        private Timer redrawTimer;
        private const UInt16 MaxSample = 1024;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;
            redrawTimer = new Timer(RedrawTimerTick, null, 0, 50);
        }

        private void RedrawTimerTick(object state = null)
        {
            this.Dispatcher.BeginInvoke(
                new Action(() => this.DrawScene()));
        }

        private void DrawScene()
        {
            DrawLines();
        }

        private const double XAxisMultiplier = 5.0;

        private void DrawLines()
        {
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                Pen dp = new Pen(Brushes.Black, 1);
                Point lastLineEnd = new Point();
                bool firstSample = true;
                int reverseIndex = viewModel.Trace1.Capacity - 1;

                foreach (var sample in viewModel.Trace1)
                {
                    if (firstSample)
                    {
                        lastLineEnd = new Point(reverseIndex * XAxisMultiplier, ScaleY(sample.Value));
                        firstSample = false;
                    }
                    else
                    {
                        Point thisLineEnd = new Point(reverseIndex * XAxisMultiplier, ScaleY(sample.Value));
                        dc.DrawLine(dp, lastLineEnd, thisLineEnd);
                        lastLineEnd = thisLineEnd;
                    }
                    reverseIndex--;
                }
            
            }

            canvas.ClearVisuals();
            canvas.AddVisual(dv);
        }

        private static int ScaleY(UInt16 y)
        {
            return (MaxSample - y) >> 1;
        }
    }
}
