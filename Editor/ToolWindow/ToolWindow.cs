/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2021/6/24 22:10:57
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using Newtonsoft.Json;

namespace XMLib
{
    /// <summary>
    /// ToolWindow
    /// </summary>
    public abstract class ToolWindow : EditorWindow
    {
        public interface ISetting
        {
            public int selectPage { get; set; }
        }

        private int lastSelect;
        private string _settingTag => this.GetType().FullName;
        private readonly List<ToolPage> _views = new List<ToolPage>();
        public bool isInited = false;
        public ISetting setting { get; protected set; }

        private JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            MissingMemberHandling = MissingMemberHandling.Ignore,
        };

        /// <summary>
        /// 在此初始化 setting 与 page
        /// </summary>
        protected abstract void Init();

        public void AppendPagesFromBaseType<T>() where T : ToolPage
        {
            var types = TypeCache.GetTypesDerivedFrom<T>();

            foreach (var type in types)
            {
                if (type.IsAbstract) { continue; }

                var page = Activator.CreateInstance(type) as T;
                page.main = this;
                _views.Add(page);
            }
        }

        protected virtual void Awake()
        {
            Init();
        }

        private void SaveSetting()
        {
            if (setting == null) { return; }
            string json = JsonConvert.SerializeObject(setting, jsonSettings);
            EditorPrefs.SetString(_settingTag, json);
        }

        private void LoadSetting()
        {
            if (!EditorPrefs.HasKey(_settingTag))
            {
                SaveSetting();
                return;
            }

            string json = EditorPrefs.GetString(_settingTag);
            setting = JsonConvert.DeserializeObject(json, setting.GetType()) as ISetting;
        }

        protected virtual void OnEnable()
        {
            lastSelect = -1;
            LoadSetting();
            isInited = false;
        }

        protected virtual void OnDisable()
        {
            SaveSetting();
        }

        protected virtual void OnGUI()
        {
            if (_views.Count <= 0)
            {
                return;
            }

            if (!isInited)
            {
                foreach (var view in _views)
                {
                    view.OnInit();
                }
                isInited = true;
            }

            string[] titles = _views.Select(t => t.title).ToArray();

            if (setting.selectPage >= titles.Length || setting.selectPage < 0)
            {//有效性检查
                lastSelect = -1;
                setting.selectPage = 0;
            }

            using (var lay = new EditorGUILayout.VerticalScope())
            {
                setting.selectPage = GUILayout.SelectionGrid(setting.selectPage, titles, titles.Length, GUI.skin.FindStyle("ToolBarButton"));
                ToolPage view = _views.Find(t => t.title == titles[setting.selectPage]);
                if (lastSelect != setting.selectPage)
                {
                    if (lastSelect >= 0)
                    {
                        ToolPage lastView = _views.Find(t => t.title == titles[lastSelect]);
                        lastView.OnDisable();
                    }
                    view.OnEnable();
                }

                view.OnGUI();
            }

            lastSelect = setting.selectPage;
        }
    }
}