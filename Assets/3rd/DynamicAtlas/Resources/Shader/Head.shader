Shader "Head"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_UVRect("UVRect", Vector) = (0, 0, 1, 1)
		_IsGray("Gray", int) = 1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
			#include "UnityCG.cginc"
            #pragma target 3.0
 
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
            	UNITY_VERTEX_INPUT_INSTANCE_ID
            };
 
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID 
			};
 
			sampler2D _MainTex;

            // float _Radius;
            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _UVRect)
            UNITY_DEFINE_INSTANCED_PROP(int, _IsGray)
            UNITY_INSTANCING_BUFFER_END(Props)
			
			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
            	UNITY_TRANSFER_INSTANCE_ID(v, o); // necessary only if you want to access instanced properties in the fragment Shader.
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 rect = UNITY_ACCESS_INSTANCED_PROP(Props, _UVRect);
				fixed2 newUV = i.uv * (rect.zw - rect.xy) + rect.xy;
				fixed4 color = tex2D(_MainTex, newUV);
				if(_IsGray == 1)
				{
					fixed c = color.r * 0.2989 + color.g * 0.587 + color.b * 0.114;
					return fixed4(c, c, c, color.a);
				}
				return color;
			}
            
			ENDCG
		}
	}
}