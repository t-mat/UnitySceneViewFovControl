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

        if(sceneView.in2DMode) {
            return;
        }

        Camera camera = sceneView.camera;
        if(fov == 0.0f) {
            fov = camera.fieldOfView;
        }

        var ev = Event.current;
        var settings = Settings.Data;
        float deltaFov = 0.0f;

        if(ev.modifiers == settings.ModifiersNormal || ev.modifiers == settings.ModifiersQuick) {
            if(ev.type == EventType.ScrollWheel) {
                // todo : Check compatibility of Event.delta.y.
                //        e.g. Platform, mice, etc.
                // note : In MacOS, ev.delta becomes zero when "Shift" pressed.  I don't know the reason.
                deltaFov = ev.delta.y;
                ev.Use();
            } else if(ev.type == EventType.KeyDown && ev.keyCode == settings.KeyCodeIncreaseFov) {
                deltaFov = +1.0f;
                ev.Use();
            } else if(ev.type == EventType.KeyDown && ev.keyCode == settings.KeyCodeDecreaseFov) {
                deltaFov = -1.0f;
                ev.Use();
            }
        }

        if(deltaFov != 0.0f) {
            deltaFov *= settings.FovSpeed;
            if(ev.modifiers == settings.ModifiersQuick) {
                deltaFov *= settings.FovQuickMultiplier;
            }
            fov += deltaFov;
            fov = Mathf.Clamp(fov, settings.MinFov, settings.MaxFov);
        }

        camera.fieldOfView = fov;
    }
}

} // namespace
