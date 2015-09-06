using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace WinRTGui
{
    public class UsbScopeDevice : IScopeDevice
    {
        private const ushort UsagePage = 0xFF56;
        private const ushort UsageId = 0xA6;
        private const ushort Vid = 0x4242;
        private const ushort Pid = 0x004;
        private const ushort NumSamplesPerPacket = 7;
        private const ushort NumPacketsPerTrace = 35;
        private const ushort PacketOverheadBytes = 2;
        private DeviceWatcher ScopeDeviceWatcher;
        private HidDevice ConnectedScope;
        private ushort NextPacketExpected = 0;
        private byte[] CurrentTrace = null;

        public UsbScopeDevice()
        {
            var usbScopeSelector = HidDevice.GetDeviceSelector(UsagePage, UsageId, Vid, Pid);
            ScopeDeviceWatcher = DeviceInformation.CreateWatcher(usbScopeSelector);
            ScopeDeviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>(OnDeviceAdded);
            ScopeDeviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(OnDeviceRemoved);
            ScopeDeviceWatcher.Start();
        }

        public event Action<byte[]> TraceReceived;
        public event Action Connected;
        public event Action Disconnected;

        public async void SendData(byte[] data)
        {
            if (ConnectedScope != null)
            {
                try
                {
                    var outputReport = ConnectedScope.CreateOutputReport();
                    WindowsRuntimeBufferExtensions.CopyTo(data, 0, outputReport.Data, 1, data.Length);
                    await ConnectedScope.SendOutputReportAsync(outputReport);
                }
                catch (Exception e)
                {
                    int y = 0;
                }
            }
        }

        public async Task<bool> Connect()
        {
            bool retval = false;

            var usbScopeSelector = HidDevice.GetDeviceSelector(UsagePage, UsageId, Vid, Pid);
            var devInfos = await DeviceInformation.FindAllAsync(usbScopeSelector);

            var scopeDeviceInfo = devInfos.FirstOrDefault();

            if (scopeDeviceInfo != null)
            {
                var scopeDevice = await HidDevice.FromIdAsync(scopeDeviceInfo.Id, FileAccessMode.ReadWrite);

                if (scopeDevice != null)
                {
                    retval = true;
                }
            }

            return retval;
        }

        private void OnDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate deviceInformationUpdate)
        {
            if (Disconnected != null)
            {
                Disconnected();
            }
        }

        private void OnDeviceAdded(DeviceWatcher sender, DeviceInformation deviceInformation)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(async () => 
            {
                ConnectedScope = await HidDevice.FromIdAsync(deviceInformation.Id, FileAccessMode.ReadWrite);

                if (ConnectedScope != null)
                {
                    ConnectedScope.InputReportReceived += ConnectedScope_InputReportReceived;

                    if (Connected != null)
                    {
                        Connected();
                    }
                }
            });
        }

        private void ConnectedScope_InputReportReceived(HidDevice sender, HidInputReportReceivedEventArgs args)
        {
            if (args.Report.Data.Length != NumSamplesPerPacket + PacketOverheadBytes)
            {
                Debug.WriteLine("Expected {0} bytes input report, received {1} bytes", NumSamplesPerPacket + PacketOverheadBytes, args.Report.Data.Length);
                return;
            }

            var bytes = new byte[args.Report.Data.Length];
            DataReader dr = DataReader.FromBuffer(args.Report.Data);
            dr.ReadBytes(bytes);

            if (bytes[1] != NextPacketExpected)
            {
                Debug.WriteLine("Expect packet {0}, received packet {1}. Discarding trace.", NextPacketExpected, bytes[1]);
                CurrentTrace = null;
                NextPacketExpected = 0;
                return;
            }

            if (NextPacketExpected == 0)
            {
                Debug.WriteLine("Packet 0 received, allocating buffer");
                CurrentTrace = new byte[NumPacketsPerTrace * NumSamplesPerPacket];
            }

            Array.Copy(bytes, PacketOverheadBytes, CurrentTrace, NextPacketExpected * NumSamplesPerPacket, NumSamplesPerPacket);

            NextPacketExpected++;

            if (NextPacketExpected == NumPacketsPerTrace)
            {
                Debug.WriteLine("Got full trace");
                if (TraceReceived != null)
                {
                    TraceReceived(CurrentTrace);
                }
                CurrentTrace = null;
                NextPacketExpected = 0;
            }

        }

    }

}
