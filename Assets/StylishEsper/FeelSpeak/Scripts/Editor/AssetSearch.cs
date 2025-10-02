//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Esper.FeelSpeak.Editor
{
    public static class AssetSearch
    {
        public static string FindFolder(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                return string.Empty;
            }

            string[] folderGuids = AssetDatabase.FindAssets($"t:folder {folderName}");

            foreach (string guid in folderGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string folderOnly = Path.GetFileName(path);

                if (folderOnly == folderName)
                {
                    return path;
                }
            }

            return null;
        }

        public static T Find<T>(string startFolder, string path) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(Path.Combine(FindFolder(startFolder), path));
        }

        public static T Find<T>(string path) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
    }
}
#endif