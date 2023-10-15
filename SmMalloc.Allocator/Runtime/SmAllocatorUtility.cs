namespace SmMalloc.Allocator.Runtime
{
    using System.Runtime.CompilerServices;
    using Unity.Collections;
    using Unity.Jobs;

    public static class SmAllocatorUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Create(uint bucketsCount, long bucketSize, out SmUnityAllocator unityAllocator)
        {
            var allocator = new SmAllocator(bucketsCount, bucketSize);
            unityAllocator = new SmUnityAllocator();
            unityAllocator.Initialize(ref allocator);
            unityAllocator.Register();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SmAllocatorHelper Create(uint bucketsCount, long bucketSize, AllocatorManager.AllocatorHandle backingAllocator)
        {
            return new SmAllocatorHelper(bucketsCount, bucketSize, backingAllocator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Destroy(ref SmUnityAllocator allocator)
        {
            allocator.Unregister();
            allocator.Dispose();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeArray<T> AllocateArray<T>(int length, ref SmUnityAllocator allocator,
            NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : unmanaged
        {
            return CollectionHelper.CreateNativeArray<T, SmUnityAllocator>(length, ref allocator, options);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeArray<T>(ref NativeArray<T> array) where T : unmanaged
        {
            CollectionHelper.Dispose(array);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JobHandle DisposeArray<T>(ref NativeArray<T> array, JobHandle dependsOn) where T : unmanaged
        {
            var disposeJob = new DisposeArrayJob<T>
            {
                Array = array
            };

            return disposeJob.ScheduleByRef(dependsOn);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T* Malloc<T>(int length, int itemSizeInBytes, int alignmentInBytes, AllocatorManager.AllocatorHandle handle) where T : unmanaged
        {
            return (T*)AllocatorManager.Allocate(handle, itemSizeInBytes, alignmentInBytes, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Free<T>(int length, T* pointer, AllocatorManager.AllocatorHandle handle) where T : unmanaged
        {
            AllocatorManager.Free(handle, pointer, length);
        }
    }
}