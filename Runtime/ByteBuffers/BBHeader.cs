/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2021/5/9 15:23:24
 */

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// BBHeader
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BBHeader
    {
        public int id;
        public int capacity;
        public int version;
        public int count;
        public int nextId;
        public int frontUsedSize;
        public int backUsedSize;

        public override string ToString()
        {
            return $"BBHeader:capacity {capacity},version {version},count {count},nextId {nextId},frontUsedSize {frontUsedSize},backUsedSize {backUsedSize}";
        }
    }
}