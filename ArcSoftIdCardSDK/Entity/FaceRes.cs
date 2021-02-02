using ArcSoftIdCardSDK.SDKModels;

namespace ArcSoftIdCardSDK.Entity
{
    public class FaceRes
    {
        /// <summary>
        /// 检测人脸数
        /// </summary>
        public int nFace { set; get; }

        /// <summary>
        /// 人脸框
        /// </summary>
        public MRECT rcFace { set; get; }
    }
}