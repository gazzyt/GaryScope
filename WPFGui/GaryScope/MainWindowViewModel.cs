﻿namespace GaryScope
{
    using System;
    using Microsoft.Research.DynamicDataDisplay.Common;
    using UsbLibrary;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MainWindowViewModel
    {
        private SpecifiedDevice scopeDevice;
        private bool isRunning = true;

        public MainWindowViewModel()
        {
            Trace1 = new RingArray<ScopeSample>(1000);
            scopeDevice = SpecifiedDevice.FindSpecifiedDevice(0x4242, 3);
            scopeDevice.DataRecieved += OnDataReceived;
        }

        public RingArray<ScopeSample> Trace1 { get; private set; }

        void OnDataReceived(object sender, DataRecievedEventArgs args)
        {
            UInt16 captureval1 = (UInt16)((args.data[1] + args.data[2] * 256) * 0.92f);

            ScopeSample newSample = new ScopeSample { Time = DateTime.Now, Value = captureval1 };

            App.Current.Dispatcher.BeginInvoke(
                new Action(() => Trace1.Add(newSample)));
        }

        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;
                StartStopScope();
            }
        }

        private void StartStopScope()
        {
            byte[] command = new byte[2];
            command[0] = 2;
            command[1] = Convert.ToByte(isRunning);
            scopeDevice.SendData(command);
        }
    }
}
