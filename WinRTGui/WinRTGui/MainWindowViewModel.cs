namespace WinRTGui
{
    using System;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using System.Threading;
    using GalaSoft.MvvmLight.Threading;



    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private bool useMockScopeDevice = false;

        private IScopeDevice scopeDevice;
        private bool isRunning = true;
        private byte clockSpeed = 0x0b;
        private const int linesPerTrace = 100;
        private int? maxValue;
        private int? minValue;
        private long samplesInLastPeriod = 0;
        private long samplesSinceBeginLastPeriod = 0;
        private Timer periodTimer;
        private bool scopeConnected = false;


        public MainWindowViewModel()
        {
            Trace1 = new ReverseRingArray(linesPerTrace);

            if (!IsInDesignMode)
            {
                if (useMockScopeDevice)
                {
                    scopeDevice = new MockScopeDevice();
                }
                else
                {
                    scopeDevice = new UsbScopeDevice();
                }

                scopeDevice.TraceReceived +=new Action<byte[]>(OnDataReceived);
                scopeDevice.Connected += ScopeDevice_Connected;
                scopeDevice.Disconnected += ScopeDevice_Disconnected;
            }

            periodTimer = new Timer(PeriodTimerTick, null, 0, 1000);
        }

        private void ScopeDevice_Disconnected()
        {
            ScopeConnected = false;
        }

        private void ScopeDevice_Connected()
        {
            ScopeConnected = true;
        }

        public ReverseRingArray Trace1 { get; private set; }

        public bool ScopeConnected
        {
            get
            {
                return scopeConnected;
            }
            set
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => Set(ref scopeConnected, value));
            }
        }

        void OnDataReceived(byte[] data)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => Trace = data);
        }

        void OnSampleReceived(byte lowByte, byte highByte)
        {
            UInt16 captureval1 = (UInt16)(lowByte + highByte * 256);

            ScopeSample newSample = new ScopeSample { Time = DateTime.Now, Value = captureval1 };

            Trace1.Add(newSample);

            Interlocked.Increment(ref samplesSinceBeginLastPeriod);

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {

               if (!MinValue.HasValue || newSample.Value < MinValue.Value)
               {
                   MinValue = newSample.Value;
               }

               if (!MaxValue.HasValue || newSample.Value > MaxValue.Value)
               {
                   MaxValue = newSample.Value;
               }
            });
        }

        private byte[] _trace;

        public byte[] Trace
        {
            get { return _trace; }
            set
            {
                _trace = value;
                RaisePropertyChanged(() => Trace);
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
            var samples = samplesSinceBeginLastPeriod;
            samplesSinceBeginLastPeriod = 0;
            DispatcherHelper.CheckBeginInvokeOnUI(() => SamplesInLastPeriod = samples);
        }
    }
}
