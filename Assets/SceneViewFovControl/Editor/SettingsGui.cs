using UnityEngine;
using UnityEditor;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditorExtension.SceneViewFovControl {

[InitializeOnLoad]
internal class SettingsGui : EditorWindow {
    private static SettingsGui _settingGui;

    [MenuItem(Settings.MenuItemName)]
    private static void Open() {
        if(_settingGui == null) {
            _settingGui = GetWindow<SettingsGui>(false, Settings.WindowTitle);
        }
    }

    private void OnGUI() {
        SettingsData d = Settings.Data;

        GUILayout.Space(4);

        GUILayout.Label("<<< Scene View FoV Control Settings - version " + Settings.VersionString + ">>>");

        GUILayout.Space(8);

        GUILayout.Label("FoV Speed:" + d.fovSpeed);
        d.fovSpeed = GUILayout.HorizontalSlider(d.fovSpeed, Settings.FovSpeedMin, Settings.FovSpeedMax);

        GUILayout.Space(8);

        GUILayout.Label("FoV <Shift> Modifier Multiplier:" + d.fovQuickMultiplier);
        d.fovQuickMultiplier = GUILayout.HorizontalSlider(d.fovQuickMultiplier, Settings.FovQuickMultiplierMin, Settings.FovQuickMultiplierMax);

        GUILayout.Space(8);

        GUILayout.Label("Min FoV:" + d.minFov);
        d.minFov = GUILayout.HorizontalSlider(d.minFov, Settings.MinFovMin, Settings.MinFovMax);

        if(d.minFov > d.maxFov) {
            d.maxFov = d.minFov;
        }

        GUILayout.Space(8);

        GUILayout.Label("Max FoV:" + d.maxFov);
        d.maxFov = GUILayout.HorizontalSlider(d.maxFov, Settings.MaxFovMin, Settings.MaxFovMax);

        if(d.maxFov < d.minFov) {
            d.minFov = d.maxFov;
        }

        GUILayout.Space(8);

        if(d.maxFov < d.minFov) {
            d.minFov = d.maxFov;
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
                Close();
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}

} // namespace
