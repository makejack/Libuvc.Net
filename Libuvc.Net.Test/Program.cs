﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using Libuvc.Net;

namespace Libuvc.Net.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Context context = new Context();

                var device = context.FindDevice();

                var deviceHandle = device.Open();

                var streamControl = deviceHandle.GetStreamControlFormatSize(FrameFormat.Yuyv, 640, 480, 30);

                deviceHandle.StartStreaming(ref streamControl, cameraFrameCallback);
                Console.ReadLine();

                deviceHandle.StopStreaming();
                deviceHandle.Dispose();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine(ex.StackTrace);
            }
        }

        private static void cameraFrameCallback(ref Frame frame, IntPtr user_ptr)
        {
            try
            {
                var bytes = Context.FrameToBytes(ref frame);

                Bitmap bitmap = new Bitmap((int)frame.Width, (int)frame.Height, PixelFormat.Format24bppRgb);
                int pos = 0;
                for (int y = 0; y < frame.Height; y++)
                {
                    for (int x = 0; x < frame.Width; x++)
                    {
                        bitmap.SetPixel(x, y, Color.FromArgb(bytes[pos], bytes[pos + 1], bytes[pos + 2]));

                        pos += 3;
                    }
                }
                bitmap.Save($"{Guid.NewGuid():N}.png");

                // using (var ms = new MemoryStream())
                // {

                //     bitmap.Save(ms, ImageFormat.Jpeg);

                //     System.Console.WriteLine(ms.Length);
                // }

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }
    }
}