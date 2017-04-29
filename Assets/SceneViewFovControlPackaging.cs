#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace UTJ.UnityEditorExtension.SceneViewFovControl {

public static class Packaging {
	const string PackageName = "SceneViewFovControl.unitypackage";
	const string PackageDir = "Assets/SceneViewFovControl";

	[MenuItem("Assets/Make " + PackageName)]
	static void MakePackage() {
		string[] files = new string[] { PackageDir };
		AssetDatabase.ExportPackage(files, PackageName, ExportPackageOptions.Recurse);
	}
}

} // namespace
#endif
