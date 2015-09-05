// -----------------------------------------------------------------------
// <copyright file="MockScopeDevice.cs" company="ioko">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WinRTGui
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MockScopeDevice : IScopeDevice
    {
        SineGenerator sineGenerator;

        public MockScopeDevice()
        {
            sineGenerator = new SineGenerator();
            sineGenerator.SampleReadyEvent += new SineGenerator.SampleReadyDelegate(sineGenerator_SampleReadyEvent);
            sineGenerator.Start();
        }

        void sineGenerator_SampleReadyEvent(double sample)
        {
            if (TraceReceived != null)
            {
                UInt16 iSample = Convert.ToUInt16((sample + 1.0) * 512);
                byte[] byteData = new byte[3];
                byteData[2] = (byte)(iSample >> 8);
                byteData[1] = (byte)(iSample & 0x00FF);
                Debug.WriteLine("{0} | {1} | {2} | {3}", sample, iSample, byteData[1], byteData[2]);
                TraceReceived(byteData);
            }
        }

        public event Action<byte[]> TraceReceived;
        public event Action Connected;
        public event Action Disconnected;

        public void SendData(byte[] data)
        {
            if (data[0] == 2)
            {
                if (data[1] == 0)
                {
                    sineGenerator.Stop();
                }
                else
                {
                    sineGenerator.Start();
                }
            }
        }
    }
}
