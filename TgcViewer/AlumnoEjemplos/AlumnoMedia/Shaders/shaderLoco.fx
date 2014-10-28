/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};


float time = 0;

float amplitud;
float frecuencia;

float offsetX;
float offsetZ;
float offsetY;

float A;
float B;
float C;
float D;

float xEnElPlano;
float yEnElPlano;
float zEnElPlano;


float alpha;
float screen_dx;					// tamaño de la pantalla en pixels
float screen_dy;


float3 LightPosition;
float3 LightDiffuseColor; // intensity multiplier
float3 LightSpecularColor; // intensity multiplier
float LightDistanceSquared;
float3 DiffuseColor;
float3 AmbientLightColor;
float3 EmissiveColor;
float3 SpecularColor;
float SpecularPower;


texture perlinNoise1;
sampler heightmap1 = sampler_state
{
	Texture = <perlinNoise1>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

texture perlinNoise2;
sampler heightmap2 = sampler_state
{
	Texture = <perlinNoise2>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};


/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT 
{
   float4 Position : POSITION0;
   float4 Color : COLOR0;
   float2 Texcoord : TEXCOORD0;
   float3 Normal : NORMAL0;
};


//Output del Vertex Shader
struct VS_OUTPUT 
{
   float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
   float4 Color : COLOR0;
   float3 WorldPos : TEXCOORD2;
   float3 Normal : TEXCOORD1;
   
};



//Vertex Shader
VS_OUTPUT vs_main( VS_INPUT Input )
{
   VS_OUTPUT Output;
   
   //Animar Posicion
   float X = Input.Position.x/frecuencia;
   float Y = Input.Position.y;
   float Z = Input.Position.z/frecuencia;
  
   Input.Position.y = (sin(X+time)*cos(Z+time) + sin(Z+time) + cos(X+time))*amplitud ;
   //float u = Input.Position.x;
   //float v = Input.Position.z;

   //float height1 = tex2Dlod(heightmap1, float4(u, v, 0, 0)).r;
   //float height1 = tex2D(heightmap1, float2(u, v)).r;
   //float height2 = tex2Dlod(heightmap2, float4(u, v, 0, 0)).r;

   //Input.Position.y = Input.Position.y + lerp(height1, height2, alpha);
   //Input.Position.y = Input.Position.y + height1;


	Output.WorldPos = mul(Input.Position, matWorld);
	Output.Normal = mul(Input.Normal, (float3x3)matWorld);

   //Proyectar posicion
   Output.Position = mul( Input.Position, matWorldViewProj);
   
   //Propago las coordenadas de textura
   Output.Texcoord = Input.Texcoord;

   //Propago el color x vertice
   Output.Color = Input.Color;
   
   return( Output );
   
}


//Pixel Shader
float4 ps_main( float2 Texcoord: TEXCOORD0, float4 Color:COLOR0, float2 Heightcoord : TEXCOORD1) : COLOR0
{      
	// Obtener el texel de textura
	// diffuseMap es el sampler, Texcoord son las coordenadas interpoladas
	float4 texColor = tex2D( diffuseMap, Texcoord );
	
	return texColor;
}


// ------------------------------------------------------------------
technique RenderScene
{
   pass Pass_0
   {
	  VertexShader = compile vs_3_0 vs_main();
	  PixelShader = compile ps_2_0 ps_main();
   }
}

//*************************************************************

VS_OUTPUT vs_heightMap(VS_INPUT Input)
{
	VS_OUTPUT Output;

	
	//Animar Posicion
	float X = (Input.Position.x + offsetX) / frecuencia;
	float Z = (Input.Position.z + offsetZ) / frecuencia;
	Input.Position.y += offsetY;


	Output.WorldPos = mul(Input.Position, matWorld);
	Output.Normal = mul(Input.Normal, (float3x3)matWorld);


	//a cada vertice de la canoa le sumo la altura del agua, mas 10 que es el "fondo" de la canoa
	Input.Position.y += 10 + (sin(X + time)*cos(Z + time) + sin(Z + time) + cos(X + time))*amplitud;

	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);

}

VS_OUTPUT vs_alturaPlano(VS_INPUT Input)
{
	VS_OUTPUT Output;

	float X = Input.Position.x + offsetX;
	Input.Position.y += offsetY;
	float Z = Input.Position.z + offsetZ;

	//Ecuacion de un plano: Ax+By+Cz+D = 0

	D = A*xEnElPlano + B*yEnElPlano + C*zEnElPlano;
	D *= -1;


	Output.WorldPos = mul(Input.Position, matWorld);
	Output.Normal = mul(Input.Normal, (float3x3)matWorld);



	//Despejando y en la posicion del plano y = (-D -Cz -Ax)/B
	//10 es el radio en Y, hay que abstraerlo
	Input.Position.y += 10 + (-D - C*Z - A*X) / B;

	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);

}

technique HeightScene
{
	pass Pass_0
	{
		VertexShader = compile vs_2_0 vs_alturaPlano();
		PixelShader = compile ps_2_0 ps_main();
	}
}

float4 PixelShaderFunctionWithoutTex(VS_OUTPUT input) : COLOR0
{
	// Phong relfection is ambient + light-diffuse + spec highlights.
	// I = Ia*ka*Oda + fatt*Ip[kd*Od(N.L) + ks(R.V)^n]
	// Ref: http://brooknovak.wordpress.com/2008/11/13/hlsl-per-pixel-point-light-using-phong-blinn-lighting-model/
	// Get light direction for this fragment
	float3 lightDir = normalize(input.WorldPos - LightPosition);

	// Note: Non-uniform scaling not supported
	float diffuseLighting = saturate(dot(input.Normal, -lightDir)); // per pixel diffuse lighting

	// Introduce fall-off of light intensity
	diffuseLighting *= (LightDistanceSquared / dot(LightPosition - input.WorldPos, LightPosition - input.WorldPos));

	// Using Blinn half angle modification for perofrmance over correctness
	float3 h = normalize(normalize(CameraPos - input.WorldPos) - lightDir);

	float specLighting = pow(saturate(dot(h, input.Normal)), SpecularPower);

	return float4(saturate(
		AmbientLightColor +
		(DiffuseColor * LightDiffuseColor * diffuseLighting * 0.6) + // Use light diffuse vector as intensity multiplier
		(SpecularColor * LightSpecularColor * specLighting * 0.5) // Use light specular vector as intensity multiplier
		), 1);
}