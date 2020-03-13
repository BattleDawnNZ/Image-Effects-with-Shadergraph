using System;
using UnityEngine;

namespace TextureArrayInspector {

	public class TextureArrayDecorator 
	{
		#if UNITY_EDITOR
		private struct SrcLayer
		{
			
			[NonSerialized] public Texture2D source;
			[NonSerialized] public Texture2D alpha;
			[NonSerialized] public Texture2D preview;
		}

		private SrcLayer[] srcArr;
		#endif

		public Texture2DArray texArr;
		

		public TextureArrayDecorator (Texture2DArray texArr)
		{ 
			this.texArr = texArr;
			
			//srcArr = new SrcLayer[texArr.depth]; //done in load sources
			LoadSources();
		}

		#region Array Tools

		public void SetSource (Texture2D tex, int ch, bool isAlpha = false, bool saveSources=true)
		{
			#if UNITY_EDITOR
			if (!isAlpha) srcArr[ch].source = tex;
			else srcArr[ch].alpha = tex;

			ApplySource(ch);
			
			if (saveSources) SaveSources();
			#endif
		}

		public void FindAndSetSource (Texture2D tex, string texPath, bool isAlpha=false)
		/// Finds texture in all of the source layers (and alphas) and forces it to assign to texArr. Useful to apply texture changes
		{
			#if UNITY_EDITOR
			//comparing using path (since TextureImported tex is different from original one)
			//string texPath = UnityEditor.AssetDatabase.GetAssetPath(tex);

			//TODO: compare tex directly
			for (int i=0; i<srcArr.Length; i++)
			{
				Texture2D lay = isAlpha ? srcArr[i].alpha : srcArr[i].source;

				//comparing using path (since TextureImported tex is different from original one)
				string layPath = UnityEditor.AssetDatabase.GetAssetPath(lay);
				if (layPath != texPath) continue;
				//if (tex != lay) continue;

				SetSource(tex, i, isAlpha, saveSources:false);
			}

			SaveSources();
			#endif
		}

		public Texture2D GetSource (int ch, bool isAlpha = false)
		{
			#if UNITY_EDITOR
			if (isAlpha) return srcArr[ch].alpha;
			else return srcArr[ch].source;
			#else
			return null;
			#endif
		}

		public Texture2D GetPreview (int ch)
		{
			#if UNITY_EDITOR
			if (srcArr[ch].preview == null)
				srcArr[ch].preview = texArr.GetTexture(ch);

			return srcArr[ch].preview;
			#else
			return null;
			#endif
		}


		public void ApplySource (int ch)
		{
			#if UNITY_EDITOR
			if (srcArr[ch].alpha == null) 
			{
				if (srcArr[ch].source != null) texArr.SetTexture(srcArr[ch].source, ch, apply:true);
			}
			else 
			{
				if (srcArr[ch].source != null) texArr.SetTextureAlpha(srcArr[ch].source, srcArr[ch].alpha, ch, apply:true);
			}

			srcArr[ch].preview = null; //resetting preview not keeping it. It will be generated on next get
			#endif
		}


		public void ApplyAllSources ()
		{
			#if UNITY_EDITOR
			for (int i=0; i<srcArr.Length; i++) 
				ApplySource(i);
			#endif
		}


		public void Add (Texture2D tex, Texture2D al)
		{
			#if UNITY_EDITOR
			ArrayTools.Add(ref srcArr, new SrcLayer());
			TextureArrayTools.Add(ref texArr, new Texture2D(texArr.width, texArr.height));

			if (tex!=null) SetSource(tex, srcArr.Length-1, isAlpha:false, saveSources:false);
			if (al!=null) SetSource(al, srcArr.Length-1, isAlpha:true, saveSources:false);

			SaveSources();
			#endif
		}


		public void Insert (Texture2D tex, Texture2D al, int index)
		{
			#if UNITY_EDITOR
			ArrayTools.Insert(ref srcArr, index, new SrcLayer());
			TextureArrayTools.Insert(ref texArr, index, new Texture2D(texArr.width, texArr.height));

			if (tex!=null) SetSource(tex, index, isAlpha:false, saveSources:false);
			if (al!=null) SetSource(al, index, isAlpha:true, saveSources:false);

			SaveSources();
			#endif
		}


