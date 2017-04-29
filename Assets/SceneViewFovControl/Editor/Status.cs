// https://github.com/anchan828/unitejapan2014/tree/master/SyncCamera/Assets
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditor.Extension.SceneViewFovControl {

class Status {
    static WeakReference masterSceneView = new WeakReference(null);
    float fov = 0.0f;
    bool reset = false;
    bool autoFov = true;
    float lastOnSceneGuiFov = 0.0f;

    static Camera[] activeCameras = new Camera[0];

    const string ButtonStringFovAuto = "FoV:Auto";
    const string ButtonStringFovUser = "FoV:{0:0.00}";

    static readonly GUIContent ButtonContentFovAuto = new GUIContent(ButtonStringFovAuto);
    GUIContent ButtonContentFovUser = new GUIContent(ButtonStringFovUser);

    enum MenuId {
        ToggleMainCameraFollowsSceneView
    }

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

        if(IsMasterSceneView(sceneView)) {
            // todo : It should choose a camera which is shown in the "Camera Preview" window in the Scene View.
            if(activeCameras.Length == 0) {
                var mainCamera = Camera.main;
                CopyCameraInfo(from: camera, to: mainCamera);
            } else {
                foreach(var toCamera in activeCameras) {
                    CopyCameraInfo(from: camera, to: toCamera);
                }
            }
        }
    }

    static void SetMasterSceneView(SceneView sceneView) {
        masterSceneView = new WeakReference(sceneView);
    }

    static bool IsMasterSceneView(SceneView sceneView) {
        return sceneView != null && masterSceneView.Target as SceneView == sceneView;
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
        var content = ButtonContentFovUser;
        if(autoFov) {
            content = ButtonContentFovAuto;
        } else {
            if(lastOnSceneGuiFov != fov) {
                lastOnSceneGuiFov = fov;
                ButtonContentFovUser = new GUIContent(string.Format(ButtonStringFovUser, fov));
            }
            content = ButtonContentFovUser;
        }

        GUIStyle style = EditorStyles.toolbarButton;
        sceneView.DoToolbarRightSideGUI(content, style, (rect) => {
            int btn = -1;
            if(Event.current.type == EventType.MouseUp) {
                btn = Event.current.button;
            }
            if (GUI.Button(rect, content, style)) {
                if(btn == 1) {
                    OnFovButtonRightClicked(sceneView);
                } else {
                    OnFovButtonLeftClicked(sceneView);
                }
            }
        });
    }

    public void ToggleAutoFov() {
        autoFov = !autoFov;
        if(!autoFov) {
            reset = true;
        }
    }

    public static void ToggleMainCameraFollowsSceneView(SceneView sceneView) {
        if(IsMasterSceneView(sceneView)) {
            SetMasterSceneView(null);
        } else {
            SetMasterSceneView(sceneView);
        }
    }

    // This procedure will be called when "FoV" button is left-clcked.
    void OnFovButtonLeftClicked(SceneView sceneView) {
        ToggleAutoFov();
    }

    // This procedure will be called when "FoV" button is right-clcked.
    void OnFovButtonRightClicked(SceneView sceneView) {
        var menu = new GenericMenu();
        //  todo: near/far clip control
        // todo: main (active) camera stick with scene view (toast, toggle)
        menu.AddItem(
            new GUIContent("Main Camera follows this Scene View")
            , false
            , (obj) => {
                ToggleMainCameraFollowsSceneView(sceneView);
            }
            , MenuId.ToggleMainCameraFollowsSceneView
        );
        //menu.AddItem(new GUIContent("Item"), false, Callback, 2);
        menu.ShowAsContext();
    }

    [InitializeOnLoadMethod]
    static void ActiveCameraCorrector() {
        Selection.selectionChanged += () => {
            Camera[] newActiveCameras = null;
            {
                var activeCameraList = new List<Camera>();
                var goes = Selection.gameObjects;
                if(goes != null) {
                    foreach(var go in goes) {
                        var camera = go.GetComponent<Camera>();
                        if(camera != null) {
                            activeCameraList.Add(camera);
                        }
                    }
                }
                if(activeCameraList.Count == 0) {
                    newActiveCameras = new Camera[1] { Camera.main };
                } else {
                    activeCameraList.Sort(delegate(Camera lhs, Camera rhs) {
                        return lhs.GetInstanceID() - rhs.GetInstanceID();
                    });
                    newActiveCameras = activeCameraList.ToArray();
                }
            }

            bool activeCamerasAreChanged = false;
            if(activeCameras.Length != newActiveCameras.Length) {
                activeCamerasAreChanged = true;
            } else {
                for(int i = 0; i < newActiveCameras.Length; ++i) {
                    if(!object.Equals(newActiveCameras[i], activeCameras[i])) {
                        activeCamerasAreChanged = true;
                        break;
                    }
                }
            }

            if(activeCamerasAreChanged) {
                // When active camera is changed, disable copying scene view transform to game view.
                SetMasterSceneView(null);
                activeCameras = newActiveCameras;
            }
        };
    }
}

} // namespace
