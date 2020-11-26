using System;
using System.Runtime.InteropServices;

namespace Libuvc.Net
{
    public enum FrameFormat
    {
        Unknown = 0,
        /** any supported format */
        Any = 0,
        Uncompressed,
        Compressed,
        /** yuyv/yuv2/yuv422: yuv encoding with one luminance value per pixel and
         * one uv (chrominance) pair for every two pixels.
         */
        Yuyv,
        Uyvy,
        /** 24-bit rgb */
        Rgb,
        Bgr,
        /** motion-jpeg (or jpeg) encoded images */
        Mjpeg,
        H264,
        /** greyscale images */
        Gray8,
        Gray16,
        /* raw colour mosaic images */
        BY8,
        BA81,
        Sgrbg8,
        Sgbrg8,
        Srggb8,
        Sbggr8,
        /* YUV420: NV12 */
        Nv12,
        /* Number of formats understood */
        Count
    };

    public enum RequestCode
    {
        Undefined = 0x00,
        SetCurrent = 0x01,
        GetCurrent = 0x81,
        GetMin = 0x82,
        GetMax = 0x83,
        GetResolution = 0x84,
        GetLength = 0x85,
        GetInfo = 0x86,
        GetDefault = 0x87
    }

    public enum AutoExposureMode : byte
    {
        Manual = 0x1,
        Auto = 0x2,
        ShutterPriority = 0x4,
        AperturePriority = 0x8
    }

    public struct FormatDescriptor
    {
        public FrameFormat Format;
        public int Width;
        public int Height;
        public int Fps;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceDescriptor
    {
        public ushort IdVendor;
        public ushort IdProduct;
        public ushort BcdUVC;
        public string SerialNumber;
        public string Manufacturer;
        public string Product;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct StreamControl
    {
        public ushort Hint;
        public byte FormatIndex;
        public byte FrameIndex;
        public uint FrameInterval;
        public ushort KeyFrameRate;
        public ushort FrameRate;
        public ushort CompressionQuality;
        public ushort CompressionWindowSize;
        public ushort Delay;
        public uint MaxVideoFrameSize;
        public uint MaxPayloadTransferSize;
        public uint ClockFrequency;
        public byte FramingInfo;
        public byte PreferredVersion;
        public byte MinVersion;
        public byte MaxVersion;
        public byte InterfaceNumber;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Frame
    {
        public IntPtr Data;
        public IntPtr DataBytes;
        public uint Width;
        public uint Height;
        public FrameFormat FrameFormat;
        public IntPtr Step;
        public uint Sequence;
        // public long TvSec;
        // public long TvUsec;
        public Timeval CaptureTime;
        public Timespec CaptureTimeFinished;
        public IntPtr Source;
        public byte LibraryOwnsData;
        public IntPtr Metadata;
        public IntPtr MetadataBytes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Timeval
    {
        public long TvSec;
        public long TvUsec;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Timespec
    {
        public long TvSec;
        public long TvNsec;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FrameCallback(ref Frame frame, IntPtr user_ptr);
}
