namespace WpgGeometry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using System.Threading;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SineGenerator
    {
        private Timer timer;
        private double frequency;
        private int samplesPerSecond;
        private double radiansPerTick;
        private double currentPhase = 0.0;

        public SineGenerator()
        {
            // Sensible defaults
            Frequency = 10.0;
            SamplesPerSecond = 10;
        }

        public double Frequency 
        {
            get { return frequency; }
            set
            {
                frequency = value;
                UpdateRadiansPerTick();
            }
        }

        private void UpdateRadiansPerTick()
        {
            radiansPerTick = Math.PI * frequency / samplesPerSecond;
        }

        public int SamplesPerSecond 
        {
            get { return samplesPerSecond; }
            set
            {
                if (timer != null)
                {
                    throw new InvalidOperationException("Cannot change samples per second while generator is running");
                }
                samplesPerSecond = value;
                UpdateRadiansPerTick();
            }
        }

        public delegate void SampleReadyDelegate(double sample);
        public event SampleReadyDelegate SampleReadyEvent;

        private void SampleTimerTick(object state)
        {
            double sample = Math.Sin(currentPhase);
            currentPhase += radiansPerTick;

            if (SampleReadyEvent != null)
            {
                SampleReadyEvent(sample);
            }
        }

        public void Start()
        {
            if (timer != null)
            {
                throw new InvalidOperationException("Generator already started");
            }

            timer = new Timer(SampleTimerTick, null, 0, 1000 / samplesPerSecond);
        }

        public void Stop()
        {
            timer.Dispose();
            timer = null;
        }
    }
}
