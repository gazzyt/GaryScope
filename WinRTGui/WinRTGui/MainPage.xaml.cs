using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WinRTGui
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainWindowViewModel viewModel;
        private Timer redrawTimer;
        private const UInt16 MaxSample = 1024;
        //private Pen[] pens;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;
            redrawTimer = new Timer(RedrawTimerTick, null, 0, 50);
        }

        private void RedrawTimerTick(object state = null)
        {
            //this.Dispatcher.BeginInvoke(
            //    new Action(() => this.DrawScene()));
        }

        private void DrawScene()
        {
            DrawLines();
        }

        private const double XAxisMultiplier = 5.0;

        private void DrawLines()
        {
            //DrawingVisual dv = new DrawingVisual();
            //using (DrawingContext dc = dv.RenderOpen())
            //{
            //    //Pen dp = new Pen(Brushes.Black, 1);
            //    Point lastLineEnd = new Point();
            //    bool firstSample = true;
            //    int reverseIndex = viewModel.Trace1.Capacity - 1;
            //    int penIndex = 0;

            //    foreach (var sample in viewModel.Trace1)
            //    {
            //        if (firstSample)
            //        {
            //            lastLineEnd = new Point(reverseIndex * XAxisMultiplier, ScaleY(sample.Value));
            //            firstSample = false;
            //        }
            //        else
            //        {
            //            Point thisLineEnd = new Point(reverseIndex * XAxisMultiplier, ScaleY(sample.Value));
            //            dc.DrawLine(pens[penIndex], lastLineEnd, thisLineEnd);
            //            lastLineEnd = thisLineEnd;
            //            penIndex ^= 1;
            //        }
            //        reverseIndex--;
            //    }

            //}

            //canvas.ClearVisuals();
            //canvas.AddVisual(dv);
        }

        private static int ScaleY(UInt16 y)
        {
            return (MaxSample - y) >> 1;
        }

        private void canvas1_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            args.DrawingSession.DrawText("Hello Win2D", 100, 100, Colors.Black);
        }
    }
}