		public void Switch (int ch1, int ch2)
		{
			#if UNITY_EDITOR
			ArrayTools.Switch(srcArr, ch1, ch2);
			TextureArrayTools.Switch(ref texArr, ch1, ch2);

			SaveSources();
			#endif
		}


		public void RemoveAt (int index)
		{
			#if UNITY_EDITOR
			ArrayTools.RemoveAt(ref srcArr, index);
			TextureArrayTools.RemoveAt(ref texArr, index);

			SaveSources();
			#endif
		}

		#endregion


		#region Tex Arr Wrapper

		public void Resize (int width, int height)
		{
			Texture2DArray prevTexArr = texArr;

			texArr = texArr.ResizedClone(width, height);
			ApplyAllSources();

			Rewrite(prevTexArr, texArr);
		}

		public void Format (TextureFormat format)
		{
			Texture2DArray prevTexArr = texArr;

			texArr = texArr.FormattedClone(format);
			ApplyAllSources();

			Rewrite(prevTexArr, texArr);
		}

		public void ReadWrite (bool readWrite)
		{
			if (readWrite) Format(texArr.format);
			else texArr.Apply(updateMipmaps:false, makeNoLongerReadable:true);
		}

		public void Linear (bool linear)
		{
			Texture2DArray prevTexArr = texArr;

			texArr = texArr.LinearClone(linear);
			ApplyAllSources();

			Rewrite(prevTexArr, texArr);
		}
		 
		
		public void Rewrite (Texture2DArray oldArr, Texture2DArray newArr)
		/// Copy newArr contents to oldArr
		{
			#if UNITY_EDITOR
			bool isSelected = UnityEditor.Selection.activeGameObject == oldArr;

			//string userData = texImporter.userData; //do not owerwrite userdata
			UnityEditor.EditorUtility.CopySerialized(newArr, oldArr);
			UnityEditor.EditorUtility.SetDirty(oldArr);   
			UnityEditor.AssetDatabase.SaveAssets(); 

			if (isSelected) UnityEditor.Selection.activeObject = oldArr;
			#endif
		}

		#endregion


		#region Save / Load sources

		public void LoadSources ()
		{
			#if UNITY_EDITOR
			if (!UnityEditor.AssetDatabase.Contains(texArr)) 
				{ Debug.Log("Texture Array is not an asset: could not get source tex"); return; }

			string[] sourceGuids = texArr.GetUserData("TexArr_sourceLayers");
			string[] alphaGuids = texArr.GetUserData("TexArr_alphaLayers");

			if (sourceGuids.Length != texArr.depth || alphaGuids.Length != texArr.depth)
			{
				//throw new System.Exception("Source GUIDs array length is not equal to TexArr depth");
				sourceGuids = new string[texArr.depth];
				alphaGuids = new string[texArr.depth];
			}

			srcArr = new SrcLayer[texArr.depth];

			for (int i=0; i<sourceGuids.Length; i++)
			{
				if (sourceGuids[i]==null || sourceGuids[i].Length==0) continue;
					srcArr[i].source = sourceGuids[i].GUIDtoObj<Texture2D>();
				
				if (alphaGuids[i]==null || alphaGuids[i].Length==0) continue;
					srcArr[i].alpha = alphaGuids[i].GUIDtoObj<Texture2D>();
			}
			#endif	
		}

