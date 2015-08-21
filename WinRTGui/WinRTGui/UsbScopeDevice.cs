using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Foundation;
using Windows.Storage;

namespace WinRTGui
{
    public class UsbScopeDevice : IScopeDevice
    {
        private const ushort UsagePage = 0x01;
        private const ushort UsageId = 0x00;
        private const ushort Vid = 0x4242;
        private const ushort Pid = 0x003;
        private DeviceWatcher ScopeDeviceWatcher;

        public UsbScopeDevice()
        {
            var usbScopeSelector = HidDevice.GetDeviceSelector(UsagePage, UsageId, Vid, Pid);
            ScopeDeviceWatcher = DeviceInformation.CreateWatcher(usbScopeSelector);
            ScopeDeviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>(OnDeviceAdded);
            ScopeDeviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(OnDeviceRemoved);
            ScopeDeviceWatcher.Start();
        }

        public event Action<byte[]> DataReceived;
        public event Action Connected;
        public event Action Disconnected;

        public void SendData(byte[] data)
        {
            throw new NotImplementedException();
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
            if (Connected != null)
            {
                Connected();
            }
        }

    }
}
