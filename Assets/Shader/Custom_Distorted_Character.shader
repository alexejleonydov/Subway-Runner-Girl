// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Distorted/Character"
{
  Properties
  {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Factor ("Factor", float) = 1
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
      ZClip Off
      Fog
      { 
        Mode  Off
      } 
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
      uniform float _SkyGradientOffset;
      uniform float4 _FogSilhouetteColor;
      uniform float4 _SkyGradientBottomColor;
      uniform float4 _SkyGradientTopColor;
      uniform sampler2D _MainTex;
      uniform float _Factor;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
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
          float4 tmpvar_3;
          tmpvar_3 = UnityObjectToClipPos(in_v.vertex);
          tmpvar_2.zw = tmpvar_3.zw;
          tmpvar_2.x = (tmpvar_3.x + ((tmpvar_3.z * tmpvar_3.z) * _Distort.x));
          tmpvar_2.y = (tmpvar_3.y + ((tmpvar_3.z * tmpvar_3.z) * _Distort.y));
          float _tmp_dvx_5 = clamp((((tmpvar_2.y / tmpvar_3.w) - _SkyGradientOffset) / (1 - _SkyGradientOffset)), 0, 1);
          tmpvar_1.xyz = lerp(_SkyGradientBottomColor.xyz, _SkyGradientTopColor.xyz, float3(_tmp_dvx_5, _tmp_dvx_5, _tmp_dvx_5));
          float _tmp_dvx_6 = clamp(((1000 - tmpvar_3.w) / 300), 0, 1);
          tmpvar_1.xyz = lerp(tmpvar_1.xyz, _FogSilhouetteColor.xyz, float3(_tmp_dvx_6, _tmp_dvx_6, _tmp_dvx_6));
          tmpvar_1.w = clamp(((700 - tmpvar_3.w) / 500), 0, 1);
          out_v.vertex = tmpvar_2;
          out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.xlv_TEXCOORD1 = tmpvar_1;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float4 tmpvar_2;
          float4 c_3;
          float4 tmpvar_4;
          tmpvar_4 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
          tmpvar_2 = tmpvar_4;
          c_3.w = tmpvar_2.w;
          c_3.xyz = (lerp(in_f.xlv_TEXCOORD1.xyz, tmpvar_2.xyz, in_f.xlv_TEXCOORD1.www) * _Factor);
          tmpvar_1 = c_3;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
