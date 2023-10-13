namespace SmMalloc.Allocator.Runtime
{
    using System;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    [GenerateTestsForBurstCompatibility]
    public unsafe struct SmAllocatorHelper : IDisposable
    {
        private SmUnityAllocator* m_Allocator;
        private AllocatorManager.AllocatorHandle m_BackingAllocator;

        public ref SmUnityAllocator Allocator => ref UnsafeUtility.AsRef<SmUnityAllocator>(m_Allocator);

        public SmAllocatorHelper(uint bucketsCount, long bucketSize, AllocatorManager.AllocatorHandle backingAllocator)
        {
            m_BackingAllocator = backingAllocator;
            m_Allocator = (SmUnityAllocator*) UnsafeUtility.MallocTracked(UnsafeUtility.SizeOf<SmUnityAllocator>(),
                UnsafeUtility.AlignOf<SmUnityAllocator>(), backingAllocator.ToAllocator, 0);
            
            var allocator = new SmAllocator(bucketsCount, bucketSize);
            *m_Allocator = new SmUnityAllocator(ref allocator);
            (*m_Allocator).Register();
        }
        
        [ExcludeFromBurstCompatTesting("DestroyAllocator is unburstable")]
        public void Dispose()
        {
            ref var allocator = ref UnsafeUtility.AsRef<SmUnityAllocator>(m_Allocator);
            
            allocator.Unregister();
            allocator.Dispose();
            
            UnsafeUtility.Free(m_Allocator, m_BackingAllocator.ToAllocator);
            m_Allocator = null;
        }
    }
}