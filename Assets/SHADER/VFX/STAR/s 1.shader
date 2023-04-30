// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "s"
{
	Properties
	{
		_EdgeWidth("EdgeWidth", Range( 1 , 1.5)) = 1.2
		[HDR]_EdgeColor("EdgeColor", Color) = (2,0,0,1)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Vector0("Vector 0", Vector) = (1,1,0,0)

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaToMask Off
		Cull Off
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _TextureSample0;
			uniform float4 _Vector0;
			uniform float _EdgeWidth;
			uniform float4 _EdgeColor;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord1 = screenPos;
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				o.ase_texcoord2.zw = v.ase_texcoord2.xy;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 appendResult41 = (float2(_Vector0.z , _Vector0.w));
				float4 screenPos = i.ase_texcoord1;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float2 appendResult32 = (float2(ase_screenPosNorm.x , ase_screenPosNorm.y));
				float2 appendResult37 = (float2(_Vector0.x , _Vector0.y));
				float2 panner35 = ( _Time.y * appendResult41 + ( appendResult32 * appendResult37 ));
				float2 texCoord1 = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float blendOpSrc2 = texCoord1.x;
				float blendOpDest2 = texCoord1.y;
				float temp_output_2_0 = ( saturate( abs( blendOpSrc2 - blendOpDest2 ) ));
				float blendOpSrc7 = texCoord1.x;
				float blendOpDest7 = ( 1.0 - texCoord1.y );
				float temp_output_7_0 = ( saturate( abs( blendOpSrc7 - blendOpDest7 ) ));
				float blendOpSrc8 = temp_output_2_0;
				float blendOpDest8 = temp_output_7_0;
				float blendOpSrc11 = temp_output_7_0;
				float blendOpDest11 = temp_output_2_0;
				float temp_output_12_0 = ( ( saturate( ( blendOpDest8/ max( 1.0 - blendOpSrc8, 0.00001 ) ) )) * ( saturate( ( blendOpDest11/ max( 1.0 - blendOpSrc11, 0.00001 ) ) )) );
				float2 texCoord26 = i.ase_texcoord2.zw * float2( 1,1 ) + float2( 0,0 );
				float temp_output_27_0 = ( 1.0 - texCoord26.x );
				float temp_output_15_0 = step( temp_output_12_0 , temp_output_27_0 );
				float saferPower13 = abs( temp_output_12_0 );
				
				
				finalColor = ( ( tex2D( _TextureSample0, panner35 ) * temp_output_15_0 ) + ( ( step( pow( saferPower13 , _EdgeWidth ) , temp_output_27_0 ) - temp_output_15_0 ) * _EdgeColor ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-885.4359,117.1496;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;2;-334.3782,131.8524;Inherit;True;Difference;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;7;-329.8445,375.1258;Inherit;True;Difference;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;6;-601.8445,406.1258;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;8;161.5555,112.9258;Inherit;True;ColorDodge;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;11;156.6033,372.8587;Inherit;True;ColorDodge;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;464.3669,284.2879;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;15;1228.992,266.0814;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;17;1244.624,508.9047;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;18;1510.624,415.9047;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;2180.274,231.6728;Float;False;True;-1;2;ASEMaterialInspector;100;5;s;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;2;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;1905.725,254.2885;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;1655.461,126.1842;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;1756.966,664.3076;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;16;742.2854,776.1312;Inherit;False;Property;_Float0;Float 0;1;0;Create;True;0;0;0;False;0;False;0.407211;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;26;779.9108,895.7866;Inherit;False;2;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;14;463.3684,584.7277;Inherit;False;Property;_EdgeWidth;EdgeWidth;0;0;Create;True;0;0;0;False;0;False;1.2;1;1;1.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;27;1076.882,727.9383;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;13;847.8668,412.9879;Inherit;True;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;21;1513.966,748.3076;Inherit;False;Property;_EdgeColor;EdgeColor;2;1;[HDR];Create;True;0;0;0;False;0;False;2,0,0,1;1,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;34;1241.191,-70.70135;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;35;983.4069,-110.1267;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;31;246.9428,-334.387;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;38;259.2995,-129.8885;Inherit;False;Property;_Vector0;Vector 0;4;0;Create;True;0;0;0;False;0;False;1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;32;489.7271,-249.7175;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;684.6578,-210.4709;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;37;501.2767,-129.2485;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;41;676.6578,9.529114;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;42;622.6578,146.5291;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
WireConnection;2;0;1;1
WireConnection;2;1;1;2
WireConnection;7;0;1;1
WireConnection;7;1;6;0
WireConnection;6;0;1;2
WireConnection;8;0;2;0
WireConnection;8;1;7;0
WireConnection;11;0;7;0
WireConnection;11;1;2;0
WireConnection;12;0;8;0
WireConnection;12;1;11;0
WireConnection;15;0;12;0
WireConnection;15;1;27;0
WireConnection;17;0;13;0
WireConnection;17;1;27;0
WireConnection;18;0;17;0
WireConnection;18;1;15;0
WireConnection;0;0;19;0
WireConnection;19;0;24;0
WireConnection;19;1;22;0
WireConnection;24;0;34;0
WireConnection;24;1;15;0
WireConnection;22;0;18;0
WireConnection;22;1;21;0
WireConnection;27;0;26;1
WireConnection;13;0;12;0
WireConnection;13;1;14;0
WireConnection;34;1;35;0
WireConnection;35;0;39;0
WireConnection;35;2;41;0
WireConnection;35;1;42;0
WireConnection;32;0;31;1
WireConnection;32;1;31;2
WireConnection;39;0;32;0
WireConnection;39;1;37;0
WireConnection;37;0;38;1
WireConnection;37;1;38;2
WireConnection;41;0;38;3
WireConnection;41;1;38;4
ASEEND*/
//CHKSM=92792789EECC594C7280B2C717F071ECAC347230