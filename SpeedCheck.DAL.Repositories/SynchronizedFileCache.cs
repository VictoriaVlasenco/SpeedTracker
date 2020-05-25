
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpeedCheck.DAL.Repositories
{
    public static class SynchronizedFileCache<T>
    {
        private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private static ConcurrentDictionary<string, List<byte>> innerCache = new ConcurrentDictionary<string, List<byte>>();// new List<byte>();

        public static int BufferCount
        { get { return innerCache.Count; } }

        public static List<byte> Read(string filePath, int offset, int bytesToRead)
        {
            var res = new List<byte>();
            cacheLock.EnterReadLock();
            try
            {
                using (FileStream fsSource = new FileStream(filePath,
                    FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
                {
                    bytesToRead = offset + bytesToRead < fsSource.Length ? bytesToRead : (int)fsSource.Length - offset;
                    if (bytesToRead < 1)
                    {
                        return new List<byte>();
                    }

                    byte[] a = new byte[bytesToRead];
                    fsSource.Seek(offset, SeekOrigin.Begin);
                    fsSource.Read(a, 0, bytesToRead);

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

        public static long Count(string filePath)
        {
            cacheLock.EnterReadLock();
            long length;
            try
            {
                using (FileStream fsSource = new FileStream(filePath,
                    FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
                {
                    length = fsSource.Length;
                }

                if (innerCache.TryGetValue(filePath, out var value))
                {
                    length += value.Count;
                }

                return length;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }


        public static void Add(T entity, string fileName)
        {

            int size = Marshal.SizeOf(default(T));
            byte[] bytes = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
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
                }
                finally
                {
                    innerCache = new ConcurrentDictionary<string, List<byte>>();
                    cacheLock.ExitWriteLock();
                }
            }
        }
    }
}
