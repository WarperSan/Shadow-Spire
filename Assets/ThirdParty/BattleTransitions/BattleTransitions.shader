Shader "Hidden/BattleTransitions"
{
	Properties
	{
		_TransitionTex("Transition Texture", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0, 1)) = 0
		_Color ("Main Color", Color) = (0, 0, 0, 1)
	}
	SubShader
	{
		Tags
		{ 
			"Queue"="Overlay" 
			"RenderType"="Transparent"
		}
		
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			// Enabling alpha blending
			Blend SrcAlpha OneMinusSrcAlpha

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
				float2 uv1 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			float4 _MainTex_TexelSize;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv1 = v.uv;

				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv1.y = 1 - o.uv1.y;
				#endif

				return o;
			}

			sampler2D _TransitionTex;

			float _Cutoff;
			fixed4 _Color;

			// Fragment shader (calculates final color)
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = (1, 1, 1, 0);
				fixed4 transit = tex2D(_TransitionTex, i.uv1);

				if (transit.b < _Cutoff)
					return lerp(col, _Color, 1);
				return col;
			}					
			ENDCG
		}
	}
}
