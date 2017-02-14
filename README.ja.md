[English](README.en.md)

# Scene View での FoV (画角) を操作する Unity Editor 拡張

|広角				|望遠 (狭角)				|
|--------------------		|-------------------------		|
|![WideFov](images/WideFov.png)	|![NarrowFov](images/NarrowFov.png)	|

Unity Editor のシーンビュー (Scene View) に対する不満の１つとして、 FoV (Field of View, 画角) が操作できないことがしばしば挙げられます。
この Unity Editor 拡張は、 Unity Editor の非公開 API を用いて、その不満の一部を解消するものです。


## 使い方

- [この .unitypackage](https://github.com/t-mat/UnitySceneViewFovControl/releases/download/0.1.4/SceneViewFovControl.unitypackage) を Unity プロジェクトにインポートします
    - ファイルからのインポートは "Assets > Import Package > Custom Package..." から行います
- インポート後、 シーンビュー (Scene View) 内で以下の操作が可能になります

|キー/マウス操作		|効果					|備考			|
|--------------------		|-------------------------		|----			|
|Ctrl + Alt + ホイール		|FoV (画角) の変更			|			|
|Ctrl + Alt + Shift + ホイール	|FoV (画角) の変更 (高速)		|Mac では使用不可	|
|Ctrl + Alt + O			|FoV (画角) の増加 (広角)		|			|
|Ctrl + Alt + P			|FoV (画角) の減少 (狭角 / 望遠)	|			|
|Ctrl + Alt + Shift + O		|FoV (画角) の増加 (高速、広角)		|			|
|Ctrl + Alt + Shift + P		|FoV (画角) の減少 (高速、狭角 / 望遠)	|			|

FoV を変更すると、シーンビューの左上に「リセット」ボタンが表示されます。
このボタンを押すと、シーンビューの FoV がリセットされます。また、このボタンは２秒間 FoV 操作を行わないと自動的に消えます。


## 設定の変更と保存

Unity Editor のメニュー "Edit > Scene View FoV Settings" を選択すると、設定ウィンドウが出ます

|項目				|意味					|
|--------------------		|-------------------------		|
|FoV Speed			|FoV 変更速度				|
|FoV <Shift> Modifier Multiplier|Shift 押下時の変更速度倍率		|
|Min FoV			|FoV 最小値 (角度単位)			|
|Max FoV			|FoV 最大値 (角度単位)			|
|				|					|
|<Reset Scene FoV>		|					|
|Button Showing Duration	|シーンビューへの「リセット」ボタンの表示時間。最小値で非表示、最大値で常時表示	|
|				|					|
|Disable			|FoV 変更の動作を停止する		|
|				|					|
|Save				|設定値の保存				|
|Restore Default Settings	|デフォルトの設定値読み込み		|
|Restore Saved Settings		|保存済みの設定値の読み直し		|
|Close				|ウィンドウを閉じる			|


## ライセンス

[MIT](LICENSE.txt)
