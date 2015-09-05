namespace WinRTGui
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IScopeDevice
    {
        event Action<byte[]> TraceReceived;
        event Action Connected;
        event Action Disconnected;
        void SendData(byte[] data);
    }
}
