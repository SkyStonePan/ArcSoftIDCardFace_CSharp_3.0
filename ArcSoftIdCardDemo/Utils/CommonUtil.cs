using ArcSoftIdCardSDK.Entity;
using System.Drawing;
using ArcSoftIdCardSDK;
using System.Windows.Media.Imaging;
using System.Text;
using System.IO;

namespace ArcSoftIdCardDemo.Utils
{
    /// <summary>
    /// 方法工具类
    /// </summary>
    public static class CommonUtil
    {
        /// <summary>
        /// 判断是否为活体
        /// </summary>
        /// <param name="idCardEngine">引擎</param>
        /// <param name="faceRes">人脸信息</param>
        /// <param name="bitmapRgb">RGB摄像头图片</param>
        /// <param name="bitmapIr">IR摄像头图片</param>
        /// <param name="livennessResultStr">活体检测字符串</param>
        /// <returns></returns>
        public static bool IsLiveness(IdCardEngine idCardEngine, FaceRes faceRes, Bitmap bitmapRgb, Bitmap bitmapIr,ref string livennessResultStr)
        {
            bool retRgb = false;
            bool retIr = false;
            StringBuilder builder = new StringBuilder();
            if (faceRes.nFace > 0)
            {
                //判断RGB活体
                if (bitmapRgb != null && idCardEngine.ArcSoft_FIC_Process(bitmapRgb, faceRes).Equals(0))
                {
                    LivenessInfo liveness = null;
                    idCardEngine.ArcSoft_FIC_GetLivenessInfo(out liveness);
                    retRgb = liveness.isLive.Equals(1);
                    builder.AppendFormat(",RGB:{0}", TransLivenessResult(liveness.isLive));
                }
                //判断IR活体
                if (bitmapIr != null && idCardEngine.ArcSoft_FIC_Process_IR(bitmapIr, faceRes).Equals(0))
                {
                    LivenessInfo livenessIR = null;
                    idCardEngine.ArcSoft_FIC_GetLivenessInfo_IR(out livenessIR);
                    retIr = livenessIR.isLive.Equals(1);
                    builder.AppendFormat(",IR:{0}", TransLivenessResult(livenessIR.isLive));
                }
            }
            livennessResultStr = builder.ToString();
            return retRgb && (retIr || bitmapIr == null);
        }

        /// <summary>
        /// Bitmap转化为BitmapImage
        /// </summary>
        /// <param name="bitmap">bitmap</param>
        /// <returns></returns>
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms, bitmap.RawFormat);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }

        /// <summary>
        /// 根据文件路径获取BitmapImage
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static BitmapImage BitmapToBitmapImage(string filePath)
        {
            BitmapImage bitmapImage;
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                FileInfo fi = new FileInfo(filePath);
                byte[] bytes = reader.ReadBytes((int)fi.Length);
                reader.Close();
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(bytes);
                bitmapImage.EndInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            }
            return bitmapImage;
        }

        /// <summary>
        /// 根据图片路径读取图片
        /// </summary>
        /// <param name="imageUrl">图片路径</param>
        /// <returns></returns>
        public static Image ReadFromFile(string imageUrl)
        {
            Image img = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(imageUrl, FileMode.Open, FileAccess.Read);
                img = Image.FromStream(fs);
            }
            finally
            {
                fs.Close();
            }
            return img;
        }
        
        /// <summary>
        /// 转化活体检测结果
        /// </summary>
        /// <param name="liveness">活体检测值</param>
        /// <returns></returns>
        public static string TransLivenessResult(int liveness)
        {
            string rel;
            switch (liveness)
            {
                case 0: rel = "非真人"; break;
                case 1: rel = "真人"; break;
                case -2: rel = "传入人脸数>1"; break;
                case -3: rel = "人脸过小"; break;
                case -4: rel = "角度过大"; break;
                case -5: rel = "人脸超出边界"; break;
                default:
                    rel = "不确定"; break;
            }
            return rel;
        }
    }
}