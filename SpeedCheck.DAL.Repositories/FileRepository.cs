using SpeedCheck.DAL.Infrastructure;
using SpeedCheck.DAL.Models;
using SpeedCheck.DAL.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SpeedCheck.DAL.Repositories
{
    public class FileRepository : IRepository<TrackingData>, IDisposable
    {
        private const string FilePath = ".\\test.data";

        //ISynchronizedFileCache<TrackingData> synchronizedFileCache
        public FileRepository()
        {
            //this.SynchronizedFileCache = synchronizedFileCache;
        }

        public void Dispose()
        {
            SynchronizedFileCache<TrackingData>.Flush();
        }

        //public ISynchronizedFileCache<TrackingData> SynchronizedFileCache { get; }

        public void Insert(TrackingData entity)
        {
            string filePath = FilePath;
            SynchronizedFileCache<TrackingData>.Add(entity, filePath);
            if (SynchronizedFileCache<TrackingData>.Count > 128)
            {
                SynchronizedFileCache<TrackingData>.Flush();
            }
            //byte[] encodedText = Encoding.Unicode.GetBytes(text);
            //var data = new TrackingData { CheckTime = DateTime.Now, Speed = 60.5, RegistrationNumber = "1234 AE-5" };
            //var writer = new BinaryFormatter();
            ////writer.Serialize(file, data); // Writes the entire list.


            //{
            //    using (FileStream sourceStream = new FileStream(filePath,
            //        FileMode.Append, FileAccess.Write, FileShare.None,
            //        bufferSize: 4096, useAsync: true))
            //    // using()
            //    {

            //        int size = Marshal.SizeOf(default(TrackingData));//Get size of struct data
            //        byte[] bytes = new byte[size];//declare byte array and initialize its size
            //        IntPtr ptr = Marshal.AllocHGlobal(size);//pointer to byte array

            //        try
            //        {
            //            Marshal.StructureToPtr(entity, ptr, true);
            //            Marshal.Copy(ptr, bytes, 0, size);

            //            //Sending struct data  packet
            //            sourceStream.Write(bytes, 0, bytes.Length);//Modified
            //        }
            //        finally
            //        {
            //            Marshal.FreeHGlobal(ptr);
            //        }



            //        //var pointer = Unsafe.AsPointer(ref entity);
            //        //var span = new Span<byte>(pointer, Marshal.SizeOf(entity));
            //        //sourceStream.Write(span);


            //        //var tSpan = MemoryMarshal.CreateSpan(ref entity, 1);
            //        //var span = MemoryMarshal.AsBytes(tSpan);
            //        //sourceStream.Write(span);

            //        //writer.Serialize(sourceStream, entity); // Writes the entire list.

            //        //await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            //    };
            //}


            //using (var file = File.OpenWrite(filename))
            //{
            //    var writer = new BinaryFormatter();
            //    writer.Serialize(file, data); // Writes the entire list.
            //}
            //throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TrackingData> SelectPage(int page, int pageSize, out int totalCount)
        {
            page = page == default(int) ? 1 : page;
            page = page - 1;
            pageSize = pageSize == default(int) ? 128 : pageSize;

            var itemSize = Marshal.SizeOf(default(TrackingData));

            var res = new List<TrackingData>();
            var offset = page * pageSize * itemSize;

            var a = SynchronizedFileCache<TrackingData>.Read(FilePath, offset, pageSize * itemSize).ToArray();
            totalCount = a.Length;
            if (a.Length < 1)
            {
                return res;
            }
            try
            {
                //Marshal.Copy(a, 0, ptr, 32);

                Span<byte> bytes;
                //unsafe { bytes = new Span<byte>((byte*)ptr, 32); }
                for (int i = 0; i < a.Length / itemSize; i++)
                {
                    unsafe
                    {
                        bytes = new Span<byte>(a, i * itemSize, itemSize);
                        fixed (byte* p = bytes)
                        {
                            var re = (TrackingData)Marshal.PtrToStructure((IntPtr)p, typeof(TrackingData));
                            res.Add(re);
                        }
                    }
                }
            }
            finally
            {

                //Marshal.FreeHGlobal(ptr);

                // must explicitly release
                //pinnedRawData.Free();
            }

            return res;
        }
    }
}
