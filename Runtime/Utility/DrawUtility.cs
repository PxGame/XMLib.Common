/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/6/12 15:37:23
 */

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// DrawUtility
    /// </summary>
    public abstract class DrawUtility
    {
        #region implement

        public readonly static Color defualtColor = Color.white;
        public readonly static GizmosDrawer G = new GizmosDrawer();
        public readonly static DebugDrawer D = new DebugDrawer();

        #endregion implement

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public abstract void DrawLine(Vector3 start, Vector3 end);

        public virtual Color color { get; set; }
        public int subdivide = 30;
        public bool fillColor;
        public bool fillColorWithOutline = true;
        public Color outlineColor => new Color(1, 1, 1, color.a);

        protected Stack<Color> _colorStack = new Stack<Color>();

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void PushColor() => _colorStack.Push(color);

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void PushAndSetColor(Color color)
        {
            _colorStack.Push(this.color);
            this.color = color;
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void PopColor()
        {
            this.color = _colorStack.Count > 0 ? _colorStack.Pop() : defualtColor;
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void ClearColor()
        {
            _colorStack.Clear();
            color = defualtColor;
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawPolygon(Vector3[] vertices)
        {
            if (fillColor)
            {
                DrawPolygonFall(vertices);

                if (fillColorWithOutline)
                {
                    PushAndSetColor(outlineColor);
                    for (int i = vertices.Length - 1, j = 0; j < vertices.Length; i = j, j++)
                    {
                        DrawLine(vertices[i], vertices[j]);
                    }
                    PopColor();
                }
            }
            else
            {
                for (int i = vertices.Length - 1, j = 0; j < vertices.Length; i = j, j++)
                {
                    DrawLine(vertices[i], vertices[j]);
                }
            }
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public virtual void DrawPolygonFall(Vector3[] vertices)
        {
            for (int i = vertices.Length - 1, j = 0; j < vertices.Length; i = j, j++)
            {
                DrawLine(vertices[i], vertices[j]);
            }
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawBox(Vector3 size, Matrix4x4 matrix)
        {
            Vector3[] points = MathUtility.CalcBoxVertex(size, matrix);
            int[] indexs = MathUtility.GetBoxSurfaceIndex();
            for (int i = 0; i < 6; i++)
            {
                Vector3[] polygon = new Vector3[] {
                    points[indexs[i * 4]],
                    points[indexs[i * 4 + 1]],
                    points[indexs[i * 4 + 2]],
                    points[indexs[i * 4 + 3]] };
                DrawPolygon(polygon);
            }
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public virtual void DrawSphere(float radius, Matrix4x4 matrix)
        {
            Matrix4x4 lookMatrix = Matrix4x4.identity;

#if UNITY_EDITOR
            UnityEditor.SceneView sceneView = UnityEditor.SceneView.currentDrawingSceneView;
            if (sceneView != null)
            {
                Camera cam = sceneView.camera;
                var cameraTrans = cam.transform;
                var rotation = Quaternion.LookRotation(cameraTrans.position - matrix.MultiplyPoint(Vector3.zero));
                lookMatrix = Matrix4x4.TRS(matrix.MultiplyPoint(Vector3.zero), rotation, matrix.lossyScale);
                DrawCircle(radius, lookMatrix);
            }
#endif

            //绘制边界
            bool oldFillColor = fillColor;
            fillColor = false;
            PushAndSetColor(outlineColor);
            DrawCircle(radius, matrix);
            DrawCircle(radius, matrix * Matrix4x4.Rotate(Quaternion.Euler(0, 90, 0)));
            DrawCircle(radius, matrix * Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0)));
            PopColor();
            fillColor = oldFillColor;
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCross(float size, Matrix4x4 matrix)
        {
            float halfSize = size / 2f;

            Vector3[] points = new Vector3[6];
            points[0] = matrix.MultiplyPoint(Vector3.up * halfSize);
            points[1] = matrix.MultiplyPoint(Vector3.down * halfSize);
            points[2] = matrix.MultiplyPoint(Vector3.left * halfSize);
            points[3] = matrix.MultiplyPoint(Vector3.right * halfSize);
            points[4] = matrix.MultiplyPoint(Vector3.forward * halfSize);
            points[5] = matrix.MultiplyPoint(Vector3.back * halfSize);

            DrawLine(points[0], points[1]);
            DrawLine(points[2], points[3]);
            DrawLine(points[4], points[5]);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawRect(Vector2 size, Matrix4x4 matrix)
        {
            Vector3[] vertices = MathUtility.CalcRectVertex(size, matrix);
            DrawPolygon(vertices);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCircle(float radius, Matrix4x4 matrix)
        {
            Vector3[] vertices = MathUtility.CalcCircleVertex(radius, matrix, subdivide);
            DrawPolygon(vertices);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawArc(float radius, float angle, float rotation, Matrix4x4 matrix)
        {
            Vector3[] vertices = MathUtility.CalcArcVertex(radius, angle, rotation, matrix, true, subdivide);
            DrawPolygon(vertices);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCapsule2D(float height, float radius, Matrix4x4 matrix)
        {
            Vector3[] vertices = MathUtility.CalcCapsuleVertex2D(height, radius, matrix, subdivide);
            DrawPolygon(vertices);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCapsule(float height, float radius, Matrix4x4 matrix)
        {
            float diameter = 2.0f * radius;
            height = height < diameter ? diameter : height;

            Vector3[] vertices = MathUtility.CalcCapsuleVertex2D(height, radius, matrix, subdivide);
            Vector3[] vertices2 = MathUtility.CalcCapsuleVertex2D(height, radius, matrix * Matrix4x4.Rotate(Quaternion.Euler(0.0f, 90.0f, 0.0f)), subdivide);
            DrawPolygon(vertices);
            DrawPolygon(vertices2);

            float offset = (height - diameter) / 2.0f;
            Vector3 highPos = new Vector3(0.0f, offset, 0.0f);
            Vector3 lowPos = new Vector3(0.0f, -offset, 0.0f);
            Quaternion rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            DrawCircle(radius, matrix * Matrix4x4.TRS(highPos, rotation, Vector3.one));
            DrawCircle(radius, matrix * Matrix4x4.TRS(lowPos, rotation, Vector3.one));
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCapsule(Vector3 lowPos, Vector3 highPos, float radius)
        {
            Vector3 dir = highPos - lowPos;
            float height = dir.magnitude + radius * 2f;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, dir.normalized);
            Vector3 pos = (highPos + lowPos) / 2.0f;
            Matrix4x4 matrix = Matrix4x4.TRS(pos, rotation, Vector3.one);
            DrawCapsule(height, radius, matrix);
        }

        #region Extersion

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCapture(Vector3 point1, Vector3 point2, float radius)
        {
            Vector3 pos = (point1 + point2) / 2.0f;
            Vector3 dir = point2 - point1;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, dir.normalized);
            float height = dir.magnitude + 2.0f * radius;
            DrawCapsule(height, radius, Matrix4x4.TRS(pos, rotation, Vector3.one));
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCapsule(Vector3 lowPos, Vector3 highPos, float radius, Color color)
        {
            PushAndSetColor(color);
            DrawCapsule(lowPos, highPos, radius);
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCapsule(float height, float radius, Matrix4x4 matrix, Color color)
        {
            PushAndSetColor(color);
            DrawCapsule(height, radius, matrix);
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            PushAndSetColor(color);
            DrawLine(start, end);
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawPolygon(Vector3[] vertices, Color color)
        {
            PushAndSetColor(color);
            DrawPolygon(vertices);
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawPolygon(Vector3[] vertices, Matrix4x4 matrix, Color color)
        {
            PushAndSetColor(color);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = matrix.MultiplyPoint(vertices[i]);
            }
            DrawPolygon(vertices);
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawBox(Vector3 position, Vector3 size, Color color)
        {
            PushAndSetColor(color);
            DrawBox(size, Matrix4x4.Translate(position));
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawSphere(Vector3 position, float radius, Color color)
        {
            PushAndSetColor(color);
            DrawSphere(radius, Matrix4x4.Translate(position));
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCross(Vector3 position, float size, Color color)
        {
            PushAndSetColor(color);
            DrawCross(size, Matrix4x4.Translate(position));
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawArc(Vector3 position, float radius, float angle, float rotation)
        {
            Vector3[] vertices = MathUtility.CalcArcVertex(radius, angle, rotation, Matrix4x4.Translate(position), true, subdivide);
            DrawPolygon(vertices);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawArc(Vector3 position, float radius, float angle, float rotation, Color color)
        {
            PushAndSetColor(color);
            DrawArc(position, radius, angle, rotation);
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawRect(Vector3 position, Vector2 size, Vector3 angle, Vector3 scale)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.Euler(angle), scale);
            DrawRect(size, matrix);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawRect(Vector3 position, Vector2 size, Vector3 angle, Vector3 scale, Color color)
        {
            PushAndSetColor(color);
            DrawRect(position, size, angle, scale);
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawRect(Vector3 position, Vector2 size)
        {
            DrawRect(position, size, Vector3.zero, Vector3.one);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawRect(Vector3 position, Vector2 size, Color color)
        {
            PushAndSetColor(color);
            DrawRect(position, size);
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawRect(Rect rect, Color color)
        {
            DrawRect(rect.center, rect.size, color);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCircle(Vector3 position, float radius)
        {
            DrawCircle(radius, Matrix4x4.Translate(position));
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCircle(Vector3 position, float radius, Color color)
        {
            PushAndSetColor(color);
            DrawCircle(position, radius);
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCameraFrustumCorners(Camera camera, Rect viewRect, Vector3 target, Color color)
        {
            //float distance = Vector3.Distance(camera.transform.position, target);
            Transform transform = camera.transform;
            Vector3[] vts = new Vector3[4];
            camera.CalculateFrustumCorners(viewRect, Vector3.Distance(transform.position, target), Camera.MonoOrStereoscopicEye.Mono, vts);
            DrawPolygon(vts, transform.localToWorldMatrix, Color.red);
            //DrawCross(transform.position + transform.forward * distance, 0.5f, color);
        }

        #endregion Extersion
    }

    /// <summary>
    /// GizmosUtility
    /// </summary>
    public class GizmosDrawer : DrawUtility
    {
        public override Color color { get => Gizmos.color; set => Gizmos.color = value; }

        [DebuggerStepThrough]
        public override void DrawLine(Vector3 start, Vector3 end)
        {
            Gizmos.DrawLine(start, end);
        }

        [DebuggerStepThrough]
        public override void DrawPolygonFall(Vector3[] vertices)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.DrawAAConvexPolygon(vertices);
#endif
        }
    }

    /// <summary>
    /// DebugUtility
    /// </summary>
    public class DebugDrawer : DrawUtility
    {
        /// <summary>
        /// 持续时间
        /// </summary>
        public float duration { get; set; } = 0.033f;

        public override Color color { get; set; } = DrawUtility.defualtColor;

        protected Stack<float> _durationStack = new Stack<float>();

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void PushAndSetDuration(float duration)
        {
            _durationStack.Push(this.duration);
            this.duration = duration;
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void PopDuration()
        {
            this.duration = _durationStack.Count > 0 ? _durationStack.Pop() : this.duration;
        }

        [DebuggerStepThrough]
        public override void DrawLine(Vector3 start, Vector3 end)
        {
            UnityEngine.Debug.DrawLine(start, end, color, duration);
        }
    }

#if UNITY_EDITOR

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
        public override void DrawPolygonFall(Vector3[] vertices)
        {
            UnityEditor.Handles.DrawAAConvexPolygon(vertices);
        }
    }

#endif
}