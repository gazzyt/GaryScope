namespace GaryScope
{
    using System;
    using UsbLibrary;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UsbScopeDevice : IScopeDevice
    {
        private SpecifiedDevice scopeDevice;

        public UsbScopeDevice()
        {
            scopeDevice = SpecifiedDevice.FindSpecifiedDevice(0x4242, 3);
            if (scopeDevice != null)
            {
                scopeDevice.DataRecieved += OnDataReceived;
            }

        }

        public event Action<byte[]> DataReceived;

        void OnDataReceived(object sender, DataRecievedEventArgs args)
        {
            if (DataReceived != null)
            {
                DataReceived(args.data);
            }
        }

        public void SendData(byte[] data)
        {
            scopeDevice.SendData(data);
        }
    }
}
