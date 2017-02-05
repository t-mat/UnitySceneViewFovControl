using UnityEngine;
using UnityEditor;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditor.Extension.SceneViewFovControl {

class Status {
    float fov = 0.0f;

    public void OnScene(SceneView sceneView) {
        if(sceneView == null || sceneView.camera == null) {
            return;
        }

        Camera camera = sceneView.camera;
        if(fov == 0.0f) {
            fov = camera.fieldOfView;
        }

        var ev = Event.current;
        float deltaFov = 0.0f;

        if(ev.modifiers == Settings.ModifiersNormal || ev.modifiers == Settings.ModifiersQuick) {
            if(ev.type == EventType.ScrollWheel) {
                // todo : Check compatibility of Event.delta.y.
                //        e.g. Platform, mice, etc.
                deltaFov = ev.delta.y;
                ev.Use();
            } else if(ev.type == EventType.KeyDown && ev.keyCode == Settings.KeyCodeIncreaseFov) {
                deltaFov = +1.0f;
                ev.Use();
            } else if(ev.type == EventType.KeyDown && ev.keyCode == Settings.KeyCodeDecreaseFov) {
                deltaFov = -1.0f;
                ev.Use();
            }
        }

        if(deltaFov != 0.0f) {
            deltaFov *= Settings.FovSpeed;
            if(ev.modifiers == Settings.ModifiersQuick) {
                deltaFov *= Settings.FovQuickMultiplier;
            }
            fov += deltaFov;
            fov = Mathf.Clamp(fov, Settings.MinFov, Settings.MaxFov);
        }

        camera.fieldOfView = fov;
    }
}

} // namespace
