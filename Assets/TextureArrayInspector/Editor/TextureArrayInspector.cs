using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Reflection;
using System;

namespace TextureArrayInspector 
{
	//[System.Serializable]
	[CustomEditor(typeof(Texture2DArray))]
	public class TextureArrayInspector : UnityEditor.Editor, Layout.ILayered
	{
		public static readonly int version = 130;

		public Layout layout;
		public TextureArrayDecorator texArrDec;
		public bool guiAbout;
		public bool guiVoxeland;
		public bool guiMapMagic;

		public const int iconTextureSize = 48;
		public const int iconDisplaySize = 24;

		public int previewMode = 0;


		public void OnEnable () 
		{
			Texture2DArray texArr = (Texture2DArray)target;
			texArrDec = new TextureArrayDecorator(texArr);
			Repaint();
		}

		public override void  OnInspectorGUI ()
		{
			if (texArrDec==null) OnEnable();

			if (layout == null) layout = new Layout();
			layout.margin = 0;
			layout.field = Layout.GetInspectorRect();
			layout.cursor = new Rect();
			//layout.undoObject = texArr;
			layout.undoName =  "TextureArrayManager change";

			#if VOXELAND
			layout.Par(18);
			layout.Label("Texture Array Inspector: a part of Voxeland tool", rect:layout.Inset(), helpbox:true);
			layout.Par(5);
			#endif

			//size
			layout.Par();
			layout.Label("Size", rect:layout.Inset(0.4f));
			int width = layout.Field(texArrDec.texArr.width, rect:layout.Inset(0.3f), delayed:true);
			int height = layout.Field(texArrDec.texArr.height, rect:layout.Inset(0.3f), delayed:true);
			if (width != texArrDec.texArr.width || height != texArrDec.texArr.height) 
				texArrDec.Resize(width, height);

			//format
			layout.fieldSize = 0.6f;
			TextureFormat format = layout.Field(texArrDec.texArr.format, "Format");
			if (format != texArrDec.texArr.format)
				texArrDec.Format(format);

			layout.Par(); layout.Inset(1f-layout.fieldSize);
			Rect buttonRect = layout.Inset(layout.fieldSize);
			if (layout.Button("Auto Compressed", rect:buttonRect))
			{
				TexureTypeWindow window = EditorWindow.CreateInstance<TexureTypeWindow>();
				window.inspector = this; window.compresed = true;
				window.ShowUtility();
				window.position = new Rect(Screen.currentResolution.width/2-100, Screen.currentResolution.height/2-75, 200, 140);
			}
			
			layout.Par(); layout.Inset(1f-layout.fieldSize);
			buttonRect = layout.Inset(layout.fieldSize);
			if (layout.Button("Auto Non-compressed", rect:buttonRect))
			{
				TexureTypeWindow window = EditorWindow.CreateInstance<TexureTypeWindow>();
				window.inspector = this; window.compresed = false;
				window.ShowUtility();
				window.position = new Rect(Screen.currentResolution.width/2-100, Screen.currentResolution.height/2-75, 200, 140);
			}

			//readable
			bool writable = texArrDec.texArr.IsReadWrite();
			layout.Field(ref writable, "Read/write enabled");
			if (layout.lastChange)
				texArrDec.ReadWrite(writable);

			//linear
			bool linear = ! layout.Field(!texArrDec.texArr.IsLinear(), "sRGB (Color Texture)");
			if (layout.lastChange)
				texArrDec.Linear(linear);

			//other
			TextureWrapMode wrapMode = layout.Field(texArrDec.texArr.wrapMode, "Wrap Mode");
			if (layout.lastChange) texArrDec.texArr.wrapMode = wrapMode;
			FilterMode filterMode = layout.Field(texArrDec.texArr.filterMode, "Filter Mode");
			if (layout.lastChange) texArrDec.texArr.filterMode = filterMode;
			int anisoLevel = layout.Field(texArrDec.texArr.anisoLevel, "Aniso Level", slider:true, min:0, max:16);
			if (layout.lastChange) texArrDec.texArr.anisoLevel = anisoLevel;
			float mipMapBias = layout.Field(texArrDec.texArr.mipMapBias, "MipMap Bias", slider:true, min:-1, max:1);
			if (layout.lastChange) texArrDec.texArr.mipMapBias = mipMapBias;

			//reload sources
			layout.Par(10);
			layout.Par(22);
			if (layout.Button("Reload From Sources", rect:layout.Inset()))
				texArrDec.ApplyAllSources();


			//drawing channels
			layout.Par(5);
			
			for (int i=0; i<texArrDec.texArr.depth; i++)
				layout.Layer(this, i);

			layout.Par(3); layout.Par();
			layout.LayerButtons(this, texArrDec.texArr.depth, rect:layout.Inset(0.6f));

			Layout.SetInspectorRect(layout.field);

			//about
			layout.Par(6); 
			layout.Foldout(ref guiAbout, "About", bold:false);
			if (guiAbout) 
			{
				Rect anchor = layout.lastRect;
				Rect savedCursor = layout.cursor;

				layout.margin = 5;
				layout.Par(60, padding:0);
				layout.Icon("LogoTAI", layout.Inset(60,padding:0));

				layout.cursor = savedCursor;
				layout.margin = 70;

				string versionName = version.ToString();
				versionName = versionName[0]+"."+versionName[1]+"."+versionName[2];
				layout.Label("Texture Array Inspector " + versionName);
				layout.Label("by Denis Pahunov");
				
				layout.Par(5);
				layout.Label("Documentation", url:"https://gitlab.com/denispahunov/shared/wikis/Texture-Array-Inspector");
				
				layout.rightMargin += 5;
				layout.Par(10);
				layout.Label("Other Tools:");
				layout.Foldout(ref guiVoxeland, "Voxeland", bold:true);
				if (guiVoxeland) 
				{
					Rect internalAnchor = layout.lastRect;

					layout.Par(83, padding:0);
					layout.Icon("VoxelandShowcase_00", layout.Inset(), horizontalAlign:Layout.IconAligment.center, verticalAlign:Layout.IconAligment.center, animationFrames:29);
					Repaint();
					layout.Label("Voxel Terrain Engine");
					layout.Label("A Smooth One");

					layout.Par(3);
					layout.Label("Asset Store", url:"http://bit.ly/Voxeland_TAI");
					layout.Label("Evaluation Version", url:"bit.ly/VoxelandEval_TAI");
					layout.Label("Documentation", url:"http://bit.ly/VoxelandDocs_TAI");
					layout.Label("Forums", url:"http://bit.ly/VoxelandForums_TAI");
					
					layout.Foreground(internalAnchor, layout.lastRect);
				}
				layout.Par(5);
				layout.Foldout(ref guiMapMagic, "MapMagic", bold:true);
				if (guiMapMagic) 
				{
					Rect internalAnchor = layout.lastRect;
					
					layout.Par(96, padding:0);
					layout.Icon("MapMagicShowcase_00", layout.Inset(), horizontalAlign:Layout.IconAligment.center, verticalAlign:Layout.IconAligment.center, animationFrames:29);
					Repaint();
					layout.Label("A node based procedural and infinite game map generator.");

					layout.Par(3);
					layout.Label("Asset Store", url:"http://bit.ly/MM_TAI");
					layout.Label("Evaluation Version", url:"bit.ly/MMeval_TAI");
					layout.Label("Documentation", url:"http://bit.ly/MMwiki_TAI");
					layout.Label("Forums", url:"http://bit.ly/MMforum_TAI");
					
					layout.Foreground(internalAnchor, layout.lastRect);
				}
				layout.rightMargin -= 5;
				layout.Par(5);
				layout.Inset();

				layout.Foreground(anchor, layout.lastRect);
			}
		}


