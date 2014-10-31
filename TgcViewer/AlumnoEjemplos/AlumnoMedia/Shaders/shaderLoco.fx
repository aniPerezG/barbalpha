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

float3 fvLightPosition = float3(-100.00, 100.00, -100.00);
float3 fvEyePosition = float3(0.00, 0.00, -100.00);
float k_la = 0.3;							// luz ambiente global
float k_ld = 0.2;							// luz difusa
float k_ls = 0.4;							// luz specular
float fSpecularPower = 4;				// exponente de la luz specular



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
	//10 es el radio en Y, hay que abstraerlo
	Input.Position.y += 10 + (-D - C*Z - A*X) / B;

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

float4 ps_light(float3 Texcoord: TEXCOORD0,  float3 Pos : TEXCOORD2) : COLOR0
{
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular

	float3 N;

	float X = Pos.x;
	float Z = Pos.z;

	float primaX;
	float primaZ;

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
	primaX = amplitud*(cos(time + X) * cos(time + Z) - sin(time + X));

	//f'z 
	primaZ = amplitud*(cos(time + Z) - sin(time + X) * sin(time + Z));
		
	N = float3(-primaX, 1, -primaZ);
	 
	N = normalize(N);

	// si hubiera varias luces, se podria iterar por c/u. 
	// Pero hay que tener en cuenta que este algoritmo es bastante pesado
	// ya que todas estas formulas se calculan x cada pixel. 
	// En la practica no es usual tomar mas de 2 o 3 luces. Generalmente 
	// se determina las luces que mas contribucion a la escena tienen, y 
	// el resto se aproxima con luz ambiente. 
	// for(int =0;i<cant_ligths;++i)
	// 1- calculo la luz diffusa
	float3 LD = normalize(fvLightPosition - float3(Pos.x, Pos.y, Pos.z));
		ld += saturate(dot(N, LD))*k_ld;

	// 2- calcula la reflexion specular
	float3 D = -normalize(float3(Pos.x, Pos.y, Pos.z) - fvEyePosition);
		float ks = saturate(dot(reflect(LD, N), D));
	ks = pow(ks, fSpecularPower);
	le += ks*k_ls;

	//Obtener el texel de textura
	float4 fvBaseColor = tex2D(diffuseMap, Texcoord);

	float4 RGBColor = 0;
	RGBColor.rgb = saturate(fvBaseColor*(saturate(k_la + ld)) + le);

	// saturate deja los valores entre [0,1]. Una tecnica muy usada en motores modernos
	// es usar floating point textures auxialres, para almacenar mucho mas que 256 valores posibles 
	// de iluminiacion. En esos casos, el valor del rgb podria ser mucho mas que 1. 
	// Imaginen una excena outdoor, a la luz de sol, hay mucha diferencia de iluminacion
	// entre los distintos puntos, que no se pueden almacenar usando solo 8bits por canal.
	// Estas tecnicas se llaman HDRLighting (High Dynamic Range Lighting). 
	// Muchas inclusive simulan el efecto de la pupila que se contrae o dilata para 
	// adaptarse a la nueva cantidad de luz ambiente. 

	return RGBColor;
}

// ------------------------------------------------------------------
technique RenderScene
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_main();
		PixelShader = compile ps_2_0 ps_light();
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