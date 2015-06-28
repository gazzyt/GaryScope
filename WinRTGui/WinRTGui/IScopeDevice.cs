namespace WinRTGui
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IScopeDevice
    {
        event Action<byte[]> DataReceived;
        void SendData(byte[] data);
    }
}
