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

        internal bool isValid => bbId != 0 && id != 0;

        internal ByteBuffer GetBuffer() => ByteBuffer.Get(bbId);

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
            Checker.Assert(bbId != 0 && id != 0);
            this.bbId = bbId;
            this.id = id;
        }

        public BBData<T> ToBBData<T>() where T : unmanaged, IByteBufferData => new BBData<T>(bbId, id);

        public BBData<T> ToBBDataCache<T>() where T : unmanaged, IByteBufferData => new BBDataCache<T>(bbId, id);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BBData<T> where T : unmanaged, IByteBufferData
    {
        private BBData _bBData;

        internal int bbId => _bBData.bbId;
        internal int id => _bBData.id;

        public ref T data => ref *GetDataPtr();

        internal ByteBuffer GetBuffer() => ByteBuffer.Get(bbId);

        internal BBDataHeader* GetHeaderPtr() => _bBData.GetHeaderPtr();

        internal T* GetDataPtr() => (T*)_bBData.GetDataPtr();

        public BBData(int bbId, int id)
        {
            _bBData = new BBData(bbId, id);
        }

        public static implicit operator BBData(BBData<T> data)
        {
            return data._bBData;
        }

        public static implicit operator BBDataCache<T>(BBData<T> data)
        {
            return new BBDataCache<T>(data.bbId, data.id);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BBDataCache<T> where T : unmanaged, IByteBufferData
    {
        private BBData<T> _bBData;
        private ByteBuffer _buffer;
        private int _version;
        private BBDataHeader* _headerPtrCache;
        private T* _dataPtrCache;

        public bool isValid => _bBData.id != 0 && _buffer != null && _buffer.isValid;

        internal int bbId => _bBData.bbId;
        internal int id => _bBData.id;

        public ref T data => ref *dataPtr;
        public int typeId => headerPtr->typeId;
        public int size => headerPtr->size;

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
            Checker.Assert(buffer != null && buffer.isValid && headerPtr != null && dataPtr != null);

            _bBData = new BBData<T>(buffer.id, headerPtr->id);

            _buffer = buffer;
            _version = buffer.version;
            _headerPtrCache = headerPtr;
            _dataPtrCache = dataPtr;
        }

        public BBDataCache(ByteBuffer buffer, int id)
        {
            Checker.Assert(buffer != null && id != 0);

            _bBData = new BBData<T>(buffer.id, id);

            _buffer = buffer;
            _version = buffer.version;
            _headerPtrCache = buffer.FindHeaderPtrWithID(id);
            _dataPtrCache = buffer.GetData<T>(_headerPtrCache);
        }

        public BBDataCache(int bbId, int id) : this(ByteBuffer.Get(bbId), id)
        {
        }

        private void CheckVersion()
        {
            if (_buffer == null)
            {
                _buffer = ByteBuffer.Get(_bBData.bbId);
            }
            else if (_version == _buffer.version)
            {
                return;
            }

            Checker.Assert(isValid);
            //版本发生变化，重新缓存指针
            _headerPtrCache = _buffer.FindHeaderPtrWithID(_bBData.id);
            _dataPtrCache = _buffer.GetData<T>(_headerPtrCache);
            _version = _buffer.version;
        }

        public override string ToString()
        {
            return $"{*headerPtr}:{*dataPtr}";
        }

        public static implicit operator BBData(BBDataCache<T> data)
        {
            return data._bBData;
        }

        public static implicit operator BBData<T>(BBDataCache<T> data)
        {
            return data._bBData;
        }
    }
}