using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditor.Extension.SceneViewFovControl {

static class SceneViewHiddenApi {
    static bool initialized = false;
    static FieldInfo fi;

    static FieldInfo onPreSceneGUIDelegateFieldInfo {
        get {
            if(fi != null || initialized) {
                return fi;
            }

            initialized = true;

            // UnityEditor.SceneView.onPreSceneGUIDelegateFieldInfo
            fi = typeof(SceneView).GetField(
                "onPreSceneGUIDelegate",
                  BindingFlags.Static
                | BindingFlags.NonPublic
                | BindingFlags.Public
            );
            return fi;
        }
    }

    static SceneView.OnSceneFunc onPreSceneGUIDelegate {
        get {
            var fi = onPreSceneGUIDelegateFieldInfo;
            if(fi == null) {
                return null;
            }
            return fi.GetValue(null) as SceneView.OnSceneFunc;
        }

        set {
            var fi = onPreSceneGUIDelegateFieldInfo;
            if(fi == null) {
                return;
            }
            fi.SetValue(null, value);
        }
    }

    // Add delegate to UnityEditor.SceneView.onPreSceneGUIDelegate
    public static void AddOnPreSceneGUIDelegate(SceneView.OnSceneFunc func) {
        onPreSceneGUIDelegate =
            Delegate.Combine(func, onPreSceneGUIDelegate)
                as SceneView.OnSceneFunc;
    }

    // Remove delegate from UnityEditor.SceneView.onPreSceneGUIDelegate
    public static void RemoveOnPreSceneGUIDelegate(SceneView.OnSceneFunc func) {
        onPreSceneGUIDelegate =
            Delegate.Remove(func, onPreSceneGUIDelegate)
                as SceneView.OnSceneFunc;
    }
}

} // namespace
