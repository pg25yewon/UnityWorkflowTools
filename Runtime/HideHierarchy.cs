// Copyright 2023 - 2024 Yewon Jung

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityWorkflowTools
{
    // if does both in edit mode and play mode 
    [ExecuteAlways]
    public class HideHierarchy : MonoBehaviour
    {
#if UNITY_EDITOR

        [SerializeField] private bool _hidden = true;

        private void OnValidate()
        {
            HideChildren(_hidden);
        }

        private void Awake()
        {
            HideChildren(_hidden);
        }

        private void HideChildren(bool hidden)
        {
            Transform[] children = GetComponentsInChildren<Transform>();

            foreach (Transform child in children)
            {
                if (child.gameObject == gameObject) continue;

                HideFlags flags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                if (!hidden) flags = HideFlags.None;
                child.gameObject.hideFlags = flags;
            }
        }
#endif
    }
}