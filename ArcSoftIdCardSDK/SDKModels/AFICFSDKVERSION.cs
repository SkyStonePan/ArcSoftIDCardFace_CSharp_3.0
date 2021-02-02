using System;

namespace ArcSoftIdCardSDK.SDKModels
{
    /// <summary>
    /// FIC 版本信息
    /// </summary>
    public struct AFICFSDKVERSION
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public IntPtr version;

        /// <summary>
        /// 版本最新构建信息
        /// </summary>
        public IntPtr buildDate;

        /// <summary>
        /// 版权所有
        /// </summary>
        public IntPtr copyRight;
    }
}
