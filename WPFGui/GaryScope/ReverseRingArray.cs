namespace GaryScope
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ReverseRingArray : IEnumerable<ScopeSample>
    {
        private ScopeSample[] samples;
        private int nextSamplePosition = 0;

        public ReverseRingArray(int capacity)
        {
            samples = new ScopeSample[capacity];
        }

        public int Capacity { get { return samples.Length;  } }

        public void Add(ScopeSample sample)
        {
            samples[nextSamplePosition] = sample;
            if (nextSamplePosition < samples.Length - 1)
            {
                nextSamplePosition++;
            }
            else
            {
                nextSamplePosition = 0;
            }
        }

        public IEnumerator<ScopeSample> GetEnumerator()
        {
            int lastSamplePosition = nextSamplePosition == 0 ? samples.Length - 1 : nextSamplePosition - 1;
            return new ReverseRingArrayEnumerator(samples, lastSamplePosition);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
