/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/11/4 10:19:20
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace XMLib
{
    /// <summary>
    /// SelectListWindow
    /// </summary>
    public class SelectListWindow : EditorWindow
    {
        #region Extension

        public static void ShowTypeWithAttr<T>(Action<Type> callback) where T : Attribute
        {
            List<Type> types = AssemblyUtility.FindAllTypeWithAttr<T>();
            List<string> typeNames = types.Select(t => $"{t.Name} <{t.Namespace}>").ToList();
            Show(typeNames, t => callback(types[t]));
        }

        #endregion Extension

        public static void Show(List<string> lists, Action<int> callback)
        {
            var win = GetWindow<SelectListWindow>(true, "Select", true);

            win._list.AddRange(lists);
            win._onCallback = callback;

            win.ShowAuxWindow();
        }

        private List<string> _list = new List<string>();
        private Action<int> _onCallback;
        private static string _search = "";
        private Vector2 _scroll = Vector2.zero;

        private void OnGUI()
        {
            DrawTool();
            DrawList();
        }

        private void DrawTool()
        {
            GUILayout.BeginVertical("HelpBox");
            GUILayout.BeginHorizontal();
            _search = EditorGUILayout.TextField("", _search, "SearchTextField");

            if (string.IsNullOrEmpty(_search))
            {
                GUILayout.Label("", "SearchCancelButtonEmpty");
            }
            else
            {
                if (GUILayout.Button("", "SearchCancelButton"))
                {
                    GUI.FocusControl(null);
                    _search = "";
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawList()
        {
            GUILayout.BeginVertical("HelpBox");
            _scroll = GUILayout.BeginScrollView(_scroll);

            string searchLow = _search.ToLower();

            for (int i = 0; i < _list.Count; i++)
            {
                string str = _list[i];

                if (!string.IsNullOrEmpty(searchLow))
                {
                    string strLow = str.ToLower();

                    if (!strLow.Contains(searchLow))
                    {
                        continue;
                    }
                }

                if (GUILayout.Button(str, "ShurikenModuleTitle"))
                {
                    GUI.FocusControl(null);
                    Selected(i);
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void Selected(int i)
        {
            if (_onCallback != null)
            {
                _onCallback(i);
            }

            Close();
        }
    }
}