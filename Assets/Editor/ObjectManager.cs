using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ObjectManager
{
    private static List<Object> _assets;

    public static List<Object> Assets { get => _assets; set => _assets = value; }

    public static void FillAssetList()
    {
        string[] assetPaths = AssetDatabase.GetAllAssetPaths();
        _assets = new List<Object>();

        foreach (string path in assetPaths)
        {
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (asset != null)
            {
                _assets.Add(asset);
            }
        }

        Debug.Log("Asset list filled\n" + $"Total assets collected = {_assets.Count}");
    }
}
