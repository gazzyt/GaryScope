using System;
using System.Windows;

using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using System.Threading;
using System.Windows.Controls;
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
        private GeometryGroup lines;

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

        private const double XAxisMultiplier = 5.0;

        private void AddLines(GeometryGroup group)
        {
            Point lastLineEnd = new Point();
            bool firstSample = true;
            int reverseIndex = viewModel.Trace1.Capacity - 1;

            foreach (var sample in viewModel.Trace1)
            {
                if (firstSample)
                {
                    lastLineEnd = new Point(reverseIndex * XAxisMultiplier, sample.Value);
                    firstSample = false;
                }
                else
                {
                    Point thisLineEnd = new Point(reverseIndex * XAxisMultiplier, sample.Value);
                    group.Children.Insert(0, new LineGeometry(lastLineEnd, thisLineEnd));
                    lastLineEnd = thisLineEnd;
                }
                reverseIndex--;
            }
        }
    }
}