		#region Layer Interface

			public void OnLayerHeader (Layout layout, int num)
			{
				layout.margin += 5; layout.rightMargin += 5;
				layout.Par(iconDisplaySize+6, padding:0);

				//drawing icon
				int iconWidth = (int)layout.cursor.height;
				Rect iconRect = layout.Inset(iconWidth);
				iconRect = iconRect.Extend(-3);

				layout.TextureIcon(texArrDec.GetPreview(num), iconRect);

				//label
				Rect labelRect = layout.Inset(layout.field.width - iconWidth - 18 - layout.margin-layout.rightMargin);
				labelRect.y += (labelRect.height-18)/2f; labelRect.height = 18;
				if (texArrDec.GetSource(num) != null) layout.Label(texArrDec.GetSource(num).name, labelRect);
				else 
				{
					layout.Label("Channel " + num, labelRect);
					labelRect.x += 70; labelRect.width-=70;
					layout.Label("(No Source)", labelRect, disabled:true);
				}

				layout.margin -= 5; layout.rightMargin -= 5;
			}

			public void OnLayerGUI (Layout layout, int num)
			{
				layout.margin += 5; layout.rightMargin += 5;

				Texture2D source = texArrDec.GetSource(num, isAlpha:false);
				Texture2D alpha = texArrDec.GetSource(num, isAlpha:true);
				Texture2D preview = texArrDec.GetPreview(num);

				//save warnings
				if(!AssetDatabase.Contains(texArrDec.texArr))
				{
					layout.Par(28); 
					layout.Label("Source objects could not be saved since texture array is not stored as an asset", rect:layout.Inset(), helpbox:true);
					layout.Par(5);
				}
				else
				{
					//source
					Texture2D newSrc = layout.Field(source, "Source", fieldSize:0.6f);
					if (source != newSrc) 
						texArrDec.SetSource(newSrc, num, isAlpha:false);

					//alpha
					Texture2D newAlf = layout.Field(alpha, "Alpha", fieldSize:0.6f, disabled:source==null);
					if (alpha != newAlf) 
						texArrDec.SetSource(newAlf, num, isAlpha:true);
				}
				
				//preview/extract
				layout.Par(5);

				layout.Par();
				layout.Label("Extract", rect:layout.Inset(0.4f));
				if (layout.Button("Save", rect:layout.Inset(0.6f)))
				{
					string savePath = EditorUtility.SaveFilePanel("Save Texture", "Assets", texArrDec.texArr.name+" layer "+num, "png");
					if (savePath != null) preview.SaveAsPNG(savePath);
				}

				layout.Par();
				layout.Inset(0.4f);
				if (layout.Button("Save Gamma", rect:layout.Inset(0.6f)))
				{
					string savePath = EditorUtility.SaveFilePanel("Save Texture", "Assets", texArrDec.texArr.name+" layer "+num, "png");
					if (savePath != null) preview.SaveAsPNG(savePath, linear:true);
				}

				layout.Par();
				layout.Inset(0.4f);
				if (layout.Button("Save as Normal Map", rect:layout.Inset(0.6f)))
				{
					string savePath = EditorUtility.SaveFilePanel("Save Texture", "Assets", texArrDec.texArr.name+" layer "+num, "png");
					if (savePath != null) preview.SaveAsPNG(savePath, normal:true);
				}

				//layout.Par(5);
				//layout.Par();
				//layout.Label("Preview", rect:layout.Inset(0.4f));
				//if (layout.Button(layer.preview==null? "Generate" : "Refresh", rect:layout.Inset(0.6f)))
				//	layer.preview = texArr.GetTexture(num);

				layout.Par( Mathf.Min(layout.field.width, preview.height));
				//layout.Texture(texArrDec.GetPreview(num), layout.Inset(), ref previewMode);
				layout.Texture(texArrDec.texArr.GetTexture(num), layout.Inset(), ref previewMode);

				//srcLayers[num] = layer;

				layout.Par(5);
				layout.margin -= 5; layout.rightMargin -= 5;
			}

