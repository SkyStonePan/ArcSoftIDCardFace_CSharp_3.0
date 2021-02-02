using System;
using System.Runtime.InteropServices;

namespace ArcSoftIdCardSDK.Utils
{
    /// <summary>
    /// SDK函数引入
    /// </summary>
    public static class FICFunctions
    {
        /// <summary>
        /// 动态链接库名
        /// </summary>
        public const string DLLPATH = "libarcsoft_idcardveri.dll";

        /// <summary>
        /// 获取激活文件信息接口
        /// </summary>
        /// <param name="pActiveFileInfo">激活文件信息</param>
        /// <returns></returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArcSoft_FIC_GetActiveFileInfo(IntPtr pActiveFileInfo);

        /// <summary>
        /// 离线激活接口
        /// </summary>
        /// <param name="filePath">许可文件路径(离线授权文件)，需要读写权限</param>
        /// <returns></returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArcSoft_FIC_OfflineActivation(string filePath);

        /// <summary>
        /// 在线激活接口
        /// </summary>
        /// <param name="appId">SDK对应的AppID</param>
        /// <param name="sdkKey">SDK对应的SDKKey</param>
        /// <param name="activeKey">激活码</param>
        /// <returns>调用结果</returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArcSoft_FIC_OnlineActivation(string appId, string sdkKey, string activeKey);

        /// <summary>
        /// 在线激活接口(deprecate later)
        /// </summary>
        /// <param name="appId">SDK对应的AppID</param>
        /// <param name="sdkKey">SDK对应的SDKKey</param>
        /// <param name="activeKey">激活码</param>
        /// <returns></returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArcSoft_FIC_Activate(string appId, string sdkKey, string activeKey);

        /// <summary>
        /// 采集设备信息（可离线）
        /// </summary>
        /// <param name="pDeviceInfo">获取的设备信息字符串</param>
        /// <returns></returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArcSoft_FIC_GetActiveDeviceInfo(ref IntPtr pDeviceInfo);

        /// <summary>
        /// 初始化引擎
        /// </summary>
        /// <param name="pEngine">初始化返回的引擎handle</param>
        /// <param name="combinedMask">初始化属性</param>
        /// <returns>调用结果</returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArcSoft_FIC_InitialEngine(ref IntPtr pEngine, Int64 combinedMask = 0);
        
        /// <summary>
        /// 人脸特征提取
        /// </summary>
        /// <param name="hFICEngine">FIC引擎Handle</param>
        /// <param name="isVideo">人脸数据类型 1-视频 0-静态图片</param>
        /// <param name="pInputFaceData">人脸图像原始数据</param>
        /// <param name="pFaceRes">人脸属性 人脸数/人脸框/角度</param>
        /// <returns></returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArcSoft_FIC_FaceDataFeatureExtraction(IntPtr hFICEngine, bool isVideo, IntPtr pInputFaceData,  IntPtr pFaceRes);

        /// <summary>
        /// 证件照特征提取
        /// </summary>
        /// <param name="hFICEngine">FIC引擎Handle</param>
        /// <param name="pInputIdcardData">图像原始数据</param>
        /// <returns></returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArcSoft_FIC_IdCardDataFeatureExtraction(IntPtr hFICEngine, IntPtr pInputIdcardData);

        /// <summary>
        /// 人证比对
        /// </summary>
        /// <param name="hFICEngine">FIC引擎Handle</param>
        /// <param name="threshold">比对阈值</param>
        /// <param name="pSimilarScore">比对结果相似度</param>
        /// <param name="pResult">比对结果</param>
        /// <returns></returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArcSoft_FIC_FaceIdCardCompare(IntPtr hFICEngine, float threshold, ref float pSimilarScore, ref int pResult);

        /// <summary>
        /// 人脸信息(活体)检测-RGB
        /// </summary>
        /// <param name="hFICEngine">引擎Handle</param>
        /// <param name="pInputFaceData">人脸图像原始数据</param>
        /// <param name="pFaceRes">人脸属性</param>
        /// <param name="combinedMask">只支持初始化时候指定需要检测的功能，在process时进一步在这个已经指定的功能集中继续筛选</param>
        /// <returns></returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArcSoft_FIC_Process(IntPtr hFICEngine, IntPtr pInputFaceData, IntPtr pFaceRes, int combinedMask);

        /// <summary>
        /// 人脸信息(IR活体)检测
        /// </summary>
        /// <param name="hFICEngine">FIC 引擎Handle</param>
        /// <param name="pInputFaceDataIR">人脸图像原始数据</param>
        /// <param name="pFaceRes">人脸属性</param>
        /// <param name="combinedMask">只支持初始化时候指定需要检测的功能，在process时进一步在这个已经指定的功能集中继续筛选</param>
        /// <returns></returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArcSoft_FIC_Process_IR(IntPtr hFICEngine, IntPtr pInputFaceDataIR, IntPtr pFaceRes, int combinedMask);

        /// <summary>
        /// 设置活体阈值，取值范围[0-1]内部默认数值RGB-0.75 用户可以根据实际需求，设置不同的阈值
        /// </summary>
        /// <param name="hFICEngine">引擎handle</param>
        /// <param name="threshold">活体置信度</param>
        /// <returns></returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArcSoft_FIC_SetLivenessParam(IntPtr hFICEngine, IntPtr threshold);
        
        /// <summary>
        /// 获取RGB活体结果
        /// </summary>
        /// <param name="pEngine">FIC引擎Handle</param>
        /// <param name="livenessInfo">检测RGB活体结果</param>
        /// <returns></returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArcSoft_FIC_GetLivenessInfo(IntPtr hFICEngine, IntPtr livenessInfo);

        /// <summary>
        /// 获取IR活体结果
        /// </summary>
        /// <param name="hFICEngine">FIC 引擎Handle</param>
        /// <param name="pLivenessInfoIR">检测IR活体结果</param>
        /// <returns></returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArcSoft_FIC_GetLivenessInfo_IR(IntPtr hFICEngine, IntPtr pLivenessInfoIR);

        /// <summary>
        /// 设置图像质量阈值，取值范围[0-1]，用户可以根据实际需求，设置不同的阈值
        /// </summary>
        /// <param name="hFICEngine">FIC引擎Handle</param>
        /// <param name="pFaceQualityThreshold">图像质量阈值</param>
        /// <returns></returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Arcsoft_FIC_SetFaceQualityThreshold(IntPtr hFICEngine, IntPtr pFaceQualityThreshold);

        /// <summary>
        /// 释放引擎
        /// </summary>
        /// <param name="hFICEngine">FIC引擎Handle</param>
        /// <returns></returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ArcSoft_FIC_UninitialEngine(IntPtr hFICEngine);

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <param name="hFICEngine">FIC引擎Handle</param>
        /// <returns></returns>
        [DllImport(DLLPATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ArcSoft_FIC_GetVersion(IntPtr hFICEngine);
    }
}
