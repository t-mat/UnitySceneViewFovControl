using UnityEngine;
using UnityEditor;
using System;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditorExtension.SceneViewFovControl {

[InitializeOnLoad]
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

        if(d.MaxFov < d.MinFov) {
            d.MinFov = d.MaxFov;
        }

        GUILayout.Space(20);

        UnityEditor.EditorGUILayout.BeginHorizontal();
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
        UnityEditor.EditorGUILayout.EndHorizontal();
    }
}

} // namespace
