using System;
using System.Reflection;
using UnityEditor;
#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditorExtension.SceneViewFovControl {
#if UNITY_2019_1_OR_NEWER
	// Finally, we have nice official API set for scene view.
#else
    internal static class SceneViewHiddenApi {
        private static readonly Type TypeSceneView = typeof(UnityEditor.SceneView);

        private static readonly FieldInfo FiOnPreSceneGUIDelegateFieldInfo =
            TypeSceneView.GetField(
                                   "onPreSceneGUIDelegate"
                                 , BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public
                                  );

        private static SceneView.OnSceneFunc OnPreSceneGUIDelegate
        {
            get
            {
                if (FiOnPreSceneGUIDelegateFieldInfo == null) {
                    return null;
                }
                return FiOnPreSceneGUIDelegateFieldInfo.GetValue(null) as SceneView.OnSceneFunc;
            }

            set
            {
                if (FiOnPreSceneGUIDelegateFieldInfo == null) {
                    return;
                }
                FiOnPreSceneGUIDelegateFieldInfo.SetValue(null, value);
            }
        }

        // Add delegate to UnityEditor.SceneView.onPreSceneGUIDelegate
        public static void AddOnPreSceneGUIDelegate(SceneView.OnSceneFunc func)
        {
            OnPreSceneGUIDelegate =
                Delegate.Combine(func, OnPreSceneGUIDelegate)
                    as SceneView.OnSceneFunc;
        }

        // Remove delegate from UnityEditor.SceneView.onPreSceneGUIDelegate
        public static void RemoveOnPreSceneGUIDelegate(SceneView.OnSceneFunc func)
        {
            OnPreSceneGUIDelegate =
                Delegate.Remove(func, OnPreSceneGUIDelegate)
                    as SceneView.OnSceneFunc;
        }
    }
#endif // UNITY_2019_1_OR_NEWER
}      // namespace
