//#define SCENE_VIEW_FOV_DEBUG

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditor.Extension.SceneViewFovControl {

[InitializeOnLoad]
public static class SceneViewFovControl
{
    static Dictionary<int, Status> statuses = new Dictionary<int, Status>();
    static public bool EnableFlag = false;

    static SceneViewFovControl() {
        Enable(true);
    }

#if SCENE_VIEW_FOV_DEBUG
    [MenuItem("Experimental/Enable SceneView FOV Control")]
    static void MenuitemEnable() {
        Enable(true);
    }

    [MenuItem("Experimental/Disable SceneView FOV Control")]
    static void MenuitemDisable() {
        Enable(false);
    }
#endif

    public static void Enable(bool enable) {
        if(enable != EnableFlag) {
            if(enable) {
                SceneViewHiddenApi.AddOnPreSceneGUIDelegate(OnScene);
            } else {
                SceneViewHiddenApi.RemoveOnPreSceneGUIDelegate(OnScene);
            }
            EnableFlag = enable;
        }
        SceneView.RepaintAll();
    }

    static void OnScene(SceneView sceneView) {
        if(sceneView == null || sceneView.camera == null) {
            return;
        }

        Status s;
        {
            int key = sceneView.GetInstanceID();
            if(! statuses.TryGetValue(key, out s)) {
                s = new Status();
                statuses[key] = s;
            }
        }
        s.OnScene(sceneView);
    }
}

} // namespace
