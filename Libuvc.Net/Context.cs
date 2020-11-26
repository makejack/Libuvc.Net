using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Libuvc.Net
{
    public class Context : IDisposable
    {
        readonly UvcContext handle;

        public Context()
        {
            var error = NativeMethods.uvc_init(out handle, IntPtr.Zero);
            UvcException.ThrowExceptionForUvcError(error);
        }

        public Device FindDevice(int vendorId = 0, int productId = 0, string serialNumber = null)
        {
            UvcDevice device;
            var error = NativeMethods.uvc_find_device(handle, out device, vendorId, productId, serialNumber);
            UvcException.ThrowExceptionForUvcError(error);
            return new Device(device);
        }

        public IEnumerable<Device> FindDevices(int vendorId = 0, int productId = 0, string serialNumber = null)
        {
            IntPtr devices;
            var error = NativeMethods.uvc_find_devices(handle, out devices, vendorId, productId, serialNumber);
            UvcException.ThrowExceptionForUvcError(error);
            try
            {
                int i = 0;
                IntPtr devh;
                while ((devh = Marshal.ReadIntPtr(devices, IntPtr.Size * i++)) != IntPtr.Zero)
                {
                    var device = new UvcDevice(devh);
                    yield return new Device(device);
                }
            }
            finally
            {
                NativeMethods.uvc_free_device_list(devices, 1);
            }
        }

        public IEnumerable<Device> GetDevices()
        {
            IntPtr devices;
            var error = NativeMethods.uvc_get_device_list(handle, out devices);
            UvcException.ThrowExceptionForUvcError(error);
            try
            {
                int i = 0;
                IntPtr devh;
                while ((devh = Marshal.ReadIntPtr(devices, IntPtr.Size * i++)) != IntPtr.Zero)
                {
                    var device = new UvcDevice(devh);
                    yield return new Device(device);
                }
            }
            finally
            {
                NativeMethods.uvc_free_device_list(devices, 1);
            }
        }

        public void Dispose()
        {
            handle.Dispose();
        }


        #region Static Function

        public static byte[] FrameToBytes(ref Frame frame)
        {
            var size = frame.Width * frame.Height * 3;

            Frame outFrame = new Frame
            {
                Width = frame.Width,
                Height = frame.Height,
                Step = (IntPtr)(frame.Width * 3),
                Sequence = frame.Sequence,
                CaptureTime = frame.CaptureTime,
                CaptureTimeFinished = frame.CaptureTimeFinished,
                Source = frame.Source,
                LibraryOwnsData = 1,
                DataBytes = (IntPtr)size,
                Data = Marshal.AllocHGlobal((int)size)
            };

            byte[] bytes = new byte[size];
            try
            {
                UvcException.ThrowExceptionForUvcError(NativeMethods.uvc_any2rgb(ref frame, ref outFrame));

                Marshal.Copy(outFrame.Data, bytes, 0, bytes.Length);
            }
            finally
            {
                Marshal.FreeHGlobal(outFrame.Data);
            }
            return bytes;
        }

        #endregion
    }
}