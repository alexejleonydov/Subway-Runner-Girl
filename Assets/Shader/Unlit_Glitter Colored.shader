// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Glitter Colored"
{
  Properties
  {
    _MainTex ("RGB", 2D) = "black" {}
    _FlowLightTex ("FlowLight Texture", 2D) = "white" {}
    _FlowLightPower ("FlowLightPower", float) = 1
    _IsOpenFlowLight ("IsOpenFlowLight", float) = 0
    _FlowLightOffset ("FlowLight Offset", float) = 0
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    LOD 200
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      LOD 200
      ZClip Off
      ZWrite Off
      Cull Off
      Offset -1, -1
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
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform sampler2D _MainTex;
      uniform sampler2D _FlowLightTex;
      uniform float _FlowLightPower;
      uniform int _IsOpenFlowLight;
      uniform float _FlowLightOffset;
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
          float2 tmpvar_1;
          tmpvar_1 = in_v.texcoord.xy;
          float2 tmpvar_2;
          tmpvar_2 = tmpvar_1;
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.xlv_TEXCOORD0 = tmpvar_2;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 main_1;
          float4 tmpvar_2;
          tmpvar_2 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
          main_1 = tmpvar_2;
          if((_IsOpenFlowLight==1))
          {
              float4 flowColor_3;
              float2 flowUV_4;
              flowUV_4.y = in_f.xlv_TEXCOORD0.y;
              flowUV_4.x = (in_f.xlv_TEXCOORD0.x * 0.5);
              flowUV_4.x = (flowUV_4.x - _FlowLightOffset);
              float4 tmpvar_5;
              tmpvar_5 = (tex2D(_FlowLightTex, flowUV_4) * _FlowLightPower);
              flowColor_3.w = tmpvar_5.w;
              flowColor_3.xyz = (tmpvar_5.xyz * tmpvar_2.xyz);
              main_1.xyz = (tmpvar_2.xyz + (flowColor_3.xyz * tmpvar_5.w));
              main_1.w = (tmpvar_2.w * tmpvar_2.w);
          }
          out_f.color = main_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    LOD 100
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      LOD 100
      ZClip Off
      ZWrite Off
      Cull Off
      Offset -1, -1
      Fog
      { 
        Mode  Off
      } 
      Blend SrcAlpha OneMinusSrcAlpha
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 UNITY_MATRIX_MVP;
      uniform float4 _MainTex_ST;
      uniform float4 _AlphaTex_ST;
      uniform sampler2D _MainTex;
      uniform sampler2D _AlphaTex;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_COLOR0 :COLOR0;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR0 :COLOR0;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float2 xlv_TEXCOORD1 :TEXCOORD1;
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
          tmpvar_2 = clamp(in_v.color, 0, 1);
          tmpvar_1 = tmpvar_2;
          float4 tmpvar_3;
          tmpvar_3.w = 1;
          tmpvar_3.xyz = in_v.vertex.xyz;
          out_v.xlv_COLOR0 = tmpvar_1;
          out_v.xlv_TEXCOORD0 = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          out_v.xlv_TEXCOORD1 = TRANSFORM_TEX(in_v.texcoord.xy, _AlphaTex);
          out_v.vertex = UnityObjectToClipPos(tmpvar_3);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 col_1;
          col_1 = (tex2D(_MainTex, in_f.xlv_TEXCOORD0) * in_f.xlv_COLOR0);
          col_1.xyz = col_1.xyz;
          col_1.w = (tex2D(_AlphaTex, in_f.xlv_TEXCOORD1).w * in_f.xlv_COLOR0.w);
          out_f.color = col_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
