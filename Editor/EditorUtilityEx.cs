/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/12 14:36:09
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace XMLib
{
    /// <summary>
    /// EditorUtility
    /// </summary>
    public static class EditorUtilityEx
    {
        /// <summary>
        /// 获取选择的文件夹
        /// </summary>
        /// <returns></returns>
        public static string GetSelectDirectory()
        {
            string[] strs = Selection.assetGUIDs;
            if (strs.Length == 0)
            {
                return "Assets";// string.Empty;
            }

            string resourceDirectory = AssetDatabase.GUIDToAssetPath(strs[0]);

            if (string.IsNullOrEmpty(resourceDirectory) || !Directory.Exists(resourceDirectory))
            {
                return "Assets";// string.Empty;
            }

            return resourceDirectory;
        }

        public static void OpenFolder(string folderPath)
        {
            if (!Path.IsPathRooted(folderPath))
            {
                folderPath = Path.Combine(UnityEngine.Application.dataPath, "..", folderPath).Replace("\\", "/");
            }
            string path = $"file://{folderPath}";
            UnityEngine.Application.OpenURL(path);
        }

        public static string ValidFilePath(string filePath)
        {
            int index = 0;
            string target = filePath;
            do
            {
                if (!File.Exists(target))
                {
                    return target;
                }

                string ext = Path.GetExtension(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string dir = Path.GetDirectoryName(filePath).Replace('\\', '/');

                index++;
                target = $"{dir}/{fileName}_{index}{ext}";
            }
            while (true);
        }
    }
}