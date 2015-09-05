using GalaSoft.MvvmLight.Threading;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
        private const byte MaxSample = 255;
        private const float GraphTopMargin = 20;
        private const float GraphBottomMargin = 20;
        private const float GraphLeftMargin = 40;
        private const float VoltageTextRequestedWidth = 40;
        private const float VoltageTextRequestedHeight = 40;

        private float VoltageLineSpacing;
        private float VoltageLineEnd;
        private float VoltageYScale;
        private CanvasTextLayout VoltageText0;
        private CanvasTextLayout VoltageText1;
        private CanvasTextLayout VoltageText2;
        private CanvasTextLayout VoltageText3;
        private CanvasTextLayout VoltageText4;
        private CanvasTextLayout VoltageText5;
        //private Pen[] pens;

        private Binding TraceBinding;
        public static DependencyProperty TraceProperty = DependencyProperty.Register("Trace", typeof(byte[]), typeof(MainPage), new PropertyMetadata(null, TracePropertyChanged));

        public byte[] Trace
        {
            get
            {
                return (byte[])GetValue(TraceProperty);
            }
        }

        private static void TracePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MainPage)d).canvas1.Invalidate();
        }


        public MainPage()
        {
            this.InitializeComponent();
            viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;
            TraceBinding = new Binding
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath("Trace")
            };

            this.SetBinding(TraceProperty, TraceBinding);

            //redrawTimer = new Timer(RedrawTimerTick, null, 0, 50);
        }

        private void RedrawTimerTick(object state = null)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => canvas1.Invalidate());
        }

        public void RecalculateLayout()
        {
            VoltageLineSpacing = (float)(canvas1.ActualHeight - GraphTopMargin - GraphBottomMargin) / 5;
            VoltageLineEnd = (float)canvas1.ActualWidth;
            VoltageYScale = (float)((canvas1.ActualHeight - GraphTopMargin - GraphBottomMargin) / MaxSample);
        }

        private void DrawScene()
        {
            //DrawLines();
        }

        private const double XAxisMultiplier = 5.0;

        private void DrawLines(CanvasDrawEventArgs args)
        {
            Vector2 lastLineStart = new Vector2();
            bool firstSample = true;
            int index = 0;
            int penIndex = 0;

            byte[] trace = Trace;

            if (trace != null)
            {
                foreach (var sample in Trace)
                {
                    if (firstSample)
                    {
                        lastLineStart = SampleToVector(index, sample);
                        firstSample = false;
                    }
                    else
                    {
                        Vector2 thisLineStart = SampleToVector(index, sample);
                        args.DrawingSession.DrawLine(lastLineStart, thisLineStart, Colors.Black);
                        lastLineStart = thisLineStart;
                        penIndex ^= 1;
                    }
                    index++;
                }
            }
        }

        private Vector2 SampleToVector(int index, byte value)
        {
            return new Vector2((float)(index * XAxisMultiplier) + GraphLeftMargin, ScaleY(value));
        }

        private float ScaleY(byte y)
        {
            return (MaxSample - y) * VoltageYScale + GraphTopMargin;
        }

        private void canvas1_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            DrawVoltageLine(args, 0);
            DrawVoltageLine(args, 1);
            DrawVoltageLine(args, 2);
            DrawVoltageLine(args, 3);
            DrawVoltageLine(args, 4);
            DrawVoltageLine(args, 5);

            DrawVoltageText(args, 0, VoltageText0);
            DrawVoltageText(args, 1, VoltageText1);
            DrawVoltageText(args, 2, VoltageText2);
            DrawVoltageText(args, 3, VoltageText3);
            DrawVoltageText(args, 4, VoltageText4);
            DrawVoltageText(args, 5, VoltageText5);

            DrawLines(args);
        }

        private float CalculateVoltageLineHeight(int indexFromTop)
        {
            return VoltageLineSpacing * indexFromTop + GraphTopMargin;
        }

        private void DrawVoltageLine(CanvasDrawEventArgs args, int indexFromTop)
        {
            float height = CalculateVoltageLineHeight(indexFromTop);
            args.DrawingSession.DrawLine(GraphLeftMargin, height, VoltageLineEnd, height, Colors.Black);
        }

        private void DrawVoltageText(CanvasDrawEventArgs args, int indexFromTop, CanvasTextLayout textLayout)
        {
            float height = CalculateVoltageLineHeight(indexFromTop) - (float)(textLayout.LayoutBounds.Height / 2);
            args.DrawingSession.DrawTextLayout(textLayout, 5, height, Colors.Black);
        }

        private void canvas1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RecalculateLayout();
            canvas1.Invalidate();
        }

        private void canvas1_CreateResources(CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            CanvasTextFormat ctf = new CanvasTextFormat
            {
                VerticalAlignment = CanvasVerticalAlignment.Top
            };

            VoltageText0 = new CanvasTextLayout(canvas1, "5V", ctf, VoltageTextRequestedWidth, VoltageTextRequestedHeight);
            VoltageText1 = new CanvasTextLayout(canvas1, "4V", ctf, VoltageTextRequestedWidth, VoltageTextRequestedHeight);
            VoltageText2 = new CanvasTextLayout(canvas1, "3V", ctf, VoltageTextRequestedWidth, VoltageTextRequestedHeight);
            VoltageText3 = new CanvasTextLayout(canvas1, "2V", ctf, VoltageTextRequestedWidth, VoltageTextRequestedHeight);
            VoltageText4 = new CanvasTextLayout(canvas1, "1V", ctf, VoltageTextRequestedWidth, VoltageTextRequestedHeight);
            VoltageText5 = new CanvasTextLayout(canvas1, "0V", ctf, VoltageTextRequestedWidth, VoltageTextRequestedHeight);

        }
    }
}
