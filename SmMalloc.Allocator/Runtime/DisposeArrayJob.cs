namespace SmMalloc.Allocator.Runtime
{
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Jobs;
    
    internal struct DisposeArrayJob<T> : IJob where T : unmanaged
    {
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<T> Array;

        public void Execute()
        {
            CollectionHelper.Dispose(Array);
        }
    }
}