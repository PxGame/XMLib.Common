/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2020/11/4 17:30:13
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace XMLib
{
    /// <summary>
    /// LogController
    /// </summary>
    public class SuperLog
    {
        public static string title { get; set; } = "XMLib";

        private static string titleStr => (string.IsNullOrEmpty(title) ? "" : $"[{title}]");

        public static void Log(string msg)
        {
            Debug.Log(titleStr + msg);
        }

        public static void LogError(string msg)
        {
            Debug.LogError(titleStr + msg);
        }

        public static void LogWarning(string msg)
        {
            Debug.LogWarning(titleStr + msg);
        }
    }

    public class SuperLogHandler
    {
        private readonly Func<string> title;
        private readonly string titleColor;
        private readonly SuperLogHandler parent;

        public static SuperLogHandler Create(Func<string> title, Color titleColor, SuperLogHandler parent = null)
        {
            return new SuperLogHandler(title, titleColor, parent);
        }

        public static SuperLogHandler Create(string title, Color titleColor, SuperLogHandler parent = null)
        {
            return new SuperLogHandler(() => title, titleColor, parent);
        }

        private SuperLogHandler(Func<string> title, Color titleColor, SuperLogHandler parent = null)
        {
            this.title = title;
            this.titleColor = ColorUtility.ToHtmlStringRGB(titleColor);
            this.parent = parent;

            //检查是否循环依赖
            while (parent != null)
            {
                if (parent == this)
                {//如果发生循环依赖，则移除父节点
                    this.parent = null;
                    SuperLog.LogError("SuperLogHandler 发现循环依赖");
                    break;
                }
                parent = parent.parent;
            }
        }

        public SuperLogHandler CreateSub(string title, Color titleColor)
        {
            return Create(title, titleColor, this);
        }

        public SuperLogHandler CreateSub(Func<string> title, Color titleColor)
        {
            return Create(title, titleColor, this);
        }

        protected string GetTitleStr()
        {
            string titleStr = null;
            try
            {
                titleStr = (parent?.GetTitleStr() ?? string.Empty) + $"[<color=#{titleColor}>{title()}</color>]";
            }
            catch (Exception ex)
            {
                titleStr = "<color=red>[Title Exception]</color>";
                SuperLog.LogWarning($"SuperLogHandler.GetTitleStr Exception !!!\n{ex}");
            }
            return titleStr;
        }

        private string CreateMsg(string msg, params object[] args)
        {
            string titleStr = GetTitleStr();
            string formatStr = $"{titleStr}{msg}";
            return string.Format(formatStr, args);
        }

        public void Log(string msg, params object[] args)
        {
            SuperLog.Log(CreateMsg(msg, args));
        }

        public void LogError(string msg, params object[] args)
        {
            SuperLog.LogError(CreateMsg(msg, args));
        }

        public void LogWarning(string msg, params object[] args)
        {
            SuperLog.LogWarning(CreateMsg(msg, args));
        }
    }
}