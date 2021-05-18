/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2021/5/9 15:10:13
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace XMLib
{
    public static class TypeManager
    {
        private static HashSet<Type> _types;
        private static Dictionary<int, Type> _id2Type;
        private static Dictionary<Type, int> _type2id;
        private static Dictionary<int, int> _id2size;

        static TypeManager()
        {
            Init();
        }

        private static void Init()
        {
            SuperLog.Log("TypeManager 初始化");

            _types = new HashSet<Type>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var assemblyTypes = assembly.GetTypes();

                foreach (var type in assemblyTypes)
                {
                    if (!typeof(IByteBufferData).IsAssignableFrom(type) || !UnsafeUtility.IsUnmanaged(type))
                        continue;

                    _types.Add(type);
                }
            }

            _id2Type = new Dictionary<int, Type>(_types.Count);
            _type2id = new Dictionary<Type, int>(_types.Count);
            _id2size = new Dictionary<int, int>(_types.Count);

            foreach (var type in _types)
            {
                int id = Hash128.Compute(type.FullName).GetHashCode();
                Checker.Assert(id != 0);
                _id2Type.Add(id, type);
                _type2id.Add(type, id);
                _id2size.Add(id, UnsafeUtility.SizeOf(type));
            }
        }

        public static Type Get(int id) => _id2Type.TryGetValue(id, out Type result) ? result : null;

        public static int Get(Type type) => _type2id.TryGetValue(type, out int result) ? result : 0;

        public static int Get<T>() => Get(typeof(T));

        public static int SizeOf(int typeId) => _id2size.TryGetValue(typeId, out int result) ? result : -1;
    }
}