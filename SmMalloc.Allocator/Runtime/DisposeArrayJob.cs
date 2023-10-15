namespace SmMalloc.Allocator.Runtime
{
    using Unity.Collections;
    using Unity.Jobs;

    internal struct DisposeArrayJob<T> : IJob where T : unmanaged
    {
        public NativeArray<T> Array;
        
        public void Execute()
        {
            CollectionHelper.Dispose(Array);
        }
    }
}