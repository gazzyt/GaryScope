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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using UsbLibrary;

namespace GaryScope
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RingArray<ScopeSample> trace1 = new RingArray<ScopeSample>(1000);
        //private UsbHidPort scopeDevice = new UsbHidPort();
        SpecifiedDevice scopeDevice;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var ds = new EnumerableDataSource<ScopeSample>(trace1);
            ds.SetXMapping(pi => pi.Time.TimeOfDay.TotalSeconds);
            ds.SetYMapping(pi => pi.Value);
            LineGraph chart = plotter.AddLineGraph(ds, 2.0, String.Format("{0} - {1}", "Scope", "Trace 1"));

            scopeDevice = SpecifiedDevice.FindSpecifiedDevice(0x4242, 3);
            scopeDevice.DataRecieved += OnDataReceived;
        }

        void OnDataReceived(object sender, DataRecievedEventArgs args)
        {
            UInt16 captureval1 = (UInt16)((args.data[1] + args.data[2] * 256) * 0.92f);

            ScopeSample newSample = new ScopeSample { Time = DateTime.Now, Value = captureval1 };

            this.Dispatcher.BeginInvoke(
                new Action( () => trace1.Add(newSample) ));
        }
    }
}
