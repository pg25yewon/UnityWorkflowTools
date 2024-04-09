// Copyright 2023 - 2024 Yewon Jung

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;

namespace UnityWorkflowTools
{
    [DefaultExecutionOrder(-10000)]
    public class AdditiveSceneManager : MonoBehaviour
    {
        [field: SerializeField, ValueDropdown("GetAllScenes")] public List<string> SceneList { get; private set; }

        private void Awake()
        {
            LoadSceneList();
        }

        [Button]
        public void LoadSceneList()
        {
            List<string> loaded = new List<string>();

            // while playing
            if (Application.isPlaying)
            {
                foreach (string scenePath in SceneList)
                {
                    string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                    if (!CheckIfSceneLoaded(sceneName))
                    {
                        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                        loaded.Add(sceneName);
                    }
                }

                return;
            }

            // while in editor
#if UNITY_EDITOR
            foreach (string scenePath in SceneList)
            {
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                if (!CheckIfSceneLoaded(sceneName))
                {
                    Scene loadedScene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                    SetExpanded(loadedScene, false);
                    loaded.Add(sceneName);
                }
            }
#endif

            Debug.Log($"AdditiveSceneManager loading...");
            foreach (string sceneName in loaded)
            {
                Debug.Log($" > {sceneName}");
            }
        }

        private bool CheckIfSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name.Equals(sceneName)) return true;
            }

            return false;
        }

#if UNITY_EDITOR
        private static IEnumerable GetAllScenes()
        {
            return UnityEditor.AssetDatabase.FindAssets("t:Scene")
                .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
                .Select(x => new ValueDropdownItem(x, x));
        }

        private string GetScenePath(string sceneName)
        {
            return UnityEditor.AssetDatabase.FindAssets("t:Scene")
                .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x)).FirstOrDefault();
        }

        private void SetExpanded(Scene scene, bool expand)
        {

            foreach (var window in Resources.FindObjectsOfTypeAll<UnityEditor.SearchableEditorWindow>())
            {
                if (window.GetType().Name != "SceneHierarchyWindow")
                    continue;

                var method = window.GetType().GetMethod("SetExpandedRecursive",
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance, null,
                    new[] { typeof(int), typeof(bool) }, null);

                if (method == null)
                {
                    Debug.LogError(
                        "Could not find method 'UnityEditor.SceneHierarchyWindow.SetExpandedRecursive(int, bool)'.");
                    return;
                }

                var field = scene.GetType().GetField("m_Handle",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (field == null)
                {
                    Debug.LogError("Could not find field 'int UnityEngine.SceneManagement.Scene.m_Handle'.");
                    return;
                }

                var sceneHandle = field.GetValue(scene);
                method.Invoke(window, new[] { sceneHandle, expand });
            }

        }
#endif
    }
}