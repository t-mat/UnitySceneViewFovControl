using UnityEngine;
using System;
using System.Reflection;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditorExtension.SceneViewFovControl {
    // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEngine/UnityEngine/GUIClip.cs
    internal static class GUIClip {
        private const           BindingFlags NonPublicStatic = BindingFlags.NonPublic | BindingFlags.Static;
        private static readonly Type         TypeGuiClip     = typeof(Transform).Assembly.GetType(name: "UnityEngine.GUIClip");
        private static readonly MethodInfo   MiGetTopRect    = TypeGuiClip.GetMethod(name: "GetTopRect", bindingAttr: NonPublicStatic);
        private static readonly MethodInfo   MiPush          = TypeGuiClip.GetMethod(name: "Push",       bindingAttr: NonPublicStatic);
        private static readonly MethodInfo   MiPop           = TypeGuiClip.GetMethod(name: "Pop",        bindingAttr: NonPublicStatic);

        // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEngine/UnityEngine/GUIClip.cs#L76
        public static Rect GetTopRect() =>
            (Rect)MiGetTopRect.Invoke(obj: null, parameters: null);

        // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEngine/UnityEngine/GUIClip.cs#L65
        public static void Push(Rect rect, Vector2 v0, Vector2 v1, bool b) =>
            MiPush.Invoke(obj: null, parameters: new object[] { rect, v0, v1, b });

        // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEngine/UnityEngine/GUIClip.cs#L73
        public static void Pop() =>
            MiPop.Invoke(obj: null, parameters: null);
    }
}
