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
        private Pen[] pens;

        public MainWindow()
        {
            InitializeComponent();
            pens = new Pen[2];
            pens[0] = new Pen(Brushes.Red, 1);
            pens[1] = new Pen(Brushes.Blue, 1);
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
                //Pen dp = new Pen(Brushes.Black, 1);
                Point lastLineEnd = new Point();
                bool firstSample = true;
                int reverseIndex = viewModel.Trace1.Capacity - 1;
                int penIndex = 0;

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
                        dc.DrawLine(pens[penIndex], lastLineEnd, thisLineEnd);
                        lastLineEnd = thisLineEnd;
                        penIndex ^= 1;
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
