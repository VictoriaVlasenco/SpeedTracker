using Microsoft.Extensions.Configuration;
using SpeedCheck.DAL.Models;
using SpeedCheck.DAL.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeedCheck.DAL.Repositories
{
    public class FileRepository : IRepository<TrackingData>, IDisposable
    {
        private readonly IConfiguration configuration;

        public FileRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Insert(TrackingData entity)
        {
            string filePath = this.GetFilePath(entity.CheckTime);

            SynchronizedFileCache<TrackingData>.Add(entity, filePath);
            if (SynchronizedFileCache<TrackingData>.BufferCount > 128)
            {
                SynchronizedFileCache<TrackingData>.Flush();
            }
        }

        public IEnumerable<TrackingData> SelectPage(DateTime date, int page, int pageSize, out int totalCount)
        {
            page = page == default(int) ? 1 : page;
            page = page - 1;
            pageSize = pageSize == default(int) ? 128 : pageSize;

            var itemSize = Marshal.SizeOf(default(TrackingData));

            var res = new List<TrackingData>();
            var offset = page * pageSize * itemSize;

            string filePath = this.GetFilePath(date);
            var streamBytes = SynchronizedFileCache<TrackingData>.Read(filePath, offset, pageSize * itemSize).ToArray();
            totalCount = streamBytes.Length;
            if (streamBytes.Length < 1)
            {
                return res;
            }
            try
            {
                Span<byte> bytes;
                for (int i = 0; i < streamBytes.Length / itemSize; i++)
                {
                    unsafe
                    {
                        bytes = new Span<byte>(streamBytes, i * itemSize, itemSize);
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
            }

            return res;
        }

        public long Count(DateTime date)
        {
            string filePath = GetFilePath(date);

            return SynchronizedFileCache<TrackingData>.Count(filePath) / Marshal.SizeOf(default(TrackingData));
        }

        public void Dispose()
        {
            SynchronizedFileCache<TrackingData>.Flush();
        }

        private string GetFilePath(DateTime date)
        {
            return string.Format(configuration["FileNameFormat"], date.Day, date.Month, date.Year);
        }
    }
}
