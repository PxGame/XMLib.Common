/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2021/7/12 23:15:47
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// MultiTypeObjectPool
    /// </summary>
    public class MultiTypeObjectPool<T> where T : class
    {
        private Dictionary<Type, Stack<T>> _objPool = new Dictionary<Type, Stack<T>>();

        public Action<T> onPush;
        public Action<T> onPop;
        public Action<T> onNew;

        public D Pop<D>() where D : T, new()
        {
            if (!_objPool.TryGetValue(typeof(D).GetType(), out Stack<T> datas))
            {
                datas = new Stack<T>();
                _objPool[typeof(D)] = datas;
            }

            D result;
            if (datas.Count > 0)
            {
                result = (D)datas.Pop();
            }
            else
            {
                result = new D();
                onNew?.Invoke(result);
            }

            onPop?.Invoke(result);
            return result;
        }

        public void Push<D>(D data) where D : T
        {
            if (!_objPool.TryGetValue(typeof(D).GetType(), out Stack<T> datas))
            {
                datas = new Stack<T>();
                _objPool[typeof(T)] = datas;
            }

            onPush?.Invoke(data);
            datas.Push(data);
        }
    }
}