using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TextureArrayInspector
{

	class TextureArrayInspectorPostprocessor : AssetPostprocessor 
	{
		//public void OnPostprocessTexture(Texture2D tex)  //using OnPostprocessAllAssets because tex here is not the same tex as the changed one

		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			for (int a=0; a<importedAssets.Length; a++)
			{
				if (AssetDatabase.GetMainAssetTypeAtPath(importedAssets[a]) != typeof(Texture2D)) continue;
				Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(importedAssets[a]);
				ReassignTexture(tex,importedAssets[a]);
			}
		}

		static void ReassignTexture (Texture2D tex, string assetPath)
		{
			HashSet<Texture2DArray> usedTexArrs = new HashSet<Texture2DArray>();

			//finding if it was a real change or just user data 
			string[] prevHashArr = tex.GetUserData("Hash");
			if (prevHashArr.Length != 0)
			{
				string prevHash = prevHashArr[0];
				string newHash = tex.GetHash().ToString(); //tex.imageContentsHash.ToString();

				if (prevHash == newHash) 
				{ 
					//Debug.Log("Skipping " + System.IO.Path.GetFileName(assetPath)); 
					return; 
				}
				else AssetsExtensions.SetUserData(tex, "Hash", new string[]{ newHash }, reload:false);  //will be reloaded on App
			}

			//sources
			string[] texarrGuids = tex.GetUserData("TexArr_textureArray_asSource");
			for (int t=0; t<texarrGuids.Length; t++)
			{
				Texture2DArray texArr = texarrGuids[t].GUIDtoObj<Texture2DArray>();
				if (!usedTexArrs.Contains(texArr)) usedTexArrs.Add(texArr);
				TextureArrayDecorator texArrDec = new TextureArrayDecorator(texArr);
				
				Debug.Log("Refreshing " + texArr + " since texture " + System.IO.Path.GetFileName(assetPath) + " is used as it's source", texArr);

				texArrDec.FindAndSetSource(tex, assetPath, isAlpha:false);
			}

			//alphas 
			texarrGuids = tex.GetUserData("TexArr_textureArray_asAlpha");
			for (int t=0; t<texarrGuids.Length; t++)
			{
				Texture2DArray texArr = texarrGuids[t].GUIDtoObj<Texture2DArray>();
				if (!usedTexArrs.Contains(texArr)) usedTexArrs.Add(texArr);
				TextureArrayDecorator texArrDec = new TextureArrayDecorator(texArr);
				
				Debug.Log("Refreshing " + texArr + " since texture " + System.IO.Path.GetFileName(assetPath) + " is used as it's source", texArr);

				texArrDec.FindAndSetSource(tex, assetPath, isAlpha:true);
			}

			//refresh preview
			TextureArrayInspector[] editors = Resources.FindObjectsOfTypeAll<TextureArrayInspector>();
			for (int i=0; i<editors.Length; i++)
			{
				Texture2DArray editorTexArr = editors[i].texArrDec.texArr;
				if (usedTexArrs.Contains(editorTexArr)) editors[i].OnEnable();
			}
		}

	}

}