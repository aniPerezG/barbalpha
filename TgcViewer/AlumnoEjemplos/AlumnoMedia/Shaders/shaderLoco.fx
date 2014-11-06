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

float3 fvLightPosition;// = float3(0, 0, -600.00);
float3 fvEyePosition;// = float3(0.00, 0.00, -100.00);
float k_la = 0.0005;							// luz ambiente global
float k_ld = 0.0001;							// luz difusa
float k_ls = 10;							// luz specular
//float fSpecularPower = 4;				// exponente de la luz specular
float LightIntensity = 0.05;


float time = 0;

float amplitud;
float frecuencia;
float mediaAlturaBarco;

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
   float3 Normal :   TEXCOORD1;			// Normales
   float3 WorldPos : TEXCOORD2;
   
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
  
	Output.WorldPos = mul(Input.Position, matWorld);

	X = Input.Position.x;
	Z = Input.Position.z;

	float primaX;
	float primaZ;
	float3 N;

	//formula del plano normal
	// Y = f(a,b) + f'x(a,b)*(x-a) + f'z(a,b)(z-b)
	//Despejando
	//f'x(a,b)*(x-a) -Y + f'z(a,b)(z-b) + f(a,b) = 0
	//Entonces podemos deducir al vector normal al punto como
	//N = (f'x(a,b), -1, f'z)
	//como a nosotros nos interesa que la normal apunte para arriba, 
	//multiplicamos a N por (-1)
	//Por lo que nuestro vector normal final seria
	//N = (-f'x(a,b), 1, -f'z)

	//f'x , siendo f(x,z) la funcion trigonometrica aplicada en el VS
	primaX = (cos(time + X) * cos(time + Z) - sin(time + X));

	//f'z 
	primaZ = (cos(time + Z) - sin(time + X) * sin(time + Z));

	N = float3(-primaX, amplitud, -primaZ);

	N = normalize(N);

	Output.Normal = N;

	Output.Normal = mul(Input.Normal, (float3x3)matWorld);

   //Proyectar posicion
   Output.Position = mul( Input.Position, matWorldViewProj);
   
   //Propago las coordenadas de textura
   Output.Texcoord = Input.Texcoord;

   //Propago el color x vertice
   //Output.Color = Input.Color;
   
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
	Input.Position.y += mediaAlturaBarco + (-D - C*Z - A*X) / B;

	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
	//Output.Color = Input.Color;

	return(Output);

}

VS_OUTPUT vs_normal(VS_INPUT Input)
{
	VS_OUTPUT Output;

	Output.WorldPos = mul(Input.Position, matWorld);
	Output.Normal = mul(Input.Normal, (float3x3)matWorld);

	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
	//Output.Color = Input.Color;

	return(Output);

}

float4 ps_light(float3 Texcoord: TEXCOORD0, float3 N : TEXCOORD1,  float3 Pos : TEXCOORD2 ) : COLOR0
{
	float4 RGBColor = 0;
	float shininess = -2.6;

	float3 ambientColor = float3(0, 0, 150);
	float3 diffuseColor = float3(0, 0, 100);
	float3 specularColor = float3(8, 14, 20);
	float4 fvBaseColor = tex2D(diffuseMap, Texcoord);

	float3 L = normalize(float3(fvLightPosition.x, fvLightPosition.y, fvLightPosition.z) - float3(Pos.x, Pos.y, Pos.z));
	float NdotL = dot(N, L);

	float V = normalize(float3(fvEyePosition.x, fvEyePosition.y, fvEyePosition.z) - float3(Pos.x, Pos.y, Pos.z));
	float R = normalize(float3(fvEyePosition.x, (fvEyePosition.y*4)/5, fvEyePosition.z) - float3(Pos.x, Pos.y, Pos.z));
	float RdotV = dot(R, V);

	RGBColor.rgb = saturate(fvBaseColor * (saturate(k_la * ambientColor*0.3 + k_ld*diffuseColor*NdotL+ k_ls) + pow(k_ls*specularColor*RdotV, shininess)));

	return RGBColor;
}

// ------------------------------------------------------------------
technique LightScene
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_main();
		PixelShader = compile ps_2_0 ps_light();
	}
}

technique RenderScene
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_main();
		PixelShader = compile ps_2_0 ps_main();
	}
}

technique HeightScene
{
	pass Pass_0
	{
		VertexShader = compile vs_2_0 vs_alturaPlano();
		PixelShader = compile ps_2_0 ps_main();
	}
}
/*
technique LightTechnique
{
	pass Pass_0
	{
		VertexShader = compile vs_2_0 vs_normal();
		PixelShader = compile ps_2_0 ps_light();
	}
}*/