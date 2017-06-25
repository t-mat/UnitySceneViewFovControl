using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditorExtension.SceneViewFovControl {

// https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditor/SceneView.cs
static class SceneViewHiddenApiExtension {
    const BindingFlags nonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;

    static readonly Type typeSceneView = typeof(UnityEditor.SceneView);
    static readonly MethodInfo mi_UseSceneFiltering = typeSceneView.GetMethod("UseSceneFiltering", nonPublicInstance);
    static readonly MethodInfo mi_SceneCameraRendersIntoRT = typeSceneView.GetMethod("SceneCameraRendersIntoRT", nonPublicInstance);
    static readonly FieldInfo fi_m_GizmosContent = typeSceneView.GetField("m_GizmosContent", nonPublicInstance);
    static readonly FieldInfo fi_m_RenderDocContent = typeSceneView.GetField("m_RenderDocContent", nonPublicInstance);

    public static bool UseSceneFiltering(this SceneView sceneView) {
        // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditor/SceneView.cs#L1033
        return (bool) mi_UseSceneFiltering.Invoke(sceneView, null);
    }

    public static bool SceneCameraRendersIntoRT(this SceneView sceneView) {
        // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditor/SceneView.cs#L1200
        return (bool) mi_SceneCameraRendersIntoRT.Invoke(sceneView, null);
    }

    public static GUIContent Get_m_GizmosContent(this SceneView sceneView) {
        // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditor/SceneView.cs#L215
        return fi_m_GizmosContent.GetValue(sceneView) as GUIContent;
    }

    public static GUIContent Get_m_RenderDocContent(this SceneView sceneView) {
        // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditor/SceneView.cs#L219
        return fi_m_RenderDocContent.GetValue(sceneView) as GUIContent;
    }

    static Rect[] PopGUIClips(this SceneView sceneView) {
        int count = 1;
#if UNITY_5_6_OR_NEWER
        // do nothing
#else
        if(!sceneView.UseSceneFiltering() && sceneView.SceneCameraRendersIntoRT()) {
            // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditor/SceneView.cs#L1255
            ++count;
        }
#endif
        var guiClips = new Rect[count];
        for(int i = 0; i < count; ++i) {
            guiClips[i] = GUIClip.GetTopRect();
            GUIClip.Pop();
        }
        return guiClips;
    }

    static void PushGUIClips(this SceneView sceneView, Rect[] guiClips) {
        for(int i = guiClips.Length-1; i >= 0; --i) {
            GUIClip.Push(guiClips[i], Vector2.zero, Vector2.zero, false);
        }
    }

    const float LeftMargin = 251f;
    const float TopOffset = -1f;
    const float MinGuiContentWidth = 16f;
    const float RightMinOffset = 274.0f;
    const float RightRenderDocOffset = 26.0f;

    static float RightOffset {
        get {
            float o = RightMinOffset;
            if(RenderDoc.IsLoaded()) {
                o += RightRenderDocOffset;
            }
            return o;
        }
    }

    // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditor/SceneView.cs#L810
    //
    // If we can put the following function after above ^ line, there's no problem.
    // But since we don't have any hook in the toolbar, the following function emulates it.
    // note : This code doesn't coexistent with other user toolbar extensions such as "Scene View Bookmarks" [1]
    //
    // [1] Scene View Bookmarks https://www.assetstore.unity3d.com/en/#!/content/22302
    public static void DoToolbarRightSideGUI(this SceneView sceneView, GUIContent content, GUIStyle style, Action<Rect> action) {
        Vector2 size = style.CalcSize(content);
        float wb = sceneView.position.width - RightOffset;
        float lx = wb - size.x;
        if(lx < LeftMargin) {
            lx = LeftMargin;
            size.x = wb - lx;
        }
        if(size.x > MinGuiContentWidth) {
            var rect = new Rect(lx, TopOffset, size.x, size.y);
            var active = RenderTexture.active;
            RenderTexture.active = null;
            Rect[] guiClips = sceneView.PopGUIClips();

            action(rect);

            sceneView.PushGUIClips(guiClips);
            RenderTexture.active = active;
        }
    }
}

}
