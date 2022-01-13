// https://github.com/anchan828/unitejapan2014/tree/master/SyncCamera/Assets
// todo: near/far clip control
// todo: skybox doesn't follow FoV

#define SCENE_VIEW_FOV_CONTROL_USE_GUI_BUTTON

using UnityEngine;
using UnityEditor;
using System;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditorExtension.SceneViewFovControl {
    internal class Status {
        private float         _fov                    = 0.0f;
        private bool          _reset                  = false;
        private bool          _autoFov                = true;
        private float         _lastOnSceneGuiFov      = 0.0f;
        private WeakReference _followerCamera         = new WeakReference(null);
        private bool          _previousAutoFov        = false;
        private System.Object _previousFollowerCamera = null;

        private const string ButtonStringFovAuto             = "FoV:Auto";
        private const string ButtonStringFovUser             = "FoV:{0:0.00}";
        private const string ButtonStringFovAutoWithFollower = "FoV:Auto>{0}";
        private const string ButtonStringFovUserWithFollower = "FoV:{0:0.00}>{1}";
        private const string FollowerCameraSubMenu           = "Follower Camera/{0}";

#if SCENE_VIEW_FOV_CONTROL_USE_GUI_BUTTON
        private string _buttonString = "";

        private enum MouseButton {
            None,
            Left,
            Right,
            Middle
        }
#else
        GUIContent ButtonContent = null;
#endif

        public void OnScene(SceneView sceneView)
        {
            if (sceneView        == null
             || sceneView.camera == null
             || sceneView.in2DMode
               ) {
                return;
            }

            Camera camera = sceneView.camera;
            SceneView.CameraSettings sceneViewCameraSettings = sceneView.cameraSettings;

            if(camera.fieldOfView != sceneViewCameraSettings.fieldOfView) {
                camera.fieldOfView = sceneViewCameraSettings.fieldOfView;
            }

            if (!_autoFov) {
                if (_fov == 0.0f || _reset) {
                    _fov   = camera.fieldOfView;
                    _reset = false;
                }

                Event        ev       = Event.current;
                SettingsData settings = Settings.Data;
                float        deltaFov = 0.0f;

                if (ev.modifiers == settings.modifiersNormal || ev.modifiers == settings.modifiersQuick) {
                    if (ev.type == EventType.ScrollWheel) {
                        // note : In MacOS, ev.delta becomes zero when "Shift" pressed.  I don't know the reason.
                        deltaFov = ev.delta.y;
                        ev.Use();
                    } else if (ev.type == EventType.KeyDown && ev.keyCode == settings.keyCodeIncreaseFov) {
                        deltaFov = +1.0f;
                        ev.Use();
                    } else if (ev.type == EventType.KeyDown && ev.keyCode == settings.keyCodeDecreaseFov) {
                        deltaFov = -1.0f;
                        ev.Use();
                    }
                }

                if (deltaFov != 0.0f) {
                    deltaFov *= settings.fovSpeed;
                    if (ev.modifiers == settings.modifiersQuick) {
                        deltaFov *= settings.fovQuickMultiplier;
                    }
                    _fov += deltaFov;
                    _fov =  Mathf.Clamp(_fov, settings.minFov, settings.maxFov);
                }

                camera.fieldOfView = _fov;
            }

            if(sceneViewCameraSettings.fieldOfView != camera.fieldOfView) {
                sceneViewCameraSettings.fieldOfView = camera.fieldOfView;
            }

            if (HasFollowerCamera()) {
                CopyCameraInfo(from: camera, to: GetFollowerCamera());
            }
        }

        private static void CopyCameraInfo(Camera from, Camera to)
        {
            if (from == null || to == null) {
                return;
            }
            to.fieldOfView                   = from.fieldOfView;
            to.gameObject.transform.position = from.transform.position;
            to.gameObject.transform.rotation = from.transform.rotation;
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            {
                bool f = false;
                if (!_autoFov && _lastOnSceneGuiFov != _fov) {
                    _lastOnSceneGuiFov = _fov;
                    f                  = true;
                }
                if (_previousAutoFov != _autoFov) {
                    _previousAutoFov = _autoFov;
                    f                = true;
                }
                if (GetFollowerCamera() as System.Object != _previousFollowerCamera) {
                    _previousFollowerCamera = GetFollowerCamera() as System.Object;
                    f                       = true;
                }
#if !SCENE_VIEW_FOV_CONTROL_USE_GUI_BUTTON
                if(ButtonContent == null) {
                    f = true;
                }
#endif
                if (f) {
                    string s;
                    if (_autoFov) {
                        if (HasFollowerCamera()) {
                            s = string.Format(ButtonStringFovAutoWithFollower, GetFollowerCameraName());
                        } else {
                            s = ButtonStringFovAuto;
                        }
                    } else {
                        if (HasFollowerCamera()) {
                            s = string.Format(ButtonStringFovUserWithFollower, _fov, GetFollowerCameraName());
                        } else {
                            s = string.Format(ButtonStringFovUser, _fov);
                        }
                    }
#if SCENE_VIEW_FOV_CONTROL_USE_GUI_BUTTON
                    _buttonString = s;
#else
                    ButtonContent = new GUIContent(s);
#endif
                }
            }

#if SCENE_VIEW_FOV_CONTROL_USE_GUI_BUTTON
            MouseButton mouseButton = MouseButton.None;
            {
                Event e = Event.current;
                if (e != null && e.type == EventType.MouseUp) {
                    switch (e.button) {
                        default:
                        case 0:
                            mouseButton = MouseButton.Left;
                            break;
                        case 1:
                            mouseButton = MouseButton.Right;
                            break;
                        case 2:
                            mouseButton = MouseButton.Middle;
                            break;
                    }
                }
            }
            GUIStyle style = EditorStyles.miniButton;
            if (GUI.Button(new Rect(x: 8, y: 8, width: 160, height: 24), _buttonString, style)) {
                switch (mouseButton) {
                    default:
                    case MouseButton.None:
                    case MouseButton.Middle:
                        break;
                    case MouseButton.Left:
                        OnFovButtonLeftClicked(sceneView);
                        break;
                    case MouseButton.Right:
                        OnFovButtonRightClicked(sceneView);
                        break;
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

        private void SetAutoFov(bool auto)
        {
            if (_autoFov != auto) {
                _autoFov = auto;
                if (!_autoFov) {
                    _reset = true;
                }
            }
        }

        // This procedure will be called when "FoV" button is left-clicked.
        private void OnFovButtonLeftClicked(SceneView sceneView) => SetAutoFov(!_autoFov);

        // This procedure will be called when "FoV" button is right-clicked.
        private void OnFovButtonRightClicked(SceneView sceneView)
        {
            var menu = new GenericMenu();

            menu.AddItem(
                         new GUIContent("FoV : Auto (Default behaviour)")
                       , _autoFov
                       , (obj) => { SetAutoFov(true); }
                       , 0
                        );

            menu.AddItem(
                         new GUIContent("FoV : Manual")
                       , !_autoFov
                       , (obj) => { SetAutoFov(false); }
                       , 0
                        );

            menu.AddSeparator(string.Empty);

            menu.AddItem(
                         new GUIContent("Reset Follower Camera")
                       , false
                       , (obj) => { SetFollowerCamera(null); }
                       , 0
                        );

            {
                menu.AddSeparator(string.Format(FollowerCameraSubMenu, string.Empty));
                foreach (var camera in Camera.allCameras) {
                    menu.AddItem(
                                 new GUIContent(string.Format(FollowerCameraSubMenu, camera.name))
                               , IsFollowerCamera(camera)
                               , (obj) => { SetFollowerCamera(obj as Camera); }
                               , camera
                                );
                }
            }

            menu.ShowAsContext();
        }

        private bool HasFollowerCamera() => GetFollowerCamera() != null;

        private bool IsFollowerCamera(Camera camera) => camera == GetFollowerCamera();

        private Camera GetFollowerCamera() => !_followerCamera.IsAlive ? null : _followerCamera.Target as Camera;

        private string GetFollowerCameraName()
        {
            Camera camera = GetFollowerCamera();
            return camera != null ? camera.name : "Camera(null)";
        }

        private void SetFollowerCamera(Camera camera) => _followerCamera = new WeakReference(camera);
    }
} // namespace
