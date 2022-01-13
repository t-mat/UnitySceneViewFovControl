using UnityEditor;
using System.Collections.Generic;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditorExtension.SceneViewFovControl {
    [InitializeOnLoad]
    public static class SceneViewFovControl {
        private static readonly Dictionary<int, Status> Statuses    = new Dictionary<int, Status>();
        private static          bool                    _enableFlag = false;

        static SceneViewFovControl() => Enable(true);

        private static void Enable(bool enable)
        {
            if (enable != _enableFlag) {
                if (enable) {
#if UNITY_2019_1_OR_NEWER
                    SceneView.beforeSceneGui += OnScene;
                    SceneView.duringSceneGui += OnSceneGUI;
#else
                    SceneViewHiddenApi.AddOnPreSceneGUIDelegate(OnScene);
                    SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
                } else {
#if UNITY_2019_1_OR_NEWER
                    SceneView.duringSceneGui -= OnSceneGUI;
                    SceneView.beforeSceneGui -= OnScene;
#else
                    SceneView.onSceneGUIDelegate -= OnSceneGUI;
                    SceneViewHiddenApi.RemoveOnPreSceneGUIDelegate(OnScene);
#endif
                }
                _enableFlag = enable;
            }
            SceneView.RepaintAll();
        }

        private static Status GetOrAddStatus(SceneView sceneView)
        {
            if (sceneView == null || sceneView.camera == null) {
                return null;
            }
            int key = sceneView.GetInstanceID();
            if (!Statuses.TryGetValue(key, out Status s)) {
                s             = new Status();
                Statuses[key] = s;
            }
            return s;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            Status s = GetOrAddStatus(sceneView);
            if (s == null) {
                return;
            }
            Handles.BeginGUI();
            s.OnSceneGUI(sceneView);
            Handles.EndGUI();
        }

        private static void OnScene(SceneView sceneView) => GetOrAddStatus(sceneView)?.OnScene(sceneView);
    }
} // namespace
