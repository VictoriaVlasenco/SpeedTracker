namespace SpeedCheck.DAL.Infrastructure
{
    public interface ISynchronizedFileCache<T>
    {
        int Count { get; }

        void Add(T entity);
        void Flush(string filePath);
    }
}