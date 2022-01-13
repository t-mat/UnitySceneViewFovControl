using System;
using System.Reflection;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditorExtension.SceneViewFovControl {
#if UNITY_2019_1_OR_NEWER
    // We don't use any hacks for recent version of Unity.
#else
    // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditorInternal/RenderDoc.cs
    internal static class RenderDoc {
        private const           BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;
        private static readonly Type TypeRenderDoc = typeof(UnityEditor.EditorWindow).Assembly.GetType(name: "UnityEditorInternal.RenderDoc");

        private static readonly MethodInfo MiIsInstalled = TypeRenderDoc.GetMethod(name: "IsInstalled", bindingAttr: PublicStatic);
        private static readonly MethodInfo MiIsLoaded    = TypeRenderDoc.GetMethod(name: "IsLoaded",    bindingAttr: PublicStatic);
        private static readonly MethodInfo MiIsSupported = TypeRenderDoc.GetMethod(name: "IsSupported", bindingAttr: PublicStatic);
        private static readonly MethodInfo MiLoad        = TypeRenderDoc.GetMethod(name: "Load",        bindingAttr: PublicStatic);

        public static bool IsInstalled() => (bool)MiIsInstalled.Invoke(obj: null, parameters: null);
        public static bool IsLoaded()    => (bool)MiIsLoaded.Invoke(obj: null, parameters: null);
        public static bool IsSupported() => (bool)MiIsSupported.Invoke(obj: null, parameters: null);
        public static void Load()        => MiLoad.Invoke(obj: null, parameters: null);
    }
#endif
}
