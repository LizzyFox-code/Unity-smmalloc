namespace SmMalloc.Allocator.Runtime
{
    using System;
    using System.Runtime.InteropServices;
    using Unity.Collections;

    [GenerateTestsForBurstCompatibility]
    [StructLayout(LayoutKind.Sequential)]
    public struct SmAllocatorHelper : IDisposable
    {
        private AllocatorHelper<SmUnityAllocator> m_AllocatorHelper;

        public ref SmUnityAllocator Allocator => ref m_AllocatorHelper.Allocator;

        public SmAllocatorHelper(uint bucketsCount, long bucketSize, AllocatorManager.AllocatorHandle backingAllocator)
        {
            m_AllocatorHelper = new AllocatorHelper<SmUnityAllocator>(backingAllocator);
            
            var allocator = new SmAllocator(bucketsCount, bucketSize);
            Allocator.Initialize(ref allocator);
        }
        
        [ExcludeFromBurstCompatTesting("DestroyAllocator is unburstable")]
        public void Dispose()
        {
            Allocator.Dispose();
            m_AllocatorHelper.Dispose();
        }
    }
}