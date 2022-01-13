# note for releasing

- Change a version number in Assets/SceneViewFovControl/Editor/Settings.cs

```C#
        namespace UTJ.UnityEditor.Extension.SceneViewFovControl {
            static class Settings {
                ...
>               public const string VersionString = "0.1.x";
                ...
```

- Push changes to the repository at GitHub.

- Create a `.unitypackage`
  - Select "Assets > Make SceneViewFovControl.unitypackage"
  - `SceneViewFovControl.unitypackage` will be put in root directory of the Unity Project.

- Upload `.unitypackage` via "Draft Release" page at GitHub.
  - Also add tag in the "Draft Release" page

- Change release URL in README.md, README.en.md and README.ja.md

```Markdown
        ## How to use

>       - Import [this .unitypackage](https://github.com/t-mat/UnitySceneViewFovControl/releases/download/0.1.6/SceneViewFovControl.unitypackage) to your Unity project.
```

```Markdown
        ## 使い方

>       - [この .unitypackage](https://github.com/t-mat/UnitySceneViewFovControl/releases/download/0.1.6/SceneViewFovControl.unitypackage) を Unity プロジェクトにインポートします
```

- Push changes to thre repository at GitHub again.
