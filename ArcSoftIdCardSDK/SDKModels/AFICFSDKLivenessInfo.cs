using System;

namespace ArcSoftIdCardSDK.SDKModels
{
    /// <summary>
    /// 活体信息检测结果结构体
    /// </summary>
    public struct AFICFSDKLivenessInfo
    {
        /// <summary>
        /// 判断是否真人
        /// 0：非真人;
        /// 1：真人;
        ///-1：不确定; 
        ///-2: 传入人脸数>1;
        ///-3：人脸过小
        ///-4：角度过大
        ///-5：人脸超出边界
        /// </summary>
        public IntPtr isLive;

        /// <summary>
        /// num
        /// </summary>
        public int num;
    }
}
