namespace SmMalloc.Allocator.Runtime
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using size_t = System.UIntPtr;

    [SuppressUnmanagedCodeSecurity]
    internal static class NativeAllocator
    {
        private const string LibraryName = "smmalloc";

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr _sm_allocator_create(uint bucketsCount, size_t bucketSizeInBytes);
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void _sm_allocator_destroy(IntPtr allocator);
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void _sm_allocator_thread_cache_create(IntPtr allocator, CacheWarmupOptions warmupOptions, IntPtr cacheSize);
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void _sm_allocator_thread_cache_destroy(IntPtr allocator);
        
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr _sm_malloc(IntPtr allocator, size_t bytesCount, size_t alignment);
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void _sm_free(IntPtr allocator, IntPtr memory);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr _sm_realloc(IntPtr allocator, IntPtr memory, size_t bytesCount, size_t alignment);
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern size_t _sm_msize(IntPtr allocator, IntPtr memory);
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int _sm_mbucket(IntPtr allocator, IntPtr memory);
    }
}