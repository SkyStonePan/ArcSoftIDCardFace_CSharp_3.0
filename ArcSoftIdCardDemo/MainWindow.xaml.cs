using System.Windows.Controls;
using System.Configuration;
using System.Windows.Forms;
using System.Threading;
using System.Windows;
using System;
using System.IO;
using System.Text;
using System.Drawing;
using ArcSoftIdCardSDK;
using Panuon.UI.Silver;
using ArcSoftIdCardSDK.Entity;
using ArcSoftIdCardDemo.Utils;
using AForge.Video.DirectShow;
using ArcSoftIdCardSDK.SDKModels;
using System.Resources;
using System.Linq;

namespace ArcSoft_IdCardDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX
    {
        #region 参数定义
        /// <summary>
        /// 记录textbox中的阈值
        /// </summary>
        private string textValue = "0.82";
        /// <summary>
        /// 身份证信息
        /// </summary>
        private IdCardText info = new IdCardText();
        /// <summary>
        /// 摄像头设备列表
        /// </summary>
        private FilterInfoCollection filterInfoCollection;
        /// <summary>
        /// RGB摄像头播放控件
        /// </summary>
        private VideoCaptureDevice cSourcePlayerRgb;
        /// <summary>
        /// IR摄像头播放控件
        /// </summary>
        private VideoCaptureDevice cSourcePlayerIr;
        /// <summary>
        /// SDK引擎:Video模式
        /// </summary>
        private IdCardEngine idCardVeriEngine = new IdCardEngine(true);
        /// <summary>
        /// 认证对比结果
        /// </summary>
        private int result = 0;
        /// <summary>
        /// 活体结果
        /// </summary>
        private bool liveness = false;
        /// <summary>
        /// 绘图锁，保证一次检测一帧
        /// </summary>
        private bool paintLocked = false;
        /// <summary>
        /// 人脸框信息
        /// </summary>
        private FaceRes faceInfo = new FaceRes();
        /// <summary>
        /// 读卡器锁，判断是否有身份证置于读卡器上
        /// </summary>
        private bool isCrvChecked = false;
        /// <summary>
        /// 阈值
        /// </summary>
        private float threshold = 0.82f;
        /// <summary>
        /// RGB活体置信度
        /// </summary>
        private AFICFSDKLivenessThreshold livenessThreshold = new AFICFSDKLivenessThreshold();
        /// <summary>
        /// 图像质量阈值
        /// </summary>
        private AFIC_FSDK_FaceQualityThreshold faceQualityThreshold = new AFIC_FSDK_FaceQualityThreshold();
        /// <summary>
        /// 激活信息
        /// </summary>
        private ActiveFileInfo activeInfo = new ActiveFileInfo();
        /// <summary>
        /// RGB摄像头索引
        /// </summary>
        private int rgbVideoIndex;
        /// <summary>
        /// IR摄像头索引
        /// </summary>
        private int irVideoIndex;
        /// <summary>
        /// 相似度
        /// </summary>
        private float similarValue = 0.0f;
        /// <summary>
        /// 活体检测字符串
        /// </summary>
        private string livennessResultStr = string.Empty;
        /// <summary>
        /// VideoPlayer 框的字体
        /// </summary>
        private Font videoFont = new Font(System.Drawing.FontFamily.GenericSerif, 10f, System.Drawing.FontStyle.Bold);
        /// <summary>
        /// 红色画笔
        /// </summary>
        private SolidBrush redBrush = new SolidBrush(Color.Red);
        /// <summary>
        /// 黄色画笔
        /// </summary>
        private SolidBrush greenBrush = new SolidBrush(Color.Green);
        /// <summary>
        /// 读取卡的锁
        /// </summary>
        private object readCardLocker = new object();
        #endregion

        #region SDK激活、引擎初始化、视频模块初始化
        /// <summary>
        /// 初始化
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            //初始化SDK引擎
            initArcsdkEngine();
            //初始化视频
            videoInit();
            //读取身份证信息
            readIdCardInfo();
        }

        /// <summary>
        /// 初始化引擎
        /// </summary>
        private void initArcsdkEngine()
        {
            try
            {
                //获取配置的信息
                AppSettingsReader reader = new AppSettingsReader();
                string appId = (string)reader.GetValue("APPID", typeof(string));
                string sdkKey32 = (string)reader.GetValue("SDKKEY32", typeof(string));
                string sdkKey64 = (string)reader.GetValue("SDKKEY64", typeof(string));
                string activeKey32 = (string)reader.GetValue("ACTIVEKEY32", typeof(string));
                string activeKey64 = (string)reader.GetValue("ACTIVEKEY64", typeof(string));
                rgbVideoIndex = (int)reader.GetValue("RGB", typeof(int));
                irVideoIndex = (int)reader.GetValue("IR", typeof(int));
                //判断CPU位数
                var is64CPU = Environment.Is64BitProcess;
                string sdkKey = is64CPU ? sdkKey64 : sdkKey32;
                string activeKey = is64CPU ? activeKey64 : activeKey32;
                //判断配置文件是否正确
                if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(sdkKey) || string.IsNullOrWhiteSpace(activeKey))
                {
                    //禁用相关功能按钮
                    System.Windows.Forms.MessageBox.Show(string.Format("请在App.config配置文件中先配置APPID、SDKKEY{0}、ACTIVEKEY{0}!", is64CPU ? "64" : "32"));
                    System.Environment.Exit(0);
                }
                //SDK激活
                int retCode = idCardVeriEngine.ArcSoft_FIC_OnlineActivation(appId, sdkKey, activeKey);
                if (retCode != 0 && retCode != 90114)
                {
                    System.Windows.Forms.MessageBox.Show("SDK激活失败,接口返回码:" + retCode);
                    Environment.Exit(0);
                }
                //初始化引擎
                retCode = idCardVeriEngine.ArcSoft_FIC_InitialEngine(EngineMask.AFIC_LIVENESS | EngineMask.AFIC_LIVENESS_IR | EngineMask.AFIC_IMAGEQUALITY);
                if (retCode != 0)
                {
                    System.Windows.Forms.MessageBox.Show("引擎初始化失败,接口返回码:" + retCode);
                    Environment.Exit(0);
                }
                //设定活体阈值
                livenessThreshold.modelThresholdRgb = 0.75f;
                livenessThreshold.modelThresholdIr = 0.5f;
                retCode = idCardVeriEngine.ArcSoft_FIC_SetLivenessParam(livenessThreshold);
                if (retCode != 0)
                {
                    System.Windows.Forms.MessageBox.Show("活体阈值设置失败,接口返回码:" + retCode);
                    Environment.Exit(0);
                }
                //设定图像质量阈值
                faceQualityThreshold.faceQualityThreshold_RGB = 0.35f;
                retCode = idCardVeriEngine.Arcsoft_FIC_SetFaceQualityThreshold(faceQualityThreshold);
                if (retCode != 0)
                {
                    System.Windows.Forms.MessageBox.Show("图像质量阈值设置失败,接口返回码:" + retCode);
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("引擎激活及初始化失败，请查看Log文件内容");
                LogUtil.LogInfo(GetType(), ex);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 视频初始化
        /// </summary>
        private void videoInit()
        {
            try
            {
                filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if(filterInfoCollection.Count <= 0)
                {
                    System.Windows.Forms.MessageBox.Show("无可用摄像头，请检查摄像头是否连接正常");
                    Environment.Exit(0);
                }
                if (irVideoIndex == rgbVideoIndex || filterInfoCollection.Count < 2)
                {
                    cSourcePlayerRgb = new VideoCaptureDevice(filterInfoCollection[rgbVideoIndex].MonikerString);
                    sourcePlayerRgb.VideoSource = cSourcePlayerRgb;
                    sourcePlayerRgb.Start();
                    sourcePlayerIrControl.Visibility = Visibility.Hidden;
                }
                else
                {
                    cSourcePlayerRgb = new VideoCaptureDevice(filterInfoCollection[rgbVideoIndex].MonikerString);
                    sourcePlayerRgb.VideoSource = cSourcePlayerRgb;
                    cSourcePlayerIr = new VideoCaptureDevice(filterInfoCollection[irVideoIndex].MonikerString);
                    sourcePlayerIr.VideoSource = cSourcePlayerIr;
                    sourcePlayerRgb.Start();
                    sourcePlayerIr.Start();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("摄像头连接失败，请查看Log文件内容");
                LogUtil.LogInfo(GetType(), ex);
                Environment.Exit(0);
            }
        }
        #endregion

        #region 模拟读卡器

        /// <summary>
        /// 读取身份证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btReadCard_Click(object sender, RoutedEventArgs e)
        {
            readIdCardInfo();
        }

        /// <summary>
        /// 模拟读取身份证
        /// </summary>
        private void readIdCardInfo()
        {
            //检测是否有身份证要读取，Demo采用获取图片的形式，模拟读卡过程，请自行对接身份证读卡器
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
            {
                try
                {
                    lock (readCardLocker)
                    {
                        string filePath = string.Empty;
                        //判断执行文件夹下是否有图片，如果有，使用此照片作为注册照，否则使用资源文件作为注册照
                        Bitmap bmp = getStartPathImage(ref filePath);
                        //模拟拿到身份证信息，模拟照片在资源文件里
                        ResourceManager rmManager = ArcSoftIdCardDemo.Properties.Resources.ResourceManager;
                        using (var tempIdImage = (bmp == null) ? (Bitmap)rmManager.GetObject("DemoIdImage") : bmp)
                        {
                            info.bitmap = tempIdImage;
                            info.PeopleName = "李华";
                            info.PeopleNation = "汉";
                            info.PeopleSex = "男";
                            info.PeopleAddress = "北京市朝阳区";
                            info.PeopleBirthday = "2020-01-01";
                            info.StartDate = "2020-01-01";
                            info.EndDate = "2025-01-01";
                            info.PeopleIDCode = "110105202001019538";
                            info.Department = "北京市朝阳区公安局";
                            info.isExistImage = info.bitmap != null;
                            if (info.isExistImage)
                            {
                                //提取证件照特征
                                int retCode = idCardVeriEngine.ArcSoft_FIC_IdCardDataFeatureExtraction(info.bitmap);
                                if (retCode != 0)
                                {
                                    LogUtil.LogInfo(GetType(), "证件照特征提取失败");
                                }
                                else
                                {
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        //判断是否显示身份证信息
                                        infoWrite(true, filePath);
                                        info.bitmap.Dispose();
                                    }));
                                }
                            }
                            else
                            {
                                LogUtil.LogInfo(GetType(), "证件照获取失败");
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.LogInfo(GetType(), ex);
                }
            }));
        }

        /// <summary>
        /// 获取运行目录下的图片文件，以第一张图片作为身份照
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Bitmap getStartPathImage(ref string filePath)
        {
            Bitmap bmp = null;
            try
            {
                string[] files = Directory.GetFiles(System.Windows.Forms.Application.StartupPath, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.ToLower().EndsWith(".bmp") || s.ToLower().EndsWith(".jpg") || s.ToLower().EndsWith(".png")).ToArray();
                if (files != null && files.Length > 0)
                {
                    filePath = files[0];
                    using (System.Drawing.Image tempBmp = CommonUtil.ReadFromFile(filePath))
                    {
                        bmp = (Bitmap)tempBmp.Clone();
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogInfo(GetType(), ex);
            }
            return bmp;
        }
        #endregion

        #region 视频处理：人脸检测、特征提取、RGB/IR活体检测、人证比对
        /// <summary>
        /// 绘制事件
        /// </summary>
        private void sourcePlayerRgbPaint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            //显示判断结果
            if (!sourcePlayerRgb.IsRunning || !isCrvChecked)
            {
                dealFailure();
            }
            try
            {
                Bitmap bitmapRgb = sourcePlayerRgb.GetCurrentVideoFrame();
                Bitmap bitmapIr = sourcePlayerIr.IsRunning ? sourcePlayerIr.GetCurrentVideoFrame() : null;
                if (bitmapRgb == null)
                {
                    return;
                }
                //检测人脸特征
                int retCode = idCardVeriEngine.ArcSoft_FIC_FaceDataFeatureExtraction(bitmapRgb, ref faceInfo);
                if (retCode != 0 || faceInfo.nFace.Equals(0))
                {
                    result = 0;
                    return;
                }
                //绘制人脸框及比对结果
                Graphics g = e.Graphics;
                float offsetX = sourcePlayerRgb.Width * 1f / bitmapRgb.Width;
                float offsetY = sourcePlayerRgb.Height * 1f / bitmapRgb.Height;
                MRECT rect = faceInfo.rcFace;
                float x = rect.left * offsetX;
                float width = rect.right * offsetX - x;
                float y = rect.top * offsetY;
                float height = rect.bottom * offsetY - y;
                bool isCertifySuccess = result.Equals(1) && liveness;
                using (Pen pen = new Pen(isCertifySuccess ? Color.Green : Color.Red))
                {
                    g.DrawRectangle(pen, x, y, width, height);
                    //将上一帧检测结果显示到页面上
                    g.DrawString(string.Format("相似度:{0}{1}", similarValue, livennessResultStr), videoFont, isCertifySuccess ? greenBrush : redBrush, x, y - 15);
                }
                //显示判断结果
                messageText(isCertifySuccess);
                //异步处理防止卡顿，一次只检测一帧
                if (paintLocked)
                {
                    return;
                }
                paintLocked = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                {
                    try
                    {
                        if (bitmapRgb != null && info.isExistImage)
                        {
                            liveness = CommonUtil.IsLiveness(idCardVeriEngine, faceInfo, bitmapRgb, bitmapIr, ref livennessResultStr);
                        }
                        //获取人证对比结果
                        idCardVeriEngine.ArcSoft_FIC_FaceIdCardCompare(threshold, ref similarValue, ref result);
                        paintLocked = false;
                    }
                    catch (Exception exx)
                    {
                        LogUtil.LogInfo(GetType(), exx);
                    }
                }));
            }
            catch (Exception ex)
            {
                LogUtil.LogInfo(GetType(), ex);
            }
        }

        /// <summary>
        /// 处理失败状态
        /// </summary>
        private void dealFailure()
        {
            try
            {
                messageText(false);
                infoWrite(false);
                liveness = false;
                result = 0;
                similarValue = 0.0f;
            }
            catch (Exception ex)
            {
                LogUtil.LogInfo(GetType(), ex);
            }
        }
        #endregion

        #region 导出激活信息
        /// <summary>
        /// 导出激活信息点击事件
        /// </summary>
        private void activeInfoClick(object sender, RoutedEventArgs e)
        {
            #region 打开保存文件选择框
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "ActiveInfo";
            dialog.Filter = "文本文件|*.txt";
            DialogResult diaResult = dialog.ShowDialog();
            if (diaResult.Equals(System.Windows.Forms.DialogResult.Cancel))
            {
                return;
            }
            string activeFilePath = dialog.FileName.Trim();
            #endregion
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
            {
                //获取激活信息
                idCardVeriEngine.ArcSoft_FIC_GetActiveFileInfo(out activeInfo);
                //截止时间戳转换
                long unixTimeStamp = long.Parse(activeInfo.endTime);
                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                DateTime dt = startTime.AddSeconds(unixTimeStamp);
                activeInfo.endTime = dt.ToString("yyyy年MM月dd日 HH:mm:ss");
                //构建输出激活信息内容
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("文件激活信息({0}){1}", DateTime.Now.ToString(), Environment.NewLine);
                builder.AppendFormat("截止时间:{0}\r\n", activeInfo.endTime);
                builder.AppendFormat("激活码:{0}\r\n", activeInfo.activeKey);
                builder.AppendFormat("平台:{0}\r\n", activeInfo.platform);
                builder.AppendFormat("SDK类型:{0}\r\n", activeInfo.sdkType);
                builder.AppendFormat("APPID:{0}\r\n", activeInfo.appId);
                builder.AppendFormat("SDKKEY:{0}\r\n", activeInfo.sdkKey);
                builder.AppendFormat("SDK版本号:{0}\r\n", activeInfo.sdkVersion);
                builder.AppendFormat("激活文件版本号:{0}\r\n", activeInfo.fileVersion);
                builder.Append(idCardVeriEngine.ArcSoft_FIC_GetVersion());
                //输出文件
                File.WriteAllText(activeFilePath, builder.ToString(), Encoding.UTF8);
            }));
        }
        #endregion

        #region 摄像头开关、阈值修改、窗体关闭事件
        /// <summary>
        /// 摄像头开关事件
        /// </summary>
        private void checkBoxChecked(object sender, RoutedEventArgs e)
        {
            if (!(bool)checkBox.IsChecked && sourcePlayerRgb.IsRunning)
            {
                sourcePlayerRgb.SignalToStop();
                if (cSourcePlayerIr != null)
                {
                    sourcePlayerIr.SignalToStop();
                }
            }
            else if ((bool)checkBox.IsChecked)
            {
                sourcePlayerRgb.Start();
                if (cSourcePlayerIr != null)
                {
                    sourcePlayerIr.Start();
                }
                readIdCardInfo();
            }
        }

        /// <summary>
        /// 阈值修改事件
        /// </summary>
        private void rgbThresholdTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                tbThreshold.SelectionStart = tbThreshold.Text.Length;
                if (!string.IsNullOrEmpty(tbThreshold.Text))
                {
                    float formTextValue = float.Parse(tbThreshold.Text);
                    if (formTextValue > 1.0 || formTextValue < 0.0)
                    {
                        tbThreshold.Text = this.textValue;
                    }
                    else
                    {
                        threshold = float.Parse(tbThreshold.Text);
                    }
                }
            }
            catch
            {
                tbThreshold.Text = textValue;
                tbThreshold.SelectionStart = tbThreshold.Text.Length;
            }
        }

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        private void windowClosed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        #endregion

        #region 相关显示方法
        /// <summary>
        /// 检测文本框显示事件
        /// </summary>
        /// <param name="isSuccessed">显示识别成功/失败的结果</param>
        private void messageText(bool isSuccessed)
        {
            lbCheckFalse.Visibility = isSuccessed ? Visibility.Hidden : Visibility.Visible;
            lbCheckTrue.Visibility = isSuccessed ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// 显示身份证信息事件
        /// </summary>
        /// <param name="isSeccessed">读卡成功</param>
        /// <param name="filePath">文件路径，默认空串</param>
        private void infoWrite(bool isSeccessed, string filePath = "")
        {
            try
            {
                lbID.Content = isSeccessed ? IdAddStar(info.PeopleIDCode) : string.Empty;
                lbName.Content = isSeccessed ? info.PeopleName : string.Empty;
                lbAdress.Content = isSeccessed ? info.PeopleAddress : string.Empty;
                lbDate.Content = isSeccessed ? info.StartDate + "-" + info.EndDate : string.Empty;
                lbDepart.Content = isSeccessed ? info.Department : string.Empty;
                lbNation.Content = isSeccessed ? info.PeopleNation : string.Empty;
                lbSex.Content = isSeccessed ? info.PeopleSex : string.Empty;
                lbBirth.Content = isSeccessed ? info.PeopleBirthday : string.Empty;
                isCrvChecked = isSeccessed;
                if (isSeccessed)
                {
                    if (image1.Source != null)
                    {
                        image1.Source = null;
                    }
                    image1.Source = string.IsNullOrWhiteSpace(filePath) ? CommonUtil.BitmapToBitmapImage(info.bitmap) : CommonUtil.BitmapToBitmapImage(filePath);
                }
                else
                {
                    image1.Source = null;
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogInfo(GetType(), ex);
            }
        }

        /// <summary>
        /// 身份证号星号处理
        /// </summary>
        /// <param name="id">身份证号</param>
        public string IdAddStar(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return id;
            }
            string midString = id.Substring(3, 11);
            id = id.Replace(midString, "*******");
            return id;
        }
        #endregion
    }
}