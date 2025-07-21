// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/IlluminationOfUnlitShader"
{
  Properties
  {
    _MainTex ("Texture", 2D) = "white" {}
    _lifgtDirection ("light direction", Vector) = (1,2,3,4)
    _diffuseColor ("diffuse Color", Color) = (1,1,1,1)
    _diffuseRange ("diffuse Range", Range(0, 1)) = 0.8
    _diffusePower ("diffuse Power", float) = 0.8
    _reflectColor ("reflect Color", Color) = (1,1,1,1)
    _reflectRange ("reflect Range", Range(0, 1)) = 0.4
    _reflectPower ("reflect Power", float) = 0.8
  }
  SubShader
  {
    Tags
    { 
      "RenderType" = "Opaque"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "RenderType" = "Opaque"
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
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 UNITY_MATRIX_MVP;
      //uniform float4x4 unity_WorldToObject;
      uniform float4 _MainTex_ST;
      uniform float4 _Distort;
      uniform float _SkyGradientOffset;
      uniform float4 _FogSilhouetteColor;
      uniform float4 _SkyGradientBottomColor;
      uniform float4 _SkyGradientTopColor;
      uniform sampler2D _MainTex;
      uniform float4 _lifgtDirection;
      uniform float4 _diffuseColor;
      uniform float _diffuseRange;
      uniform float _diffusePower;
      uniform float4 _reflectColor;
      uniform float _reflectRange;
      uniform float _reflectPower;
      uniform float _Factor;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float3 xlv_TEXCOORD2 :TEXCOORD2;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float3 xlv_TEXCOORD2 :TEXCOORD2;
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
          float4 tmpvar_4;
          tmpvar_4.w = 1;
          tmpvar_4.xyz = in_v.vertex.xyz;
          tmpvar_3 = UnityObjectToClipPos(tmpvar_4);
          float3x3 tmpvar_5;
          tmpvar_5[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_5[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_5[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          tmpvar_2.zw = tmpvar_3.zw;
          tmpvar_2.x = (tmpvar_3.x + ((tmpvar_3.z * tmpvar_3.z) * _Distort.x));
          tmpvar_2.y = (tmpvar_3.y + ((tmpvar_3.z * tmpvar_3.z) * _Distort.y));
          float _tmp_dvx_0 = clamp((((tmpvar_2.y / tmpvar_3.w) - _SkyGradientOffset) / (1 - _SkyGradientOffset)), 0, 1);
          tmpvar_1.xyz = lerp(_SkyGradientBottomColor.xyz, _SkyGradientTopColor.xyz, float3(_tmp_dvx_0, _tmp_dvx_0, _tmp_dvx_0));
          float _tmp_dvx_1 = clamp(((1000 - tmpvar_3.w) / 300), 0, 1);
          tmpvar_1.xyz = lerp(tmpvar_1.xyz, _FogSilhouetteColor.xyz, float3(_tmp_dvx_1, _tmp_dvx_1, _tmp_dvx_1));
          tmpvar_1.w = clamp(((700 - tmpvar_3.w) / 500), 0, 1);
          out_v.vertex = tmpvar_2;
          out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.xlv_TEXCOORD1 = tmpvar_1;
          out_v.xlv_TEXCOORD2 = normalize(mul(in_v.normal, tmpvar_5));
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
          float tmpvar_5;
          tmpvar_5 = max(0, (((-dot(in_f.xlv_TEXCOORD2, normalize(_lifgtDirection.xyz))) - _diffuseRange) * _diffusePower));
          tmpvar_2.xyz = ((_diffuseColor.xyz * tmpvar_5) + (tmpvar_2.xyz * (1 - tmpvar_5)));
          float tmpvar_6;
          tmpvar_6 = min(0, ((in_f.xlv_TEXCOORD2.y + _reflectRange) * _reflectPower));
          tmpvar_2.xyz = ((tmpvar_2.xyz * (1 + tmpvar_6)) - (_reflectColor.xyz * tmpvar_6));
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
