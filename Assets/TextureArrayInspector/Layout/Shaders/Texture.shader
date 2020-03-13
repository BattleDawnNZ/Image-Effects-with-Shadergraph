Shader "Hidden/DPLayout/Texture"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Mode ("Mode", Int) = 0
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
			int _Mode;
			float _GammaGray;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2Dlod(_MainTex, float4(i.uv,0,0));

				if (_Mode == 0) 
				{
					//if (_GammaGray < 0.45) col.rgb = pow(col.rgb, 0.454545);
				}

				if (_Mode == 1) col = col.a;

				if (_Mode == 2) 
				{
					float3 norm = 0;
					norm.xy = col.wy * 2 - 1;
					norm.z = sqrt(1 - saturate(dot(norm.xy, norm.xy)));
					col = float4(norm/2 + 0.5, 1);
				}

				col.a = 1;
				return col;
			}
			ENDCG
		}
	}
}

