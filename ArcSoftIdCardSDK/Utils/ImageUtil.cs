using System;
using ArcSoftIdCardSDK.SDKModels;
using System.Drawing;
using System.Drawing.Imaging;

namespace ArcSoftIdCardSDK.Utils
{
    /// <summary>
    /// 图片处理工具类
    /// </summary>
    public static class ImageUtil
    {
        /// <summary>
        /// 获取RGB图片信息
        /// </summary>
        /// <param name="image">RGB图片</param>
        /// <returns></returns>
        public static ASVLOFFSCREEN ReadBmp(Image image, bool checkWidth = true)
        {
            if (checkWidth && image.Width % 4 != 0)
            {
                image = ScaleImage(image, image.Width - (image.Width % 4), image.Height);
            }
            ASVLOFFSCREEN offInput = new ASVLOFFSCREEN();
            //将Image转换为Format24bppRgb格式的BMP
            Bitmap bm = new Bitmap(image);
            //将Bitmap锁定到系统内存中,获得BitmapData
            System.Drawing.Imaging.BitmapData data = bm.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
            IntPtr ptr = data.Scan0;
            //定义数组长度
            int soureBitArrayLength = data.Height * Math.Abs(data.Stride);
            byte[] sourceBitArray = new byte[soureBitArrayLength];
            //将bitmap中的内容拷贝到ptr_bgr数组中
            MemoryUtil.Copy(ptr, 0, sourceBitArray, soureBitArrayLength);
            int width = data.Width;
            int height = data.Height;
            int pitch = Math.Abs(data.Stride);
            bm.UnlockBits(data);
			bm.Dispose();
            IntPtr imageDataPtr = MemoryUtil.Malloc(sourceBitArray.Length);
            MemoryUtil.Copy(sourceBitArray, 0, imageDataPtr, sourceBitArray.Length);
            offInput.u32PixelArrayFormat = (uint)ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8;
            offInput.ppu8Plane = new IntPtr[4];
            offInput.ppu8Plane[0] = imageDataPtr;
            offInput.i32Width = width;
            offInput.i32Height = height;
            offInput.pi32Pitch = new int[4];
            offInput.pi32Pitch[0] = pitch;
            return offInput;
        }

        /// <summary>
        /// 获取IR图片信息
        /// </summary>
        /// <param name="image">图片</param>
        /// <returns>图片数据</returns>
        public static ASVLOFFSCREEN ReadBmpIR(Image image, bool checkWidth = true)
        {
            if (checkWidth && image.Width % 4 != 0)
            {
                image = ScaleImage(image, image.Width - (image.Width % 4), image.Height);
            }
            ASVLOFFSCREEN offInput = new ASVLOFFSCREEN();
            Bitmap bm = new Bitmap(image);
            BitmapData data = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
                IntPtr ptr = data.Scan0;

                //定义数组长度
                int soureBitArrayLength = data.Height * Math.Abs(data.Stride);
                byte[] sourceBitArray = new byte[soureBitArrayLength];
                //将bitmap中的内容拷贝到数组中
                MemoryUtil.Copy(ptr, 0, sourceBitArray,  soureBitArrayLength);
                //填充引用对象字段值
                offInput.i32Width = data.Width;
                offInput.i32Height = data.Height;
                offInput.u32PixelArrayFormat = (int)ASF_ImagePixelFormat.ASVL_PAF_GRAY;

                //获取去除对齐位后度图像数据
                int ir_len = offInput.i32Width * offInput.i32Height;
                byte[] destBitArray = new byte[ir_len];
                offInput.pi32Pitch = new int[4];
                offInput.pi32Pitch[0] = offInput.i32Width;

                //灰度化
                int j = 0;
                double colortemp = 0;
                for (int i = 0; i < sourceBitArray.Length; i += 3)
                {
                    colortemp = sourceBitArray[i + 2] * 0.299 + sourceBitArray[i + 1] * 0.587 + sourceBitArray[i] * 0.114;
                    destBitArray[j++] = (byte)colortemp;
                }
                
                offInput.ppu8Plane = new IntPtr[4];
                offInput.ppu8Plane[0] = MemoryUtil.Malloc(destBitArray.Length);
                MemoryUtil.Copy(destBitArray, 0, offInput.ppu8Plane[0], destBitArray.Length);

                return offInput;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                bm.UnlockBits(data);
				bm.Dispose();
            }
        }

        /// <summary>
        /// 缩放图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="dstWidth"></param>
        /// <param name="dstHeight"></param>
        /// <returns></returns>
        public static Image ScaleImage(Image image, int dstWidth, int dstHeight)
        {
            Graphics g = null;
            //按比例缩放           
            float scaleRate = getWidthAndHeight(image.Width, image.Height, dstWidth, dstHeight);
            int width = (int)(image.Width * scaleRate);
            int height = (int)(image.Height * scaleRate);

            //将宽度调整为4的整数倍
            if (width % 4 != 0)
            {
                width -= width % 4;
            }

            Bitmap destBitmap = new Bitmap(width, height);
            g = Graphics.FromImage(destBitmap);
            g.Clear(Color.Transparent);

            //设置画布的描绘质量         
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(image, new Rectangle(0, 0, width, height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

            //设置压缩质量     
            EncoderParameters encoderParams = new EncoderParameters();
            long[] quality = new long[1];
            quality[0] = 100;
            EncoderParameter encoderParam = new EncoderParameter(Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;

            return destBitmap;
        }

        /// <summary>
        /// 得到缩放比例值
        /// </summary>
        /// <param name="oldWidth">旧的宽度</param>
        /// <param name="oldHeigt">旧的高度</param>
        /// <param name="newWidth">新的宽度</param>
        /// <param name="newHeight">新的高度</param>
        /// <returns></returns>
        public static float getWidthAndHeight(int oldWidth, int oldHeigt, int newWidth, int newHeight)
        {
            //按比例缩放           
            float scaleRate = 0.0f;
            if (oldWidth >= newWidth && oldHeigt >= newHeight)
            {
                int widthDis = oldWidth - newWidth;
                int heightDis = oldHeigt - newHeight;
                if (widthDis > heightDis)
                {
                    scaleRate = newWidth * 1f / oldWidth;
                }
                else
                {
                    scaleRate = newHeight * 1f / oldHeigt;
                }
            }
            else if (oldWidth >= newWidth)
            {
                scaleRate = newWidth * 1f / oldWidth;
            }
            else if (oldHeigt >= newHeight)
            {
                scaleRate = newHeight * 1f / oldHeigt;
            }
            else
            {
                int widthDis = newWidth - oldWidth;
                int heightDis = newHeight - oldHeigt;
                if (widthDis > heightDis)
                {
                    scaleRate = newHeight * 1f / oldHeigt;
                }
                else
                {
                    scaleRate = newWidth * 1f / oldWidth;
                }
            }
            return scaleRate;
        }

    }
}
