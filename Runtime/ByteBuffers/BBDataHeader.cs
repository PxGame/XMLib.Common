/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2021/5/9 15:23:43
 */

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// BBDataHeader
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BBDataHeader
    {
        public int id;
        public int typeId;
        public int offset;
        public int size;

        public override string ToString()
        {
            return $"id={id},typeId={typeId},offset={offset},size={size}";
        }
    }
}