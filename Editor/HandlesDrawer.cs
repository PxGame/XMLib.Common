/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/7/21 21:00:30
 */

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// HandlesDrawer
    /// </summary>
    public class HandlesDrawer : DrawUtility
    {
        public readonly static HandlesDrawer H = new HandlesDrawer();

        public override Color color { get => UnityEditor.Handles.color; set => UnityEditor.Handles.color = value; }

        [DebuggerStepThrough]
        public override void DrawLine(Vector3 start, Vector3 end)
        {
            UnityEditor.Handles.DrawLine(start, end);
        }

        [DebuggerStepThrough]
        public override void DrawPolygonFill(Vector3[] vertices)
        {
            UnityEditor.Handles.DrawAAConvexPolygon(vertices);
        }
    }
}