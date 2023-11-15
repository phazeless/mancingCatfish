Shader "Shaders 102/Displacement"
{
  Properties
  {
    _MainTex ("Texture", 2D) = "white" {}
    _DisplaceTex ("Displacement Texture", 2D) = "white" {}
    _Magnitude ("Magnitude", Range(-0.2, 0.2)) = 1
  }
  SubShader
  {
    Tags
    { 
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
      }
      ZTest Always
      ZWrite Off
      Cull Off
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform sampler2D _MainTex;
      uniform sampler2D _DisplaceTex;
      uniform float _Magnitude;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 tmpvar_1;
          tmpvar_1.w = 1;
          tmpvar_1.xyz = float3(in_v.vertex.xyz);
          out_v.xlv_TEXCOORD0 = in_v.texcoord.xy;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_1));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 col_1;
          float2 disp_2;
          float2 tmpvar_3;
          tmpvar_3 = tex2D(_DisplaceTex, in_f.xlv_TEXCOORD0).xy.xy;
          disp_2 = tmpvar_3;
          disp_2 = (((disp_2 * 2) - 1) * _Magnitude);
          float4 tmpvar_4;
          float2 P_5;
          P_5 = (in_f.xlv_TEXCOORD0 + disp_2);
          tmpvar_4 = tex2D(_MainTex, P_5);
          col_1 = tmpvar_4;
          out_f.color = col_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