		public void SaveSources ()
		{
			#if UNITY_EDITOR
			//checking if user data could be saved
			if (!UnityEditor.AssetDatabase.Contains(texArr)) 
				{ Debug.Log("Texture Array is not an asset: could not set source tex"); return; }

			string arrGuid = texArr.GUID();

			string[] newSrcGuids = new string[srcArr.Length];
			string[] newAlGuids = new string[srcArr.Length];

			for (int i=0; i<newSrcGuids.Length; i++)
			{
				if (srcArr[i].source != null) newSrcGuids[i] = srcArr[i].source.GUID();
				if (srcArr[i].alpha != null) newAlGuids[i] = srcArr[i].alpha.GUID();
			}

			string[] oldSrcGuids = texArr.GetUserData("TexArr_sourceLayers");
			string[] oldAlGuids = texArr.GetUserData("TexArr_alphaLayers");
			

			//unlinking sources that are not used anymore
			for (int i=0; i<oldSrcGuids.Length; i++)
			{
				if (oldSrcGuids[i]==null || oldSrcGuids[i].Length==0) continue;
				if (newSrcGuids.Contains(oldSrcGuids[i])) continue;  //still used

				UnlinkTexture(oldSrcGuids[i], arrGuid, isAlpha:false);
			}

			for (int i=0; i<oldAlGuids.Length; i++)
			{
				if (oldAlGuids[i]==null || oldAlGuids[i].Length==0) continue;
				if (newAlGuids.Contains(oldAlGuids[i])) continue;  //still used

				UnlinkTexture(oldSrcGuids[i], arrGuid, isAlpha:true);
			}


			//linking new textures
			for (int i=0; i<newSrcGuids.Length; i++)
			{
				if (newSrcGuids[i]==null || newSrcGuids[i].Length==0) continue;
				LinkTexture(newSrcGuids[i], arrGuid, isAlpha:false);
			}

			for (int i=0; i<newAlGuids.Length; i++)
			{
				if (newAlGuids[i]==null || newAlGuids[i].Length==0) continue;
				LinkTexture(newSrcGuids[i], arrGuid, isAlpha:true);
			}


			//saving array data
			texArr.SetUserData("TexArr_sourceLayers", newSrcGuids);
			texArr.SetUserData("TexArr_alphaLayers", newAlGuids);

			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.EditorUtility.SetDirty(texArr);  
			UnityEditor.AssetDatabase.Refresh();
			#endif
		}

		public static void UnlinkTexture (string texGuid, string arrGuid, bool isAlpha=false)
		{
			#if UNITY_EDITOR

			string userDataTag =  "TexArr_textureArray_as" + (isAlpha? "Source" : "Alpha");
			UnityEditor.AssetImporter importer = AssetsExtensions.GetImporter(texGuid);

			if (importer == null)
			{
				Debug.Log("Could not get importer for " + UnityEditor.AssetDatabase.GUIDToAssetPath(texGuid) + " to unassign texture");
				return;
			}

			string[] prevTexData = AssetsExtensions.GetUserData(importer, userDataTag);
				if (prevTexData==null || prevTexData.Length==0) 
				{ 
					//Debug.Log("Previous texture has no data assigned"); 
					return; 
				}
			ArrayTools.RemoveAll(ref prevTexData, arrGuid);
			AssetsExtensions.SetUserData(importer, userDataTag, prevTexData, reload:false);

			#endif
		}

		public static void LinkTexture (string texGuid, string arrGuid, bool isAlpha=false)
		{
			#if UNITY_EDITOR

			string userDataTag =  "TexArr_textureArray_as" + (isAlpha? "Source" : "Alpha");
			UnityEditor.AssetImporter importer = AssetsExtensions.GetImporter(texGuid);

			if (importer == null)
			{
				Debug.Log("Could not get importer for " + UnityEditor.AssetDatabase.GUIDToAssetPath(texGuid) + " to assign texture");
				return;
			}

			string[] texData = AssetsExtensions.GetUserData(importer, userDataTag);
			if (!texData.Contains(arrGuid)) //not already signed as used
			{
				ArrayTools.Add(ref texData, arrGuid);			
				AssetsExtensions.SetUserData(texGuid, userDataTag, texData, reload:false);
			}
			
			//writing hash for the newly assigned textures
			string path = UnityEditor.AssetDatabase.GUIDToAssetPath(texGuid);
			Texture2D tex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path);
			string texHash = tex.GetHash().ToString(); //tex.imageContentsHash.ToString();
			AssetsExtensions.SetUserData(importer, "Hash", new string[]{texHash}, reload:false); //will write only if hash changed

			#endif
		}

		#endregion
	}

}