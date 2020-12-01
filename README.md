# Libuvc.Net
Libuvc.Net 是使用linux下的libuvc获取uvc摄像头

需要使用cmake构建
```
git clone https://github.com/ktossell/libuvc.git
cd libuvc
mkdir build
cd build
cmake -DCMAKE_BUILD_TYPE=Release ..
make && sudo make install
```

使用System.Drawing.Common保存图片

linux下使用System.Drawing.Common需要安装libgdiplus

`apt-get install -y libgdiplus`

# 使用

``` c#

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
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }
        
```

# RGb 转 BMP
``` c#

        private void RgbToBMP(byte[] buffer, int width, int height)
        {
            int yu = width * 3 % 4;
            int bytePerLine = 0;
            yu = yu != 0 ? 4 - yu : yu;
            bytePerLine = width * 3 + yu; //1920

            using (var stream = new MemoryStream())
            {
                using (var bw = new BinaryWriter(stream))
                {
                    bw.Write('B');
                    bw.Write('M');
                    bw.Write(bytePerLine * height + 54);
                    bw.Write(0);
                    bw.Write(54);
                    bw.Write(40);
                    bw.Write(width);
                    bw.Write(height);
                    bw.Write((ushort)1);
                    bw.Write((ushort)24);
                    bw.Write(0);
                    bw.Write(bytePerLine * height);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(0);

                    byte[] data = new byte[bytePerLine * height]; //921600

                    var pos = 0;
                    for (int y = height - 1; y >= 0; y--)
                    {
                        for (int x = 0, i = 0; x < width; x++, i += 3)
                        {
                            data[y * bytePerLine + i] = buffer[pos];  // R
                            data[y * bytePerLine + i + 1] = buffer[pos + 1]; // G
                            data[y * bytePerLine + i + 2] = buffer[pos + 2]; //B
                            pos += 3;
                        }
                    }

                    bw.Write(data, 0, data.Length);
                    bw.Flush();

                    stream.Seek(0, SeekOrigin.Begin);

                      // other code
                }
            }
        }
```
     
