using System;
using System.Reflection;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditorExtension.SceneViewFovControl {
    // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditorInternal/RenderDoc.cs
    static class RenderDoc {
        const BindingFlags publicStatic = BindingFlags.Public | BindingFlags.Static;

        static readonly Type typeRenderDoc = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditorInternal.RenderDoc");
        static readonly MethodInfo mi_IsInstalled = typeRenderDoc.GetMethod("IsInstalled", publicStatic);
        static readonly MethodInfo mi_IsLoaded = typeRenderDoc.GetMethod("IsLoaded", publicStatic);
        static readonly MethodInfo mi_IsSupported = typeRenderDoc.GetMethod("IsSupported", publicStatic);
        static readonly MethodInfo mi_Load = typeRenderDoc.GetMethod("Load", publicStatic);

        public static bool IsInstalled() {
            if(mi_IsInstalled == null) {
                return false;
            }
            return (bool) mi_IsInstalled.Invoke(null, null);
        }

        public static bool IsLoaded() {
            if(mi_IsLoaded == null) {
                return false;
            }
            return (bool) mi_IsLoaded.Invoke(null, null);
        }

        public static bool IsSupported() {
            if(mi_IsSupported == null) {
                return false;
            }
            return (bool) mi_IsSupported.Invoke(null, null);
        }

        public static void Load() {
            if(mi_Load == null) {
                return;
            }
            mi_Load.Invoke(null, null);
        }
    }
}
