using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TextureArrayInspector
{
	public partial class Layout
	{
		public struct Val 
		{
			public float val;
			public bool ovd;

			public static implicit operator Val(bool b) { return new Val() {val=b? 1 : 0, ovd=true}; }
			public static implicit operator Val(float f) { return new Val() {val=f, ovd=true}; }
			public static implicit operator Val(int i) { return new Val() {val=i, ovd=true}; }

			public static implicit operator bool(Val v) { if (v.val>0.5f) return true; else return false; }
			public static implicit operator float(Val v) { return v.val; }
			public static implicit operator int(Val v) { return (int)v.val; }

			//public static implicit operator Val(NumericType t) { return new Val() {val=(int)t, ovd=true}; }
			//public static implicit operator Val(LabelType t) { return new Val() {val=(int)t, ovd=true}; }
			//public static implicit operator Val(ToggleType t) { return new Val() {val=(int)t, ovd=true}; }

			public void Verify (float def) { if (!ovd) val = def; }
			public void Verify (int def) { if (!ovd) val = def; }
			public void Verify (bool def) { if (!ovd) val = def? 1 : 0; }
		}
		
		
		public Rect field; //background rect of layout. Height increases every new line (or stays fixed, which is max)
		public Rect cursor; //rect with zero width. X and Y are relative to field
		public Rect lastRect;

		public int margin = 10;
		public int rightMargin = 10;
		public int lineHeight = 18;
		public float bottomPoint = 0; //the lowest point set with Par

		public int verticalPadding = 2;
		public int horizontalPadding = 3;

		public UnityEngine.Object undoObject;
		public string undoName = "";

		
		//gets rect to draw in inspector
		public static Rect GetInspectorRect ()
		{
			#if UNITY_EDITOR
			UnityEditor.EditorGUI.indentLevel = 0;
			return UnityEditor.EditorGUILayout.GetControlRect(GUILayout.Height(0));
			//UnityEditor.EditorGUIUtility.currentViewWidth;
			//GUILayoutUtility.GetRect(1, 0);
			#else
			return new Rect();
			#endif
		}

		//sets field to inspector
		static public void SetInspectorRect (Rect rect)
		{
			if (Event.current.type == EventType.Layout) GUILayoutUtility.GetRect(1, rect.height, "TextField");
		}


		public void Par (
			Val height = new Val(),
			Val margin = new Val(),
			Val padding = new Val() )
		{ 
			//current params
			int _height = height.ovd? (int)height.val : this.lineHeight;
			int _margin = margin.ovd? (int)margin.val : this.margin;
			int _padding = padding.ovd? (int)padding.val : this.verticalPadding;

			//setting rects
			cursor = new Rect(field.x+_margin, cursor.y + cursor.height + _padding, 0, _height - _padding);
			field = new Rect(field.x, field.y, field.width, Mathf.Max(field.height, cursor.y+cursor.height));
			bottomPoint = Mathf.Max(bottomPoint, cursor.y+cursor.height);
		}

		public Rect Inset (
			Val width = new Val(), 
			Val margin = new Val(), 
			Val rightMargin = new Val(), 
			Val padding = new Val() )
		{
			//finding current params
			int _margin = margin.ovd? (int)margin.val : this.margin;
			int _rightMargin = rightMargin.ovd? (int)rightMargin.val : this.rightMargin;
			int _padding = 0;// padding.ovd? (int)padding.val : this.verticalPadding;
			float _width = width.ovd? width.val : 1; 
			if (_width < 1.0001f) _width *= field.width-_margin-_rightMargin; //width should be in pixels
			
			//setting rects
			cursor.x += _width;
			lastRect = new Rect (cursor.x-_width, cursor.y+field.y, _width-_padding, cursor.height);
			return lastRect;
		}

		public Rect ParInset (
			Val height = new Val(),
			Val width = new Val(), 
			Val margin = new Val(), 
			Val rightMargin = new Val(), 
			Val verticalPadding = new Val(),
			Val horizontalPadding = new Val() )
		{ 
			Par(height, margin, verticalPadding); 
			return Inset(width, margin, rightMargin, horizontalPadding); 
		}

		public int repaintsLeft = 0;
		public void Repaint (int numTimes)
		{
			//enqueuing repaint
			repaintsLeft = Mathf.Max(repaintsLeft, numTimes);

			//repainting now
			Repaint();
			repaintsLeft --;
		}
		public void Repaint ()
		{
			#if UNITY_EDITOR
			if (UnityEditor.EditorWindow.focusedWindow != null && repaintsLeft>0) UnityEditor.EditorWindow.focusedWindow.Repaint(); 
			#endif
		}


		#region Scroll/zoom

			public Vector2 scroll = new Vector2(0,0);
			public float zoom = 1;

			public float zoomStep = 0.0625f;
			public float minZoom=0.25f;
			public float maxZoom=2f;
			public int scrollButton = 2;
			

			public void Zoom ()
			{
				if (Event.current == null) return;

				//reading control
				#if UNITY_EDITOR_OSX
				bool control = Event.current.command;
				#else
				bool control = Event.current.control;
				#endif

				float delta = 0;
				if (Event.current.type==EventType.ScrollWheel) delta = Event.current.delta.y / 3f;
				else if (Event.current.type==EventType.MouseDrag && Event.current.button==0 && control)
					delta = Event.current.delta.y / 15f;
				//else if (control && Event.current.alt && Event.current.type==EventType.KeyDown && Event.current.keyCode==KeyCode.Equals) delta --;
				//else if (control && Event.current.alt && Event.current.type==EventType.KeyDown && Event.current.keyCode==KeyCode.Minus) delta ++;
				
				if (Mathf.Abs(delta)<0.001f) return;

				float zoomChange = - zoom*zoomStep*delta; //progressive step

				//returning if zoom will be out-of-range
				//if (zoom+zoomChange > maxZoom || zoom+zoomChange < minZoom) return;

				//clamping zoom change so it will never be out of range
				if (zoom+zoomChange > maxZoom) zoomChange = maxZoom-zoom;
				if (zoom+zoomChange < minZoom) zoomChange = minZoom-zoom;
			
				//record mouse position in worldspace
				Vector2 worldMousePos = (Event.current.mousePosition - scroll)/zoom;

				//changing zoom
				zoom += zoomChange;
			
				if (zoom >= minZoom && zoom <= maxZoom) scroll -= worldMousePos*zoomChange;
				//zoom = Mathf.Clamp(zoom, minZoom, maxZoom); //returning on out-of-range instead

				#if UNITY_EDITOR
				if (UnityEditor.EditorWindow.focusedWindow != null) UnityEditor.EditorWindow.focusedWindow.Repaint(); 
				#endif
			}

			public void Scroll ()
			{
				if (Event.current == null || Event.current.type!=EventType.MouseDrag) return;
				if (!(Event.current.button==scrollButton || (Event.current.button==0 && Event.current.alt))) return;			 
				
				scroll += Event.current.delta;
				 
				#if UNITY_EDITOR
				UnityEditor.EditorWindow.focusedWindow.Repaint(); 
				#endif
			}

			public void ScrollWheel (int step=3) 
			{
				float delta = 0;
				if (Event.current.type==EventType.ScrollWheel) delta = Event.current.delta.y / 3f;
				scroll.y -= delta*lineHeight*step;
			}


			public Rect ToDisplay (Rect rect)
				{ return new Rect(rect.x*zoom+scroll.x, rect.y*zoom+scroll.y, rect.width*zoom, rect.height*zoom ); }

			public Rect ToInternal (Rect rect)
				{ return new Rect( (rect.x-scroll.x)/zoom, (rect.y-scroll.y)/zoom, rect.width/zoom, rect.height/zoom ); }

			public Vector2 ToInternal (Vector2 pos) { return (pos-scroll)/zoom; } //return new Vector2( (pos.x-scroll.x)/zoom, (pos.y-scroll.y)/zoom); }
			public Vector2 ToDisplay (Vector2 pos) { return pos*zoom + scroll; }

			public void Focus (Vector2 pos)
			{
				pos *= zoom;
				scroll = -pos;
				scroll += new Vector2(field.width/2f, field.height/2f); //note that field size should be equal to window size (use layout.field = this.position)
			}
				

		#endregion

		#region Styles

			[System.NonSerialized] public GUIStyle labelStyle = null;
			[System.NonSerialized] public GUIStyle boldLabelStyle = null;
			[System.NonSerialized] public GUIStyle foldoutStyle = null;
			[System.NonSerialized] public GUIStyle fieldStyle = null;
			[System.NonSerialized] public GUIStyle dragFieldStyle = null;
			[System.NonSerialized] public GUIStyle buttonStyle = null;
			[System.NonSerialized] public GUIStyle enumZoomStyle = null;
			[System.NonSerialized] public GUIStyle urlStyle = null;
			[System.NonSerialized] public GUIStyle toolbarStyle = null;
			[System.NonSerialized] public GUIStyle toolbarButtonStyle = null;
			[System.NonSerialized] public GUIStyle helpBoxStyle = null;

			public void CheckStyles ()
			{
				#if UNITY_EDITOR
				if (labelStyle == null) 
				{
					labelStyle = new GUIStyle(UnityEditor.EditorStyles.label); labelStyle.active.textColor = Color.black; labelStyle.focused.textColor = labelStyle.active.textColor = labelStyle.normal.textColor;
					boldLabelStyle = new GUIStyle(UnityEditor.EditorStyles.label); boldLabelStyle.fontStyle = FontStyle.Bold; boldLabelStyle.focused.textColor = boldLabelStyle.active.textColor = boldLabelStyle.normal.textColor;
					urlStyle = new GUIStyle(UnityEditor.EditorStyles.label); urlStyle.normal.textColor = new Color(0.3f, 0.5f, 1f); 
					foldoutStyle = new GUIStyle(UnityEditor.EditorStyles.foldout);  foldoutStyle.fontStyle = FontStyle.Bold; foldoutStyle.focused.textColor = Color.black; foldoutStyle.active.textColor = Color.black; foldoutStyle.onActive.textColor = Color.black;
					
					buttonStyle = new GUIStyle("Button"); 
					enumZoomStyle = new GUIStyle(UnityEditor.EditorStyles.miniButton); enumZoomStyle.alignment = TextAnchor.MiddleLeft;
					toolbarStyle = new GUIStyle(UnityEditor.EditorStyles.toolbar);
					toolbarButtonStyle = new GUIStyle(UnityEditor.EditorStyles.toolbarButton);  
					helpBoxStyle = new GUIStyle(UnityEditor.EditorStyles.helpBox);  
				}

				if (fieldStyle == null)
				{
					fieldStyle = new GUIStyle(UnityEditor.EditorStyles.numberField);

					//20 skin
					fieldStyle.normal.background = GetIcon("DPLayout_Field"); //Resources.Load("MapMagic_Window") as Texture2D;
					fieldStyle.border = new RectOffset(4,4,4,4);
				}

				int fontSize = Mathf.RoundToInt(this.fontSize * zoom);
				if (labelStyle.fontSize != fontSize)
				{
					labelStyle.fontSize = fontSize;
					boldLabelStyle.fontSize = fontSize;
					urlStyle.fontSize = fontSize; 
					foldoutStyle.fontSize = fontSize;		
					
					buttonStyle.fontSize = fontSize;
					
					toolbarStyle.fontSize = fontSize;
					toolbarButtonStyle.fontSize = fontSize;
					
				}

				int fieldFontSize = Mathf.RoundToInt(14 * zoom * 0.8f);
				if (fieldStyle.fontSize != fieldFontSize)
				{
					fieldStyle.fontSize = fieldFontSize;
					enumZoomStyle.fontSize = fieldFontSize; 
				}

				int helpBoxFontSize = Mathf.RoundToInt(9*zoom*0.5f*(1+zoom));
				if (helpBoxStyle.fontSize != helpBoxFontSize) helpBoxStyle.fontSize = helpBoxFontSize;
				#endif
			}

		#endregion

		#region Icon

			[System.NonSerialized] Dictionary<string, Texture2D> icons = new Dictionary<string, Texture2D>();

			public Texture2D GetIcon (string textureName)
			{
				string nonProName = textureName;
				#if UNITY_EDITOR
				if (UnityEditor.EditorGUIUtility.isProSkin) textureName += "_pro";
				#endif
				
				Texture2D texture=null;
				if (!icons.ContainsKey(textureName))
				{
					texture = Resources.Load(textureName) as Texture2D;
					if (texture==null) texture = Resources.Load(nonProName) as Texture2D; //trying to load a texture without _pro

					icons.Add(textureName, texture);
				}
				else texture = icons[textureName]; 
				return texture;
			}

			public enum IconAligment { resize, min, max, center }

			public bool Icon (string textureName, Rect rect, IconAligment horizontalAlign=IconAligment.resize, IconAligment verticalAlign=IconAligment.resize, int animationFrames=0, bool frame=false, bool tile=false, bool clickable=false)
			{
				//drawing animation frames
				if (animationFrames != 0)
				{
					System.DateTime now = System.DateTime.Now;
					int frameNum = (int)((now.Second*5f + now.Millisecond*5f/1000f) % animationFrames);
					string frameString = (frameNum+1<10? "0" : "") + (frameNum+1).ToString();
					return Icon(textureName + frameString, rect, horizontalAlign, verticalAlign, 0, frame, tile, clickable);
				}
				
				//drawig texture
				return Icon(GetIcon(textureName), rect, horizontalAlign, verticalAlign, frame, tile, clickable);
			}

			public bool Icon (Texture2D texture, Rect rect, IconAligment horizontalAlign=IconAligment.resize, IconAligment verticalAlign=IconAligment.resize, bool frame=false, bool tile=false, bool clickable=false, bool alphaBlend=true)
			{
				#if UNITY_EDITOR
				if (frame) UnityEditor.EditorGUI.DrawRect(ToDisplay(rect.Extend(1)), Color.black);
				#endif
			
				if (texture==null) 
				{
					#if UNITY_EDITOR
					UnityEditor.EditorGUI.DrawRect(ToDisplay(rect), Color.gray);
					#endif
					return false;
				}
				
				//aligning texture if the rect width or height is more than icon size
				if (rect.width > texture.width) 
				{
					switch (horizontalAlign)
					{
						case IconAligment.min: rect.width = texture.width; break;
						case IconAligment.center: rect.x += rect.width/2; rect.x -= texture.width/2; rect.width = texture.width; break;
						case IconAligment.max: rect.x += rect.width; rect.x -= texture.width; rect.width = texture.width; break;
					}
				}
				if (rect.height > texture.height)
				{
					switch (verticalAlign)
					{
						case IconAligment.min: rect.height = texture.height; break;
						case IconAligment.center: rect.y += rect.height/2; rect.y -= texture.height/2; rect.height = texture.height; break;
						case IconAligment.max: rect.y += rect.height; rect.y -= texture.height; rect.height = texture.height; break;
					}
				}

				//click area
				bool result = false;
				//#if UNITY_EDITOR
				//if (clickable) result = UnityEditor.EditorGUI.Toggle(ToDisplay(rect), false, GUIStyle.none);
				//#endif

				//drawing texture
				if (!tile) 
				{
					//if (alphaBlend) 
					GUI.DrawTexture(ToDisplay(rect), texture, ScaleMode.ScaleAndCrop); 
					//#if UNITY_EDITOR
					//else 
					//UnityEditor.EditorGUI.DrawPreviewTexture(ToDisplay(rect), texture, null, ScaleMode.ScaleToFit);
					//#endif
				}
				else
				{
					//Debug.Log(zoom);
					Rect localRect = ToDisplay(rect);
					for (float x=0; x<rect.width; x+=texture.width*zoom)
						for (float y=0; y<rect.height; y+=texture.height*zoom)
						{
							//if (alphaBlend) 
							GUI.DrawTexture(new Rect(x+localRect.x, y+localRect.y, texture.width*zoom, texture.height*zoom), texture, ScaleMode.StretchToFill);
							//#if UNITY_EDITOR
							//else UnityEditor.EditorGUI.DrawPreviewTexture(new Rect(x+localRect.x, y+localRect.y, texture.width*zoom, texture.height*zoom), texture, null, ScaleMode.StretchToFill);
							//#endif
						}
				}

				return result;
			}

			private static Material iconMat;
			private static Material textureMat;

			public void TextureIcon (Texture2D texture, Rect rect)
			{
				if (iconMat == null) iconMat = new Material( Shader.Find("Hidden/DPLayout/TextureIcon") );
				//iconMat.SetFloat("_Roundness", 1/rect.width);
				#if UNITY_EDITOR
				if (texture != null) UnityEditor.EditorGUI.DrawPreviewTexture(ToDisplay(rect), texture, iconMat, ScaleMode.ScaleToFit);
				#endif
			}

			
			public void Texture (Texture2D texture, Rect rect, ref int mode)
			{
				if (textureMat==null) textureMat = new Material( Shader.Find("Hidden/DPLayout/Texture") );
				textureMat.SetInt("_Mode", mode);
				#if UNITY_EDITOR
				if (texture != null) UnityEditor.EditorGUI.DrawPreviewTexture(ToDisplay(rect), texture, textureMat, ScaleMode.ScaleAndCrop);
				#endif

				//simple modes
				int buttonSize = 20;
				for (int i=0; i<textureSimpleIcons.Length; i++)
				{
					Rect iconRect = new Rect(rect.x + i*buttonSize, rect.y, buttonSize, buttonSize);
					if (Button(null, rect:iconRect, icon:textureSimpleIcons[i], style:GUIStyle.none)) mode = i;
				}

				//save button
				//Rect saveRect = new Rect(rect.x+rect.width-buttonSize, rect.y, buttonSize, buttonSize);
				//if (Button(null, rect:saveRect, icon:"DPLayout_TexCh_Save", style:GUIStyle.none))
				//	SaveRawBytes(texture.ReadableUncompressedClone().EncodeToPNG(), filename:"Texture", type:"png");
			}

			public void TextureTool (ref int mode, ref int channel, Rect rect=new Rect())
			{
				int buttonSize = 20;
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(buttonSize, padding:0); rect = Inset(1, margin:0, padding:0); }

				//modes
				Rect modesRect = new Rect(rect.x, rect.y, textureModeIcons.Length*buttonSize, rect.height);
				#if UNITY_EDITOR
				UnityEditor.EditorGUI.LabelField(modesRect, "", UnityEditor.EditorStyles.toolbarButton);
				#endif

				for (int i=0; i<textureModeIcons.Length; i++)
				{
					Rect iconRect = new Rect(rect.x + i*buttonSize, rect.y, buttonSize, rect.height);
					if (Button(null, rect:iconRect, icon:textureModeIcons[i], style:GUIStyle.none)) mode = i;
				}

				//separator
				Rect sepRect = new Rect(modesRect.x+modesRect.width, rect.y, buttonSize/3, rect.height);
				#if UNITY_EDITOR
				UnityEditor.EditorGUI.LabelField(sepRect, "", UnityEditor.EditorStyles.toolbarButton);
				#endif

				//channels
				Rect channelsRect = new Rect(sepRect.x+sepRect.width, rect.y, textureChannelsIcons.Length*buttonSize, rect.height);
				#if UNITY_EDITOR
				UnityEditor.EditorGUI.LabelField(channelsRect, "", UnityEditor.EditorStyles.toolbarButton);
				#endif

				for (int i=0; i<textureChannelsIcons.Length; i++)
				{
					Rect iconRect = new Rect(channelsRect.x + i*buttonSize, rect.y, buttonSize, rect.height);
					if (Button(null, rect:iconRect, icon:textureChannelsIcons[i], style:GUIStyle.none)) mode = i;
				}
			}

			private static readonly string[] textureSimpleIcons = new string[] {"DPLayout_TexCh_Color", "DPLayout_TexCh_Alpha", "DPLayout_TexCh_Normal" };
			private static readonly string[] textureModeIcons = new string[] {"DPLayout_TexCh_Color", "DPLayout_TexCh_Linear", "DPLayout_TexCh_Normal" };
			private static readonly string[] textureChannelsIcons = new string[] {"DPLayout_TexCh_RGB", "DPLayout_TexCh_Red", "DPLayout_TexCh_Green", "DPLayout_TexCh_Blue",  "DPLayout_TexCh_Alpha" };

				
				

				/*if (texture==null) 
				{
					#if UNITY_EDITOR
					UnityEditor.EditorGUI.DrawRect(ToDisplay(rect), Color.gray);
					#endif
					return false;
				}
				
				//aligning texture if the rect width or height is more than icon size
				if (rect.width > texture.width) 
				{
					switch (horizontalAlign)
					{
						case IconAligment.min: rect.width = texture.width; break;
						case IconAligment.center: rect.x += rect.width/2; rect.x -= texture.width/2; rect.width = texture.width; break;
						case IconAligment.max: rect.x += rect.width; rect.x -= texture.width; rect.width = texture.width; break;
					}
				}
				if (rect.height > texture.height)
				{
					switch (verticalAlign)
					{
						case IconAligment.min: rect.height = texture.height; break;
						case IconAligment.center: rect.y += rect.height/2; rect.y -= texture.height/2; rect.height = texture.height; break;
						case IconAligment.max: rect.y += rect.height; rect.y -= texture.height; rect.height = texture.height; break;
					}
				}

				//click area
				bool result = false;
				//#if UNITY_EDITOR
				//if (clickable) result = UnityEditor.EditorGUI.Toggle(ToDisplay(rect), false, GUIStyle.none);
				//#endif

				//drawing texture
				if (!tile) 
				{
				//	if (alphaBlend) GUI.DrawTexture(ToDisplay(rect), texture, ScaleMode.ScaleAndCrop); 
				//	#if UNITY_EDITOR
				//	else UnityEditor.EditorGUI.DrawPreviewTexture(ToDisplay(rect), texture, null, ScaleMode.ScaleToFit);
				//	#endif

					

				//	GUI.DrawTexture(ToDisplay(rect), texture, ScaleMode.ScaleAndCrop);
					Material mat = new Material( Shader.Find("Hidden/Invert") );
					UnityEditor.EditorGUI.DrawPreviewTexture(ToDisplay(rect), texture, mat, ScaleMode.ScaleToFit);
				}
				else
				{
					//Debug.Log(zoom);
					Rect localRect = ToDisplay(rect);
					for (float x=0; x<rect.width; x+=texture.width*zoom)
						for (float y=0; y<rect.height; y+=texture.height*zoom)
						{
							UnityEditor.EditorGUI.DrawPreviewTexture(new Rect(x+localRect.x, y+localRect.y, texture.width*zoom, texture.height*zoom), texture, null, ScaleMode.StretchToFill);
						}
				}

				return result;*/

		#endregion

		#region Element

			[System.NonSerialized] Dictionary<string, GUIStyle> elementStyles = new Dictionary<string, GUIStyle>();

			public void Element (string textureName, Rect rect, RectOffset borders, RectOffset offset)
			{
				if (Event.current.type != EventType.Repaint) return;

				GUIStyle elementStyle = elementStyles.CheckGet(textureName);
	
				if (elementStyle == null || elementStyle.normal.background == null || elementStyle.hover.background == null)
				{
					elementStyle = new GUIStyle();
					elementStyle.normal.background = GetIcon(textureName); //Resources.Load("MapMagic_Window") as Texture2D;
					elementStyle.hover.background = GetIcon(textureName+"_pro");

					elementStyles.CheckAdd(textureName, elementStyle);
				}

				if (borders != null)
					elementStyle.border = borders;

				Rect paddedRect = ToDisplay(rect);
				if (offset != null)
					paddedRect = new Rect(paddedRect.x-offset.left, paddedRect.y-offset.top, paddedRect.width+offset.left+offset.right, paddedRect.height+offset.top+offset.bottom);
				
				#if UNITY_EDITOR
				elementStyle.Draw(paddedRect, UnityEditor.EditorGUIUtility.isProSkin, false, false, false);
				#endif
			}

		#endregion

		#region Drag Change

		float StepRound (float src)
		{
			if (src < 1) src= ((int)(src*1000))/1000f;
			else if (src < 10) src= ((int)(src*100))/100f;
			else if (src < 100) src= ((int)(src*10))/10f;
			else src= (int)(src);
			return src;
		}

		Vector2 sliderClickPos;
			int sliderDraggingId = -20000000;
			float sliderOriginalValue;

			public float DragChangeField (float val, Rect sliderRect, float min = 0, float max = 0, float minStep=0.2f)
			{
				sliderRect = ToDisplay(sliderRect);
				int controlId = GUIUtility.GetControlID(FocusType.Passive);
				#if UNITY_EDITOR
				UnityEditor.EditorGUIUtility.AddCursorRect (sliderRect, UnityEditor.MouseCursor.SlideArrow);
				#endif
				if (Event.current.type == EventType.MouseDown && sliderRect.Contains(Event.current.mousePosition) ) 
				{ 
					sliderClickPos = Event.current.mousePosition; 
					sliderOriginalValue = val;
					sliderDraggingId = controlId; 
				}

				if (sliderDraggingId == controlId) // && Event.current.type == EventType.MouseDrag)
				{
					int steps = (int)((Event.current.mousePosition.x - sliderClickPos.x) / 5);
					
					val = sliderOriginalValue;

					for (int i=0; i<Mathf.Abs(steps); i++)
					{
						float absVal = val>=0? val : -val;

						float step = 0.01f;
						if (absVal > 0.99f) step=0.02f; if (absVal > 1.99f) step=0.1f;   if (absVal > 4.999f) step = 0.2f; if (absVal > 9.999f) step=0.5f;
						if (absVal > 39.999f) step=1f;  if (absVal > 99.999f) step = 2f; if (absVal > 199.999f) step = 5f; if (absVal > 499.999f) step = 10f; 
						if (step < minStep) step = minStep;

						val = steps>0? val+step : val-step;
						val = Mathf.Round(val*10000)/10000f;

						if (Mathf.Abs(min)>0.001f && val<min) val=min;
						if (Mathf.Abs(max)>0.001f && val>max) val=max;
					}

					#if UNITY_EDITOR
					if (UnityEditor.EditorWindow.focusedWindow!=null) UnityEditor.EditorWindow.focusedWindow.Repaint(); 
					UnityEditor.EditorGUI.FocusTextInControl("");
					#endif
				}
				if (Event.current.rawType == EventType.MouseUp) 
				{
					sliderDraggingId = -20000000;

					#if UNITY_EDITOR
					if (UnityEditor.EditorWindow.focusedWindow!=null) UnityEditor.EditorWindow.focusedWindow.Repaint(); 
					//UnityEditor.EditorGUI.FocusTextInControl("");
					#endif
				}
				if (Event.current.isMouse && sliderDraggingId == controlId) Event.current.Use();

				return val;
			}

		#endregion

		#region Fields

			#pragma warning disable 0219,0414 
			//disabling warnings "value never used" for builds skipping Unity_editor

			//Determining Change
			public delegate void ChangeAction();
			public event ChangeAction OnBeforeChange;
			//public event ChangeAction OnAfterChange;

			private static Rect currentlyFocusedRect;

			public bool change = false; //any of the controllers have changed. Once set to true will not reset to false
			public bool lastChange = false; //last controller have changed. Resets to false each new element.

			public void SetChange (bool change)
			{
				if (change)
				{
					this.change = true; 
					this.lastChange = true; 
					if (OnBeforeChange != null) OnBeforeChange();
					#if UNITY_EDITOR
					if (undoObject != null) 
					{
						UnityEditor.Undo.RecordObject (undoObject, undoName);
						UnityEditor.EditorUtility.SetDirty(undoObject);
					}
					#endif
				}
				else this.lastChange = false;

				//this.change = false;
				//this.lastChange = false;
			}


			//Fields
			public float fieldSize = 0.5f;
			public float sliderSize = 0.5f; //in percent to field rect
			public bool monitorChange = true;
			public bool markup = false;
			public bool useEvent = false;
			public bool disabled = false;
			public int fontSize = 11;
			public int iconOffset = 4;
			public bool dragChange = false;
			public bool slider = false;
			public bool delayed = false;
			public enum HelpboxType { off, empty, info, warning, error };

			public void Field<T> (ref T src, string label=null, Rect rect = new Rect(), float min=-200000000, float max=200000000, bool limit=true, Val fieldSize = new Val(), Val sliderSize = new Val(), Val monitorChange = new Val(), Val useEvent = new Val(), Val disabled = new Val(), Val dragChange = new Val(), Val slider = new Val(), Val quadratic = new Val(), Val allowSceneObject = new Val(), Val delayed = new Val(), GUIStyle style=null, string tooltip=null)
			{ src = Field<T> (src, label, rect, min, max, limit, fieldSize, sliderSize, monitorChange, markup, useEvent, disabled, dragChange, slider, quadratic, allowSceneObject, delayed, style, tooltip); }

			public T Field<T> (
				T src, 
				string label=null,
				Rect rect = new Rect(), 
				float min=-200000000, float max=200000000, bool limit=true,
				Val fieldSize = new Val(),
				Val sliderSize = new Val(),
				Val monitorChange = new Val(), 
				Val markup = new Val(), 
				Val useEvent = new Val(),
				Val disabled = new Val(),
				Val dragChange = new Val(),
				Val slider = new Val(),
				Val quadratic = new Val(),
				Val allowSceneObject = new Val(),
				Val delayed = new Val(),
				GUIStyle style = null, 
				string tooltip=null)
			{
				//finding current params
				fieldSize.Verify(this.fieldSize); sliderSize.Verify(this.sliderSize);
				useEvent.Verify(this.useEvent); disabled.Verify(this.disabled); markup.Verify(this.markup);
				dragChange.Verify(this.dragChange); slider.Verify(this.slider);
				delayed.Verify(this.delayed);

				//loading styles
				CheckStyles();
				
				//if no rect specified - taking all of the next line
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }

				//exiting on markup
				if (markup) return src;

				//disabling
				disabled.Verify(this.disabled);
				#if UNITY_EDITOR
				if (disabled) UnityEditor.EditorGUI.BeginDisabledGroup(true);
				#endif

				//finding field and label rects
				if (label==null) fieldSize = 1;
				Rect labelRect = rect.Clamp(1f-fieldSize); //new Rect(rect.x, rect.y, (1f-_fieldSize)*rect.width, rect.height);
				Rect fieldRect = rect.ClampFromLeft((float)fieldSize); //new Rect(rect.x+(1f-_fieldSize)*rect.width, rect.y, _fieldSize*rect.width, rect.height);
				
				Rect sliderRect = fieldRect.Clamp((float)sliderSize); sliderRect = sliderRect.Clamp((int)sliderRect.width-4);
				if (slider) fieldRect = fieldRect.ClampFromLeft(1f-sliderSize);

				//prefix label 
				if (label!=null && zoom>0.3f) Label(label, labelRect, tooltip:tooltip); //prefix:true does not have a changable width

				//setting focus name
			//	GUI.SetNextControlName("LayoutField");

				//drawing field
				object srcObj = (object)src;
				object dstObj = default(T);

				#if UNITY_EDITOR
				System.Type type = typeof(T);
				if (type == typeof(float)) 
				{
					float val = (float)srcObj;

					if (slider)
					{
						float newval = val;
						if (!quadratic) newval = GUI.HorizontalSlider(ToDisplay(sliderRect), val, min, max); 
						else newval = Mathf.Pow(GUI.HorizontalSlider(ToDisplay(sliderRect), Mathf.Pow(val,0.5f), Mathf.Pow(min,0.5f), Mathf.Pow(max,0.5f)),2); 
						if (!Mathf.Approximately(val,newval)) val = StepRound(newval);
					}
					if (dragChange && zoom>0.45f) val = DragChangeField(val, fieldRect.ClampFromLeft((int)22), minStep:0);
					
					if (delayed) 
					{
						#if UNITY_5_5_OR_NEWER
						val = UnityEditor.EditorGUI.DelayedFloatField(ToDisplay(fieldRect), val, fieldStyle);
						#else
						val = UnityEditor.EditorGUI.FloatField(ToDisplay(fieldRect), val, fieldStyle);	
						#endif
					}	
					else val = UnityEditor.EditorGUI.FloatField(ToDisplay(fieldRect), val, fieldStyle);	

					if (limit) { if (val > max) val = max; if (val < min) val = min; }
					dstObj = val;

					if (dragChange && zoom>0.45f) Icon("DPLayoutIcon_Slider", fieldRect, horizontalAlign:IconAligment.max, verticalAlign:IconAligment.center);
				}		
				else if (type == typeof(int)) 
				{
					int val = (int)srcObj;

					if (slider)
					{
						if (!quadratic) val = (int)GUI.HorizontalSlider(ToDisplay(sliderRect), val, (int)min, (int)max); 
						else val = (int)Mathf.Pow(GUI.HorizontalSlider(ToDisplay(sliderRect), Mathf.Pow(val,0.5f), Mathf.Pow(min,0.5f), Mathf.Pow(max,0.5f)),2); 
					}
					if (dragChange && zoom>0.45f) val = (int)DragChangeField(val, fieldRect.ClampFromLeft((int)22), minStep:1);
					
					if (delayed) 
					{
						#if UNITY_5_5_OR_NEWER
						val = UnityEditor.EditorGUI.DelayedIntField(ToDisplay(fieldRect), val, fieldStyle);
						#else
						val = UnityEditor.EditorGUI.IntField(ToDisplay(fieldRect), val, fieldStyle);
						#endif
					}
					else val = UnityEditor.EditorGUI.IntField(ToDisplay(fieldRect), val, fieldStyle);

					if (val > max) val = (int)max; if (val < min) val = (int)min;
					dstObj = val;

					if (dragChange && zoom>0.45f) Icon("DPLayoutIcon_Slider", fieldRect, horizontalAlign:IconAligment.max, verticalAlign:IconAligment.center);
				}
				else if (type == typeof(Vector2)) 
				{
					Rect leftRect = fieldRect.Clamp((int)(fieldRect.width/2f-1));
					Rect rightRect = fieldRect.ClampFromLeft((int)(fieldRect.width/2f-1));

					Vector2 val = (Vector2)srcObj;

					if (slider)
					{
						Vector2 newval = val;
						if (!quadratic) UnityEditor.EditorGUI.MinMaxSlider(ToDisplay(sliderRect), ref newval.x, ref newval.y, min, max);
						else 
						{
							newval = new Vector2(Mathf.Pow(newval.x,0.5f), Mathf.Pow(newval.y,0.5f));
							UnityEditor.EditorGUI.MinMaxSlider(ToDisplay(sliderRect), ref newval.x, ref newval.y, Mathf.Pow(min,0.5f), Mathf.Pow(max,0.5f));
							newval = new Vector2(Mathf.Pow(newval.x,2f), Mathf.Pow(newval.y,2f));
						}
						if (Mathf.Abs(val.x-newval.x)>0.001f) val.x = StepRound(newval.x);
						if (Mathf.Abs(val.y-newval.y)>0.001f) val.y = StepRound(newval.y);
					}

					val.x = Field(val.x, rect:leftRect, min:min, max:max, limit:limit, monitorChange:false, disabled:disabled, dragChange:dragChange, slider:false, quadratic:quadratic, tooltip:tooltip);
					val.y = Field(val.y, rect:rightRect, min:min, max:max, limit:limit, monitorChange:false, disabled:disabled, dragChange:dragChange, slider:false, quadratic:quadratic, tooltip:tooltip);

					dstObj = val;
				}
				else if (type == typeof(Coord)) 
				{
					Rect leftRect = fieldRect.Clamp((int)(fieldRect.width/2f-1));
					Rect rightRect = fieldRect.ClampFromLeft((int)(fieldRect.width/2f-1));

					Coord val = (Coord)srcObj;

					val.x = Field(val.x, rect:leftRect, min:min, max:max, limit:limit, monitorChange:false, disabled:disabled, dragChange:dragChange, slider:false, quadratic:quadratic, tooltip:tooltip);
					val.z = Field(val.z, rect:rightRect, min:min, max:max, limit:limit, monitorChange:false, disabled:disabled, dragChange:dragChange, slider:false, quadratic:quadratic, tooltip:tooltip);

					dstObj = val;
				}
				else if (type == typeof(Vector3)) 
				{
					Rect leftRect = fieldRect.Clamp((int)(fieldRect.width/3f-1));
					Rect midRect = fieldRect.Clamp((int)(fieldRect.width/3f-1)); midRect.x += leftRect.width + 3;
					Rect rightRect = fieldRect.ClampFromLeft((int)(fieldRect.width/3f-1));

					Vector3 val = (Vector3)srcObj;

					val.x = Field(val.x, rect:leftRect, min:min, max:max, limit:limit, monitorChange:false, disabled:disabled, dragChange:dragChange, slider:false, quadratic:quadratic, tooltip:tooltip);
					val.y = Field(val.y, rect:midRect, min:min, max:max, limit:limit, monitorChange:false, disabled:disabled, dragChange:dragChange, slider:false, quadratic:quadratic, tooltip:tooltip);
					val.z = Field(val.z, rect:rightRect, min:min, max:max, limit:limit, monitorChange:false, disabled:disabled, dragChange:dragChange, slider:false, quadratic:quadratic, tooltip:tooltip);

					dstObj = val;
				}
				else if (type == typeof(bool)) 
				{
					Rect fRect = fieldRect;//ToDisplay(fieldRect);
					if (zoom > 0.75f) dstObj = UnityEditor.EditorGUI.Toggle(ToDisplay(new Rect(fRect.x,fRect.y,20,fRect.height)), (bool)srcObj);
					else  dstObj = UnityEditor.EditorGUI.Toggle(ToDisplay(new Rect(fRect.x,fRect.y,20,fRect.height)), (bool)srcObj, UnityEditor.EditorStyles.miniButton);
				}
				else if (type == typeof(string)) dstObj = UnityEditor.EditorGUI.TextField(ToDisplay(fieldRect), (string)srcObj, style!=null? style : fieldStyle);
				else if (type == typeof(Color)) dstObj = UnityEditor.EditorGUI.ColorField(ToDisplay(fieldRect), (Color)srcObj);
				else if (type == typeof(Texture2D))
				{
					if (zoom>0.55f) dstObj = UnityEditor.EditorGUI.ObjectField(ToDisplay(fieldRect), (Texture2D)srcObj, typeof(Texture2D), false);
					else 
					{
						dstObj = srcObj;
						//dstObj = UnityEditor.EditorGUI.ObjectField(ToDisplay(fieldRect), (Texture2D)srcObj, typeof(Object), false);
						if (srcObj == null) UnityEditor.EditorGUI.HelpBox(ToDisplay(fieldRect),"",UnityEditor.MessageType.None);
						else UnityEditor.EditorGUI.DrawPreviewTexture(ToDisplay(fieldRect), (Texture2D)srcObj);
						
					}
				}
				else if (type == typeof(Material)) dstObj = UnityEditor.EditorGUI.ObjectField(ToDisplay(fieldRect), (Material)srcObj, typeof(Material), false);
				else if (type == typeof(Transform)) dstObj = UnityEditor.EditorGUI.ObjectField(ToDisplay(fieldRect), (Transform)srcObj, typeof(Transform), false);
				else if (type == typeof(GameObject)) dstObj = UnityEditor.EditorGUI.ObjectField(ToDisplay(fieldRect), (GameObject)srcObj, typeof(GameObject), false);
				else if (type.IsEnum) 
				{
					if (zoom > 0.99f) dstObj = UnityEditor.EditorGUI.EnumPopup(ToDisplay(fieldRect), (System.Enum)srcObj);
					else dstObj = (T)(object)UnityEditor.EditorGUI.EnumPopup(ToDisplay(fieldRect), (System.Enum)srcObj, enumZoomStyle);
				}
				else if (type.IsSubclassOf(typeof(UnityEngine.Object))) dstObj = UnityEditor.EditorGUI.ObjectField(ToDisplay(fieldRect), (UnityEngine.Object)srcObj, type, allowSceneObject);
				#endif

				T dst = (T)dstObj;

				//end disabling
				#if UNITY_EDITOR
				if (disabled) UnityEditor.EditorGUI.EndDisabledGroup();
				#endif

				//monitoring change
				monitorChange.Verify(this.monitorChange);
				if (monitorChange && !EqualityComparer<T>.Default.Equals(src, dst)) SetChange(true);
				else SetChange(false);

				//deselecting when clicking somewhere else
			//	if (Event.current.type == EventType.MouseDown)// && !ToDisplay(fieldRect).Contains(Event.current.mousePosition)) 
			//	{
			//		Debug.Log(GUI.GetNameOfFocusedControl() + " " + Event.current.type);
			//	}
				
				
				//UnityEditor.EditorGUI.FocusTextInControl(""); //stop writing on selection change

				//using event
				//if (useEvent && Event.current.type != EventType.Repaint && ToDisplay(fieldRect).Contains(Event.current.mousePosition)) Event.current.Use();

				//returning
				return dst;
			}

			/*public void Curve (Curve curve, Rect rect, Color color=new Color(), string tooltip=null)
			{
				//if no rect specified - taking all of the next line
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }
				if (color.a < 0.001f) color = Color.black;

				//exit on markup
				if (markup) return;

				#if UNITY_EDITOR
				UnityEditor.EditorGUI.HelpBox(ToDisplay(rect), null, UnityEditor.MessageType.None);
				for (int p=0; p<curve.points.Length-1; p++)
				{
					Curve.Point prev = curve.points[p];
					Curve.Point next = curve.points[p+1];
					
					Vector2 pos1 = new Vector2(prev.time, prev.val);
					Vector2 pos2 = new Vector2(next.time, next.val);

					Vector2 tan1 = new Vector2((next.time-prev.time)/4f, prev.outTangent/4f) + pos1;
					Vector2 tan2 = new Vector2((prev.time-next.time)/4f, -next.inTangent/4f) + pos2;
					
					pos1.x = pos1.x*rect.width + rect.x; pos1.y = -pos1.y*rect.height + rect.y + rect.height; 
					pos2.x = pos2.x*rect.width + rect.x; pos2.y = -pos2.y*rect.height + rect.y + rect.height; 
					tan1.x = tan1.x*rect.width + rect.x; tan1.y = -tan1.y*rect.height + rect.y + rect.height; 
					tan2.x = tan2.x*rect.width + rect.x; tan2.y = -tan2.y*rect.height + rect.y + rect.height; 
					
					UnityEditor.Handles.DrawBezier(ToDisplay(pos1), ToDisplay(pos2), ToDisplay(tan1), ToDisplay(tan2), color, null, 1.5f);
				
				}
				#endif
			}*/

			System.Type curveWindowType;
			AnimationCurve windowCurveRef = null;
			public void Curve (AnimationCurve src, Rect rect, Rect ranges=new Rect(), Color color=new Color(), string tooltip=null)
			{
				//if no rect specified - taking all of the next line
				if (ranges.width < Mathf.Epsilon && ranges.height < Mathf.Epsilon) { ranges.width = 1; ranges.height = 1; }
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }
				if (color.a < 0.001f) color = Color.white;
				lastChange = false; //custom set change

				//exit on markup
				if (markup) return;

				#if UNITY_EDITOR

				//recording undo on change if the curve editor window is opened (and this current curve is selected)
				try
				{
					if (curveWindowType == null) curveWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.CurveEditorWindow");
					if (UnityEditor.EditorWindow.focusedWindow != null && UnityEditor.EditorWindow.focusedWindow.GetType() == curveWindowType)
					{
						AnimationCurve windowCurve = curveWindowType.GetProperty("curve").GetValue(UnityEditor.EditorWindow.focusedWindow, null) as AnimationCurve;
						if (windowCurve == src)
						{
							if (windowCurveRef == null) windowCurveRef = windowCurve.Copy();
							if (!windowCurve.IdenticalTo(windowCurveRef))
							{
								
								Keyframe[] tempKeys = windowCurve.keys;
								windowCurve.keys = windowCurveRef.keys;
								SetChange(true);
								SetChange(true);
								
								windowCurve.keys = tempKeys;

								windowCurveRef = windowCurve.Copy();
							}
						}
					}
					else windowCurveRef = null;
				}
				catch {};

				UnityEditor.EditorGUI.CurveField(ToDisplay(rect), src, Color.white, ranges); 

				#endif
			}


			public void Label (
				string label = null, 
				Rect rect = new Rect(), 
				string url = null,
				bool helpbox = false, 
				int messageType = 0,
				Val fontSize = new Val(),
				Val disabled = new Val(),
				FontStyle fontStyle = FontStyle.Normal,
				TextAnchor textAnchor = TextAnchor.UpperLeft,
				bool prefix = false,
				string icon = null,
				string tooltip=null )
			{
				//if no rect specified - taking all of the next line
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }
				
				//exit on markup
				if (markup) return;

				//disabling
				disabled.Verify(this.disabled);
				#if UNITY_EDITOR
				if (disabled) UnityEditor.EditorGUI.BeginDisabledGroup(true);
				#endif
			
				//setting styles
				CheckStyles();

				GUIStyle style = labelStyle;
				if (url != null) style = urlStyle;

				fontSize.Verify(this.fontSize);
				if (helpbox) fontSize /= 1.2f;
				fontSize = Mathf.RoundToInt(fontSize*zoom);
				if (style.fontSize != fontSize) style.fontSize = fontSize;

				if (style.alignment != textAnchor) labelStyle.alignment = textAnchor;
				if (style.fontStyle != fontStyle) labelStyle.fontStyle = fontStyle;
				if (helpbox) labelStyle.wordWrap = true; else labelStyle.wordWrap = false;
				
				//helpbox background
				#if UNITY_EDITOR
				if (helpbox) UnityEditor.EditorGUI.HelpBox(ToDisplay(rect), "", (UnityEditor.MessageType)messageType);
				#endif

				//icon
				if (icon!=null) Icon(icon, new Rect(rect.x+4, rect.y, rect.width-8, rect.height), horizontalAlign:IconAligment.min, verticalAlign:IconAligment.center);

				//gui content
				GUIContent content = new GUIContent(label, tooltip);

				//drawing label
				#if UNITY_EDITOR
				if (prefix) UnityEditor.EditorGUI.PrefixLabel(ToDisplay(rect), content, labelStyle); 
				//else if (helpbox) UnityEditor.EditorGUI.HelpBox(ToDisplay(rect), label, (UnityEditor.MessageType)messageType);
				else if (url != null) 
				{
					if (GUI.Button(ToDisplay(rect), content, urlStyle)) Application.OpenURL(url); 
					UnityEditor.EditorGUIUtility.AddCursorRect (ToDisplay(rect), UnityEditor.MouseCursor.Link);
				}
				else UnityEditor.EditorGUI.LabelField(ToDisplay(rect), content, style);
				#endif

				//end disabling
				#if UNITY_EDITOR
				if (disabled) UnityEditor.EditorGUI.EndDisabledGroup();
				#endif
			}

			public string EditableLabel (
				string label = null, 
				Rect rect = new Rect(), 
				Val fontSize = new Val(),
				Val disabled = new Val(),
				FontStyle fontStyle = FontStyle.Normal,
				TextAnchor textAnchor = TextAnchor.UpperLeft,
				string tooltip=null )
			{
				//if no rect specified - taking all of the next line
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }
				
				//exit on markup
				if (markup) return label;

				//setting styles
				CheckStyles();
				GUIStyle style = labelStyle;

				fontSize.Verify(this.fontSize);
				fontSize = Mathf.RoundToInt(fontSize*zoom);
				if (style.fontSize != fontSize) style.fontSize = fontSize;

				if (style.alignment != textAnchor) labelStyle.alignment = textAnchor;
				if (style.fontStyle != fontStyle) labelStyle.fontStyle = fontStyle;

				//changing name button
				float changeNameButtonWidth = 20;
				Rect changeNameButtonRect = new Rect(rect.x+rect.width-changeNameButtonWidth, rect.y, changeNameButtonWidth, rect.height);
				#if UNITY_EDITOR
				UnityEditor.EditorGUIUtility.AddCursorRect (ToDisplay(changeNameButtonRect), UnityEditor.MouseCursor.Link);
				#endif
				Icon("DPLayout_EditableLabel", changeNameButtonRect, horizontalAlign:IconAligment.center, verticalAlign:IconAligment.center);
				if (GUI.Button(ToDisplay(changeNameButtonRect), "", GUIStyle.none))
				//if (Event.current.type==EventType.MouseDown && changeNameButtonRect.Contains(ToInternal(Event.current.mousePosition)))
					currentlyFocusedRect = rect;

				//using rect as id to determine if we are changing the name now
				bool editing = CoordinatesExtensions.Approximately(rect, currentlyFocusedRect);

				//label
				Rect labelRect = new Rect(rect.x, rect.y, rect.width-changeNameButtonWidth, rect.height);

				if (editing)
				{
					#if UNITY_EDITOR
					GUI.SetNextControlName("LayerFoldoutNextFocus"); //to focus in text right after pressing edit button
					label = UnityEditor.EditorGUI.TextField(ToDisplay(labelRect), label, fieldStyle);
					UnityEditor.EditorGUI.FocusTextInControl("LayerFoldoutNextFocus"); 
					Repaint();

					//exit editing
					if (Event.current.keyCode==KeyCode.KeypadEnter || Event.current.keyCode==KeyCode.Return || Event.current.keyCode==KeyCode.Escape || //if enter or esc
						(Event.current.type==EventType.MouseDown && !labelRect.Contains(ToInternal(Event.current.mousePosition)))) //if clicked somewhere else
						currentlyFocusedRect = new Rect(0,0,-1,-2000000);
					#endif
				}

				else //standard
				{
					#if UNITY_EDITOR
					UnityEditor.EditorGUI.LabelField(ToDisplay(labelRect), label, labelStyle);
					#endif

					//selection
					/*if (Event.current.type==EventType.MouseUp && labelRect.Contains(ToInternal(Event.current.mousePosition)))
					{
						if (selected != num) selected = num;
						else selected = -1;

						UnityEditor.EditorGUI.FocusTextInControl(null);
							
						Repaint();
					}*/
				}

				return label;
			}



			public bool Button (
				string label = null,
				Rect rect = new Rect(),  
				Val monitorChange = new Val(),
				Val disabled = new Val(),
				string icon = null,
				bool iconCenter = false,
				int iconAnimFrames=0,
				GUIStyle style = null,
				string tooltip=null )
			{
				CheckStyles();
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }
				GUIContent content = new GUIContent(label, tooltip);

				//exit on markup
				if (markup) return false;
				
				//disabling
				disabled.Verify(this.disabled);
				#if UNITY_EDITOR
				if (disabled) UnityEditor.EditorGUI.BeginDisabledGroup(true);
				#endif

				bool result = false;
				if (style==null) result = GUI.Button(ToDisplay(rect), content, buttonStyle);
				else result = GUI.Button(ToDisplay(rect), content, style);

				//end disabling
				#if UNITY_EDITOR
				if (disabled) UnityEditor.EditorGUI.EndDisabledGroup();
				#endif

				monitorChange.Verify(this.monitorChange);
				if (monitorChange)
				{
					if (result) SetChange(true);
					else SetChange(false);
				}
					
				if (icon!=null && !iconCenter) Icon(icon, new Rect(rect.x+4, rect.y, rect.width-8, rect.height), horizontalAlign:IconAligment.min, verticalAlign:IconAligment.center, animationFrames:iconAnimFrames);
				if (icon!=null && iconCenter) Icon(icon, rect, horizontalAlign:IconAligment.center, verticalAlign:IconAligment.center, animationFrames:iconAnimFrames);

				return result;
			}



			//TODO: group with a button
			public void CheckButton (ref bool src, string label = null, Rect rect = new Rect(), Val monitorChange = new Val(), Val disabled = new Val(), string icon = null, string tooltip=null )
			{ src = CheckButton(src, label, rect, monitorChange, disabled, icon, tooltip);}

			public bool CheckButton (
				bool src,
				string label = null,
				Rect rect = new Rect(),  
				Val monitorChange = new Val(),
				Val disabled = new Val(),
				string icon = null,
				string tooltip=null )
			{
				CheckStyles();
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }		
				if (markup) return src; //exit on markup
				GUIContent content = new GUIContent(label, tooltip);
				
				bool dst = GUI.Toggle(ToDisplay(rect), src, content, buttonStyle);

				monitorChange.Verify(this.monitorChange);
				if (monitorChange)
				{
					if (dst != src) SetChange(true);
					else SetChange(false);
				}
					
				if (icon!=null) Icon(icon, new Rect(rect.x+4, rect.y, rect.width-8, rect.height), horizontalAlign:IconAligment.min, verticalAlign:IconAligment.center);

				return dst;
			}

			public void Toggle (ref bool src, string label = null, Rect rect = new Rect(), Val monitorChange = new Val(), Val disabled = new Val(), string onIcon=null, string offIcon=null, string tooltip=null )
			{ src = Toggle(src, label, rect, monitorChange, disabled, onIcon, offIcon, tooltip);}

			public bool Toggle (
				bool src,
				string label = null,
				Rect rect = new Rect(),  
				Val monitorChange = new Val(),
				Val disabled = new Val(),
				string onIcon=null, string offIcon=null, 
				string tooltip=null )
			{
				CheckStyles();
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }
				if (markup) return src; //exit on markup

				//disabling
				disabled.Verify(this.disabled);
				#if UNITY_EDITOR
				if (disabled) UnityEditor.EditorGUI.BeginDisabledGroup(true);
				#endif

				Rect fieldRect = new Rect(rect.x, rect.y, 20, rect.height);
				Rect labelRect = new Rect(rect.x+20, rect.y, rect.width-20, rect.height);

				if (label != null) Label(label, labelRect);

				bool dst = src;
				#if UNITY_EDITOR
				if (onIcon!=null && offIcon!=null)
				{
					dst = UnityEditor.EditorGUI.Toggle(ToDisplay(fieldRect), src, GUIStyle.none);
					if (src) Icon(onIcon, fieldRect, Layout.IconAligment.center, Layout.IconAligment.center);
					else Icon(offIcon, fieldRect, Layout.IconAligment.center, Layout.IconAligment.center);
				}
				else if (zoom > 0.75f) dst = UnityEditor.EditorGUI.Toggle(ToDisplay(fieldRect), src);
				else  dst = UnityEditor.EditorGUI.Toggle(ToDisplay(new Rect(fieldRect.x,fieldRect.y,20,fieldRect.height)), src, UnityEditor.EditorStyles.miniButton);
				#endif

				//end disabling
				#if UNITY_EDITOR
				if (disabled) UnityEditor.EditorGUI.EndDisabledGroup();
				#endif

				monitorChange.Verify(this.monitorChange);
				if (monitorChange)
				{
					if (dst != src) SetChange(true);
					else SetChange(false);
				}
					
				return dst;
			}


			public void LayersField (ref int src, string label = null, Rect rect = new Rect(), Val monitorChange = new Val(), Val disabled = new Val(), string onIcon=null, string offIcon=null, string tooltip=null )
				{ src = LayersField(src, label, rect, monitorChange, disabled, onIcon, offIcon, tooltip);}
			public int LayersField (
				int src,
				string label = null,
				Rect rect = new Rect(),  
				Val monitorChange = new Val(),
				Val disabled = new Val(),
				string onIcon=null, string offIcon=null, 
				string tooltip=null )
			{
				CheckStyles();
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }
				if (markup) return src; //exit on markup

				//disabling
				disabled.Verify(this.disabled);
				#if UNITY_EDITOR
				if (disabled) UnityEditor.EditorGUI.BeginDisabledGroup(true);
				#endif

				//finding field and label rects
				if (label==null) fieldSize = 1;
				Rect labelRect = rect.Clamp(1f-fieldSize); //new Rect(rect.x, rect.y, (1f-_fieldSize)*rect.width, rect.height);
				Rect fieldRect = rect.ClampFromLeft((float)fieldSize); //new Rect(rect.x+(1f-_fieldSize)*rect.width, rect.y, _fieldSize*rect.width, rect.height);

				if (label != null) Label(label, labelRect);

				int dst = src;
				#if UNITY_EDITOR
				if (zoom > 0.99f) dst = UnityEditor.EditorGUI.MaskField(ToDisplay(fieldRect),src, UnityEditorInternal.InternalEditorUtility.layers);
				else dst = UnityEditor.EditorGUI.LayerField(ToDisplay(fieldRect), src, enumZoomStyle);
				#endif

				//end disabling
				#if UNITY_EDITOR
				if (disabled) UnityEditor.EditorGUI.EndDisabledGroup();
				#endif

				monitorChange.Verify(this.monitorChange);
				if (monitorChange)
				{
					if (dst != src) SetChange(true);
					else SetChange(false);
				}
					
				return dst;
			}

			public void Foldout (ref bool src, string label = null, Rect rect = new Rect(), Val disabled = new Val(), string tooltip=null, bool bold=true )
			{ src = Foldout(src, label, rect, disabled, tooltip, bold);}

			public bool Foldout (
				bool src,
				string label = null,
				Rect rect = new Rect(),  
				Val disabled = new Val(),
				string tooltip=null,
				bool bold=true )
			{
				CheckStyles();
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }
				if (markup) return src; //exit on markup
				GUIContent content = new GUIContent(label, tooltip);
				if (bold) foldoutStyle.fontStyle = FontStyle.Bold; else foldoutStyle.fontStyle = FontStyle.Normal;

				//offset rect to make triangle within field
				rect.x += 12; rect.width -= 12;
				
				#if UNITY_EDITOR
				GUIUtility.GetControlID(FocusType.Passive);
				bool dst = UnityEditor.EditorGUI.Foldout(ToDisplay(rect), src, content, true, foldoutStyle);
				//if (ToDisplay(rect).Contains( GUIUtility.GUIToScreenPoint(Event.current.mousePosition-) )) Debug.Log("H");
				//GUIUtility.
				if (src != dst) UnityEditor.EditorGUI.FocusTextInControl("");
				return dst;
				#else
				return false;
				#endif
			}

			public void ToggleFoldout (
				ref bool unfolded,
				ref bool enabled,
				string label = null,
				Rect rect = new Rect(),  
				Val disabled = new Val(),
				string tooltip=null)
			{
				CheckStyles();
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }
				if (markup) return; //exit on markup
				label = "     " + label;
				GUIContent content = new GUIContent(label, tooltip);
				foldoutStyle.fontStyle = FontStyle.Normal;
				
				#if UNITY_EDITOR
				Rect foldoutRect = new Rect(rect.x+12, rect.y, 10, rect.height);
				unfolded = UnityEditor.EditorGUI.Foldout(ToDisplay(foldoutRect), unfolded, " ", true, foldoutStyle);
				
				Rect fieldRect = new Rect(rect.x+20, rect.y, 20, rect.height);
				if (zoom > 0.75f) enabled = UnityEditor.EditorGUI.Toggle(ToDisplay(fieldRect), enabled);
				else  enabled = UnityEditor.EditorGUI.Toggle(ToDisplay(fieldRect), enabled, UnityEditor.EditorStyles.miniButton);

				Rect labelRect = new Rect(rect.x+20, rect.y, rect.width-50, rect.height);
				UnityEditor.EditorGUI.LabelField(ToDisplay(labelRect), content);
				#endif
			}


			public void Gauge (float progress, string label, Rect rect = new Rect(),  Val disabled = new Val(), string tooltip=null)
			{
				CheckStyles();
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }
				if (markup) return; //exit on markup
				GUIContent content = new GUIContent(label, tooltip);

				#if UNITY_EDITOR
				if (disabled) UnityEditor.EditorGUI.BeginDisabledGroup(true);
				UnityEditor.EditorGUI.ProgressBar(ToDisplay(rect), progress, label);
				if (disabled) UnityEditor.EditorGUI.EndDisabledGroup();
				#endif
			}

			public int Popup (int selected, string[] displayedOptions, string label=null, Rect rect = new Rect(),  Val disabled = new Val(), string tooltip=null)
			{
				CheckStyles();
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }
				if (markup) return selected; //exit on markup
				GUIContent content = new GUIContent(label, tooltip);

				int newSelected = selected;
				#if UNITY_EDITOR
				newSelected = UnityEditor.EditorGUI. Popup(ToDisplay(rect), selected, displayedOptions);
				#endif

				if (newSelected != selected) SetChange(true);
				else SetChange(false);

				return newSelected;
			}

			#if UNITY_EDITOR
			public void MaterialEditor (UnityEditor.MaterialEditor matEd)
			{
				UnityEditor.EditorGUILayout.BeginVertical();
				UnityEditor.EditorGUILayout.LabelField("", GUILayout.Height(cursor.y + cursor.height));
				UnityEditor.EditorGUI.indentLevel = 0;
				matEd.DrawHeader (); 
				matEd.OnInspectorGUI (); 
				UnityEditor.EditorGUILayout.EndVertical();
				//TODO: have to determine space used to make an offset, but 
			}
			#endif


			public T ScriptableAssetField<T> (T asset, System.Func<T> construct, string savePath=null, Val fieldSize = new Val()) where T : ScriptableObject, ISerializationCallbackReceiver
			//construct should always be determined, even if it is null. Otherwise it will cause unhandled exception in unity pre-5.5
			{
				fieldSize.Verify(this.fieldSize);
				bool prevChange = change;
				change = false;

				//data label
				Par(); Label("Data", Inset(1-fieldSize.val));
				asset = Field(asset, rect:Inset(fieldSize.val));
				if (lastChange && asset!=null) asset.OnAfterDeserialize();
					
				//create/reset buttons
				#if UNITY_EDITOR
				Par(18); Inset(1-fieldSize.val);
				if (asset==null)
				{
					if (Button("Create", rect:Inset(fieldSize.val))) 
					{
						if (construct==null) asset = ScriptableObject.CreateInstance<T>();
						else asset = construct();
					}
				}
				else 
				{
					if (Button("Reset", rect:Inset(fieldSize.val))) 
					{
						if (UnityEditor.EditorUtility.DisplayDialog("Reset to Default", "This will remove all of the data and create a default one. Are you sure you wsih to continue?", "Reset to Default", "Cancel"))
						{
							if (construct==null) asset = ScriptableObject.CreateInstance<T>();
							else asset = construct();
						}
					}
				}

				//save/release buttons
				Par(18); Inset(1-fieldSize.val);
				if (asset==null || !UnityEditor.AssetDatabase.Contains(asset))
				{ 
					if (Button("Store to Assets", rect:Inset(fieldSize.val), disabled:asset==null)) asset = SaveAsset(asset,savePath);
				}
				else 
				{ 
					if (Button("Release", rect:Inset(fieldSize.val), disabled:asset==null)) asset = ReleaseAsset(asset);
				}

				//save copy button
				Par(18); Inset(1-fieldSize.val);
				if (Button("Save as Copy", rect:Inset(fieldSize.val), disabled:asset==null))
				{
					T copyAsset = ScriptableObject.Instantiate<T>(asset);
					SaveAsset(copyAsset,savePath);
				}
				#endif

				if (change) lastChange = true;
				change = prevChange || lastChange;

				return asset;
			}

			public void AssetNewSaveField<T> (ref T asset, string label, Rect rect=new Rect(), string saveFilename="Data.asset", string saveType="asset", Func<T> create=null) where T: UnityEngine.Object
				{ asset = AssetNewSaveField<T>(asset, label, rect, saveFilename, saveType, create); }
			public T AssetNewSaveField<T> (T asset, string label, Rect rect=new Rect(), string saveFilename="Data", string saveType="asset", Func<T> create=null) where T: UnityEngine.Object
			{
				bool prevChange = change;
				change = false;

				CheckStyles();
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }
				if (markup) return asset; //exit on markup

				Rect fieldRect = rect; fieldRect.width *= 0.62f;
				asset = Field(asset, label, rect:fieldRect, fieldSize:0.55f);

				Rect newRect = rect; newRect.width=newRect.width*0.19f - horizontalPadding; newRect.x+=fieldRect.width+horizontalPadding;
				if (Button("New", rect:newRect)) 
				{
					#if UNITY_EDITOR
					if (asset == null ||
						UnityEditor.AssetDatabase.Contains(asset) ||
						UnityEditor.EditorUtility.DisplayDialog("New " + label, "This will remove previous asset. Are you sure you wish to continue?", "Clear", "Cancel"))
							asset = create();
					#endif
				}

				#if UNITY_EDITOR
				Rect saveRect = rect; saveRect.width=saveRect.width*0.19f - horizontalPadding; saveRect.x+=fieldRect.width+newRect.width+horizontalPadding*2;
				if (Button("Save", rect:saveRect, disabled:asset==null || UnityEditor.AssetDatabase.Contains(asset))) 
					asset = SaveAsset(asset, filename:saveFilename);
				#endif
				
				if (change) lastChange = true;
				change = change || prevChange;

				return asset;
			}

			public T SaveAsset<T> (T asset, string savePath=null, string filename="Data", string type="asset", string caption="Save Data as Unity Asset") where T : UnityEngine.Object
			{
				#if UNITY_EDITOR
				if (savePath==null) savePath = UnityEditor.EditorUtility.SaveFilePanel(
					caption,
					"Assets",
					filename, 
					type);
				if (savePath!=null && savePath.Length!=0)
				{
					savePath = savePath.Replace(Application.dataPath, "Assets");

					UnityEditor.Undo.RecordObject(undoObject, undoName+" Save Data");
					
					UnityEditor.AssetDatabase.CreateAsset(asset, savePath);
					if (asset is ISerializationCallbackReceiver) ((ISerializationCallbackReceiver)asset).OnBeforeSerialize();
					UnityEditor.AssetDatabase.SaveAssets();

					UnityEditor.EditorUtility.SetDirty(undoObject);
					change = true;
				}
				#endif

				return asset;
			} 

			public void SaveRawBytes (byte[] bytes, string savePath=null, string filename="Data", string type="asset")
			{
				#if UNITY_EDITOR
				if (savePath==null) savePath = UnityEditor.EditorUtility.SaveFilePanel(
					"Save Data as Unity Asset",
					"Assets",
					filename, 
					type);
				if (savePath!=null && savePath.Length!=0)
				{
					savePath = savePath.Replace(Application.dataPath, "Assets");
					System.IO.File.WriteAllBytes(savePath, bytes);
				}
				#endif
			}

			private T ReleaseAsset<T> (T asset, string savePath=null) where T : ScriptableObject, ISerializationCallbackReceiver
			{
				#if UNITY_EDITOR
				UnityEditor.Undo.RecordObject(undoObject, undoName+" Release Data");
				UnityEditor.EditorUtility.SetDirty(undoObject);

				asset = ScriptableObject.Instantiate<T>(asset); 

				change = true;
				#endif

				return asset;
			}

			public T LoadAsset<T> (string label="Load Unity Asset", string[] filters=null) where T : UnityEngine.Object
			{
				#if UNITY_EDITOR
				if (filters==null && typeof(T).IsSubclassOf(typeof(Texture))) filters = new string[] { "Textures", "PSD,TIFF,TIF,JPG,TGA,PNG,GIF,BMP,IFF,PICT" };
				if (filters==null && typeof(T) == typeof(Transform)) filters = new string[] { "Meshes", "FBX,DAE,3DS,DXF,OBJ,SKP" };
				ArrayTools.Add(ref filters, "All files");
				ArrayTools.Add(ref filters, "*");

				string path= UnityEditor.EditorUtility.OpenFilePanelWithFilters(label, "Assets", filters);
				if (path!=null && path.Length!=0)
				{
					path = path.Replace(Application.dataPath, "Assets");
					T asset = (T)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(T));
					return asset;
				}
				#endif
				return null;
				
			}

			#pragma warning restore 0219,0414 
			
		#endregion

		#region DragAndDrop

			//enum MouseState { None, Down, Drag, Up }
			//MouseState mouseState;
			//Vector2 mousePos;

			public enum DragState { Pressed, Drag, Released }
			public DragState dragState;
			public Rect dragRect;
			public Vector2 dragPos;
			public Vector2 dragDelta; //drag pos relative to previous position
		
			public Vector2 dragOffset; //click position relative to field rect
			public int dragId = -2000000000;

			public bool DragDrop (Rect initialRect, int id, System.Action<Vector2,Rect> onDrag=null, System.Action<Vector2,Rect> onPress=null, System.Action<Vector2,Rect> onRelease=null)
			//actions args are mouse pos and dragged rect
			//both actions and if (DragDrop) switch (dragState) could be used
			{
				Vector2 mousePos = ToInternal(Event.current.mousePosition);

				
				//dragging
				if (id==dragId) dragState = DragState.Drag;

				//pressing
				if (Event.current.type==EventType.MouseDown && Event.current.button==0 && initialRect.Contains(mousePos))
				{
					dragOffset = new Vector2(initialRect.x, initialRect.y) - mousePos;
					dragId=id; //dragging = true;
					dragState = DragState.Pressed;
				}

				//releasing
				if (Event.current.rawType == EventType.MouseUp && id==dragId)
				{
					dragState = DragState.Released;
					//setting drag id in the end, we'll need it
				}


				//returning if not dragging
				if (id!=dragId) return false; //does not work on release

				//settin pos and rect
				dragDelta = mousePos-dragPos;
				dragPos = mousePos;
				dragRect = new Rect(mousePos.x+dragOffset.x, mousePos.y+dragOffset.y, initialRect.width, initialRect.height);
			
				//performing actions
				switch (dragState)
				{
					case DragState.Pressed: if (onPress!=null) onPress(dragPos, dragRect); break;
					case DragState.Drag: if (onDrag!=null) onDrag(dragPos, dragRect); break;
					case DragState.Released: if (onRelease!=null) onRelease(dragPos, dragRect); break;
				}

				#if UNITY_EDITOR
				if (Event.current.isMouse) Event.current.Use();
				if (UnityEditor.EditorWindow.focusedWindow != null) UnityEditor.EditorWindow.focusedWindow.Repaint(); 
				#endif

				if (dragState == DragState.Released) dragId=-2000000000; //dragging = false;
				return true;
			}

			public enum DragSide { right, left, top, bottom, rightTop, leftTop, rightBottom, leftBottom };
			public DragSide dragSide = DragSide.right;
			public Rect dragInitialRect = new Rect();

			public Rect ResizeRect (Rect rectBase, int id, int border=6, bool sideResize=true)
			{
				Rect rect = ToDisplay(rectBase);
				
				//bound rects
				Rect rightRect = new Rect(rect.x+rect.width-border/2, rect.y, border, rect.height);
				Rect leftRect = new Rect(rect.x-border/2, rect.y, border, rect.height);
				Rect topRect = new Rect(rect.x, rect.y-border/2, rect.width, border);
				Rect bottomRect = new Rect(rect.x, rect.y+rect.height-border/2, rect.width, border);

				Rect rightTopRect = new Rect(rect.x+rect.width-border, rect.y-border, border*2, border*2);
				Rect leftTopRect = new Rect(rect.x-border, rect.y-border, border*2, border*2);
				Rect rightBottomRect = new Rect(rect.x+rect.width-border, rect.y+rect.height-border, border*2, border*2);
				Rect leftBottomRect = new Rect(rect.x-border, rect.y+rect.height-border, border*2, border*2);
				
				//drawing cursor
				#if UNITY_EDITOR
				UnityEditor.EditorGUIUtility.AddCursorRect (rightTopRect, UnityEditor.MouseCursor.ResizeUpRight);
				UnityEditor.EditorGUIUtility.AddCursorRect (leftTopRect, UnityEditor.MouseCursor.ResizeUpLeft);
				UnityEditor.EditorGUIUtility.AddCursorRect (rightBottomRect, UnityEditor.MouseCursor.ResizeUpLeft);
				UnityEditor.EditorGUIUtility.AddCursorRect (leftBottomRect, UnityEditor.MouseCursor.ResizeUpRight);
				
				if (sideResize)
				{
					UnityEditor.EditorGUIUtility.AddCursorRect (rightRect, UnityEditor.MouseCursor.ResizeHorizontal);
					UnityEditor.EditorGUIUtility.AddCursorRect (leftRect, UnityEditor.MouseCursor.ResizeHorizontal);
					UnityEditor.EditorGUIUtility.AddCursorRect (topRect, UnityEditor.MouseCursor.ResizeVertical);
					UnityEditor.EditorGUIUtility.AddCursorRect (bottomRect, UnityEditor.MouseCursor.ResizeVertical);
				}
				#endif

				//pressing
				Vector2 mp = Event.current.mousePosition;
				bool anyRectsContains = rightTopRect.Contains(mp) || leftTopRect.Contains(mp) || rightBottomRect.Contains(mp) || leftBottomRect.Contains(mp);
				if (sideResize) anyRectsContains = anyRectsContains || rightRect.Contains(mp) || leftRect.Contains(mp) || topRect.Contains(mp) || bottomRect.Contains(mp);

				if (Event.current.type==EventType.MouseDown && anyRectsContains) 
				{ 
					dragId=id; 
					dragPos=Event.current.mousePosition; 
					dragInitialRect = rect;
					
					if (sideResize)
					{
						if (rightRect.Contains(mp)) dragSide=DragSide.right; 
						else if (leftRect.Contains(mp)) dragSide=DragSide.left; 
						else if (topRect.Contains(mp)) dragSide=DragSide.top; 
						else if (bottomRect.Contains(mp)) dragSide=DragSide.bottom; 
					}

					if (rightTopRect.Contains(mp)) dragSide=DragSide.rightTop; 
					if (leftTopRect.Contains(mp)) dragSide=DragSide.leftTop; 
					if (rightBottomRect.Contains(mp)) dragSide=DragSide.rightBottom; 
					if (leftBottomRect.Contains(mp)) dragSide=DragSide.leftBottom; 
				}

				//dragging
				if (id==dragId)
				{
					Vector2 dragDist = Event.current.mousePosition - dragPos;

					if (dragSide==DragSide.right || dragSide==DragSide.rightTop || dragSide==DragSide.rightBottom) rect.width = dragInitialRect.width + dragDist.x;
					if (dragSide==DragSide.left || dragSide==DragSide.leftTop || dragSide==DragSide.leftBottom) { rect.width = dragInitialRect.width - dragDist.x; rect.x = dragInitialRect.x + dragDist.x; }
					if (dragSide==DragSide.top || dragSide==DragSide.leftTop || dragSide==DragSide.rightTop) { rect.height = dragInitialRect.height - dragDist.y; rect.y = dragInitialRect.y + dragDist.y; }
					if (dragSide==DragSide.bottom || dragSide==DragSide.leftBottom || dragSide==DragSide.rightBottom) { rect.height = dragInitialRect.height + dragDist.y; }
				}

				//releasing
				if (Event.current.rawType==EventType.MouseUp && id==dragId) dragId = -2000000000;

				#if UNITY_EDITOR
				if (id==dragId) 
				{
					UnityEditor.EditorWindow.focusedWindow.Repaint();
					if (Event.current.isMouse) Event.current.Use();
				}
				#endif

				if (id==dragId) return ToInternal(rect);
				else return rectBase;
			}

		#endregion

		#region Layered


			public interface ILayered 
			{
				//T[] layers {get; set;} //no layers itself in interface. It could be two arrays, or an array and texture arrays index

				void OnLayerHeader (Layout layout, int num);
				void OnLayerGUI (Layout layout, int num);

				void AddLayer (int num);
				void RemoveLayer (int num);
				void SwitchLayers (int n1, int n2);

				int selected {get; set;}
				bool expanded {get; set;}

			}

			public Rect GetBackgroundRect (Action<Layout> onGUI, bool fullWidth=true)
			{
				//saving margins (margins changed during ongui can cause increasing odffset)
				int lMargn = margin;
				int rMargin = rightMargin;

				//rendering in markup mode
				Par(0, padding:0); //to make start and end cursor height 0
				Rect startCursor = cursor;
				bool prevMarkup = markup; 
				markup = true;
				onGUI(this); 
				markup = prevMarkup;
				Par(0, padding:0);
				Rect endCursor = cursor;
				
				//calculating rect
				Rect layerRect = new Rect(
					startCursor.x, 
					startCursor.y, 
					endCursor.x-startCursor.x, 
					endCursor.y-startCursor.y); //-1 is initial padding
				layerRect.y += field.y;



				//making rect full width
				if (fullWidth)
				{
					layerRect.x = field.x;// + margin;
					layerRect.width = field.width;// - margin - rightMargin;
				}

				//returning cursor
				cursor = startCursor;

				//restoring margins
				margin = lMargn;
				rightMargin = rMargin;

				return layerRect;
			}

			public void Layer (ILayered obj, int num,
				Val disabled = new Val(),
				bool displayFoldout = true,
				int expandHeaderBackground = 0,
				string tooltip=null)
			{
				CheckStyles();
//				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }
				if (markup) return; //exit on markup

				bool curIsSelected = obj.selected==num;
				bool curIsExpanded = obj.expanded && curIsSelected; //expanded num should not be changed unless layer is rendered twice

				//calculate rect
				Rect backgroundRect;
				if (curIsExpanded) backgroundRect = GetBackgroundRect( (Layout tmp) => { obj.OnLayerHeader(this, num); obj.OnLayerGUI(this, num); } );
				else backgroundRect = GetBackgroundRect( (Layout tmp) => { obj.OnLayerHeader(this, num); } );

				backgroundRect.x = field.x + margin; //rect.x;
				backgroundRect.width = field.width - margin - rightMargin; //rect.width;

				backgroundRect.height += expandHeaderBackground;

				//draw background
				#if UNITY_EDITOR
				if (curIsSelected) Element("DPLayout_LayerActive", backgroundRect, new RectOffset(4,4,4,4), new RectOffset(0,0,0,0)); 
				else Element("DPLayout_LayerInactive", backgroundRect, new RectOffset(4,4,4,4), new RectOffset(0,0,0,0));
				#endif

				obj.OnLayerHeader(this, num);

				//select
				float foldoutWidth = displayFoldout? 18 : 0;
				#if UNITY_EDITOR
				Rect selectRect = backgroundRect; selectRect.width -= foldoutWidth;
				if (Event.current.type==EventType.MouseDown && selectRect.Contains(ToInternal(Event.current.mousePosition))) 
				{
					if (!curIsSelected) UnityEditor.EditorGUI.FocusTextInControl(""); //stop writing on selection change
					obj.selected = num;
					Repaint(3);
				}
				#endif

				//foldout
				if (displayFoldout)
				{
					bool newIsExpanded = curIsExpanded;
					Rect foldoutRect = Inset(foldoutWidth); //drawing foldout where the cursor left	 //new Rect(rect.x+rect.width-foldoutWidth, rect.y, foldoutWidth, rect.height);
				
					#if UNITY_EDITOR
					UnityEditor.EditorGUIUtility.AddCursorRect (ToDisplay(foldoutRect), UnityEditor.MouseCursor.Link);
					#endif

					Icon(curIsExpanded? "DPLayout_ChevronDown" : "DPLayout_ChevronLeft", foldoutRect, horizontalAlign:IconAligment.center, verticalAlign:IconAligment.center);
					if (Event.current.type==EventType.MouseDown && foldoutRect.Contains(ToInternal(Event.current.mousePosition))) newIsExpanded = !curIsExpanded;

					if (newIsExpanded != curIsExpanded)
					{
						if (!curIsSelected && newIsExpanded) obj.selected = num; //selecting if it was not selected
						obj.expanded = newIsExpanded;
						Repaint(3);
					}
				}
				cursor.y += expandHeaderBackground;

				//drawing layer
				if (curIsExpanded) //with old expanded value
				{
					bool prevChange = change;
					change = false;

				//	Par(spaceBefore, margin:0);
				//	Par(0, margin:0);
					obj.OnLayerGUI(this, num); //with old selected value
				//	Par(spaceAfter, margin:0);
				//	Par(0, margin:0);

					if (change) lastChange = true;
					change = change || prevChange;
				}
			}


			public void LayerButtons (
				ILayered obj,
				int count, //number of layers, to clamp selection range
				string label = null,
				Rect rect = new Rect(),  
				bool addBeforeSelected = false,
				string tooltip=null)
			{
				if (rect.width < 0.9f && rect.height < 0.9f) { Par(); rect = Inset(); }
				if (markup) return; //exit on markup

				bool savedChange = change;
				change = false;

				float labelWidth = label!=null? rect.width - 25*4 : 0;
				if (label != null)
				{
					Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
					Label(label, labelRect);
				}

				Rect buttonRect = new Rect(rect.x+labelWidth, rect.y, 25, rect.height);
				if (Button(rect:buttonRect, icon:"DPLayout_Add", iconCenter:true, tooltip:"Add new array element"))
				{ 
					int addPos;
					if (!addBeforeSelected)
					{
						addPos = obj.selected + 1;
						if (obj.selected < 0) addPos = count; //if nothing selected adding to the end
					}
					else
					{
						addPos = obj.selected;
						if (obj.selected < 0) addPos = 0; //if nothing selected adding to the top
					}

					obj.AddLayer(addPos);

					obj.selected++;

					if (obj.selected >= 0)
					{
						if (addBeforeSelected) obj.selected++;
						obj.selected = Mathf.Clamp(obj.selected, 0, count-1);
					}
					change = true;

					#if UNITY_EDITOR
					UnityEditor.EditorGUI.FocusTextInControl("");
					#endif
				}

				buttonRect.x += 25;
				if (Button(rect:buttonRect, icon:"DPLayout_Remove", iconCenter:true, tooltip:"Remove element") && 
					obj.selected < count && obj.selected >= 0)
				{
					obj.RemoveLayer(obj.selected);

					obj.selected = Mathf.Clamp(obj.selected, 0, count-1);
					change = true;

					#if UNITY_EDITOR
					UnityEditor.EditorGUI.FocusTextInControl("");
					#endif
				}

				buttonRect.x += 25;
				if (Button(rect:buttonRect, icon:"DPLayout_Up", iconCenter:true, tooltip:"Move selected up") && 
					obj.selected < count && obj.selected >= 1)
				{  
					obj.SwitchLayers(obj.selected, obj.selected-1);

					obj.selected--; 
					obj.selected = Mathf.Clamp(obj.selected, 0, count-1);
					change = true;

					#if UNITY_EDITOR
					UnityEditor.EditorGUI.FocusTextInControl("");
					#endif
				}

				buttonRect.x += 25;
				if (Button(rect:buttonRect, icon:"DPLayout_Down", iconCenter:true, tooltip:"Move selected up") && 
					obj.selected < count-1 && obj.selected >= 0)
				{  
					obj.SwitchLayers(obj.selected, obj.selected+1);

					obj.selected++; 
					obj.selected = Mathf.Clamp(obj.selected, 0, count-1);
					change = true;

					#if UNITY_EDITOR
					UnityEditor.EditorGUI.FocusTextInControl("");
					#endif
				}

				lastChange = change;
				change = savedChange || change;
			}


			public void Foreground (Rect startAnchor, Rect endAnchor, int padding=3)
			{
				Vector2 start = startAnchor.position;
				Vector2 end = endAnchor.position + endAnchor.size;

				Element("DPLayout_FoldoutBackground", 
					new Rect(
						start.x-padding, 
						start.y-padding, 
						end.x-start.x + padding*2, 
						end.y-start.y + padding*2), 
					new RectOffset(6,6,6,6), null);
			}

			public void Foreground (Rect startAnchor, int padding=3)
			{
				Par(0,padding:0); Inset(1);
				Rect endAnchor = lastRect;
				
				Vector2 start = startAnchor.position;
				Vector2 end = endAnchor.position + endAnchor.size;

				Element("DPLayout_FoldoutBackground", 
					new Rect(
						start.x-padding, 
						start.y-padding, 
						end.x-start.x + padding*2, 
						end.y-start.y + padding*2), 
					new RectOffset(6,6,6,6), null);
			}

		#endregion

		#region Material
			
			public void MatKeyword (Material mat, string keyword, string label)
			{
				bool state = mat.IsKeywordEnabled(keyword);
				Toggle(ref state, label);
				if (lastChange) 
				{
					if (state) mat.EnableKeyword(keyword); 
					else mat.DisableKeyword(keyword);
				}
			}

			public void MatField<T> (Material mat, string name, string label=null, Rect rect = new Rect(), float min=-200000000, float max=200000000, bool limit=true, Val fieldSize = new Val(), Val sliderSize = new Val(), Val monitorChange = new Val(), Val useEvent = new Val(), Val disabled = new Val(), Val dragChange = new Val(), Val slider = new Val(), Val quadratic = new Val(), Val allowSceneObject = new Val(), Val delayed = new Val(), GUIStyle style=null, string tooltip=null, bool zwOfVector4=false)
			{ 
				if (mat==null || !mat.HasProperty(name)) return; //Field(default(T), label, disabled: true);
				Vector4 fullVector = new Vector4();

				T prop = default(T);
				if (typeof(T) == typeof(float)) prop = (T)(object)mat.GetFloat(name);
				else if (typeof(T) == typeof(int)) prop = (T)(object)mat.GetInt(name);
				else if (typeof(T) == typeof(Color)) prop = (T)(object)mat.GetColor(name);
				else if (typeof(T) == typeof(Vector2))
				{
					fullVector = mat.GetVector(name);
					if (!zwOfVector4) prop = (T)(object)new Vector2(fullVector.x, fullVector.y);
					else prop = (T)(object)new Vector2(fullVector.z, fullVector.w);
				}
				else if (typeof(T) == typeof(Vector4)) prop = (T)(object)mat.GetVector(name);
				else if (typeof(T) == typeof(Texture)) prop = (T)(object)mat.GetTexture(name);
				

				prop = Field<T> (prop, label, rect, min, max, limit, fieldSize, sliderSize, monitorChange, markup, useEvent, disabled, dragChange, slider, quadratic, allowSceneObject, delayed, style, tooltip); 


				if (lastChange)
				{
					if (typeof(T) == typeof(float)) mat.SetFloat(name, (float)(object)prop);
					else if (typeof(T) == typeof(int)) mat.SetInt(name, (int)(object)prop);
					else if (typeof(T) == typeof(Color)) mat.SetColor(name, (Color)(object)prop);
					else if (typeof(T) == typeof(Vector2))
					{
						Vector2 vec = (Vector2)(object)prop;
						if (!zwOfVector4) { fullVector.x=vec.x; fullVector.y=vec.y; }
						else { fullVector.z=vec.x; fullVector.w=vec.y; }
						mat.SetVector(name, fullVector);
					}
					else if (typeof(T) == typeof(Vector4) || typeof(T) == typeof(Vector2)) mat.SetVector(name, (Vector4)(object)prop);
					else if (typeof(T) == typeof(Texture)) mat.SetTexture(name, (Texture)(object)prop);
				}
			
			
			}

		#endregion
	}
}
