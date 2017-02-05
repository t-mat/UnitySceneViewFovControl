using UnityEngine;
using UnityEditor;

#if !UNITY_EDITOR
#error This script must be placed under "Editor/" directory.
#endif

namespace UTJ.UnityEditor.Extension.SceneViewFovControl {
    static class Settings {
        public static EventModifiers ModifiersNormal = EventModifiers.Alt | EventModifiers.Control;
        public static EventModifiers ModifiersQuick  = EventModifiers.Alt | EventModifiers.Control | EventModifiers.Shift;

        public static KeyCode KeyCodeIncreaseFov = KeyCode.O;
        public static KeyCode KeyCodeDecreaseFov = KeyCode.P;

        public static float FovSpeed = 0.5f * 0.3f;
        public static float FovQuickMultiplier = 5.0f;
        public static float MinFov = 2.0f;
        public static float MaxFov = 160.0f;
    }
}
