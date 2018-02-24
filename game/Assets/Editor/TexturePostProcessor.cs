using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

/* Some texture setting hacking - they change this every time they release a new Unity3D, sorry for the mess */
public class TexturePostProcessor : AssetPostprocessor {


	void OnPreprocessTexture(){
		Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D));
		if(asset)
			return; // set defaults for new textures only
		
		TextureImporter importer = assetImporter as TextureImporter;

		importer.textureType = TextureImporterType.Sprite;
		importer.spritePackingTag = "tile";
		importer.spritePixelsPerUnit = 1;
		importer.mipmapEnabled = false;
		importer.filterMode = FilterMode.Point;
		importer.textureCompression = TextureImporterCompression.Uncompressed;
		TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();

		defaultSettings.maxTextureSize = 2048;
		defaultSettings.compressionQuality = 0;
		defaultSettings.overridden = true;
		defaultSettings.crunchedCompression = false;
		defaultSettings.format = TextureImporterFormat.RGBA32;

		importer.SetPlatformTextureSettings(defaultSettings);



		// setting pivot based on texture size
		object[] args = new object[2] {0,0};
		MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
		mi.Invoke(importer, args);
		// set to center of top left corner tile
//		float step = 8;
//		int width = (int)args[0];
//		int height = (int)args[1];

		TextureImporterSettings settings = new TextureImporterSettings();
		importer.ReadTextureSettings(settings);
		settings.spriteAlignment = (int)SpriteAlignment.Center;
		settings.spriteMode = 1;
		settings.alphaIsTransparency = true;
		importer.SetTextureSettings(settings);
//		importer.spritePivot = new Vector2((1.0f / width) * step, 1.0f - ((1.0f / height) * step));
//		importer.spritePivot = new Vector2((1.0f / width) * step, 0.0f);


//		importer.spritePivot = new Vector2(0.25f, 0.25f);
//		Debug.Log(args[0]+" "+args[1]);
		/*
		*/

	}



}
