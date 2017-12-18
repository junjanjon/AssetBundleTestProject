using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetBundleLoadTester
{
public class GuiAssetBundleLoadTest : MonoBehaviour
{
    private readonly Dictionary<string, AssetBundle> _loadedAssetBundles = new Dictionary<string, AssetBundle>();
    private readonly Dictionary<string, Object> _loadedAssetDictionary = new Dictionary<string, Object>();
    private List<Action<Object>> _loadAssetActions;

    private string _targetDirectoryPath;
    private List<string> _assetBundleFilePathList;
    private AssetBundleManifest _assetBundleManifest;
    private LoadedAssetListHolder _loadedAssetList;

    private Vector2 _scrollPosition = Vector2.zero;

    void Start()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        // Android 実機ビルドでは StreamingAssets の処理に制限があるため、
        // Android 向けのアセットバンドルが StreamingAssets 以下にあることを前提にしています.
        _targetDirectoryPath = Application.streamingAssetsPath;
        Debug.LogFormat("Target Directory Path : {0}", _targetDirectoryPath);
        _assetBundleManifest = AssetBundle.LoadFromFile(Path.Combine(_targetDirectoryPath, "Android")).LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        _assetBundleFilePathList = _assetBundleManifest
                                   .GetAllAssetBundles()
                                   .Select(name => Path.Combine(_targetDirectoryPath, name))
                                   .ToList();
#else
        _targetDirectoryPath = Application.streamingAssetsPath;
        Debug.LogFormat("Target Directory Path : {0}", _targetDirectoryPath);
        _assetBundleFilePathList = Directory.GetFiles(_targetDirectoryPath)
                                   .Where(path => !path.EndsWith(".meta"))
                                   .Where(path => !path.EndsWith(".manifest"))
                                   .ToList();
#endif

        _loadedAssetList = gameObject.AddComponent<LoadedAssetListHolder>();
        _loadAssetActions = LoadAssetActions.CreateLoadAssetActions();
    }

    void Update()
    {
        _loadedAssetList.LoadedAssetList = _loadedAssetDictionary.Values.ToList();
    }

    void OnGUI()
    {
#if UNITY_EDITOR
        GUI.skin.button.fontSize = 30;
#endif
        int y = 0;
        int height = Screen.height / 8;

        _scrollPosition = GUI.BeginScrollView(
                              new Rect(0, 0, Screen.width, Screen.height),
                              _scrollPosition,
                              new Rect(0, 0, Screen.width, 10000));

        if (_assetBundleFilePathList != null)
        {
            foreach (var targetFilePath in _assetBundleFilePathList)
            {
                y += CreateButton(targetFilePath, targetFilePath.Replace(_targetDirectoryPath, ""), y, height);
            }
        }

        GUI.EndScrollView();
    }

    private int CreateButton(string targetFullPath, string targetSimplePath, int y, int height)
    {
        if (!_loadedAssetBundles.ContainsKey(targetFullPath))
        {
            GUI.color = Color.white;

            if (GUI.Button(CreateRect(y, height, ButtonType.Left), "Load : " + Environment.NewLine + targetSimplePath))
            {
                StartCoroutine(LoadAssetBundleOperation(targetFullPath));
            }

            return 1;
        }
        else
        {
            GUI.color = Color.red;

            if (GUI.Button(CreateRect(y, height, ButtonType.Left), "Unload : " + Environment.NewLine + targetSimplePath))
            {
                _loadedAssetBundles[targetFullPath].Unload(true);
                _loadedAssetBundles.Remove(targetFullPath);
                return 1;
            }

            GUI.color = Color.cyan;

            if (GUI.Button(CreateRect(y + 1, height, ButtonType.Left), "LoadAllAssets : " + Environment.NewLine + targetSimplePath))
            {
                StartCoroutine(LoadAllAssetsOperation(targetFullPath));
            }

            GUI.color = Color.white;
            var assetNames = _loadedAssetBundles[targetFullPath].GetAllAssetNames();

            foreach (var assetName in assetNames)
            {
                string text = "LoadAssetWithSubAssets : " + Environment.NewLine + NewLineAtCount(assetName, 30);

                if (GUI.Button(CreateRect(y, height, ButtonType.Right), text))
                {
                    StartCoroutine(LoadAssetOperation(targetFullPath, assetName));
                }

                y++;
            }

            return Math.Max(assetNames.Length, 3);
        }
    }

    private IEnumerator LoadAssetBundleOperation(string targetFullPath)
    {
        if (_loadedAssetBundles.ContainsKey(targetFullPath))
        {
            yield break;
        }

#if !UNITY_EDITOR && UNITY_ANDROID
        WWW www = new WWW(targetFullPath);
        yield return www;
        byte[] rawData = www.bytes;
#else
        byte[] rawData = File.ReadAllBytes(targetFullPath);
#endif
        AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(rawData);
        yield return assetBundleCreateRequest;

        AssetBundle assetBundle = assetBundleCreateRequest.assetBundle;
        _loadedAssetBundles[targetFullPath] = assetBundle;
    }

    private IEnumerator LoadAssetOperation(string targetFullPath, string assetName)
    {
        AssetBundle assetBundle = _loadedAssetBundles[targetFullPath];

        yield return WaitByDependency(assetBundle);

        AssetBundleRequest assetBundleRequest = assetBundle.LoadAssetWithSubAssetsAsync(assetName);
        yield return assetBundleRequest;

        foreach (var asset in assetBundleRequest.allAssets)
        {
            Debug.LogFormat("Load Asset {0} : {1} From {2}", asset.name, asset.GetType(), targetFullPath);
            _loadedAssetDictionary[asset.name] = asset;

            foreach (var loadAssetAction in _loadAssetActions)
            {
                loadAssetAction(asset);
            }

            {
                var manifest = asset as AssetBundleManifest;

                if (manifest != null)
                {
                    Debug.LogFormat("AssetBundleManifest を設定しました. 依存関係を考慮するようにします.");
                    _assetBundleManifest = manifest;
                }
            }
        }
    }

    private IEnumerator LoadAllAssetsOperation(string targetFullPath)
    {
        AssetBundle assetBundle = _loadedAssetBundles[targetFullPath];

        yield return WaitByDependency(assetBundle);

        AssetBundleRequest assetBundleRequest = assetBundle.LoadAllAssetsAsync();
        yield return assetBundleRequest;

        foreach (var asset in assetBundleRequest.allAssets)
        {
            Debug.LogFormat("Load {0} : {1} From {2}", asset.name, asset.GetType(), targetFullPath);
            _loadedAssetDictionary[asset.name] = asset;

            foreach (var loadAssetAction in _loadAssetActions)
            {
                loadAssetAction(asset);
            }
        }
    }

    /// <summary>
    /// 依存先のアセットバンドルがロードされるのを待つ.
    /// </summary>
    private IEnumerator WaitByDependency(AssetBundle assetBundle)
    {
        if (_assetBundleManifest != null)
        {
            while (true)
            {
                string[] waitDependencies = _assetBundleManifest.GetAllDependencies(assetBundle.name).Where(dependency =>
                {
                    return !_loadedAssetBundles.Any(loaded => string.Equals(loaded.Value.name, dependency));
                }).ToArray();

                if (waitDependencies.Length != 0)
                {
                    Debug.LogFormat("依存先アセットバンドルのロード待ち: {0}", string.Join(",", waitDependencies));
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    break;
                }
            }
        }
    }

    private enum ButtonType
    {
        Left,
        Right,
    }

    private static Rect CreateRect(int y, int height, ButtonType type)
    {
        if (type.Equals(ButtonType.Left))
        {
            return new Rect(0, y * height, Screen.width * 0.5f, height);
        }

        return new Rect(Screen.width * 0.5f, y * height, Screen.width * 0.5f, height);
    }

    private static string NewLineAtCount(string self, int count)
    {
        var result = new List<string>();
        var length = ( int )Math.Ceiling( ( double )self.Length / count );

        for ( int i = 0; i < length; i++ )
        {
            int start = count * i;

            if ( self.Length <= start )
            {
                break;
            }

            if ( self.Length < start + count )
            {
                result.Add( self.Substring( start ) );
            }
            else
            {
                result.Add( self.Substring( start, count ) );
            }
        }

        return string.Join(Environment.NewLine, result.ToArray());
    }
}
}
