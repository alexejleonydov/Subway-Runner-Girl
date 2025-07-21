// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Projector/Multiply Test"
{
  Properties
  {
    _Color ("Main Color", Color) = (1,1,1,1)
    _ShadowTex ("Cookie", 2D) = "gray" {}
    _FalloffTex ("FallOff", 2D) = "white" {}
    _ShadowStrength ("Strength", float) = 1
    _Diffuse ("Diffuse", Color) = (1,1,1,1)
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
      Offset -5, -1
      Fog
      { 
        Mode  Off
      } 
      Blend One One
      ColorMask RGB
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
      uniform float4 _Color;
      uniform float4 _Distort;
      uniform float4 _Diffuse;
      uniform float4x4 unity_Projector;
      uniform float4x4 unity_ProjectorClip;
      uniform sampler2D _ShadowTex;
      uniform sampler2D _FalloffTex;
      uniform float _ShadowStrength;
      uniform float _Factor;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float3 normal :NORMAL;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float3 xlv_COLOR :COLOR;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_TEXCOORD0 :TEXCOORD0;
          float4 xlv_TEXCOORD1 :TEXCOORD1;
          float3 xlv_COLOR :COLOR;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float3 worldNormal_1;
          float4 tmpvar_2;
          float4 tmpvar_3;
          tmpvar_3 = UnityObjectToClipPos(in_v.vertex);
          tmpvar_2.zw = tmpvar_3.zw;
          tmpvar_2.x = (tmpvar_3.x + ((tmpvar_3.z * tmpvar_3.z) * _Distort.x));
          tmpvar_2.y = (tmpvar_3.y + ((tmpvar_3.z * tmpvar_3.z) * _Distort.y));
          float3x3 tmpvar_4;
          tmpvar_4[0] = conv_mxt4x4_0(unity_WorldToObject).xyz;
          tmpvar_4[1] = conv_mxt4x4_1(unity_WorldToObject).xyz;
          tmpvar_4[2] = conv_mxt4x4_2(unity_WorldToObject).xyz;
          float3 tmpvar_5;
          tmpvar_5 = normalize(mul(in_v.normal, tmpvar_4));
          worldNormal_1 = tmpvar_5;
          out_v.xlv_TEXCOORD0 = mul(unity_Projector, in_v.vertex);
          out_v.xlv_TEXCOORD1 = mul(unity_ProjectorClip, in_v.vertex);
          out_v.vertex = tmpvar_2;
          out_v.xlv_COLOR = ((_Color.xyz * _Diffuse.xyz) * clamp(dot(worldNormal_1, float3(0.5773503, 0.5773503, 0.5773503)), 0, 1));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 res_1;
          float4 texS_2;
          float4 tmpvar_3;
          tmpvar_3 = tex2D(_ShadowTex, in_f.xlv_TEXCOORD0);
          texS_2.xyz = (tmpvar_3.xyz * (in_f.xlv_COLOR * _ShadowStrength));
          texS_2.w = (1 - tmpvar_3.w);
          float4 tmpvar_4;
          tmpvar_4 = tex2D(_FalloffTex, in_f.xlv_TEXCOORD1);
          float4 tmpvar_5;
          tmpvar_5 = ((texS_2 * tmpvar_4.w) * _Factor);
          res_1 = tmpvar_5;
          out_f.color = res_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
