/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/9/21 23:20:32
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace XMLib
{
    [Serializable]
    public struct PFloat
    {
        public float value;

        public static implicit operator float(PFloat v) => v.value;

        public static implicit operator PFloat(float v) => new PFloat() { value = v };
    }

    [Serializable]
    public struct PInt
    {
        public int value;

        public static implicit operator int(PInt v) => v.value;

        public static implicit operator PInt(int v) => new PInt() { value = v };
    }

    [Serializable]
    public struct PString
    {
        public string value;

        public static implicit operator string(PString v) => v.value;

        public static implicit operator PString(string v) => new PString() { value = v };
    }

    [Serializable]
    public struct PBool
    {
        public bool value;

        public static implicit operator bool(PBool v) => v.value;

        public static implicit operator PBool(bool v) => new PBool() { value = v };
    }

    [Serializable]
    public struct PVector2
    {
        public float x;
        public float y;

        public static implicit operator Vector2(PVector2 v) => new Vector2(v.x, v.y);

        public static implicit operator PVector2(Vector2 v) => new PVector2() { x = v.x, y = v.y };
    }

    [Serializable]
    public struct PVector3
    {
        public float x;
        public float y;
        public float z;

        public static implicit operator Vector3(PVector3 v) => new Vector3(v.x, v.y, v.z);

        public static implicit operator PVector3(Vector3 v) => new PVector3() { x = v.x, y = v.y, z = v.z };
    }
}