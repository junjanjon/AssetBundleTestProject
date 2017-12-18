using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetBundleLoadTester
{
public static class LoadAssetActions
{

    public static List<Action<Object>> CreateLoadAssetActions()
    {
        List<Action<Object>> actions = new List<Action<Object>>();
        actions.Add(LoadPrefab);
        actions.Add(LoadTexture2D);
        actions.Add(LoadSprite);
        return actions;
    }

    private static void LoadPrefab (Object asset)
    {
        var prefab = asset as GameObject;

        if (prefab != null)
        {

            Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
    }

    private static void LoadTexture2D (Object asset)
    {
        var texture = asset as Texture2D;

        if (texture != null)
        {
            var gameObject = new GameObject("LoadTexture2D");
            var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }

    private static void LoadSprite (Object asset)
    {
        var sprite = asset as Sprite;

        if (sprite != null)
        {
            var gameObject = new GameObject("LoadSprite");
            var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
        }
    }


}
}
