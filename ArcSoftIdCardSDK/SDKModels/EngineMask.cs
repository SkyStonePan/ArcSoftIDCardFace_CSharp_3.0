namespace ArcSoftIdCardSDK.SDKModels
{
    /// <summary>
    /// SDK引擎
    /// </summary>
    public static class EngineMask
    {
        /// <summary>
        /// RGB活体
        /// </summary>
        public const int AFIC_LIVENESS = 0x00000080;

        /// <summary>
        /// IR活体
        /// </summary>
        public const int AFIC_LIVENESS_IR = 0x00000400;

        /// <summary>
        /// 图像质量检测
        /// </summary>
        public const int AFIC_IMAGEQUALITY = 0x00000200;
    }
}
