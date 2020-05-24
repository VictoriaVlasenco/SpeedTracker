using SpeedCheck.DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpeedCheck.DAL.Repositories
{
    public static class SynchronizedFileCache<T>// : ISynchronizedFileCache<T>
    {
        private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private static List<byte> innerCache = new List<byte>();

        public static int Count
        { get { return innerCache.Count; } }

        //public string Read(int key)
        //{
        //    cacheLock.EnterReadLock();
        //    try
        //    {
        //        return innerCache[key];
        //    }
        //    finally
        //    {
        //        cacheLock.ExitReadLock();
        //    }
        //}

        public static void Add(T entity)
        {
            cacheLock.EnterWriteLock();
            try
            {
                int size = Marshal.SizeOf(default(T));//Get size of struct data
                byte[] bytes = new byte[size];//declare byte array and initialize its size
                IntPtr ptr = Marshal.AllocHGlobal(size);//pointer to byte array
                try
                {
                    Marshal.StructureToPtr(entity, ptr, true);
                    Marshal.Copy(ptr, bytes, 0, size);
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
                innerCache.AddRange(bytes);
                //Flush();
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public static void Flush(string filePath)
        {
            cacheLock.EnterWriteLock();
            try
            {
                using (FileStream sourceStream = new FileStream(filePath,
                            FileMode.Append, FileAccess.Write, FileShare.None,
                            bufferSize: 128, useAsync: true))
                {
                    var bytes = innerCache.ToArray();
                    sourceStream.Write(bytes, 0, bytes.Length);
                }
            }
            finally
            {
                innerCache = new List<byte>();
                cacheLock.ExitWriteLock();
            }
        }

        //~SynchronizedFileCache()
        //{
        //    if (cacheLock != null)
        //    {
        //        cacheLock.Dispose();
        //    }
        //}
    }
}
