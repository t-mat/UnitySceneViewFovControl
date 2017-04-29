using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditorExtension.SceneViewFovControl {

// https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEngine/UnityEngine/GUIClip.cs
static class GUIClip {
    const BindingFlags nonPublicStatic = BindingFlags.NonPublic | BindingFlags.Static;

    static readonly Type typeGuiClip = typeof(UnityEngine.Transform).Assembly.GetType("UnityEngine.GUIClip");
    static readonly MethodInfo mi_GetTopRect = typeGuiClip.GetMethod("GetTopRect", nonPublicStatic);
    static readonly MethodInfo mi_Push = typeGuiClip.GetMethod("Push", nonPublicStatic);
    static readonly MethodInfo mi_Pop = typeGuiClip.GetMethod("Pop", nonPublicStatic);

    public static Rect GetTopRect() {
        // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEngine/UnityEngine/GUIClip.cs#L76
        return (Rect) mi_GetTopRect.Invoke(null, null);
    }

    public static void Push(Rect rect, Vector2 v0, Vector2 v1, bool b) {
        // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEngine/UnityEngine/GUIClip.cs#L65
        mi_Push.Invoke(null, new object[] { rect, v0, v1, b });
    }

    public static void Pop() {
        // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEngine/UnityEngine/GUIClip.cs#L73
        mi_Pop.Invoke(null, null);
    }
}

} // namespace
