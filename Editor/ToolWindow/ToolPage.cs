/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2021/6/24 22:11:26
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// ToolPage
    /// </summary>
    public abstract class ToolPage
    {
        public ToolWindow main { get; set; }

        public Vector2 scrollPosition { get; set; }

        public abstract string title { get; }

        public abstract void OnGUI();

        public abstract void OnInit();

        public abstract void OnEnable();

        public abstract void OnDisable();
    }
}