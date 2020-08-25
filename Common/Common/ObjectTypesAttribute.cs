/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/4/2 22:25:02
 */

using System;
using System.Diagnostics;

namespace XMLib
{
    /// <summary>
    /// ObjectTypesAttribute
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ObjectTypesAttribute : Attribute
    {
        public Type[] types { get; protected set; }

        public ObjectTypesAttribute(params Type[] types)
        {
            this.types = types;
        }
    }
}