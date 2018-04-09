using UnityEngine;
using UnityEditor;
using System;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditorExtension.SceneViewFovControl {
    static class Settings {
        public const string VersionString = "0.1.11";
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

        public const string WindowTitle = "FoV Control";

        public static SettingsData Data { get; set; }
        public static SettingsData LoadedData { get; set; }

        static Settings() {
            Data = new SettingsData();
            LoadedData = new SettingsData();

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
        }
    }
}
