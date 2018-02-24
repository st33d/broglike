using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MakeLib  {
	[MenuItem("Assets/Create/Prefab Library")]
	public static void CreateMyAsset() {
		Lib asset = ScriptableObject.CreateInstance<Lib>();
		AssetDatabase.CreateAsset(asset, "Assets/lib.asset");
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}
}