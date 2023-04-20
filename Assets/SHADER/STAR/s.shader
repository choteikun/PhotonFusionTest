// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "s"
{
	Properties
	{
		_EdgeWidth("EdgeWidth", Range( 0.2 , 3)) = 1.068235
		_Float0("Float 0", Range( 0 , 1)) = 0.04912828
		[HDR]_EdgeColor("EdgeColor", Color) = (1,0,0,1)
		[HDR]_MainColor("MainColor", Color) = (1,1,1,1)

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
			

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float4 _MainColor;
			uniform float _Float0;
			uniform float _EdgeWidth;
			uniform float4 _EdgeColor;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
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
				float2 texCoord1 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
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
				float temp_output_15_0 = step( temp_output_12_0 , _Float0 );
				
				
				finalColor = ( ( _MainColor * temp_output_15_0 ) + ( ( step( pow( temp_output_12_0 , _EdgeWidth ) , _Float0 ) - temp_output_15_0 ) * _EdgeColor ) );
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
Node;AmplifyShaderEditor.PowerNode;13;847.8668,412.9879;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;492.9665,572.8885;Inherit;False;Property;_EdgeWidth;EdgeWidth;0;0;Create;True;0;0;0;False;0;False;1.068235;1;0.2;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;897.2854,766.1312;Inherit;False;Property;_Float0;Float 0;1;0;Create;True;0;0;0;False;0;False;0.04912828;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;15;1228.992,266.0814;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;17;1244.624,508.9047;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;18;1510.624,415.9047;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;2180.274,231.6728;Float;False;True;-1;2;ASEMaterialInspector;100;5;s;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;2;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;1905.725,254.2885;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;1655.461,126.1842;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;1756.966,664.3076;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;21;1513.966,748.3076;Inherit;False;Property;_EdgeColor;EdgeColor;2;1;[HDR];Create;True;0;0;0;False;0;False;1,0,0,1;1,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;23;1404.461,-85.8158;Inherit;False;Property;_MainColor;MainColor;3;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
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
WireConnection;13;0;12;0
WireConnection;13;1;14;0
WireConnection;15;0;12;0
WireConnection;15;1;16;0
WireConnection;17;0;13;0
WireConnection;17;1;16;0
WireConnection;18;0;17;0
WireConnection;18;1;15;0
WireConnection;0;0;19;0
WireConnection;19;0;24;0
WireConnection;19;1;22;0
WireConnection;24;0;23;0
WireConnection;24;1;15;0
WireConnection;22;0;18;0
WireConnection;22;1;21;0
ASEEND*/
//CHKSM=27599255CA7EE0E894501482109A391E23E54BE0