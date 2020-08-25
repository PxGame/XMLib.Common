/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/4/3 17:19:34
 */

using System;

namespace XMLib
{
    /// <summary>
    /// TypeUtility
    /// </summary>
    public static class TypeUtility
    {
        public static object CreateInstance(Type type)
        {
            return type == typeof(string) ? string.Empty : Activator.CreateInstance(type);
        }

        public static T ConvertTo<T>(this object from, T defaultValue)
        {
            return (T)ConvertTo(from, typeof(T), defaultValue);
        }

        public static object ConvertTo(this object from, Type to, object defaultValue)
        {
            try
            {
                if (!ConvertToChecker(from.GetType(), to))
                {
                    return defaultValue;
                }
                return ConvertTo(from, to);
            }
            catch
            {
                return defaultValue;
            }
        }


        public static object ConvertTo(this object from, Type to)
        {
            if (to.IsEnum)
            {
                return Enum.ToObject(to, from);
            }

            return Convert.ChangeType(from, to);
        }

        public static bool ConvertToChecker(this Type from, Type to)
        {
            if (from == null || to == null)
            {
                return false;
            }

            // 总是可以隐式类型转换为 Object。
            if (to == typeof(object))
            {
                return true;
            }

            if (to.IsAssignableFrom(from))
            {
                return true;
            }

            if (to.IsEnum)
            {
                switch (Type.GetTypeCode(from))
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                        return true;
                }
            }

            if (PrimitiveConvert(from, to))
            {
                return true;
            }

            return false;
        }

        static private bool PrimitiveConvert(Type from, Type to)
        {
            if (!(from.IsPrimitive && to.IsPrimitive))
            {
                return false;
            }

            TypeCode typeCodeFrom = Type.GetTypeCode(from);
            TypeCode typeCodeTo = Type.GetTypeCode(to);

            if (typeCodeFrom == typeCodeTo)
                return true;

            if (typeCodeFrom == TypeCode.Char)
                switch (typeCodeTo)
                {
                    case TypeCode.UInt16: return true;
                    case TypeCode.UInt32: return true;
                    case TypeCode.Int32: return true;
                    case TypeCode.UInt64: return true;
                    case TypeCode.Int64: return true;
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    default: return false;
                }

            // Possible conversions from Byte follow.
            if (typeCodeFrom == TypeCode.Byte)
                switch (typeCodeTo)
                {
                    case TypeCode.Char: return true;
                    case TypeCode.UInt16: return true;
                    case TypeCode.Int16: return true;
                    case TypeCode.UInt32: return true;
                    case TypeCode.Int32: return true;
                    case TypeCode.UInt64: return true;
                    case TypeCode.Int64: return true;
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            // Possible conversions from SByte follow.
            if (typeCodeFrom == TypeCode.SByte)
                switch (typeCodeTo)
                {
                    case TypeCode.Int16: return true;
                    case TypeCode.Int32: return true;
                    case TypeCode.Int64: return true;
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            // Possible conversions from UInt16 follow.
            if (typeCodeFrom == TypeCode.UInt16)
                switch (typeCodeTo)
                {
                    case TypeCode.UInt32: return true;
                    case TypeCode.Int32: return true;
                    case TypeCode.UInt64: return true;
                    case TypeCode.Int64: return true;
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            // Possible conversions from Int16 follow.
            if (typeCodeFrom == TypeCode.Int16)
                switch (typeCodeTo)
                {
                    case TypeCode.Int32: return true;
                    case TypeCode.Int64: return true;
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            // Possible conversions from UInt32 follow.
            if (typeCodeFrom == TypeCode.UInt32)
                switch (typeCodeTo)
                {
                    case TypeCode.UInt64: return true;
                    case TypeCode.Int64: return true;
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            // Possible conversions from Int32 follow.
            if (typeCodeFrom == TypeCode.Int32)
                switch (typeCodeTo)
                {
                    case TypeCode.Int64: return true;
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            // Possible conversions from UInt64 follow.
            if (typeCodeFrom == TypeCode.UInt64)
                switch (typeCodeTo)
                {
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            // Possible conversions from Int64 follow.
            if (typeCodeFrom == TypeCode.Int64)
                switch (typeCodeTo)
                {
                    case TypeCode.Single: return true;
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }

            // Possible conversions from Single follow.
            if (typeCodeFrom == TypeCode.Single)
                switch (typeCodeTo)
                {
                    case TypeCode.Double: return true;
                    case TypeCode.Boolean: return true;
                    default: return false;
                }
            return false;
        }
    }
}