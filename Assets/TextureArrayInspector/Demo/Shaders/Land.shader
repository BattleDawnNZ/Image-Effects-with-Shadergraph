Shader "Voxeland/Land" 
{
	Properties 
	{
		

		[Enum(Off, 0, Front, 1, Back, 2)] _Culling("Culling", Int) = 2 

		_MainTexArr("Tex2D Array: Albedo (RGB), Height (A)", 2DArray) = "" {}
		_BumpMapArr("Tex 2D Array: Normals", 2DArray) = "" {}

		[HideInInspector] _MainTex0("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap0("Normals", 2D) = "bump" {}	[HideInInspector] _Height0("Height", Float) = 1		[HideInInspector] _SpecParams0("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex1("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap1("Normals", 2D) = "bump" {}	[HideInInspector] _Height1("Height", Float) = 1		[HideInInspector] _SpecParams1("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex2("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap2("Normals", 2D) = "bump" {}	[HideInInspector] _Height2("Height", Float) = 1		[HideInInspector] _SpecParams2("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex3("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap3("Normals", 2D) = "bump" {}	[HideInInspector] _Height3("Height", Float) = 1		[HideInInspector] _SpecParams3("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex4("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap4("Normals", 2D) = "bump" {}	[HideInInspector] _Height4("Height", Float) = 1		[HideInInspector] _SpecParams4("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex5("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap5("Normals", 2D) = "bump" {}	[HideInInspector] _Height5("Height", Float) = 1		[HideInInspector] _SpecParams5("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex6("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap6("Normals", 2D) = "bump" {}	[HideInInspector] _Height6("Height", Float) = 1		[HideInInspector] _SpecParams6("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex7("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap7("Normals", 2D) = "bump" {}	[HideInInspector] _Height7("Height", Float) = 1		[HideInInspector] _SpecParams7("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex8("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap8("Normals", 2D) = "bump" {}	[HideInInspector] _Height8("Height", Float) = 1		[HideInInspector] _SpecParams8("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex9("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap9("Normals", 2D) = "bump" {}	[HideInInspector] _Height9("Height", Float) = 1		[HideInInspector] _SpecParams9("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex10("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap10("Normals", 2D) = "bump" {}	[HideInInspector] _Height10("Height", Float) = 1	[HideInInspector] _SpecParams10("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex11("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap11("Normals", 2D) = "bump" {}	[HideInInspector] _Height11("Height", Float) = 1	[HideInInspector] _SpecParams11("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex12("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap12("Normals", 2D) = "bump" {}	[HideInInspector] _Height12("Height", Float) = 1	[HideInInspector] _SpecParams12("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex13("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap13("Normals", 2D) = "bump" {}	[HideInInspector] _Height13("Height", Float) = 1	[HideInInspector] _SpecParams13("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex14("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap14("Normals", 2D) = "bump" {}	[HideInInspector] _Height14("Height", Float) = 1	[HideInInspector] _SpecParams14("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex15("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap15("Normals", 2D) = "bump" {}	[HideInInspector] _Height15("Height", Float) = 1	[HideInInspector] _SpecParams15("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex16("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap16("Normals", 2D) = "bump" {}	[HideInInspector] _Height16("Height", Float) = 1	[HideInInspector] _SpecParams16("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex17("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap17("Normals", 2D) = "bump" {}	[HideInInspector] _Height17("Height", Float) = 1	[HideInInspector] _SpecParams17("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex18("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap18("Normals", 2D) = "bump" {}	[HideInInspector] _Height18("Height", Float) = 1	[HideInInspector] _SpecParams18("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		[HideInInspector] _MainTex19("Albedo (RGB)", 2D) = "black" {}		[HideInInspector] _BumpMap19("Normals", 2D) = "bump" {}	[HideInInspector] _Height19("Height", Float) = 1	[HideInInspector] _SpecParams19("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)



		//arrays are not placed into the properties block, and are instead declared within the CGPROGRAM block with the uniform keyword
		//and they are NOT SERIALIZED
		//_HeightArr("Height", Float[24]) = []  
		//_SpecParamsArr("SpecParams", Vector[24]) = []

		//so using this clumsy hack
		//_Height0("Height", Float) = 1	_Height1("Height", Float) = 1	_Height2("Height", Float) = 1	_Height3("Height", Float) = 1	_Height4("Height", Float) = 1	_Height5("Height", Float) = 1	_Height6("Height", Float) = 1	_Height7("Height", Float) = 1	_Height8("Height", Float) = 1	_Height9("Height", Float) = 1	
		//_Height10("Height", Float) = 1	_Height11("Height", Float) = 1	_Height12("Height", Float) = 1	_Height13("Height", Float) = 1	_Height14("Height", Float) = 1	_Height15("Height", Float) = 1	_Height16("Height", Float) = 1	_Height17("Height", Float) = 1	_Height18("Height", Float) = 1	_Height19("Height", Float) = 1
		//_Height20("Height", Float) = 1	_Height21("Height", Float) = 1	_Height22("Height", Float) = 1	_Height23("Height", Float) = 1	

		//_SpecParams0("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)		_SpecParams1("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)		_SpecParams2("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)		_SpecParams3("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)		_SpecParams4("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)		_SpecParams5("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)		_SpecParams6("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)		_SpecParams7("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)		_SpecParams8("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)		_SpecParams9("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		//_SpecParams10("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)	_SpecParams11("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)	_SpecParams12("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)	_SpecParams13("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)	_SpecParams14("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)	_SpecParams15("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)	_SpecParams16("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)	_SpecParams17("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)	_SpecParams18("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)	_SpecParams19("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)
		//_SpecParams20("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)	_SpecParams21("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)	_SpecParams22("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)	_SpecParams23("SpecParams", Vector) = (-0.75,0.25,-0.35,1.5)


		_Tile("Tile", Float) = 0.1
		//_TriplanarCrisp("Triplanar Crispness", Float) = 0.1
		_FarTile("Far Tile", Float) = 0.01
		_FarStart("Far Transition Start", Float) = 10
		_FarEnd("Far Transition End", Float) = 100

		_BlendMapFactor("Blend Map Factor", Float) = 3
		_BlendCrisp("Blend Crispness", Float) = 2

		_Mips("MipMap Factor", Float) = 0.4
		_AmbientOcclusion("Ambient Occlusion", Float) = 1

		_PreviewType("Preview Type", Int) = 0

		_HorizonHeightScale("Horizon Height Scale", Float) = 200
		_HorizonHeightmap("Horizon Heightmap", 2D) = "black" {}
		_HorizonTypemap("Horizon Typemap", 2D) = "black" {}
		_HorizonVisibilityMap("Horizon Visibility Map", 2D) = "white" {}
		_HorizonBorderLower("Horizon Border Lower", Float) = 2

		//_Applied("tmp", Float) = 0
	}
	SubShader 
	{
		Cull[_Culling]
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM

		//#pragma shader_feature _TRIPLANAR
		//#pragma shader_feature _DOUBLELAYER
		#pragma shader_feature _PREVIEW
		#pragma shader_feature _HORIZON
		#pragma shader_feature _TEX2DARRAY

		//#pragma only_renderers d3d11
		#pragma surface surf StandardSpecular addshadow fullforwardshadows vertex:Vert nolightmap  //should be outside "if defined dx11"

		#pragma target 4.0


		struct Input
		{
			float3 wNormal;
			float3 wPos;
			half ambient;
			//float blends[24];
			float test;

			fixed4 blendsA;
			fixed4 blendsB;
			fixed4 blendsC;
			fixed4 blendsD;
			fixed4 blendsE;
			fixed4 blendsF;

			#if _HORIZON
				half2 wTexcoord; //should be float, but it does not work in DX9
				half visibility;
			#endif
		};


		#if _TEX2DARRAY
			#if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X) || defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE) || defined(SHADER_API_METAL) || defined(SHADER_API_VULKAN) || defined(SHADER_API_PS4) || defined(SHADER_API_XBOXONE) //these platforms support texture arrays. Second #if: texarr shader should try using texarrs no mater of what
			Texture2DArray _MainTexArr;		///UNITY_DECLARE_TEX2DARRAY(_MainTexArr);
			SamplerState sampler_MainTexArr;
			Texture2DArray _BumpMapArr;
			#endif
		#else
			#if SHADER_API_D3D11 || SHADER_API_D3D11_9X || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN || SHADER_API_D3D11_9X || SHADER_API_PS4 || SHADER_API_XBOXONE || SHADER_API_PSP2 || SHADER_API_WIIU  //these platforms support samples
			Texture2D _MainTex0, _MainTex1, _MainTex2, _MainTex3;
			SamplerState sampler_MainTex0;
			Texture2D _BumpMap0, _BumpMap1, _BumpMap2, _BumpMap3;
			#endif
		#endif



		float _Height0, _Height1, _Height2, _Height3, _Height4, _Height5, _Height6, _Height7, _Height8, _Height9, _Height10, _Height11, _Height12, _Height13, _Height14, _Height15, _Height16, _Height17, _Height18, _Height19, _Height20, _Height21, _Height22, _Height23;
		float4 _SpecParams0, _SpecParams1, _SpecParams2, _SpecParams3, _SpecParams4, _SpecParams5, _SpecParams6, _SpecParams7, _SpecParams8, _SpecParams9, _SpecParams10, _SpecParams11, _SpecParams12, _SpecParams13, _SpecParams14, _SpecParams15, _SpecParams16, _SpecParams17, _SpecParams18, _SpecParams19, _SpecParams20, _SpecParams21, _SpecParams22, _SpecParams23;


		float _Tile;
		float _TriplanarCrisp;
		float _FarTile; 
		float _FarStart; 
		float _FarEnd;

		float _BlendMapFactor;
		float _BlendCrisp;
		float _Mips;
		float _AmbientOcclusion;

		half _PreviewType;

		#if _HORIZON
			float _HorizonHeightScale;
			sampler2D _HorizonHeightmap;
			sampler2D _HorizonTypemap;
			sampler2D _HorizonVisibilityMap; 
			float _HorizonBorderLower;
		#endif

		uniform float _Applied[1];



		inline float4 GetTangent(float3 worldNormal)
		{
			float4 tangent;
			float3 absWorldNormal = abs(worldNormal);

			if (absWorldNormal.z >= absWorldNormal.x && absWorldNormal.z >= absWorldNormal.y)
			{
				if (worldNormal.z>0) tangent = float4(-1, 0, 0, -1);
				else tangent = float4(1, 0, 0, -1);
			}
			else if (absWorldNormal.y >= absWorldNormal.x && absWorldNormal.y >= absWorldNormal.z)
			{
				if (worldNormal.y>0) tangent = float4(0, 0, -1, -1);
				else tangent = float4(0, 0, 1, -1);
			}
			else //if (absWorldNormal.x >= absWorldNormal.x && absWorldNormal.y >= absWorldNormal.z)
			{
				if (worldNormal.x>0) tangent = float4(0, 0, 1, -1);
				else tangent = float4(0, 0, -1, -1);
			}
			return tangent;
		}


		void Vert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);

			data.blendsA = half4(
				((int)v.tangent.x >> 0) & 0xF,
				((int)v.tangent.x >> 4) & 0xF,
				((int)v.tangent.x >> 8) & 0xF,
				((int)v.tangent.x >> 12) & 0xF) / 16;
			data.blendsB = half4(
				((int)v.tangent.x >> 16) & 0xF,
				((int)v.tangent.x >> 20) & 0xF,
				((int)v.tangent.y >> 0) & 0xF,
				((int)v.tangent.y >> 4) & 0xF) / 16;
			data.blendsC = half4(
				((int)v.tangent.y >> 8) & 0xF,
				((int)v.tangent.y >> 12) & 0xF,
				((int)v.tangent.y >> 16) & 0xF,
				((int)v.tangent.y >> 20) & 0xF) / 16;
			data.blendsD = half4(
				((int)v.tangent.z >> 0) & 0xF,
				((int)v.tangent.z >> 4) & 0xF,
				((int)v.tangent.z >> 8) & 0xF,
				((int)v.tangent.z >> 12) & 0xF) / 16;
			data.blendsE = half4(
				((int)v.tangent.z >> 16) & 0xF,
				((int)v.tangent.z >> 20) & 0xF,
				((int)v.tangent.w >> 0) & 0xF,
				((int)v.tangent.w >> 4) & 0xF) / 16;
			data.blendsF = half4(
				((int)v.tangent.w >> 8) & 0xF,
				((int)v.tangent.w >> 12) & 0xF,
				((int)v.tangent.w >> 16) & 0xF,
				((int)v.tangent.w >> 20) & 0xF) / 16;

			//pos, normal, tangent, ambient
			data.wPos = mul(unity_ObjectToWorld, v.vertex);
			data.wNormal = normalize(mul(unity_ObjectToWorld, float4(v.normal, 0))); //world normal
			v.tangent = GetTangent(data.wNormal); //vertex tangent
			data.ambient = v.texcoord3.x; 

			#if _HORIZON
				//height
				half4 heightColor = tex2Dlod(_HorizonHeightmap, float4(v.texcoord.xy, 0, 0));
				v.vertex.y = (heightColor.r*250 + heightColor.g)*256;

				//visibility and border
				float4 visibilityDirs = float4(
					tex2Dlod(_HorizonVisibilityMap, float4(v.texcoord.x+0.001, v.texcoord.y, 0, 0)).a,
					tex2Dlod(_HorizonVisibilityMap, float4(v.texcoord.x-0.001, v.texcoord.y, 0, 0)).a,
					tex2Dlod(_HorizonVisibilityMap, float4(v.texcoord.x, v.texcoord.y+0.001, 0, 0)).a,
					tex2Dlod(_HorizonVisibilityMap, float4(v.texcoord.x, v.texcoord.y-0.001, 0, 0)).a ); 
				data.wPos.x += (visibilityDirs.x - visibilityDirs.y)*_HorizonBorderLower;
				data.wPos.z += (visibilityDirs.z - visibilityDirs.w)*_HorizonBorderLower;

				data.visibility = (visibilityDirs.x+ visibilityDirs.y+ visibilityDirs.z+ visibilityDirs.w)*4; //if >0 then visible, if <1 then border
				if (data.visibility < 0.999) v.vertex.y -= _HorizonBorderLower * (1-data.visibility);

				//types
				int type = tex2Dlod(_HorizonTypemap, float4(v.texcoord.xy, 0, 0)).a * 256;
				data.blendsA = half4(type==0? 1:0, type==1? 1:0, type==2? 1:0, type==3? 1:0);
				data.blendsB = half4(type==4? 1:0, type==5? 1:0, type==6? 1:0, type==7? 1:0);
				data.blendsC = half4(type==8? 1:0, type==9? 1:0, type==10? 1:0, type==11? 1:0);
				data.blendsD = half4(type==12? 1:0, type==13? 1:0, type==14? 1:0, type==15? 1:0);
				data.blendsE = half4(type==16? 1:0, type==17? 1:0, type==18? 1:0, type==19? 1:0);
				data.blendsF = half4(type==20? 1:0, type==21? 1:0, type==22? 1:0, type==23? 1:0);

				//uv
				data.wTexcoord = v.texcoord;

				//filling ambient with white
				data.ambient = 1;
			#endif
		}





		inline half4 SoftLight(half4 a, half4 b, float percent)
		{
			half4 sl = (a + b) / 2; //(1 - 2*b)*a*a + 2*a*b;

			float bPercent = saturate((percent - 0.5) * 2);
			float aPercent = saturate(((1 - percent) - 0.5) * 2);

			return a*aPercent + b*bPercent + sl*(1 - aPercent - bPercent);
		}

		inline void Get4TopBlends(int currNum, fixed currVal, inout int4 topNums, inout fixed4 topVals)
		{
			if (currVal > topVals.w) { topNums.x = currNum; topVals.w = currVal; }

			//sorting topVals
			fixed tempVal; int tempNum;
			if (topVals.w > topVals.z) { tempVal = topVals.w; topVals.w = topVals.z; topVals.z = tempVal;  tempNum = topNums.w; topNums.w = topNums.z; topNums.z = tempNum; }
			if (topVals.z > topVals.y) { tempVal = topVals.z; topVals.z = topVals.y; topVals.y = tempVal;  tempNum = topNums.z; topNums.z = topNums.y; topNums.y = tempNum; }
			if (topVals.y > topVals.x) { tempVal = topVals.y; topVals.y = topVals.x; topVals.x = tempVal;  tempNum = topNums.y; topNums.y = topNums.x; topNums.x = tempNum; }
		}



		inline float4 SampleMainTex (float2 uv, int ch)
		{
			#if _TEX2DARRAY
				#if SHADER_API_D3D11 || SHADER_API_D3D11_9X || SHADER_API_GLES3 || SHADER_API_GLCORE || SHADER_API_METAL || SHADER_API_VULKAN || SHADER_API_PS4 || SHADER_API_XBOXONE //these platforms support texture arrays
				return _MainTexArr.SampleBias(sampler_MainTexArr, float3(uv, ch), _Mips);
				#endif

			#else
				#if SHADER_API_D3D11 || SHADER_API_D3D11_9X || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN || SHADER_API_D3D11_9X || SHADER_API_PS4 || SHADER_API_XBOXONE || SHADER_API_PSP2 || SHADER_API_WIIU  //these platforms support samples
				switch (ch)
				{
					
					case 0: return _MainTex0.SampleBias(sampler_MainTex0, uv, _Mips);	
					case 1: return _MainTex0.SampleBias(sampler_MainTex0, uv, _Mips);	
					case 2: return _MainTex2.SampleBias(sampler_MainTex0, uv, _Mips);	
					case 3: return _MainTex3.SampleBias(sampler_MainTex0, uv, _Mips);
					default: return half4(0, 0, 0, 0.5);
				}
				#else
				return half4(0, 0, 0, 0.5);
				#endif
			#endif

			return half4(0, 0, 0, 0.5);
		}

		inline float4 SampleBumpMap (float2 uv, int ch)
		{
			#if _TEX2DARRAY
				#if SHADER_API_D3D11 || SHADER_API_D3D11_9X || SHADER_API_GLES3 || SHADER_API_GLCORE || SHADER_API_METAL || SHADER_API_VULKAN || SHADER_API_PS4 || SHADER_API_XBOXONE //these platforms support texture arrays
				return _BumpMapArr.SampleBias(sampler_MainTexArr, float3(uv, ch), _Mips);
				#endif

			#else
				#if SHADER_API_D3D11 || SHADER_API_D3D11_9X || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN || SHADER_API_D3D11_9X || SHADER_API_PS4 || SHADER_API_XBOXONE || SHADER_API_PSP2 || SHADER_API_WIIU  //these platforms support samples
				switch (ch)
				{
					case 0: return _BumpMap0.SampleBias(sampler_MainTex0, uv, _Mips);	
					case 1: return _BumpMap1.SampleBias(sampler_MainTex0, uv, _Mips);	
					case 2: return _BumpMap2.SampleBias(sampler_MainTex0, uv, _Mips);	
					case 3: return _BumpMap3.SampleBias(sampler_MainTex0, uv, _Mips);
					default: return half4(0, 0, 0, 0.5);
				}
				#else
				return half4(0, 0, 0, 0.5);
				#endif
			#endif

			return half4(0, 0, 0, 0.5);
		}



		void surf (Input IN, inout SurfaceOutputStandardSpecular o) 
		{
			//getting blend
			fixed blends[24] = { IN.blendsA.r, IN.blendsA.g, IN.blendsA.b, IN.blendsA.a, IN.blendsB.r, IN.blendsB.g, IN.blendsB.b, IN.blendsB.a, IN.blendsC.r, IN.blendsC.g, IN.blendsC.b, IN.blendsC.a, IN.blendsD.r, IN.blendsD.g, IN.blendsD.b, IN.blendsD.a, IN.blendsE.r, IN.blendsE.g, IN.blendsE.b, IN.blendsE.a, IN.blendsF.r, IN.blendsF.g, IN.blendsF.b, IN.blendsF.a };

			float heights[24] = {_Height0, _Height1, _Height2, _Height3, _Height4, _Height5, _Height6, _Height7, _Height8, _Height9, _Height10, _Height11, _Height12, _Height13, _Height14, _Height15, _Height16, _Height17, _Height18, _Height19, _Height20, _Height21, _Height22, _Height23 };
			float4 specParams[24] = {_SpecParams0, _SpecParams1, _SpecParams2, _SpecParams3, _SpecParams4, _SpecParams5, _SpecParams6, _SpecParams7, _SpecParams8, _SpecParams9, _SpecParams10, _SpecParams11, _SpecParams12, _SpecParams13, _SpecParams14, _SpecParams15, _SpecParams16, _SpecParams17, _SpecParams18, _SpecParams19, _SpecParams20, _SpecParams21, _SpecParams22, _SpecParams23 };
			//TODO: seems to have no affect to performance, but have to remove it when Unity will be able to serialize material arrays

			//finding camera distance (for 2 layer blend)
			#if (_DOUBLELAYER)
			float dist = distance(IN.wPos, _WorldSpaceCameraPos);
			float doublelayerPercent = (dist - _FarStart) / (_FarEnd - _FarStart);
			#endif

			//switching directions to avoid texture inversion
			float3 pos = IN.wPos*_Tile;
			float2 yUV = float2(IN.wNormal.y>0 ? -pos.z : pos.z, pos.x);
			float2 zUV = float2(IN.wNormal.z>0 ? -pos.x : pos.x, pos.y);
			float2 xUV = float2(IN.wNormal.x<0 ? -pos.z : pos.z, pos.y);

			//absolute directions
			float3 absDirections = abs( pow(IN.wNormal, 16) );  //pow(norm,_TriplanarCrisp) does not work
			absDirections /= absDirections.x + absDirections.y + absDirections.z;

			absDirections = saturate(absDirections - 0.1);
			absDirections /= absDirections.x + absDirections.y + absDirections.z;
		
			//preparing values sum
			float4 totalAlbedo = 0;
			float4 totalNormal = 0;
			float4 totalSpecParams = 0;
			float blendSum = 0;

			//per-layer sampling
			for (int i = 0; i<24; i++)
				if (blends[i] > 0.000001)
				{
					//sampling textures
					half4 currentAlbedo = half4(0, 0, 0, 0);
					half4 currentNormal = half4(0, 0, 0, 0);

					//sampling main layer triplanar usual way
					#if !HORIZON
						if (absDirections.y > 0.00001)
						{
							currentAlbedo += SampleMainTex(yUV, i) * absDirections.y;
							currentNormal += SampleBumpMap(yUV, i) * absDirections.y;
						}

						if (absDirections.z > 0.00001)
						{
							currentAlbedo += SampleMainTex(zUV, i) * absDirections.z;
							currentNormal += SampleBumpMap(zUV, i) * absDirections.z;
						}

						if (absDirections.x > 0.00001)
						{
							currentAlbedo += SampleMainTex(xUV, i) * absDirections.x;
							currentNormal += SampleBumpMap(xUV, i) * absDirections.x;
						}
					#endif

					//additional sampling far layer triplanar
					#if (_DOUBLELAYER && !HORIZON)
						half4 far_currentAlbedo = half4(0, 0, 0, 0);
						half4 far_currentNormal = half4(0, 0, 0, 0);

						if (doublelayerPercent > 0.000001)
						{
							if (absDirections.y > 0.00001)
							{
								far_currentAlbedo += SampleMainTex(yUV*_Tile, i) * absDirections.y;
								far_currentNormal += SampleBumpMap(yUV*_Tile, i) * absDirections.y;
							}

							if (absDirections.z > 0.00001)
							{
								far_currentAlbedo += SampleMainTex(zUV*_Tile, i) * absDirections.z;
								far_currentNormal += SampleBumpMap(zUV*_Tile, i) * absDirections.z;
							}

							if (absDirections.x > 0.00001)
							{
								far_currentAlbedo += SampleMainTex(xUV*_Tile, i) * absDirections.x;
								far_currentNormal += SampleBumpMap(xUV*_Tile, i) * absDirections.x;
							}
						}

						//mixing in far layer
						currentAlbedo = far_currentAlbedo*doublelayerPercent + currentAlbedo*(1- doublelayerPercent);
						currentNormal = far_currentNormal*doublelayerPercent + currentNormal*(1- doublelayerPercent);
					#endif

					//sampling horizon
					#if HORIZON
						currentAlbedo = SampleDiffuse(yUV, i); // _MainTexArr.SampleGrad(sampler_MainTexArr, float3(yUV, i), yDDX, yDDY);
						currentNormal = SampleBump(yUV, i); // _BumpMapArr.SampleGrad(sampler_MainTexArr, float3(yUV, i), yDDX, yDDY);
					#endif

					//converting vertex blend to height blend
					fixed blend = blends[i];
					float vb = blend * _BlendMapFactor*_BlendCrisp * 2; //_BlendMapFactor*_BlendCrisp*2 to avoid too low values
					float hm = pow(currentAlbedo.a*heights[i], _BlendMapFactor) * _BlendMapFactor*_BlendCrisp * 2; //pow(currentAlbedos[i].a*heights[i], _BlendMapFactor) * _BlendMapFactor*_BlendCrisp*2;
					blend = vb*hm; //vb*vb + 2 * vb*hm*(1 - vb);
					blend = pow(blend, _BlendCrisp);
					blends[i] = blend; //for debug purpose

					//adding height blended values
					totalAlbedo += currentAlbedo * blend;
					totalNormal += currentNormal * blend;
					totalSpecParams += specParams[i] * blend;

					blendSum += blend;
				}

			//normalizing blend
			o.Albedo = totalAlbedo.rgb / blendSum;
			o.Normal = UnpackNormal(totalNormal / blendSum);
			totalSpecParams /= blendSum;
			
			//calculating specular
			float albedoGrayscale = dot(o.Albedo, float3(0.3, 0.58, 0.12)); // RGB to Grayscale magic numbers
			//o.Metallic = saturate(albedoGrayscale*totalSpecParams.y + totalSpecParams.x);
			o.Specular = saturate(albedoGrayscale*totalSpecParams.y + totalSpecParams.x);
			o.Smoothness = saturate(albedoGrayscale*totalSpecParams.w + totalSpecParams.z);

			o.Occlusion = 1 - _AmbientOcclusion + IN.ambient*_AmbientOcclusion;

			//horizon mesh normal
			#if _HORIZON
				//calculating normal
				half4 heightColor = tex2D(_HorizonHeightmap, IN.wTexcoord);

				//visibility
				if (IN.visibility<0.01 || heightColor.r+heightColor.g<0.01) clip(-1);

				float3 baseNormal = float3(0,0,1);
				baseNormal.x = (heightColor.a - 0.5f)*2;
				baseNormal.y = -(heightColor.b - 0.5f)*2;
				baseNormal.z = sqrt(1 - saturate(dot(o.Normal.xy, o.Normal.xy)));

				//add to existing one
				o.Normal = baseNormal + float3(o.Normal.x, o.Normal.y, 0);
				o.Normal = normalize(o.Normal);
			#endif

			#if _PREVIEW
			if (_PreviewType != 0)
			{
				if (_PreviewType == 1) o.Emission = o.Albedo;
				if (_PreviewType == 2) o.Emission = o.Occlusion;
				if (_PreviewType == 3) o.Emission = float3(totalNormal.g/blendSum, totalNormal.a/blendSum, 0); //IN.wNormal / 2 + 0.5;
				if (_PreviewType == 4) o.Emission = o.Specular;
				if (_PreviewType == 5) o.Emission = o.Smoothness;
				if (_PreviewType == 6) o.Emission = absDirections;
				if (_PreviewType == 7) o.Emission = IN.blendsA;
				if (_PreviewType == 8) o.Emission = float3(blends[0], blends[1], blends[2]);
				if (_PreviewType == 9) o.Emission = frac(IN.wPos);

				#if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X) || defined(SHADER_API_GLES3)
				if (_PreviewType == 10) o.Emission = half3(1,0,0);
				#elif defined(SHADER_API_GLCORE)
				if (_PreviewType == 10) o.Emission = half3(0,1,0);
				#else
				if (_PreviewType == 10) o.Emission = half3(0,0,1);
				#endif

				if (_PreviewType != 0)
				{
					o.Alpha = 0;
					o.Albedo = 0;
					//o.Metallic = 0;
					o.Specular = 0;
					o.Smoothness = 1;
					o.Occlusion = 0;
					//o.Normal = 0;
				}
			}
			#endif
		}
		






		ENDCG
	}
	FallBack "Diffuse"
	//CustomEditor "LandMaterialInspector"
}
