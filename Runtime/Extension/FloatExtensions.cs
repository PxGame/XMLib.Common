/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/9/28 15:03:53
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMLib.Extensions
{
    /// <summary>
    /// FloatExtension
    /// </summary>
    public static class FloatExtensions
    {
        public static float Round(this float floatA, int decimals) => (float)Math.Round(floatA, decimals);

        public static bool IsEqualToZero(this float floatA) => Mathf.Abs(floatA) < Mathf.Epsilon;

        public static bool NotEqualToZero(this float floatA) => Mathf.Abs(floatA) > Mathf.Epsilon;

        /// <summary>
        /// Wraps a float between -180 and 180.
        /// </summary>
        /// <param name="toWrap">The float to wrap.</param>
        /// <returns>A value between -180 and 180.</returns>
        public static float Wrap180(this float toWrap)
        {
            toWrap %= 360.0f;
            if (toWrap < -180.0f)
            {
                toWrap += 360.0f;
            }
            else if (toWrap > 180.0f)
            {
                toWrap -= 360.0f;
            }
            return toWrap;
        }

        /// <summary>
        /// Wraps a float between 0 and 1.
        /// </summary>
        /// <param name="toWrap">The float to wrap.</param>
        /// <returns>A value between 0 and 1.</returns>
        public static float Wrap1(this float toWrap)
        {
            toWrap %= 1.0f;
            if (toWrap < 0.0f)
            {
                toWrap += 1.0f;
            }
            return toWrap;
        }

        /// <summary>
        /// Gets the fraction portion of a float.
        /// </summary>
        /// <param name="number">The float.</param>
        /// <returns>The fraction portion of a float.</returns>
        public static float GetFraction(this float number) => number - Mathf.Floor(number);
    }
}