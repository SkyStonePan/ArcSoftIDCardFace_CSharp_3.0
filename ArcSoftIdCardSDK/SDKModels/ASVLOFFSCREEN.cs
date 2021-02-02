using System.Runtime.InteropServices;

namespace ArcSoftIdCardSDK.SDKModels
{
    /// <summary>
    /// 图像信息
    /// </summary>
    public struct ASVLOFFSCREEN
    {
        /// <summary>
        /// 图像格式
        /// </summary>
        public uint u32PixelArrayFormat;

        /// <summary>
        /// 宽
        /// </summary>
        public int i32Width;

        /// <summary>
        /// 高
        /// </summary>
        public int i32Height;

        /// <summary>
        /// 指针数组
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.SysUInt)]
        public System.IntPtr[] ppu8Plane;

        /// <summary>
        /// 整形数组
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I4)]
        public int[] pi32Pitch;
    }
}
