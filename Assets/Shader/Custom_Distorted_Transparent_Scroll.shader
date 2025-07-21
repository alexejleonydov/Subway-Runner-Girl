// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Distorted/Transparent_Scroll"
{
  Properties
  {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _MainColor ("Color (RGBC)", Color) = (1,1,1,0)
    _ScrollX ("Base layer Scroll speed X", float) = 1
    _ScrollY ("Base layer Scroll speed Y", float) = 0
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
      Blend SrcAlpha OneMinusSrcAlpha
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 _Time;
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform float4 _MainTex_ST;
      uniform float4 _Distort;
      uniform float _ScrollX;
      uniform float _ScrollY;
      uniform sampler2D _MainTex;
      uniform float4 _MainColor;
      uniform float _Factor;
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
          float4 tmpvar_2;
          tmpvar_2 = UnityObjectToClipPos(in_v.vertex);
          tmpvar_1.zw = tmpvar_2.zw;
          tmpvar_1.x = (tmpvar_2.x + ((tmpvar_2.z * tmpvar_2.z) * _Distort.x));
          tmpvar_1.y = (tmpvar_2.y + ((tmpvar_2.z * tmpvar_2.z) * _Distort.y));
          float2 tmpvar_3;
          tmpvar_3.x = _ScrollX;
          tmpvar_3.y = _ScrollY;
          out_v.vertex = tmpvar_1;
          out_v.xlv_TEXCOORD0 = (TRANSFORM_TEX(in_v.texcoord.xy, _MainTex) + frac((tmpvar_3 * _Time.xy)));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          tmpvar_1 = ((tex2D(_MainTex, in_f.xlv_TEXCOORD0) * _MainColor) * _Factor);
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
