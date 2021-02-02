using System.Drawing;

namespace ArcSoftIdCardDemo.Utils
{
    /// <summary>
    /// 身份证信息
    /// </summary>
    public class IdCardText
    {
        /// <summary>
        /// 姓名信息
        /// </summary>
        public string PeopleName { get; set; }

        /// <summary>
        /// 性别信息
        /// </summary>
        public string PeopleSex { get; set; }

        /// <summary>
        /// 民族信息 
        /// </summary>
        public string PeopleNation { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public string PeopleBirthday { get; set; }

        /// <summary>
        /// 地址信息
        /// </summary>
        public string PeopleAddress { get; set; }

        /// <summary>
        /// 卡号信息
        /// </summary>
        public string PeopleIDCode { get; set; }

        /// <summary>
        /// 发证机关信息
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// 有效开始日期
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// 有效截止日期
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 位图
        /// </summary>
        public Bitmap bitmap { set; get; }

        /// <summary>
        /// 是否存在图片的标识
        /// </summary>
        public bool isExistImage { get; set; }

    }
}