			public void AddLayer (int index) 
			{
				texArrDec.Insert(null, null, index);
				AssetDatabase.Refresh(); //what for?
			}
			public void RemoveLayer (int index) 
			{  
				texArrDec.RemoveAt(index);
				AssetDatabase.Refresh();
			}
			public void SwitchLayers (int i1, int i2) 
			{ 
				texArrDec.Switch(i1, i2);
				AssetDatabase.Refresh();
			}

			public int selected { get; set; }
			public bool expanded { get; set; }




		#endregion

		#region TexureType Window

		class TexureTypeWindow : EditorWindow 
		{
			public TextureArrayInspector inspector;
			public bool compresed;

			public void OnGUI () 
			{
				titleContent = new GUIContent("Select Texture Type:"); 
				//position = new Rect(position.x, position.y, 50, 150);

				EditorGUILayout.BeginVertical();

				if (GUILayout.Button("RGBA")) { inspector.texArrDec.Format( TextureExtensions.AutoFormat(TextureExtensions.TextureType.RGBA, compresed)); Close();  }
				if (GUILayout.Button("RGB (no alpha)")) { inspector.texArrDec.Format( TextureExtensions.AutoFormat(TextureExtensions.TextureType.RGB, compresed)); Close();  }
				if (GUILayout.Button("Normap Map")) { inspector.texArrDec.Format( TextureExtensions.AutoFormat(TextureExtensions.TextureType.Normal, compresed)); Close();  }
				if (GUILayout.Button("Monochrome")) { inspector.texArrDec.Format( TextureExtensions.AutoFormat(TextureExtensions.TextureType.Monochrome, compresed)); Close();  }
				if (GUILayout.Button("Monochrome 32bit float")) { inspector.texArrDec.Format( TextureExtensions.AutoFormat(TextureExtensions.TextureType.MonochromeFloat, compresed)); Close();  }

				EditorGUILayout.Space();

				if (GUILayout.Button("Cancel")) Close();

				EditorGUILayout.EndVertical();
			}
		}

		#endregion

		#region TextureArray Factory

			[MenuItem("Assets/Create/Texture Array", priority = 202)]
			static void MenuCreatePostProcessingProfile(MenuCommand menuCommand)
			{
				var icon = EditorGUIUtility.FindTexture("Texture Icon"); //https://gist.github.com/MattRix/c1f7840ae2419d8eb2ec0695448d4321
				ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
					0, 
					ScriptableObject.CreateInstance<TmpCallbackReciever>(), 
					"TextureArray.asset", 
					icon, 
					null);
			}

			class TmpCallbackReciever : UnityEditor.ProjectWindowCallback.EndNameEditAction
			{
				public override void Action(int instanceId, string pathName, string resourceFile)
				{
					Texture2DArray ta = new Texture2DArray(128, 128, 1, TextureFormat.ARGB32, true);;
					ta.name = System.IO.Path.GetFileName(pathName);
					AssetDatabase.CreateAsset(ta, pathName);

					ProjectWindowUtil.ShowCreatedAsset(ta);
				} 
			}

		#endregion
	}



}
