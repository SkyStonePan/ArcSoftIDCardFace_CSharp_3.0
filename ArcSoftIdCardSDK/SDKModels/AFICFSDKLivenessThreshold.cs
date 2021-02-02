namespace ArcSoftIdCardSDK.SDKModels
{
    /// <summary>
    /// 活体设置参数
    /// </summary>
    public struct AFICFSDKLivenessThreshold
    {
        /// <summary>
        /// RGB模型阈值
        /// </summary>
        public float modelThresholdRgb;

        /// <summary>
        /// IR模型阈值
        /// </summary>
        public float modelThresholdIr;
    }
}
