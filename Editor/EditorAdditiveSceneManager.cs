// Copyright 2023 - 2024 Yewon Jung

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityWorkflowTools;

namespace UnityWorkflowTools.Editor
{
    [InitializeOnLoad]
    public class EditorAdditiveSceneManager
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorSceneManager.sceneOpened += OnEditorSceneOpened;
        }

        private static void OnEditorSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (scene != SceneManager.GetActiveScene()) return;

            AdditiveSceneManager sceneManager = UnityEngine.Object.FindAnyObjectByType<AdditiveSceneManager>();
            sceneManager?.LoadSceneList();
        }
    }
}