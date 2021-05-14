/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2021/5/9 15:06:51
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// ByteBuffer
    /// </summary>
    public sealed unsafe class ByteBuffer : IDisposable
    {
        public int version => headerPtr->version;
        public int capacity => headerPtr->capacity;
        public int count => headerPtr->count;
        public int usedSize => headerPtr->frontUsedSize + headerPtr->backUsedSize;
        public int freeSize => capacity - usedSize;
        public IntPtr bufferPtr => _bufferPtr;

        /// <summary>
        /// |header|data|...|data body|
        /// </summary>
        private IntPtr _bufferPtr;

        private BBHeader* headerPtr => (BBHeader*)_bufferPtr;

        public readonly static int headerSize = UnsafeUtility.SizeOf<BBHeader>();
        public readonly static int dataHeaderSize = UnsafeUtility.SizeOf<BBDataHeader>();

        private ByteBuffer(int capacity)
        {
            Checker.Assert(capacity > headerSize);
            _bufferPtr = (IntPtr)UnsafeUtility.Malloc(capacity, 1, Allocator.Persistent);

            //初始化头
            *headerPtr = new BBHeader()
            {
                capacity = capacity,
                count = 0,
                nextId = 0,
                version = 0,
                frontUsedSize = headerSize,
                backUsedSize = 0
            };
        }

        public static ByteBuffer Create(int capacity)
        {
            return new ByteBuffer(capacity);
        }

        public BBData<T> Write<T>() where T : unmanaged, IByteBufferData
        {
            int typeId = TypeManager.Get<T>();
            Checker.Assert(typeId != 0);
            int dataSize = TypeManager.SizeOf(typeId);

            int expendSize = dataSize + headerSize;
            if (expendSize > freeSize)
            {
                int newCapacity = (expendSize > capacity ? expendSize : capacity) + capacity;
                Resize(newCapacity);
            }

            //写头，由前往后
            BBDataHeader* dataHeaderPtr = (BBDataHeader*)(_bufferPtr + headerPtr->frontUsedSize);
            dataHeaderPtr->id = headerPtr->nextId;
            dataHeaderPtr->typeId = typeId;
            dataHeaderPtr->size = dataSize;
            dataHeaderPtr->offset = headerPtr->backUsedSize;

            //写数据，由后往前
            T* dataPtr = GetData<T>(dataHeaderPtr);

            //更新参数
            headerPtr->count++;
            headerPtr->nextId++;
            headerPtr->frontUsedSize += dataHeaderSize;
            headerPtr->backUsedSize += dataSize;

            return new BBData<T>(this, dataHeaderPtr, dataPtr);
        }

        public BBData<T> Write<T>(T data) where T : unmanaged, IByteBufferData
        {
            BBData<T> item = Write<T>();
            UnsafeUtility.MemCpy(item.dataPtr, &data, item.size);
            return item;
        }

        public void RemoveUnused()
        {
            Resize(usedSize);
        }

        public void Resize(int size)
        {
            Checker.Assert(size >= capacity);
            IntPtr nextBuffer = (IntPtr)UnsafeUtility.Malloc(size, 1, Allocator.Persistent);
            UnsafeUtility.MemCpy((void*)nextBuffer, (void*)_bufferPtr, headerPtr->frontUsedSize);
            UnsafeUtility.MemCpy((void*)(nextBuffer + headerPtr->frontUsedSize), (void*)(_bufferPtr + headerPtr->capacity - headerPtr->backUsedSize), headerPtr->backUsedSize);
            UnsafeUtility.Free((void*)_bufferPtr, Allocator.Persistent);
            _bufferPtr = nextBuffer;

            //更新参数
            headerPtr->capacity = size;
            headerPtr->version++;
        }

        internal BBDataHeader* FindHeaderPtrWithID(int id)
        {
            int count = headerPtr->count;
            if (count < 0) { return null; }

            BBDataHeader* firstDataHandlerPtr = (BBDataHeader*)(_bufferPtr + headerSize);
            //TODO 可以优化为二分查找
            for (int i = 0; i < count; i++)
            {
                BBDataHeader* dataHandlerPtr = firstDataHandlerPtr + i;
                if (dataHandlerPtr->id == id)
                {
                    return dataHandlerPtr;
                }
            }

            return null;
        }

        internal T* GetData<T>(BBDataHeader* dataHeaderPtr) where T : unmanaged
        {
            return (T*)GetData(dataHeaderPtr);
        }

        internal IntPtr GetData(BBDataHeader* dataHeaderPtr)
        {
            if (dataHeaderPtr == null) { return IntPtr.Zero; }
            return (_bufferPtr + headerPtr->capacity - (dataHeaderPtr->offset + dataHeaderPtr->size - 1) - 1);
        }

        internal BBDataHeader* GetDataHeader(int index)
        {
            if (index < 0 || index >= headerPtr->count) { return null; }
            return (BBDataHeader*)(_bufferPtr + headerSize + index * dataHeaderSize);
        }

        public void ShowMessage()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ByteBuffer Message");
            sb.Append($"{ToString()}");
            for (int i = 0; i < headerPtr->count; i++)
            {
                BBDataHeader* headerPtr = GetDataHeader(i);
                IntPtr dataPtr = GetData(headerPtr);
                Type t = TypeManager.Get(headerPtr->typeId);

                object obj = Marshal.PtrToStructure(dataPtr, t);
                sb.AppendLine($"{headerPtr->ToString()}:{obj.ToString()}");
            }
            SuperLog.Log(sb.ToString());
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=========== ByteBuffer ===========");
            sb.AppendLine($"{headerPtr->ToString()}");
            sb.AppendFormat("usedSize {0},freeSize {1}\n", usedSize, freeSize);
            sb.AppendLine("==================================");
            return sb.ToString();
        }

        #region IDisposable Support

        public bool isDisposed { get; private set; } = false; // 要检测冗余调用

        private void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {//释放托管
                }

                //释放非托管
                if (_bufferPtr != IntPtr.Zero)
                {
                    UnsafeUtility.Free((void*)_bufferPtr, Allocator.Persistent);
                    _bufferPtr = IntPtr.Zero;
                }

                isDisposed = true;
            }
        }

        ~ByteBuffer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}