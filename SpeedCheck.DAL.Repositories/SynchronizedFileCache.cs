using SpeedCheck.DAL.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpeedCheck.DAL.Repositories
{
    public static class SynchronizedFileCache<T>// : ISynchronizedFileCache<T>
    {
        private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private static ConcurrentDictionary<string, List<byte>> innerCache = new ConcurrentDictionary<string, List<byte>>();// new List<byte>();

        public static int Count
        { get { return innerCache.Count; } }

        public static List<byte> Read(string filePath, int offset, int bytesToRead)
        {
            var res = new List<byte>();
            cacheLock.EnterReadLock();
            try
            {
                using (FileStream fsSource = new FileStream(filePath,
                    FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    bytesToRead = offset + bytesToRead < fsSource.Length ? bytesToRead : (int)fsSource.Length - offset;
                    if (bytesToRead < 1)
                    {
                        //totalCount = 0;
                        return new List<byte>();
                    }

                    byte[] a = new byte[bytesToRead];
                    //fsSource.Read(a, offset - 1, (int)bytesToRead);
                    fsSource.Seek(offset, SeekOrigin.Begin);
                    fsSource.Read(a, 0, bytesToRead);

                    //totalCount = a.Length;
                    res.AddRange(a);
                }
            }
            finally
            {
                cacheLock.ExitReadLock();
            }

            if (innerCache.TryGetValue(filePath, out var value))
            {
                res.AddRange(value);
                return res;
            }

            return res;
        }

        public static void Add(T entity, string fileName)
        {
            //cacheLock.EnterWriteLock();
            //try
            //{
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
            if (innerCache.TryGetValue(fileName, out var value))
            {
                value.AddRange(bytes);
            }
            else
            {
                innerCache.AddOrUpdate(fileName, new List<byte>(bytes), (k, v) => { v.AddRange(bytes); return v; }); ;
            }
            //innerCache.TryGetValue()
            //innerCache.AddRange(bytes);
            //Flush();
            //}
            //finally
            //{
            //    cacheLock.ExitWriteLock();
            //}
        }

        public static void Flush()
        {
            foreach (KeyValuePair<string, List<byte>> entry in innerCache)
            {
                cacheLock.EnterWriteLock();
                try
                {

                    using (FileStream sourceStream = new FileStream(entry.Key,
                            FileMode.Append, FileAccess.Write, FileShare.None,
                            bufferSize: 128, useAsync: true))
                    {
                        var bytes = entry.Value.ToArray();
                        sourceStream.Write(bytes, 0, bytes.Length);
                    }
                    // do something with entry.Value or entry.Key
                }
                finally
                {
                    innerCache = new ConcurrentDictionary<string, List<byte>>();
                    cacheLock.ExitWriteLock();
                }
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
