namespace SmMalloc.Allocator.Runtime
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using AOT;
    using Unity.Burst;
    using Unity.Collections;
    using UnityEngine.Assertions;

    [StructLayout(LayoutKind.Sequential)]
    [BurstCompile(CompileSynchronously = true)]
    public struct SmUnityAllocator : AllocatorManager.IAllocator
    {
        private SmAllocator m_SmAllocator;
        private AllocatorManager.AllocatorHandle m_Handle;
        private int m_AllocationCount;
        
        public AllocatorManager.TryFunction Function => AllocatorFunction;
        
        public AllocatorManager.AllocatorHandle Handle 
        { 
            get => m_Handle;
            set => m_Handle = value;
        }

        public Allocator ToAllocator => m_Handle.ToAllocator;
        
        public bool IsCustomAllocator => m_Handle.IsCustomAllocator;
        
        public bool IsAutoDispose => false;
        
        public int AllocationCount => m_AllocationCount;

        public SmUnityAllocator(ref SmAllocator smAllocator) : this()
        {
            m_SmAllocator = smAllocator;
        }

        public int Try(ref AllocatorManager.Block block)
        {
            if (block.Range.Pointer == IntPtr.Zero) // Allocate
            {
                block.Range.Pointer = m_SmAllocator.Malloc(block.Bytes, block.Alignment);
                block.AllocatedItems = block.Range.Items;

                if (block.Range.Pointer == IntPtr.Zero)
                    return 1;
                
                m_AllocationCount++;
                return 0;
            }

            // Deallocate
            m_SmAllocator.Free(block.Range.Pointer);
            block.Range.Pointer = IntPtr.Zero;
            m_AllocationCount--;
            
            return 0;
        }
        
        public void Dispose()
        {
            Assert.AreEqual(0, m_AllocationCount);
            
            m_Handle.Dispose();
            m_SmAllocator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CreateThreadCache([NotNull] uint[] cacheSize, CacheWarmupOptions warmupOptions)
        {
            m_SmAllocator.CreateThreadCache(cacheSize, warmupOptions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyThreadCache()
        {
            m_SmAllocator.DestroyThreadCache();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long SizeOf(IntPtr memory)
        {
            return m_SmAllocator.SizeOf(memory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BucketOf(IntPtr memory)
        {
            return m_SmAllocator.BucketOf(memory);
        }

        [BurstCompile(CompileSynchronously = true)]
        [MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
        public static unsafe int AllocatorFunction(IntPtr customAllocatorPtr, ref AllocatorManager.Block block)
        {
            return ((SmUnityAllocator*)customAllocatorPtr)->Try(ref block);
        }
    }
}