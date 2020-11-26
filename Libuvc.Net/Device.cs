using System;
using System.Runtime.InteropServices;

namespace Libuvc.Net
{
    public class Device : IDisposable
    {
        readonly UvcDevice handle;
        readonly ushort vendorId;
        readonly ushort productId;
        readonly ushort complianceLevel;
        readonly string serialNumber;
        readonly string manufacturer;
        readonly string product;

        internal Device(UvcDevice device)
        {
            handle = device;
            IntPtr descriptor;
            var error = NativeMethods.uvc_get_device_descriptor(handle, out descriptor);
            UvcException.ThrowExceptionForUvcError(error);
            try
            {
                var deviceDescriptor = Marshal.PtrToStructure<DeviceDescriptor>(descriptor);
                vendorId = deviceDescriptor.IdVendor;
                productId = deviceDescriptor.IdProduct;
                complianceLevel = deviceDescriptor.BcdUVC;
                product = deviceDescriptor.Product;
                manufacturer = deviceDescriptor.Manufacturer;
                serialNumber = deviceDescriptor.SerialNumber;
            }
            finally { NativeMethods.uvc_free_device_descriptor(descriptor); }
        }

        public ushort VendorId
        {
            get { return vendorId; }
        }

        public ushort ProductId
        {
            get { return productId; }
        }

        public ushort ComplianceLevel
        {
            get { return complianceLevel; }
        }

        public string SerialNumber
        {
            get { return serialNumber; }
        }

        public string Manufacturer
        {
            get { return manufacturer; }
        }

        public string Product
        {
            get { return product; }
        }

        public DeviceHandle Open()
        {
            UvcDeviceHandle devh;
            var error = NativeMethods.uvc_open(handle, out devh);
            UvcException.ThrowExceptionForUvcError(error);
            return new DeviceHandle(devh);
        }

        public void Dispose()
        {
            handle.Dispose();
        }
    }
}
