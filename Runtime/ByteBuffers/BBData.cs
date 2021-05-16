/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2021/5/9 15:18:00
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace XMLib
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BBData
    {
        internal int bbId;
        internal int id;

        internal BBDataHeader* GetHeaderPtr()
        {
            ByteBuffer bb = ByteBuffer.Get(bbId);
            if (bb == null) { return null; }
            return bb.FindHeaderPtrWithID(id);
        }

        internal IntPtr GetDataPtr()
        {
            ByteBuffer bb = ByteBuffer.Get(bbId);
            if (bb == null) { return IntPtr.Zero; }
            return bb.GetData(id);
        }

        public BBData(int bbId, int id)
        {
            this.bbId = bbId;
            this.id = id;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BBData<T> where T : unmanaged, IByteBufferData
    {
        internal BBData bBData;

        internal BBDataHeader* GetHeaderPtr() => bBData.GetHeaderPtr();

        internal T* GetDataPtr() => (T*)bBData.GetDataPtr();

        public BBData(int bbId, int id)
        {
            bBData = new BBData(bbId, id);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BBDataCache<T> where T : unmanaged, IByteBufferData
    {
        internal BBData<T> bBData;

        public bool isValid => bBData.bBData.id > 0 && _buffer != null && !_buffer.isDisposed;
        public int id => headerPtr->id;
        public ref T data => ref *dataPtr;
        public int typeId => headerPtr->typeId;
        public int size => headerPtr->size;

        private ByteBuffer _buffer;
        private int _version;
        private BBDataHeader* _headerPtrCache;
        private T* _dataPtrCache;

        internal T* dataPtr
        {
            get
            {
                CheckVersion();
                return _dataPtrCache;
            }
        }

        internal BBDataHeader* headerPtr
        {
            get
            {
                CheckVersion();
                return _headerPtrCache;
            }
        }

        internal BBDataCache(ByteBuffer buffer, BBDataHeader* headerPtr, T* dataPtr)
        {
            Checker.Assert(buffer != null && headerPtr != null && dataPtr != null);

            bBData = new BBData<T>(buffer.id, headerPtr->id);

            _buffer = buffer;
            _version = buffer.version;
            _headerPtrCache = headerPtr;
            _dataPtrCache = dataPtr;
        }

        public BBDataCache(ByteBuffer buffer, int id)
        {
            Checker.Assert(buffer != null && id > 0);
            _headerPtrCache = buffer.FindHeaderPtrWithID(id);
            Checker.Assert(_headerPtrCache != null);

            _dataPtrCache = buffer.GetData<T>(_headerPtrCache);
            Checker.Assert(_dataPtrCache != null);

            bBData = new BBData<T>(buffer.id, id);

            _buffer = buffer;
            _version = buffer.version;
        }

        private void CheckVersion()
        {
            Checker.Assert(isValid);
            if (_buffer == null || _buffer.id != bBData.bBData.bbId)
            {
                _buffer = ByteBuffer.Get(bBData.bBData.bbId);
            }
            else if (_version == _buffer.version)
            {
                return;
            }

            //版本发生变化，重新缓存指针
            _headerPtrCache = _buffer.FindHeaderPtrWithID(bBData.bBData.id);
            _dataPtrCache = _buffer.GetData<T>(_headerPtrCache);
            _version = _buffer.version;
        }

        public override string ToString()
        {
            return $"{*headerPtr}:{*dataPtr}";
        }
    }
}