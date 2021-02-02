using System;

namespace ArcSoftIdCardSDK.Entity
{
    /// <summary>
    /// 图像信息
    /// </summary>
    public class ImageData
    {
        /// <summary>
        /// 图像格式
        /// </summary>
        public uint u32PixelArrayFormat { get; set; }

        /// <summary>
        /// 宽
        /// </summary>
        public int i32Width { get; set; }

        /// <summary>
        /// 高
        /// </summary>
        public int i32Height { get; set; }

        /// <summary>
        /// 图像信息
        /// </summary>
        public IntPtr ppu8Plane { get; set; }

        /// <summary>
        /// 图像步长
        /// </summary>
        public int pi32Pitch { get; set; }
    }
}