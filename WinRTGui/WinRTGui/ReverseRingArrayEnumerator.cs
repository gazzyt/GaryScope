// -----------------------------------------------------------------------
// <copyright file="ReverseRingArrayEnumerator.cs" company="ioko">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WinRTGui
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ReverseRingArrayEnumerator : IEnumerator<ScopeSample>
    {
        private ScopeSample[] allSamples;
        private int firstSamplePosition;
        private int lastSamplePosition;
        private int currentSamplePosition;

        public ReverseRingArrayEnumerator(ScopeSample[] allSamples, int lastSample)
        {
            this.allSamples = allSamples;
            this.firstSamplePosition = lastSample;
            currentSamplePosition = firstSamplePosition;
            lastSamplePosition = firstSamplePosition == (allSamples.Length - 1) ? 0 : firstSamplePosition + 1;
        }

        public ScopeSample Current
        {
            get { return allSamples[currentSamplePosition]; }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { throw new NotImplementedException(); }
        }

        public bool MoveNext()
        {
            if (currentSamplePosition == lastSamplePosition)
            {
                return false;
            }
            else
            {
                currentSamplePosition = currentSamplePosition == 0 ? allSamples.Length - 1 : currentSamplePosition - 1;
                return true;
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
