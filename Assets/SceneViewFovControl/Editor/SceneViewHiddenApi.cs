using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditorExtension.SceneViewFovControl {

static class SceneViewHiddenApi {
    static readonly Type typeSceneView = typeof(UnityEditor.SceneView);
    static readonly FieldInfo fi_onPreSceneGUIDelegateFieldInfo = typeSceneView.GetField(
        "onPreSceneGUIDelegate"
        , BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public
    );

    static SceneView.OnSceneFunc onPreSceneGUIDelegate {
        get {
            if(fi_onPreSceneGUIDelegateFieldInfo == null) {
                return null;
            }
            return fi_onPreSceneGUIDelegateFieldInfo.GetValue(null) as SceneView.OnSceneFunc;
        }

        set {
            if(fi_onPreSceneGUIDelegateFieldInfo == null) {
                return;
            }
            fi_onPreSceneGUIDelegateFieldInfo.SetValue(null, value);
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
