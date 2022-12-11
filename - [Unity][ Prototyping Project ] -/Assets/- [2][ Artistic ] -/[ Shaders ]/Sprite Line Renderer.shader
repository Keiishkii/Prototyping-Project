Shader "Keiishkii/Unlit/Sprite Line Renderer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Colour ("Colour", Color) = (1, 1, 1)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha 
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            // Data
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VertexToFragment
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Parameters
            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Colour;



            

            VertexToFragment vert (appdata vertex)
            {
                VertexToFragment output;
                
                output.vertex = UnityObjectToClipPos(vertex.vertex);
                output.uv = TRANSFORM_TEX(vertex.uv, _MainTex);
                
                return output;
            }

            fixed4 frag (VertexToFragment input) : SV_Target
            {
                float4 textureColour = tex2D(_MainTex, input.uv);
                float4 colour = textureColour * _Colour;
                
                return colour;
            }
            ENDCG
        }
    }
}
