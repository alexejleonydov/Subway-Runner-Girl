// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Sky"
{
  Properties
  {
    _GradientFade ("Fade", float) = 1
    _GradientOffset ("Gradient offset", float) = 0
  }
  SubShader
  {
    Tags
    { 
      "QUEUE" = "Background+2000"
      "RenderType" = "Opaque"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "QUEUE" = "Background+2000"
        "RenderType" = "Opaque"
      }
      ZClip Off
      ZWrite Off
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
      uniform float _SkyGradientOffset;
      uniform float4 _SkyGradientBottomColor;
      uniform float4 _SkyGradientTopColor;
      struct appdata_t
      {
          float4 vertex :POSITION;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_COLOR :COLOR;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR :COLOR;
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
          tmpvar_1 = UnityObjectToClipPos(in_v.vertex);
          float _tmp_dvx_2 = clamp((((tmpvar_1.y / tmpvar_1.w) - _SkyGradientOffset) / (1 - _SkyGradientOffset)), 0, 1);
          tmpvar_2.xyz = lerp(_SkyGradientBottomColor.xyz, _SkyGradientTopColor.xyz, float3(_tmp_dvx_2, _tmp_dvx_2, _tmp_dvx_2));
          tmpvar_2.w = 1;
          out_v.vertex = tmpvar_1;
          out_v.xlv_COLOR = tmpvar_2;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          tmpvar_1 = in_f.xlv_COLOR;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
