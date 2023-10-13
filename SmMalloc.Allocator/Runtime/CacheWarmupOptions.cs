namespace SmMalloc.Allocator.Runtime
{
    public enum CacheWarmupOptions
    {
        /// <summary>
        /// None TLS buckets are filled from centralized storage.
        /// </summary>
        CacheCold = 0,
        /// <summary>
        /// Half TLS buckets are filled from centralized storage.
        /// </summary>
        CacheWarm = 1,
        /// <summary>
        /// All TLS buckets are filled from centralized storage.
        /// </summary>
        CacheHot = 2
    }
}