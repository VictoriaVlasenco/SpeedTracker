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
    public class FileRepository : IRepository<TrackingData>
    {
        private const string FilePath = ".\\test.data";

        public FileRepository(ISynchronizedFileCache<TrackingData> synchronizedFileCache)
        {
            this.SynchronizedFileCache = synchronizedFileCache;
        }

        public ISynchronizedFileCache<TrackingData> SynchronizedFileCache { get; }

        public void Insert(TrackingData entity)
        {
            string filePath = FilePath;
            SynchronizedFileCache.Add(entity);
            if (SynchronizedFileCache.Count > 128)
            {
                SynchronizedFileCache.Flush(filePath);
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

            using (FileStream fsSource = new FileStream(FilePath,
                    FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var offset = page * pageSize * itemSize;
                //var size = fsSource.Length < pageSize * itemSize ? (int)fsSource.Length : pageSize * itemSize;
                //var bytesToRead = offset + size < fsSource.Length ? size : fsSource.Length - offset + size;
                var bytesToRead = offset + pageSize * itemSize < fsSource.Length ? pageSize : fsSource.Length - offset;

                if (bytesToRead < 1)
                {
                    totalCount = 0;
                    return new List<TrackingData>();
                }
                byte[] a = new byte[bytesToRead];
                //fsSource.Read(a, offset - 1, (int)bytesToRead);
                fsSource.Seek(offset, SeekOrigin.Begin);
                fsSource.Read(a, 0, (int)bytesToRead);

                totalCount = a.Length;

                //IntPtr ptr = Marshal.AllocHGlobal(32);
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

                    //bytes = a;
                    //Span<byte> bytes = a;
                    //bytes.Slice(0, 32);
                    //var pinnedRawData = GCHandle.Alloc(bytes, GCHandleType.Pinned);

                    //var pointer = Unsafe.AsPointer(ref a);
                    //var span = new Span<byte>(pointer, Marshal.SizeOf(default(TrackingData)));
                    // Get the address of the data array
                    //var pinnedRawDataPtr = pinnedRawData.AddrOfPinnedObject();


                    //var d = bytes.GetPinnableReference();
                    //unsafe
                    //{
                    //    fixed (byte* p = bytes)
                    //    {
                    //        var re = (TrackingData)Marshal.PtrToStructure((IntPtr)p, typeof(TrackingData));

                    //    }
                    //    //var r = MemoryMarshal.GetReference(bytes);
                    //    var res = MemoryMarshal.Cast<byte, TrackingData>(bytes);
                    //    //var ad = (TrackingData)bytes;
                    //    //var f = (byte*)bytes;
                    //    var ds = (IntPtr)Unsafe.AsPointer(ref bytes.GetPinnableReference());
                    //    var s = (TrackingData)Marshal.PtrToStructure((IntPtr)ds, typeof(TrackingData));

                    //};
                    // overlay the data type on top of the raw data
                }
                finally
                {

                    //Marshal.FreeHGlobal(ptr);

                    // must explicitly release
                    //pinnedRawData.Free();
                }

                return res;
                //var result = default(TrackingData);
                //unsafe
                //{

                //    var pointer = Unsafe.AsPointer(ref result);
                //    var span = new Span<byte>(pointer, Marshal.SizeOf(default(TrackingData)));
                //    fsSource.Read(span);
                //    var s = result;
                //}

                //var result = default(TrackingData);
                //var tSpan = MemoryMarshal.CreateSpan(ref result, 2);
                //var span = MemoryMarshal.AsBytes(tSpan);
                //fsSource.Read(span);
                //var t = result;
                //var a = result;
            }
        }

        //public void SelectPage(int page = 1, int pageSize = 128)//, out int totalCount)
        //{
        //    using (FileStream fsSource = new FileStream(FilePath,
        //    FileMode.Open, FileAccess.Read))
        //    {

        //        // Read the source file into a byte array.
        //        byte[] bytes = new byte[128 * 211];
        //        //byte[] bytes = new byte[fsSource.Length];
        //        int numBytesToRead = 128 * 211;// (int)fsSource.Length;
        //        int numBytesRead = 0;

        //        int n = 0;
        //        while (true)
        //        {
        //            // Read may return anything from 0 to numBytesToRead.
        //            n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

        //            // Break when the end of the file is reached.
        //            if (n == 0)
        //                break;

        //            numBytesRead += n;
        //            numBytesToRead -= n;
        //        }

        //        for (int i = 0; i < n / 211; i++)
        //        {
        //            using (MemoryStream ms = new MemoryStream(bytes))
        //            {
        //                IFormatter br = new BinaryFormatter();
        //                var a = (TrackingData)br.Deserialize(ms);//numBytesToRead/32
        //            }
        //        }

        //        //while (numBytesToRead > 0)
        //        //{
        //        //    // Read may return anything from 0 to numBytesToRead.
        //        //    int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

        //        //    // Break when the end of the file is reached.
        //        //    if (n == 0)
        //        //        break;

        //        //    numBytesRead += n;
        //        //    numBytesToRead -= n;
        //        //}
        //        numBytesToRead = bytes.Length;

        //        using (MemoryStream ms = new MemoryStream(bytes))
        //        {
        //            IFormatter br = new BinaryFormatter();
        //            var a = (TrackingData)br.Deserialize(ms);//numBytesToRead/32
        //        }

        //        // Write the byte array to the other FileStream.
        //        //using (FileStream fsNew = new FileStream(pathNew,
        //        //    FileMode.Create, FileAccess.Write))
        //        //{
        //        //    fsNew.Write(bytes, 0, numBytesToRead);
        //        //}
        //    }
        //    //using (MemoryStream ms = new MemoryStream(param))
        //    //{
        //    //    IFormatter br = new BinaryFormatter();
        //    //    return (T)br.Deserialize(ms);
        //    //}
        //    //using (var file = File.OpenRead(this.FilePath))
        //    //{
        //    //    var reader = new BinaryFormatter();
        //    //    var data = (TrackingData)reader.Deserialize(file); // Reads the entire list.
        //    //}
        //}
    }
}
