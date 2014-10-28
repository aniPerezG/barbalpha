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

texture perlinNoise1;
sampler heightmap1 = sampler_state
{
	Texture = <perlinNoise1>;
	MipFilter = Point;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

texture perlinNoise2;
sampler heightmap2 = sampler_state
{
	Texture = <perlinNoise2>;
	MipFilter = Point;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
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

	float u = (Input.Position.x / screen_dx);
	float v = (Input.Position.z / screen_dy);

	float height1 = tex2Dlod(heightmap1, float4(u, v, 0, 0)).r;
	float height2 = tex2Dlod(heightmap2, float4(u, v, 0, 0)).r;

	Input.Position.y = Input.Position.y + lerp(height1, height2, alpha);

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

