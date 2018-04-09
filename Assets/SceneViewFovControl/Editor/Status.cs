// https://github.com/anchan828/unitejapan2014/tree/master/SyncCamera/Assets
// todo: near/far clip control
// todo: skybox doesn't follow FoV
#define SCENE_VIEW_FOV_CONTROL_USE_GUI_BUTTON

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditorExtension.SceneViewFovControl {

class Status {
    float fov = 0.0f;
    bool reset = false;
    bool autoFov = true;
    float lastOnSceneGuiFov = 0.0f;
    WeakReference slaveCamera = new WeakReference(null);

    bool previousAutoFov = false;
    System.Object previousSlaveCamera = null;

    const string ButtonStringFovAuto = "FoV:Auto";
    const string ButtonStringFovUser = "FoV:{0:0.00}";
    const string ButtonStringFovAutoWithSlave = "FoV:Auto>{0}";
    const string ButtonStringFovUserWithSlave = "FoV:{0:0.00}>{1}";
    const string SlaveCameraSubMenu = "Slave Camera/{0}";

#if SCENE_VIEW_FOV_CONTROL_USE_GUI_BUTTON
    string buttonString = "";

    enum MouseButton {
        None,
        Left,
        Right,
        Middle
    }
#else
    GUIContent ButtonContent = null;
#endif

    public void OnScene(SceneView sceneView) {
        if(sceneView == null
        || sceneView.camera == null
        || sceneView.in2DMode
        ) {
            return;
        }

        Camera camera = sceneView.camera;

        if(!autoFov) {
            if(fov == 0.0f || reset) {
                fov = camera.fieldOfView;
                reset = false;
            }

            var ev = Event.current;
            var settings = Settings.Data;
            float deltaFov = 0.0f;

            if(ev.modifiers == settings.ModifiersNormal || ev.modifiers == settings.ModifiersQuick) {
                if(ev.type == EventType.ScrollWheel) {
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

        if(HasSlaveCamera()) {
            CopyCameraInfo(from: camera, to: GetSlaveCamera());
        }
    }

    public static void CopyCameraInfo(Camera from, Camera to) {
        if(from == null || to == null) {
            return;
        }
        to.fieldOfView = from.fieldOfView;
        to.gameObject.transform.position = from.transform.position;
        to.gameObject.transform.rotation = from.transform.rotation;
    }

    public void OnSceneGUI(SceneView sceneView) {
        {
            bool f = false;
            if(!autoFov && lastOnSceneGuiFov != fov) {
                lastOnSceneGuiFov = fov;
                f = true;
            }
            if(previousAutoFov != autoFov) {
                previousAutoFov = autoFov;
                f = true;
            }
            if(GetSlaveCamera() as System.Object != previousSlaveCamera) {
                previousSlaveCamera = GetSlaveCamera() as System.Object;
                f = true;
            }
#if !SCENE_VIEW_FOV_CONTROL_USE_GUI_BUTTON
            if(ButtonContent == null) {
                f = true;
            }
#endif
            if(f) {
                string s;
                if(autoFov) {
                    if(HasSlaveCamera()) {
                        s = string.Format(ButtonStringFovAutoWithSlave, GetSlaveCameraName());
                    } else {
                        s = ButtonStringFovAuto;
                    }
                } else {
                    if(HasSlaveCamera()) {
                        s = string.Format(ButtonStringFovUserWithSlave, fov, GetSlaveCameraName());
                    } else {
                        s = string.Format(ButtonStringFovUser, fov);
                    }
                }
#if SCENE_VIEW_FOV_CONTROL_USE_GUI_BUTTON
                buttonString = s;
#else
                ButtonContent = new GUIContent(s);
#endif
            }
        }

#if SCENE_VIEW_FOV_CONTROL_USE_GUI_BUTTON
        MouseButton mouseButton = MouseButton.None;
        {
            var e = Event.current;
            if(e != null && e.type == EventType.MouseUp) {
                switch(e.button) {
                default:
                case 0: mouseButton = MouseButton.Left;     break;
                case 1: mouseButton = MouseButton.Right;    break;
                case 2: mouseButton = MouseButton.Middle;   break;
                }
            }
        }
        GUIStyle style = EditorStyles.miniButton;
        if (GUI.Button(new Rect(8, 8, 160, 24), buttonString, style)) {
            switch(mouseButton) {
            default:                                                    break;
            case MouseButton.Left:  OnFovButtonLeftClicked(sceneView);  break;
            case MouseButton.Right: OnFovButtonRightClicked(sceneView); break;
            }
        }
#else
        GUIStyle style = EditorStyles.toolbarDropDown;
        sceneView.DoToolbarRightSideGUI(ButtonContent, style, (rect) => {
            int btn = -1;
            if(Event.current.type == EventType.MouseUp) {
                btn = Event.current.button;
            }
            if (GUI.Button(rect, ButtonContent, style)) {
                if(btn == 1) {
                    OnFovButtonRightClicked(sceneView);
                } else {
                    OnFovButtonLeftClicked(sceneView);
                }
            }
        });
#endif
    }

    void SetAutoFov(bool auto) {
        if(autoFov != auto) {
            autoFov = auto;
            if(!autoFov) {
                reset = true;
            }
        }
    }

    // This procedure will be called when "FoV" button is left-clcked.
    void OnFovButtonLeftClicked(SceneView sceneView) {
        SetAutoFov(!autoFov);
    }

    // This procedure will be called when "FoV" button is right-clcked.
    void OnFovButtonRightClicked(SceneView sceneView) {
        var menu = new GenericMenu();

        menu.AddItem(
            new GUIContent("FoV : Auto (Default behaviour)")
            , autoFov
            , (obj) => {
                SetAutoFov(true);
            }
            , 0
        );

        menu.AddItem(
            new GUIContent("FoV : Manual")
            , !autoFov
            , (obj) => {
                SetAutoFov(false);
            }
            , 0
        );

        menu.AddSeparator(string.Empty);

        menu.AddItem(
            new GUIContent("Reset Slave Camera")
            , false
            , (obj) => {
                SetSlaveCamera(null);
            }
            , 0
        );

        {
            menu.AddSeparator(string.Format(SlaveCameraSubMenu, string.Empty));
            foreach(var camera in Camera.allCameras) {
                menu.AddItem(
                    new GUIContent(string.Format(SlaveCameraSubMenu, camera.name))
                    , IsSlaveCamera(camera)
                    , (obj) => {
                        SetSlaveCamera(obj as Camera);
                    }
                    , camera
                );
            }
        }

        menu.ShowAsContext();
    }

    bool HasSlaveCamera() {
        return GetSlaveCamera() != null;
    }

    bool IsSlaveCamera(Camera camera) {
        return camera == GetSlaveCamera();
    }

    Camera GetSlaveCamera() {
        if(! slaveCamera.IsAlive) {
            return null;
        }
        return slaveCamera.Target as Camera;
    }

    string GetSlaveCameraName() {
        var camera = GetSlaveCamera();
        if(camera != null) {
            return camera.name;
        }
        return "Camera(null)";
    }

    void SetSlaveCamera(Camera camera) {
        slaveCamera = new WeakReference(camera);
    }
}

} // namespace
