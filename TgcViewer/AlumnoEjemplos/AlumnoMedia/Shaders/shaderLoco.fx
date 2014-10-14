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


/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT 
{
   float4 Position : POSITION0;
   float4 Color : COLOR0;
   float2 Texcoord : TEXCOORD0;
};


struct VS_INPUT_BARCO
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT 
{
   float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
   float4 Color : COLOR0;
   
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
	  VertexShader = compile vs_2_0 vs_main();
	  PixelShader = compile ps_2_0 ps_main();
   }
}

//*************************************************************

VS_OUTPUT vs_heightMap(VS_INPUT_BARCO Input)
{
	VS_OUTPUT Output;

	
	//Animar Posicion
	float X = (Input.Position.x + offsetX) / frecuencia;
	float Z = (Input.Position.z + offsetZ) / frecuencia;
	Input.Position.y += offsetY;


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

technique HeightScene
{
	pass Pass_0
	{
		VertexShader = compile vs_2_0 vs_heightMap();
		PixelShader = compile ps_2_0 ps_main();
	}
}

