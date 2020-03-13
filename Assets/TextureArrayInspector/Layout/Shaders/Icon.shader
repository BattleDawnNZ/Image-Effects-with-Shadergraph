Shader "Hidden/DPLayout/TextureIcon"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Roundness ("Roundness", Range(0.0, 1.0)) = 0.25
		_Borders ("Borders", Range(0.0, 1.0)) = 0.025
		_Crispness ("Crispness", Float) = 25
		[Gamma] _GammaGray("Gray", Range(0.0, 1.0)) = 0.5
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _Roundness;
			float _Borders;
			float _Crispness;
			float _GammaGray;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2Dlod(_MainTex, float4(i.uv,0,0));

				float2 halfUV = i.uv;
				if (i.uv.x > 0.5) halfUV.x = 1 - i.uv.x;
				if (i.uv.y > 0.5) halfUV.y = 1 - i.uv.y;
				
				float dist = min(halfUV.x, halfUV.y);

				//roundness
				if (halfUV.x < _Roundness && halfUV.y < _Roundness)
				{
					float2 roundnessPivot = float2(_Roundness, _Roundness);
					float roundnessDist = distance(halfUV, roundnessPivot);
					dist = _Roundness-roundnessDist;
				}
				col.a = saturate(dist*_Crispness);

				//borders
				col.rgb = col.rgb * saturate((dist-_Borders)*_Crispness);

				//if (_GammaGray < 0.45) col.rgb = pow(col.rgb, 0.454545);

				return col;
			}
			ENDCG
		}
	}
}

