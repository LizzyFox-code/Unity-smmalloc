namespace SmMalloc.Allocator.Runtime
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using size_t = System.UIntPtr;
    
    [StructLayout(LayoutKind.Sequential)]
    public struct SmAllocator : IDisposable
    {
        public const int MaxBucketCount = 61;
        
        private readonly uint m_BucketsCount;

        private IntPtr m_NativePtr;

        public bool IsCreated => m_NativePtr != IntPtr.Zero;

        public SmAllocator(uint bucketsCount, long bucketSize) : this()
        {
            if(bucketsCount == 0 || bucketsCount > MaxBucketCount)
                throw new ArgumentOutOfRangeException($"{nameof(bucketsCount)} should be greater than zero and less or equal than {MaxBucketCount}");
            
            m_BucketsCount = bucketsCount;
            m_NativePtr = NativeAllocator._sm_allocator_create(bucketsCount, (size_t)bucketSize);
            
            if(m_NativePtr == IntPtr.Zero)
                throw new InvalidOperationException("Native memory allocator not created.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntPtr Malloc(long bytesCount, int alignment = 0)
        {
            if(bytesCount <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(bytesCount)} should be greater than zero.");

            return NativeAllocator._sm_malloc(m_NativePtr, (size_t)bytesCount, (size_t)alignment);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free(IntPtr memoryPtr)
        {
            if(memoryPtr == IntPtr.Zero)
                throw new ArgumentNullException($"{nameof(memoryPtr)} is already released.");
            
            NativeAllocator._sm_free(m_NativePtr, memoryPtr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntPtr Realloc(IntPtr memory, long bytesCount, int alignment = 0)
        {
            if(memory == IntPtr.Zero)
                throw new ArgumentNullException($"{nameof(memory)} is already released.");
            
            if(bytesCount <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(bytesCount)} should be greater than zero.");

            return NativeAllocator._sm_realloc(m_NativePtr, memory, (size_t) bytesCount, (size_t) alignment);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CreateThreadCache([NotNull]uint[] cacheSize, CacheWarmupOptions warmupOptions)
        {
            if(cacheSize.Length < m_BucketsCount)
                throw new InvalidOperationException("Thread cache configuration is invalid.");
        
            var gcHandle = GCHandle.Alloc(cacheSize, GCHandleType.Pinned);
            NativeAllocator._sm_allocator_thread_cache_create(m_NativePtr, warmupOptions, gcHandle.AddrOfPinnedObject());
            gcHandle.Free();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyThreadCache()
        {
            NativeAllocator._sm_allocator_thread_cache_destroy(m_NativePtr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long SizeOf(IntPtr memory)
        {
            if(memory == IntPtr.Zero)
                throw new ArgumentNullException($"{nameof(memory)} is already released.");

            return (long)NativeAllocator._sm_msize(m_NativePtr, memory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BucketOf(IntPtr memory)
        {
            if(memory == IntPtr.Zero)
                throw new ArgumentNullException($"{nameof(memory)} is already released.");
            
            return NativeAllocator._sm_mbucket(m_NativePtr, memory);
        }
        
        public void Dispose()
        {
            if(m_NativePtr == IntPtr.Zero)
                return;
        
            NativeAllocator._sm_allocator_destroy(m_NativePtr);
            m_NativePtr = IntPtr.Zero;
        }
    }
}