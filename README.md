## About
This is [smmalloc](https://github.com/SergeyMakeev/smmalloc) allocator version for Unity Engine with support of AllocatorManager.IAllocator and Unity.Collections.

smmalloc is a fast and efficient "proxy" allocator designed to handle many small allocations/deallocations in heavy multithreaded scenarios.
The allocator created for using in applications where the performance is critical such as video games.
Designed to speed up the typical memory allocation pattern in C++ such as many small allocations and deletions.
This is a proxy allocator means that smmalloc handles only specific allocation sizes and pass-through all other allocations to generic heap allocator.
