## About
This is [smmalloc](https://github.com/SergeyMakeev/smmalloc) allocator version for Unity Engine with support of AllocatorManager.IAllocator and Unity.Collections.

smmalloc is a fast and efficient "proxy" allocator designed to handle many small allocations/deallocations in heavy multithreaded scenarios.
The allocator created for using in applications where the performance is critical such as video games.
Designed to speed up the typical memory allocation pattern in C++ such as many small allocations and deletions.
This is a proxy allocator means that smmalloc handles only specific allocation sizes and pass-through all other allocations to generic heap allocator.

## Usage
#### Create a new allocator instance
```c#
// 8 buckets, 4 BM each
SmAllocatorUtility.Create(8, 4 * 1024 * 1024, out var allocator);
```

#### Destroy the allocator instance and free allocated memory
```c#
SmAllocatorUtility.Destroy(ref allocator);
```

#### Allocate memory block
```c#
// buffer with 100 int elements (400 bytes memory block)
var buffer = SmAllocatorUtility.Malloc<int>(100, UnsafeUtility.SizeOf<int>(), UnsafeUtility.AlignOf<int>(),
                allocator.Handle);
```
P.S.: Don't forget unsafe word!

#### Deallocate memory block
```c#
SmAllocatorUtility.Free(100, buffer, allocator.Handle);
```
P.S.: Don't forget unsafe word!

#### Allocate NativeArray<T> from Unity.Collections package
```c#
// NativeArray<T> with int 100 elements
var array = SmAllocatorUtility.AllocateArray<int>(100, ref allocator);
```

#### Deallocate NativeArray<T> from Unity.Collections package
```c#
SmAllocatorUtility.DisposeArray(ref array);
```

#### Allocate another collection from Unity.Collections package
For example NativeList<T>:
```c#
// allocate
var nativeList = new NativeList<int>(allocator.Handle);
// deallocate
nativeList.Dispose();
```
Or NativeHashSet<T>:
```c#
// allocate with 10 initial capacity
var nativeHashSet = new NativeHashSet<int>(10, allocator.Handle);
// deallocate
nativeHashSet.Dispose();
```

#### Create thread cache for a current thread
```c#
// create cache config with description (2048 bytes) for each allocator block
var cacheConfig = Enumerable.Repeat(2048u, 8).ToArray();
// create thread cache
allocator.CreateThreadCache(cacheConfig, CacheWarmupOptions.CacheHot);
```

#### Destroy thread cache for a current thread
```c#
allocator.DestroyThreadCache();
```
