# AssetBundleTestProject

アセットバンドルの中身を確認するための Unityプロジェクトです。

「ほかのUnityプロジェクトで生成したアセットバンドル」、「このUnityプロジェクトでアセットバンドルビルドを行い生成したアセットバンドル」のどちらも確認できます。

「ほかのUnityプロジェクトで生成したアセットバンドル」を確認する場合は、このプロジェクトのみで確認できます。Unity v5.5 以降の任意のバージョンの Unity で利用できます。

「このUnityプロジェクトでアセットバンドルビルドを行い生成したアセットバンドル」を確認する場合は、このプロジェクトに加えて [AssetBundles-Browser](https://github.com/Unity-Technologies/AssetBundles-Browser) を利用してください。Unity v5.6 以降の任意のバージョンの Unity で利用できます。

## 使い方

以下の3つで確認ができます。

- UnityEditor で確認する
- iOS 実機で確認する
- Android 実機で確認する

アセットバンドルはプラットフォーム特有の情報を含むため、UnityEditor だけでなく実機での動作確認もおすすめします。


### UnityEditor の場合

1. 任意のバージョンの Unity でプロジェクトを開く。
2. Assetsの直下に「StreamingAssets」というフォルダを置いて、その中に調査対象のアセットバンドルを配置する。
3. 「AssetBundleLoadTester/GuiAssetBundleLoadTestScene.unity」を開いて再生する。


### iOS の場合

プロジェクトのビルド設定はないため、通常通りのビルド設定を行いビルドできます。
「AssetBundleLoadTester/GuiAssetBundleLoadTestScene.unity」をScene in build に設定してください。

ビルド設定の「strip engine code」が有効になっている場合、注意すべき点があります。
詳しくは以下のリンク先を参照してください。
http://tsubakit1.hateblo.jp/entry/2015/12/16/233336#AssetBundleにのみ使われているクラスがある場合の注意

### Android の場合

プロジェクトのビルド設定はないため、通常通りのビルド設定を行いビルドできます。
「AssetBundleLoadTester/GuiAssetBundleLoadTestScene.unity」をScene in build に設定してください。

Android 実機ビルドでは StreamingAssets の処理に制限があるため、Android 向けのアセットバンドル（シングルマニフェストファイル含む）が StreamingAssets 以下にあることを前提にしています。
「Window/AssetBundle Browser」 を使う場合は、 Copy to StreamingAssets のフラグを有効にしてアセットバンドルビルドすれば前提を満たせます。


# ライセンス

このソフトウェアはMIT Licenseのもとでリリースされています。

