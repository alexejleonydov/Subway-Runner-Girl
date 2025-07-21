// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Distorted/Additive (falloff)"
{
  Properties
  {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _MainColor ("Color (RGBC)", Color) = (1,1,1,0)
    _Falloff ("Falloff Distance", float) = 200
  }
  SubShader
  {
    Tags
    { 
      "QUEUE" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "QUEUE" = "Transparent"
      }
      ZClip Off
      ZWrite Off
      Fog
      { 
        Mode  Off
      } 
      Blend One One
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform float4 _MainTex_ST;
      uniform float4 _Distort;
      uniform float _Falloff;
      uniform sampler2D _MainTex;
      uniform float _Factor;
      uniform float4 _MainColor;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_COLOR :COLOR;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_COLOR :COLOR;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 tmpvar_5_1;
          float4 tmpvar_2;
          float4 tmpvar_3;
          tmpvar_3 = UnityObjectToClipPos(in_v.vertex);
          tmpvar_2.zw = tmpvar_3.zw;
          tmpvar_2.x = (tmpvar_3.x + ((tmpvar_3.z * tmpvar_3.z) * _Distort.x));
          tmpvar_2.y = (tmpvar_3.y + ((tmpvar_3.z * tmpvar_3.z) * _Distort.y));
          float tmpvar_4;
          tmpvar_4 = (tmpvar_3.z / _Falloff);
          tmpvar_5_1.yzw = float3(0, 0, 0);
          tmpvar_5_1.x = (1 - (tmpvar_4 * tmpvar_4));
          out_v.vertex = tmpvar_2;
          out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.xlv_COLOR = tmpvar_5_1;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float4 diffuseColor_2;
          float4 tmpvar_3;
          tmpvar_3 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
          float4 tmpvar_4;
          tmpvar_4 = ((tmpvar_3 * _MainColor) * in_f.xlv_COLOR.x);
          diffuseColor_2 = tmpvar_4;
          tmpvar_1 = (diffuseColor_2 * _Factor);
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
