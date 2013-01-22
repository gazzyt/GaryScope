using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;

namespace WpgGeometry
{
    /// <summary>
    /// Interaction logic for CodeLines.xaml
    /// </summary>
    public partial class CodeLines : Window
    {
        private int nextSample = 0;
        private Timer redrawTimer;
        private GeometryGroup lines;
        private const int linesPerTrace = 100;
        private double[] samples = new double[linesPerTrace];
        private SineGenerator generator;

        public CodeLines()
        {
            InitializeComponent();
            generator = new SineGenerator();
            generator.Frequency = 200;
            generator.SamplesPerSecond = 300;
            generator.SampleReadyEvent += AddSample;
        }

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            redrawTimer = new Timer(RedrawTimerTick, null, 0, 50);
            generator.Start();
        }

        private void RedrawTimerTick(object state = null)
        {
            this.Dispatcher.BeginInvoke(
                new Action(() => this.DrawScene()));
        }
        private void DrawScene()
        {
            // 
            // Create the Geometry to draw. 
            //
            lines = new GeometryGroup();


            AddLines(lines);
            //lines.Children.Add(
            //    new EllipseGeometry(new Point(50, 50), 45, 20)
            //    );
            //lines.Children.Add(
            //    new EllipseGeometry(new Point(50, 50), 20, 45)
            //    );


            // 
            // Create a GeometryDrawing. 
            //
            GeometryDrawing aGeometryDrawing = new GeometryDrawing();
            aGeometryDrawing.Geometry = lines;

            // Paint the drawing with a gradient.
            aGeometryDrawing.Brush =
                new LinearGradientBrush(
                    Colors.Blue,
                    Color.FromRgb(204, 204, 255),
                    new Point(0, 0),
                    new Point(1, 1));

            // Outline the drawing with a solid color.
            aGeometryDrawing.Pen = new Pen(Brushes.Black, 1);

            // 
            // Use a DrawingImage and an Image control 
            // to display the drawing. 
            //
            DrawingImage geometryImage = new DrawingImage(aGeometryDrawing);

            // Freeze the DrawingImage for performance benefits.
            //geometryImage.Freeze();

            Image anImage = new Image();
            anImage.Source = geometryImage;
            anImage.Stretch = Stretch.None;
            anImage.HorizontalAlignment = HorizontalAlignment.Left;

            // 
            // Place the image inside a border and 
            // add it to the page. 
            //
            /*            Border exampleBorder = new Border();
                        exampleBorder.Child = anImage;
                        exampleBorder.BorderBrush = Brushes.Gray;
                        exampleBorder.BorderThickness = new Thickness(1);
                        exampleBorder.HorizontalAlignment = HorizontalAlignment.Left;
                        exampleBorder.VerticalAlignment = VerticalAlignment.Top;
                        exampleBorder.Margin = new Thickness(10);

                        this.Margin = new Thickness(20);
                        this.Background = Brushes.White;
                        this.Content = exampleBorder; */

            canvas.Children.Clear();
            canvas.Children.Add(anImage);
        }

        private void AddLines(GeometryGroup group)
        {
            Point lastLineEnd = new Point(0.0, 0.0);
            int sample = GetPreviousSample(nextSample);
            for (int s = 0; s < linesPerTrace; s++)
            {
                Point thisLineEnd = new Point(s * 5, samples[sample]);
                group.Children.Add(new LineGeometry(lastLineEnd, thisLineEnd));
                lastLineEnd = thisLineEnd;
                sample = GetPreviousSample(sample);
            }
        }


        private void AddSample(double sample)
        {
            samples[nextSample] = sample * 100;
            nextSample++;
            if (nextSample >= linesPerTrace)
            {
                nextSample = 0;
            }
        }

        private static int GetPreviousSample(int nextSample)
        {
            int retval = nextSample - 1;
            if (retval < 0)
            {
                retval = linesPerTrace - 1;
            }
            return retval;
        }
    }
}
