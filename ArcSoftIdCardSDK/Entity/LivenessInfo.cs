namespace ArcSoftIdCardSDK.Entity
{
    /// <summary>
    /// 活体检测信息
    /// </summary>
    public class LivenessInfo
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
        public int isLive { get; set; }

        /// <summary>
        /// num
        /// </summary>
        public int num { get; set; }
    }
}
