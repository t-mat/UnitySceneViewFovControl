using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditorExtension.SceneViewFovControl {
#if UNITY_2019_1_OR_NEWER
	// We don't use any hacks for recent version of Unity.
#else
// https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditor/SceneView.cs
    internal static class SceneViewHiddenApiExtension {
        private const           BindingFlags NonPublicInstance          = BindingFlags.NonPublic | BindingFlags.Instance;
        private static readonly Type         TypeSceneView              = typeof(UnityEditor.SceneView);
        private static readonly MethodInfo   MiUseSceneFiltering        = TypeSceneView.GetMethod(name: "UseSceneFiltering",        bindingAttr: NonPublicInstance);
        private static readonly MethodInfo   MiSceneCameraRendersIntoRT = TypeSceneView.GetMethod(name: "SceneCameraRendersIntoRT", bindingAttr: NonPublicInstance);
        private static readonly FieldInfo    FiMGizmosContent           = TypeSceneView.GetField(name: "m_GizmosContent",    bindingAttr: NonPublicInstance);
        private static readonly FieldInfo    FiMRenderDocContent        = TypeSceneView.GetField(name: "m_RenderDocContent", bindingAttr: NonPublicInstance);

        public static bool UseSceneFiltering(this SceneView sceneView)
        {
            // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditor/SceneView.cs#L1033
            return (bool)MiUseSceneFiltering.Invoke(obj: sceneView, parameters: null);
        }

        public static bool SceneCameraRendersIntoRT(this SceneView sceneView)
        {
            // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditor/SceneView.cs#L1200
            return (bool)MiSceneCameraRendersIntoRT.Invoke(obj: sceneView, parameters: null);
        }

        public static GUIContent Get_m_GizmosContent(this SceneView sceneView)
        {
            // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditor/SceneView.cs#L215
            return FiMGizmosContent.GetValue(obj: sceneView) as GUIContent;
        }

        public static GUIContent Get_m_RenderDocContent(this SceneView sceneView)
        {
            // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditor/SceneView.cs#L219
            return FiMRenderDocContent.GetValue(obj: sceneView) as GUIContent;
        }

        private static Rect[] PopGUIClips(this SceneView sceneView)
        {
            const int count = 1;
#if UNITY_5_6_OR_NEWER
            // do nothing
#else
        if(!sceneView.UseSceneFiltering() && sceneView.SceneCameraRendersIntoRT()) {
            // https://github.com/MattRix/UnityDecompiled/blob/753fde37d331b2100f93cc5f9eb343f1dcff5eee/UnityEditor/UnityEditor/SceneView.cs#L1255
            ++count;
        }
#endif
            var guiClips = new Rect[count];
            for (int i = 0; i < count; ++i) {
                guiClips[i] = GUIClip.GetTopRect();
                GUIClip.Pop();
            }
            return guiClips;
        }

        private static void PushGUIClips(this SceneView sceneView, Rect[] guiClips)
        {
            for (int i = guiClips.Length - 1; i >= 0; --i) {
                GUIClip.Push(rect: guiClips[i], v0: Vector2.zero, v1: Vector2.zero, b: false);
            }
        }

        private const float LeftMargin           = 251f;
        private const float TopOffset            = -1f;
        private const float MinGuiContentWidth   = 16f;
        private const float RightMinOffset       = 274.0f;
        private const float RightRenderDocOffset = 26.0f;

        private static float RightOffset
        {
            get
            {
                float o = RightMinOffset;
                if (RenderDoc.IsLoaded()) {
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
        public static void DoToolbarRightSideGUI(this SceneView sceneView, GUIContent content, GUIStyle style, Action<Rect> action)
        {
            Vector2 size = style.CalcSize(content: content);
            float   wb   = sceneView.position.width - RightOffset;
            float   lx   = wb                       - size.x;
            if (lx < LeftMargin) {
                lx     = LeftMargin;
                size.x = wb - lx;
            }
            if (size.x > MinGuiContentWidth) {
                Rect rect   = new Rect(x: lx, y: TopOffset, width: size.x, height: size.y);
                RenderTexture active = RenderTexture.active;
                RenderTexture.active = null;
                Rect[] guiClips = sceneView.PopGUIClips();

                action(obj: rect);

                sceneView.PushGUIClips(guiClips);
                RenderTexture.active = active;
            }
        }
    }
#endif // UNITY_2019_1_OR_NEWER
}
