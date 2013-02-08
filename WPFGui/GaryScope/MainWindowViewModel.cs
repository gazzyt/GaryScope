namespace GaryScope
{
    using System;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using System.Threading;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private IScopeDevice scopeDevice;
        private bool isRunning = true;
        private byte clockSpeed = 0x0b;
        private const int linesPerTrace = 100;
        private int? maxValue;
        private int? minValue;
        private long samplesInLastPeriod = 0;
        private long samplesSinceBeginLastPeriod = 0;
        private Timer periodTimer;


        public MainWindowViewModel()
        {
            Trace1 = new ReverseRingArray(linesPerTrace);

            if (!IsInDesignMode)
            {
                //scopeDevice = new UsbScopeDevice();
                scopeDevice = new MockScopeDevice();
                scopeDevice.DataReceived +=new Action<byte[]>(OnDataReceived);
            }

            periodTimer = new Timer(PeriodTimerTick, null, 0, 1000);
        }

        public ReverseRingArray Trace1 { get; private set; }

        void OnDataReceived(byte[] data)
        {
            UInt16 captureval1 = (UInt16)(data[1] + data[2] * 256);

            ScopeSample newSample = new ScopeSample { Time = DateTime.Now, Value = captureval1 };

            Trace1.Add(newSample);

            Interlocked.Increment(ref samplesSinceBeginLastPeriod);

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

        public long SamplesInLastPeriod
        {
            get { return samplesInLastPeriod; }
            set
            {
                samplesInLastPeriod = value;
                RaisePropertyChanged(() => SamplesInLastPeriod);
            }
        }

        public ICommand ResetMaxMinCommand
        {
            get
            {
                return new RelayCommand(
                    () => 
                    { 
                        MaxValue = null; 
                        MinValue = null; 
                    });
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

        private void PeriodTimerTick(object state = null)
        {
            SamplesInLastPeriod = samplesSinceBeginLastPeriod;
            samplesSinceBeginLastPeriod = 0;
        }
    }
}
