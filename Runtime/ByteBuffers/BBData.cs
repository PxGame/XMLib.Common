/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2021/5/9 15:18:00
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// BBData
    /// </summary>
    public unsafe struct BBData<T> where T : unmanaged, IByteBufferData
    {
        public bool isValid => _id >= 0 && _buffer != null && !_buffer.isDisposed;
        public int id => headerPtr->id;
        public ref T data => ref *dataPtr;
        public int typeId => headerPtr->typeId;
        public int size => headerPtr->size;

        private ByteBuffer _buffer;
        private int _version;
        private int _id;
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

        internal BBData(ByteBuffer buffer, BBDataHeader* headerPtr, T* dataPtr)
        {
            Checker.Assert(buffer != null && headerPtr != null && dataPtr != null);

            _buffer = buffer;
            _version = buffer.version;
            _id = headerPtr->id;
            _headerPtrCache = headerPtr;
            _dataPtrCache = dataPtr;
        }

        public BBData(ByteBuffer buffer, int id)
        {
            Checker.Assert(buffer != null);
            _headerPtrCache = buffer.FindHeaderPtrWithID(id);
            Checker.Assert(_headerPtrCache != null);

            _dataPtrCache = buffer.GetData<T>(_headerPtrCache);
            Checker.Assert(_dataPtrCache != null);

            _buffer = buffer;
            _version = buffer.version;
            _id = id;
        }

        private void CheckVersion()
        {
            Checker.Assert(isValid);

            if (_version == _buffer.version)
            {
                return;
            }

            //版本发生变化，重新缓存指针
            _headerPtrCache = _buffer.FindHeaderPtrWithID(_id);
            _dataPtrCache = _buffer.GetData<T>(_headerPtrCache);
            _version = _buffer.version;
        }

        public override string ToString()
        {
            return $"{*headerPtr}:{*dataPtr}";
        }
    }
}