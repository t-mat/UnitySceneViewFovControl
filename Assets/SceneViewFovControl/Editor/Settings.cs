using UnityEngine;
using UnityEditor;
using System;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditor.Extension.SceneViewFovControl {
    static class Settings {
        static SettingsData data = new SettingsData();
        static SettingsData loadedData = new SettingsData();

        public const string VersionString = "0.1.4";
        public const string MenuItemName = "Edit/Scene View FoV Settings";
        public const string EditorPrefsKey = "UTJ.UnityEditor.Extension.SceneViewFovControl";

        public const float FovSpeedMin = 0.01f;
        public const float FovSpeedMax = 10.0f;
        public const float FovQuickMultiplierMin = 0.1f;
        public const float FovQuickMultiplierMax = 20.0f;
        public const float MinFovMin = 1.0f;
        public const float MinFovMax = 160.0f;
        public const float MaxFovMin = 1.0f;
        public const float MaxFovMax = 160.0f;
        public const float ButtonShowingDurationInSeconds = 2.0f;
        public const float MinButtonShowingDurationInSeconds = 0.0f;
        public const float MaxButtonShowingDurationInSeconds = 5.0f;

        public const string WindowTitle = "FoV Control";

        public static SettingsData Data {
            get {
                return data;
            }

            set {
                data = value;
            }
        }

        public static SettingsData LoadedData {
            get {
                return loadedData;
            }

            set {
                loadedData = value;
            }
        }

        static Settings() {
            Reset();

            if(EditorPrefs.HasKey(EditorPrefsKey)) {
                LoadedData = Load(EditorPrefsKey);
                Data = LoadedData.Clone();
            }
        }

        public static void Reset() {
            Data.Reset();
        }

        public static void Save() {
            Store(EditorPrefsKey, Data);
            LoadedData = Data.Clone();
        }

        public static SettingsData Load(string key) {
            var jsonString = EditorPrefs.GetString(key);
            return JsonUtility.FromJson<SettingsData>(jsonString);
        }

        public static void Store(string key, SettingsData settingsData) {
            var jsonString = JsonUtility.ToJson(settingsData);
            EditorPrefs.SetString(key, jsonString);
        }
    }

    [Serializable]
    public class SettingsData {
        public string VersionString;
        public EventModifiers ModifiersNormal;
        public EventModifiers ModifiersQuick;

        public KeyCode KeyCodeIncreaseFov;
        public KeyCode KeyCodeDecreaseFov;

        public float FovSpeed;
        public float FovQuickMultiplier;
        public float MinFov;
        public float MaxFov;

        public float ButtonShowingDurationInSeconds;

        public SettingsData Clone() {
            return (SettingsData) this.MemberwiseClone();
        }

        public void Reset() {
            VersionString = Settings.VersionString;
            ModifiersNormal = EventModifiers.Alt | EventModifiers.Control;
            ModifiersQuick  = EventModifiers.Alt | EventModifiers.Control | EventModifiers.Shift;

            KeyCodeIncreaseFov = KeyCode.O;
            KeyCodeDecreaseFov = KeyCode.P;

            FovSpeed = 0.15f;
            FovQuickMultiplier = 5.0f;
            MinFov = 2.0f;
            MaxFov = 160.0f;

            ButtonShowingDurationInSeconds = Settings.ButtonShowingDurationInSeconds;
        }

        public bool AlwaysShowResetButton {
            get {
                return ButtonShowingDurationInSeconds == Settings.MaxButtonShowingDurationInSeconds;
            }
        }

        public bool NeverShowResetButton {
            get {
                return ButtonShowingDurationInSeconds == Settings.MinButtonShowingDurationInSeconds;
            }
        }

        public bool Dirty {
            get; set;
        }
    }


    class SettingsGui : EditorWindow {
        static SettingsGui settingGui;

        [MenuItem(Settings.MenuItemName)]
        static void Open() {
            if(settingGui == null) {
                settingGui = EditorWindow.GetWindow<SettingsGui>(false, Settings.WindowTitle);
            }
        }

        void OnGUI() {
            var d = Settings.Data;

            GUILayout.Space(4);

            GUILayout.Label("<<< Scene View FoV Control Settings - version " + Settings.VersionString + ">>>");

            GUILayout.Space(8);

            GUILayout.Label("FoV Speed:" + d.FovSpeed);
            d.FovSpeed = GUILayout.HorizontalSlider(d.FovSpeed, Settings.FovSpeedMin, Settings.FovSpeedMax);

            GUILayout.Space(8);

            GUILayout.Label("FoV <Shift> Modifier Multiplier:" + d.FovQuickMultiplier);
            d.FovQuickMultiplier = GUILayout.HorizontalSlider(d.FovQuickMultiplier, Settings.FovQuickMultiplierMin, Settings.FovQuickMultiplierMax);

            GUILayout.Space(8);

            GUILayout.Label("Min FoV:" + d.MinFov);
            d.MinFov = GUILayout.HorizontalSlider(d.MinFov, Settings.MinFovMin, Settings.MinFovMax);

            if(d.MinFov > d.MaxFov) {
                d.MaxFov = d.MinFov;
            }

            GUILayout.Space(8);

            GUILayout.Label("Max FoV:" + d.MaxFov);
            d.MaxFov = GUILayout.HorizontalSlider(d.MaxFov, Settings.MaxFovMin, Settings.MaxFovMax);

            if(d.MaxFov < d.MinFov) {
                d.MinFov = d.MaxFov;
            }

            GUILayout.Space(8);

            if(d.AlwaysShowResetButton) {
                GUILayout.Label("<Reset Scene FoV> Button Showing Duration (sec): Infinite");
            } else if(d.NeverShowResetButton) {
                GUILayout.Label("<Reset Scene FoV> Button Showing Duration (sec): Never");
            } else {
                GUILayout.Label("<Reset Scene FoV> Button Showing Duration (sec):" + d.ButtonShowingDurationInSeconds);
            }
            d.ButtonShowingDurationInSeconds = GUILayout.HorizontalSlider(d.ButtonShowingDurationInSeconds, Settings.MinButtonShowingDurationInSeconds, Settings.MaxButtonShowingDurationInSeconds);

            if(d.MaxFov < d.MinFov) {
                d.MinFov = d.MaxFov;
            }

            GUILayout.Space(20);

            if(SceneViewFovControl.EnableFlag) {
                if(GUILayout.Button("Disable")) {
                    SceneViewFovControl.Enable(false);
                }
            } else {
                if(GUILayout.Button("Enable")) {
                    SceneViewFovControl.Enable(true);
                }
            }

            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            {
                if(GUILayout.Button("Save")) {
                    Settings.Save();
                }

                GUILayout.Space(20);

                if(GUILayout.Button("Restore default settings")) {
                    d.Reset();
                }

                GUILayout.Space(20);

                if(GUILayout.Button("Restore saved settings")) {
                    Settings.Data = Settings.LoadedData.Clone();
                    d = Settings.Data;
                }

                GUILayout.Space(20);

                if(GUILayout.Button("Close")) {
                    this.Close();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
