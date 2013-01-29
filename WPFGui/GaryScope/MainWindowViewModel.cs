namespace GaryScope
{
    using System;
    using Microsoft.Research.DynamicDataDisplay.Common;
    using UsbLibrary;
    using GalaSoft.MvvmLight;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private SpecifiedDevice scopeDevice;
        private bool isRunning = true;
        private byte clockSpeed = 0x0b;
        private const int linesPerTrace = 100;
        private int? maxValue;
        private int? minValue;


        public MainWindowViewModel()
        {
            Trace1 = new ReverseRingArray(linesPerTrace);

            if (!IsInDesignMode)
            {
                scopeDevice = SpecifiedDevice.FindSpecifiedDevice(0x4242, 3);
                scopeDevice.DataRecieved += OnDataReceived;
            }
        }

        public ReverseRingArray Trace1 { get; private set; }

        void OnDataReceived(object sender, DataRecievedEventArgs args)
        {
            UInt16 captureval1 = (UInt16)((args.data[1] + args.data[2] * 256) * 0.92f);

            ScopeSample newSample = new ScopeSample { Time = DateTime.Now, Value = captureval1 };

            Trace1.Add(newSample);

            if (!MinValue.HasValue || newSample.Value < MinValue.Value)
            {
                MinValue = newSample.Value;
            }

            if (!MaxValue.HasValue || newSample.Value > MaxValue.Value)
            {
                MaxValue = newSample.Value;
            }
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

        public double ClockSpeed
        {
            get { return Convert.ToDouble(clockSpeed); }
            set
            {
                var newClockSpeed = Convert.ToByte(value);
                if (newClockSpeed != clockSpeed)
                {
                    clockSpeed = newClockSpeed;
                    SetClockSpeed();
                }
            }
        }

        public int? MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                RaisePropertyChanged(() => MaxValue);
            }
        }

        public int? MinValue
        {
            get { return minValue; }
            set
            {
                minValue = value;
                RaisePropertyChanged(() => MinValue);
            }
        }

        private void SetClockSpeed()
        {
            if (!IsInDesignMode)
            {
                byte[] command = new byte[2];
                command[0] = 1;
                command[1] = clockSpeed;
                scopeDevice.SendData(command);
            }
        }

        private void StartStopScope()
        {
            if (!IsInDesignMode)
            {
                byte[] command = new byte[2];
                command[0] = 2;
                command[1] = Convert.ToByte(isRunning);
                scopeDevice.SendData(command);
            }
        }
    }
}
