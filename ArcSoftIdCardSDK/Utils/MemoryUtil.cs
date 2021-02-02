using System;
using System.Runtime.InteropServices;

namespace ArcSoftIdCardSDK.Utils
{
    /// <summary>
    /// 内存处理工具类
    /// </summary>
    public static class MemoryUtil
    {
        /// <summary>
        /// 申请内存
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IntPtr Malloc(int length)
        {
            return Marshal.AllocHGlobal(length);
        }

        /// <summary>
        /// 获取数据结构类型大小
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int SizeOf<T>()
        {
            return Marshal.SizeOf<T>();
        }

        /// <summary>
        /// 释放非托管内存
        /// </summary>
        /// <param name="ptr"></param>
        public static void Free(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }

        /// <summary>
        /// 非托管转为结构体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptr"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T PtrToStruct<T>(IntPtr ptr)
        {
            return Marshal.PtrToStructure<T>(ptr);
        }

        /// <summary>
        /// 结构体复制到非托管内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="ptr"></param>
        public static void StructToPtr<T>(T t, IntPtr ptr)
        {
            Marshal.StructureToPtr<T>(t, ptr, false);
        }

        /// <summary>
        /// 字节数组拷贝到非托管内存中
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="destination"></param>
        /// <param name="length"></param>
        public static void Copy(byte[] source, int startIndex, IntPtr destination, int length)
        {
            Marshal.Copy(source, startIndex, destination, length);
        }

        /// <summary>
        /// 托管内存拷贝到字节数组中
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="destination"></param>
        /// <param name="length"></param>
        public static void Copy(IntPtr source, int startIndex, byte[] destination, int length)
        {
            Marshal.Copy(source, destination, startIndex, length);
        }
    }
}
