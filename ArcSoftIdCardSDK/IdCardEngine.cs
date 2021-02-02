using System;
using ArcSoftIdCardSDK.SDKModels;
using ArcSoftIdCardSDK.Utils;
using ArcSoftIdCardSDK.Entity;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace ArcSoftIdCardSDK
{
    /// <summary>
    /// SDK功能函数
    /// </summary>
    public class IdCardEngine
    {
        /// <summary>
        /// 引擎Handle
        /// </summary>
        private IntPtr pEngine;

        /// <summary>
        /// 人脸特征提取模式(默认为true：视频)
        /// false:静态图片
        /// true：视频
        /// </summary>
        private readonly bool mode = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pMode">人脸特征提取模式：true->Video模式；false->Image模式</param>
        public IdCardEngine(bool pMode=true)
        {
            mode = pMode;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~IdCardEngine()
        {
            ArcSoft_FIC_UninitialEngine();
        }

        /// <summary>
        /// 获取激活文件信息接口
        /// </summary>
        /// <param name="activeFileInfo"></param>
        /// <returns>接口返回码，返回0表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int ArcSoft_FIC_GetActiveFileInfo(out ActiveFileInfo activeFileInfo)
        {
            int retCode = -1;
            activeFileInfo = new ActiveFileInfo();
            IntPtr pActiveFileInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<AFIC_FSDK_ActiveFileInfo>());
            //获取激活文件信息接口
            retCode = FICFunctions.ArcSoft_FIC_GetActiveFileInfo(pActiveFileInfo);
            if (!retCode.Equals(0))
            {
                MemoryUtil.Free(pActiveFileInfo);
                return retCode;
            }
            AFIC_FSDK_ActiveFileInfo sActiveFileInfo = MemoryUtil.PtrToStruct<AFIC_FSDK_ActiveFileInfo>(pActiveFileInfo);
            MemoryUtil.Free(pActiveFileInfo);
            #region 指针转化字符串
            activeFileInfo.startTime = Marshal.PtrToStringAnsi(sActiveFileInfo.startTime);
            activeFileInfo.endTime = Marshal.PtrToStringAnsi(sActiveFileInfo.endTime);
            activeFileInfo.activeKey = Marshal.PtrToStringAnsi(sActiveFileInfo.activeKey);
            activeFileInfo.platform = Marshal.PtrToStringAnsi(sActiveFileInfo.platform);
            activeFileInfo.sdkType = Marshal.PtrToStringAnsi(sActiveFileInfo.sdkType);
            activeFileInfo.appId = Marshal.PtrToStringAnsi(sActiveFileInfo.appId);
            activeFileInfo.sdkKey = Marshal.PtrToStringAnsi(sActiveFileInfo.sdkKey);
            activeFileInfo.sdkVersion = Marshal.PtrToStringAnsi(sActiveFileInfo.sdkVersion);
            activeFileInfo.fileVersion = Marshal.PtrToStringAnsi(sActiveFileInfo.fileVersion);
            #endregion
            return retCode;
        }

        /// <summary>
        /// 离线激活接口
        /// </summary>
        /// <param name="filePath">许可文件路径(离线授权文件)</param>
        /// <returns>接口返回码，返回0或90114表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int ArcSoft_FIC_OfflineActivation(string filePath)
        {
            int retCode = -1;
            //调用SDK离线激活接口
            retCode = FICFunctions.ArcSoft_FIC_OfflineActivation(filePath);
            return retCode;
        }

        /// <summary>
        /// 在线激活接口
        /// </summary>
        /// <param name="appId">SDK对应的AppID</param>
        /// <param name="sdkKey">SDK对应的SDKKey</param>
        /// <param name="activeKey">激活码</param>
        /// <returns>接口返回码，返回0或90114表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int ArcSoft_FIC_OnlineActivation(string appId, string sdkKey, string activeKey)
        {
            //调用SDK的在线激活函数
            return FICFunctions.ArcSoft_FIC_OnlineActivation(appId, sdkKey, activeKey);
        }

        /// <summary>
        /// 激活SDK(deprecate later)
        /// </summary>
        /// <param name="appId">SDK对应的AppID</param>
        /// <param name="sdkKey">SDK对应的SDKKey</param>
        /// <param name="activeKey">激活码</param>
        /// <returns>接口返回码，返回0或90114表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int ArcSoft_FIC_Activate(string appId, string sdkKey, string activeKey)
        {
            //调用SDK的在线激活函数
            return FICFunctions.ArcSoft_FIC_Activate(appId, sdkKey, activeKey);
        }

        /// <summary>
        /// 采集设备信息（可离线）
        /// </summary>
        /// <param name="ActiveDeviceInfo">获取的设备信息字符串</param>
        /// <returns>接口返回码，返回0表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int ArcSoft_FIC_GetActiveDeviceInfo(out string ActiveDeviceInfo)
        {
            int retCode = -1;
            ActiveDeviceInfo = string.Empty;
            IntPtr pActiveDeviceInfo = IntPtr.Zero;
            retCode = FICFunctions.ArcSoft_FIC_GetActiveDeviceInfo(ref pActiveDeviceInfo);
            if (!retCode.Equals(0))
            {
                MemoryUtil.Free(pActiveDeviceInfo);
                return retCode;
            }
            ActiveDeviceInfo = Marshal.PtrToStringAnsi(pActiveDeviceInfo);
            return retCode;
        }

        /// <summary>
        /// 初始化引擎
        /// </summary>
        /// <param name="combinedMask">初始化属性</param>
        /// <returns>接口返回码，返回0表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int ArcSoft_FIC_InitialEngine(int engineMask)
        {
            //调用SDK的初始化引擎函数
            return FICFunctions.ArcSoft_FIC_InitialEngine(ref pEngine, engineMask);
        }

        /// <summary>
        /// 人脸特征提取
        /// </summary>
        /// <param name="image">人脸图像</param>
        /// <param name="faceRes">人脸信息检测结果</param>
        /// <returns>接口返回码，返回0表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int ArcSoft_FIC_FaceDataFeatureExtraction(Image image, ref FaceRes faceRes)
        {
            faceRes = new FaceRes();
            IntPtr pFaceRes = MemoryUtil.Malloc(MemoryUtil.SizeOf<AFICFSDKFACERES>());
            ASVLOFFSCREEN imagetemp = ImageUtil.ReadBmp(image, false);
            IntPtr pInputFaceData = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASVLOFFSCREEN>());
            MemoryUtil.StructToPtr(imagetemp, pInputFaceData);
            //调用SDK的人脸特征提取接口
            int retCode = FICFunctions.ArcSoft_FIC_FaceDataFeatureExtraction(pEngine, mode, pInputFaceData, pFaceRes);
            if (!retCode.Equals(0))
            {
                MemoryUtil.Free(pFaceRes);
                MemoryUtil.Free(pInputFaceData);
                MemoryUtil.Free(imagetemp.ppu8Plane[0]);
                return retCode;
            }
            AFICFSDKFACERES faceResStruct = MemoryUtil.PtrToStruct<AFICFSDKFACERES>(pFaceRes);
            faceRes.nFace = faceResStruct.nFace;
            faceRes.rcFace = faceResStruct.rcFace;
            MemoryUtil.Free(pFaceRes);
            MemoryUtil.Free(pInputFaceData);
            MemoryUtil.Free(imagetemp.ppu8Plane[0]);
            return retCode;
        }

        /// <summary>
        /// 证件照特征提取
        /// </summary>
        /// <param name="image">证件照</param>
        /// <returns>接口返回码，返回0表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int ArcSoft_FIC_IdCardDataFeatureExtraction(Image image)
        {
            int retCode = -1;
            if (image != null)
            {
                ASVLOFFSCREEN imagetemp = ImageUtil.ReadBmp(image);
                IntPtr pInputIdcardData = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASVLOFFSCREEN>());
                MemoryUtil.StructToPtr(imagetemp, pInputIdcardData);
                //调用SDK
                retCode = FICFunctions.ArcSoft_FIC_IdCardDataFeatureExtraction(pEngine, pInputIdcardData);
                MemoryUtil.Free(pInputIdcardData);
                MemoryUtil.Free(imagetemp.ppu8Plane[0]);
            }
            return retCode;
        }

        /// <summary>
        /// 人证比对
        /// </summary>
        /// <param name="threshold">比对阈值</param>
        /// <param name="pSimilarScore">比对结果相似度</param>
        /// <param name="result">比对结果</param>
        /// <returns>接口返回码，返回0表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int ArcSoft_FIC_FaceIdCardCompare(float gThreshold, ref float pSimilarScore, ref int result)
        {
            //调用SDK的人证比对接口
            return FICFunctions.ArcSoft_FIC_FaceIdCardCompare(pEngine, gThreshold, ref pSimilarScore, ref result);
        }

        /// <summary>
        /// 人脸信息(活体)检测
        /// </summary>
        /// <param name="bitmap">人脸图像</param>
        /// <param name="faceRes">人脸属性</param>
        /// <returns>接口返回码，返回0表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int ArcSoft_FIC_Process(Bitmap bitmap, FaceRes faceRes)
        {
            int retCode = -1;
            ASVLOFFSCREEN imageinfo = ImageUtil.ReadBmp(bitmap);
            IntPtr pImageInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASVLOFFSCREEN>());
            MemoryUtil.StructToPtr(imageinfo, pImageInfo);
            AFICFSDKFACERES sFacerects = new AFICFSDKFACERES();
            sFacerects.nFace = faceRes.nFace;
            sFacerects.rcFace = faceRes.rcFace;
            IntPtr pFacerects = MemoryUtil.Malloc(MemoryUtil.SizeOf<AFICFSDKFACERES>());
            MemoryUtil.StructToPtr(sFacerects, pFacerects);
            //调用SDK
            retCode = FICFunctions.ArcSoft_FIC_Process(pEngine, pImageInfo, pFacerects, EngineMask.AFIC_LIVENESS);
            MemoryUtil.Free(imageinfo.ppu8Plane[0]);
            MemoryUtil.Free(pImageInfo);
            MemoryUtil.Free(pFacerects);
            return retCode;
        }

        /// <summary>
        /// 人脸信息(IR活体)检测
        /// </summary>
        /// <param name="bitmap">人脸图像</param>
        /// <param name="faceRes">人脸信息</param>
        /// <param name="combinedMask">检测属性，默认IR活体检测</param>
        /// <returns>接口返回码，返回0表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int ArcSoft_FIC_Process_IR(Bitmap bitmap, FaceRes faceRes, int combinedMask = EngineMask.AFIC_LIVENESS_IR)
        {
            int retCode = -1;
            ASVLOFFSCREEN imageinfo = ImageUtil.ReadBmpIR(bitmap);
            AFICFSDKFACERES sFacerects = new AFICFSDKFACERES();
            sFacerects.nFace = faceRes.nFace;
            sFacerects.rcFace = faceRes.rcFace;
            IntPtr pImageInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASVLOFFSCREEN>());
            MemoryUtil.StructToPtr(imageinfo, pImageInfo);
            IntPtr pFacerects = MemoryUtil.Malloc(MemoryUtil.SizeOf<AFICFSDKFACERES>());
            MemoryUtil.StructToPtr(sFacerects, pFacerects);
            //调用SDK
            retCode = FICFunctions.ArcSoft_FIC_Process_IR(pEngine, pImageInfo, pFacerects, combinedMask);
            MemoryUtil.Free(imageinfo.ppu8Plane[0]);
            MemoryUtil.Free(pImageInfo);
            MemoryUtil.Free(pFacerects);
            return retCode;
        }

        /// <summary>
        /// 设置活体阈值，取值范围[0-1]内部默认数值RGB-0.75 用户可以根据实际需求，设置不同的阈值
        /// </summary>
        /// <param name="livenessThreshold">活体置信度</param>
        /// <returns>接口返回码，返回0表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int ArcSoft_FIC_SetLivenessParam(AFICFSDKLivenessThreshold livenessThreshold)
        {
            int retCode = -1;
            IntPtr pLivenessThreshold = MemoryUtil.Malloc(MemoryUtil.SizeOf<AFICFSDKLivenessThreshold>());
            MemoryUtil.StructToPtr(livenessThreshold, pLivenessThreshold);
            //调用SDK
            retCode = FICFunctions.ArcSoft_FIC_SetLivenessParam(pEngine, pLivenessThreshold);
            MemoryUtil.Free(pLivenessThreshold);
            return retCode;
        }

        /// <summary>
        /// 获取RGB活体结果
        /// </summary>
        /// <param name="livenessInfo">RGB活体结果</param>
        /// <returns>接口返回码，返回0表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int ArcSoft_FIC_GetLivenessInfo(out LivenessInfo livenessInfo)
        {
            livenessInfo = new LivenessInfo();
            int retCode = -1;
            IntPtr plivenessInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<AFICFSDKLivenessInfo>());
            //调用SDK
            retCode = FICFunctions.ArcSoft_FIC_GetLivenessInfo(pEngine, plivenessInfo);
            if (!retCode.Equals(0))
            {
                MemoryUtil.Free(plivenessInfo);
                return retCode;
            }
            AFICFSDKLivenessInfo slivenessInfo = MemoryUtil.PtrToStruct<AFICFSDKLivenessInfo>(plivenessInfo);
            livenessInfo.isLive = MemoryUtil.PtrToStruct<int>(slivenessInfo.isLive);
            livenessInfo.num = slivenessInfo.num;
            MemoryUtil.Free(plivenessInfo);
            return retCode;
        }

        /// <summary>
        /// 获取IR活体结果
        /// </summary>
        /// <param name="livenessInfo">IR活体结果</param>
        /// <returns>接口返回码，返回0表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int ArcSoft_FIC_GetLivenessInfo_IR(out LivenessInfo livenessInfo)
        {
            livenessInfo = new LivenessInfo();
            int retCode = -1;
            IntPtr plivenessInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<AFICFSDKLivenessInfo>());
            //调用SDK
            retCode = FICFunctions.ArcSoft_FIC_GetLivenessInfo_IR(pEngine, plivenessInfo);
            if (!retCode.Equals(0))
            {
                MemoryUtil.Free(plivenessInfo);
                return retCode;
            }
            AFICFSDKLivenessInfo slivenessInfo = MemoryUtil.PtrToStruct<AFICFSDKLivenessInfo>(plivenessInfo);
            livenessInfo.isLive = MemoryUtil.PtrToStruct<int>(slivenessInfo.isLive);
            livenessInfo.num = slivenessInfo.num;
            MemoryUtil.Free(plivenessInfo);
            return retCode;
        }

        /// <summary>
        /// 设置图像质量阈值，设置图像质量阈值，取值范围[0-1].
        /// 用户可以根据实际需求，设置不同的阈值
        /// </summary>
        /// <param name="faceQualityThreshold">图像质量阈值</param>
        /// <returns>接口返回码，返回0表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int Arcsoft_FIC_SetFaceQualityThreshold(AFIC_FSDK_FaceQualityThreshold faceQualityThreshold)
        {
            int retCode = -1;
            IntPtr pFaceQualityThreshold = MemoryUtil.Malloc(MemoryUtil.SizeOf<AFIC_FSDK_FaceQualityThreshold>());
            MemoryUtil.StructToPtr(faceQualityThreshold, pFaceQualityThreshold);
            //调用SDK
            retCode = FICFunctions.Arcsoft_FIC_SetFaceQualityThreshold(pEngine, pFaceQualityThreshold);
            MemoryUtil.Free(pFaceQualityThreshold);
            return retCode;
        }

        /// <summary>
        /// 释放引擎
        /// </summary>
        /// <returns>接口返回码，返回0表示正常，返回其他值请在开发者中心-帮助中心查询</returns>
        public int ArcSoft_FIC_UninitialEngine()
        {
            //判断引擎是否已初始化
            if (!pEngine.Equals(IntPtr.Zero))
            {
                return FICFunctions.ArcSoft_FIC_UninitialEngine(pEngine);
            }
            return 0;            
        }

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        public string ArcSoft_FIC_GetVersion()
        {
            string version=string.Empty;
            //调用SDK
            IntPtr pVersion = FICFunctions.ArcSoft_FIC_GetVersion(pEngine);
            if(pVersion.Equals(IntPtr.Zero))
            {
                MemoryUtil.Free(pVersion);
                return version;
            }
            AFICFSDKVERSION Version = MemoryUtil.PtrToStruct<AFICFSDKVERSION>(pVersion);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("版本号:{0}{1}", Marshal.PtrToStringAnsi(Version.version) , Environment.NewLine);
            sb.AppendFormat("版本最新构建信息:{0}{1}", Marshal.PtrToStringAnsi(Version.buildDate) , Environment.NewLine);
            sb.AppendFormat("版权所有:{0}", Marshal.PtrToStringAnsi(Version.copyRight));
            version = sb.ToString();
            return version;
        }        
    }
}
