namespace ArcSoftIdCardSDK.SDKModels
{
    /// <summary>
    /// FIC FT/FD人脸特征检测
    /// </summary>
    public struct AFICFSDKFACERES
    {
        /// <summary>
        /// 检测人脸数
        /// </summary>
        public int nFace;

        /// <summary>
        /// 人脸框
        /// </summary>
        public MRECT rcFace;
    }
}
