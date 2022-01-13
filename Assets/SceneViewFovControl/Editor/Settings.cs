using UnityEngine;
using UnityEditor;
using System;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditorExtension.SceneViewFovControl {
    internal static class Settings {
        public const string VersionString         = "0.1.12";
        public const string MenuItemName          = "Edit/Scene View FoV Settings";
        public const string EditorPrefsKey        = "UTJ.UnityEditor.Extension.SceneViewFovControl";
        public const float  FovSpeedMin           = 0.01f;
        public const float  FovSpeedMax           = 10.0f;
        public const float  FovQuickMultiplierMin = 0.1f;
        public const float  FovQuickMultiplierMax = 20.0f;
        public const float  MinFovMin             = 1.0f;
        public const float  MinFovMax             = 160.0f;
        public const float  MaxFovMin             = 1.0f;
        public const float  MaxFovMax             = 160.0f;

        public const string WindowTitle = "FoV Control";

        public static SettingsData Data       { get; set; }
        public static SettingsData LoadedData { get; private set; }

        static Settings()
        {
            Data       = new SettingsData();
            LoadedData = new SettingsData();

            Reset();

            if (EditorPrefs.HasKey(EditorPrefsKey)) {
                LoadedData = Load(EditorPrefsKey);
                Data       = LoadedData.Clone();
            }
        }

        private static void Reset() => Data.Reset();

        public static void Save()
        {
            Store(EditorPrefsKey, Data);
            LoadedData = Data.Clone();
        }

        private static SettingsData Load(string key)
        {
            string jsonString = EditorPrefs.GetString(key);
            return JsonUtility.FromJson<SettingsData>(jsonString);
        }

        private static void Store(string key, SettingsData settingsData)
        {
            string jsonString = JsonUtility.ToJson(settingsData);
            EditorPrefs.SetString(key, jsonString);
        }
    }

    [Serializable]
    public class SettingsData {
        public string         versionString;
        public EventModifiers modifiersNormal;
        public EventModifiers modifiersQuick;
        public KeyCode        keyCodeIncreaseFov;
        public KeyCode        keyCodeDecreaseFov;
        public float          fovSpeed;
        public float          fovQuickMultiplier;
        public float          minFov;
        public float          maxFov;

        public SettingsData Clone() => (SettingsData)MemberwiseClone();

        public void Reset()
        {
            versionString   = Settings.VersionString;
            modifiersNormal = EventModifiers.Alt | EventModifiers.Control;
            modifiersQuick  = EventModifiers.Alt | EventModifiers.Control | EventModifiers.Shift;

            keyCodeIncreaseFov = KeyCode.O;
            keyCodeDecreaseFov = KeyCode.P;

            fovSpeed           = 0.15f;
            fovQuickMultiplier = 5.0f;
            minFov             = 2.0f;
            maxFov             = 160.0f;
        }
    }
}
