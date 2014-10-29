using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Shaders;
using TgcViewer.Example;
using System.Drawing;
using TgcViewer.Utils.Terrain;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Collision.ElipsoidCollision;
using AlumnoEjemplos.BarbaAlpha.Barco;

namespace AlumnoEjemplos.BarbaAlpha
{

    public struct Plano
    {
        public Vector3 normal, punto;

        public Plano(Vector3 n1, Vector3 p1)
        {
            normal = n1;
            punto = p1;
        }
    }

    public class marAbierto : TgcExample
    {

        Microsoft.DirectX.Direct3D.Effect effect;
        BarcoIA barcoIA;
        BarcoJugador barcoJugador;
        float time;
        float scaleXZ;
        float scaleY;
        string heightmap;
        string textura;
        TgcSimpleTerrain terreno;
        TgcSkyBox skyBox;

        //variables necesarias para el render
        float frecuenciaOlas;
        float alturaOlas;
        float frecuenciaDeDisparo;
        float velocidadMaxima;

        //variables inclinacion
        float cosAngulo;
        float prodInterno;
        Vector3 sentidoBarco;

        Vector3 vecAux;
        Vector3 ortogonal;

        Texture textPerlinNoise1, textPerlinNoise2;

        Lluvia lluvia;
        Sol sol;
        
       

        // Buffers
        public static CustomVertex.PositionNormalTextured[] _vertices;
        public static VertexBuffer _vertexBuffer;


        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "BarbAlpha";
        }

        public override string getDescription()
        {
            return "Trata de derribar al otro barco antes de que el te derribe a vos! \n Te mueves con las flechas y disparas con la barra de Espacio";
        }

        public override void init()
        {
            //Device de DirectX para crear primitivas
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcSceneLoader loader = new TgcSceneLoader();

            barcoJugador = new BarcoJugador(new Vector3(0, 0, 0), this, GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\Canoa\\Canoa-TgcScene.xml");
            barcoIA = new BarcoIA(new Vector3(200, 0, 200), this, GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\Canoa\\Canoa-TgcScene.xml");
            //canoa = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\Canoa\\Canoa-TgcScene.xml").Meshes[0];

            string shaderFolder = GuiController.Instance.AlumnoEjemplosMediaDir +"\\shaders";
            time = 0;

            scaleXZ = 20f;
            scaleY = 1.3f;

            effect = TgcShaders.loadEffect(shaderFolder + "\\shaderLoco.fx");

            heightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "Heightmap\\" + "heightmap500.jpg";
            textura = GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Textures\\Liquidos" + "\\water_flow.jpg";

            terreno = new TgcSimpleTerrain();
            terreno.loadHeightmap(heightmap, scaleXZ, scaleY, new Vector3(0, 0, 0));
            terreno.loadTexture(textura);
            terreno.Effect = effect;
            terreno.Technique = "RenderScene";

            // Creo SkyBox
            string texturesPath = GuiController.Instance.ExamplesMediaDir + "Texturas\\Quake\\SkyBox LostAtSeaDay\\";
            string skyboxFolder = GuiController.Instance.AlumnoEjemplosMediaDir + "skybox\\";

            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 0, 0);
            skyBox.Size = new Vector3(8000, 8000, 8000);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, skyboxFolder + "skyboxArriba.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, skyboxFolder + "skyboxAbajo.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, skyboxFolder + "skybox2.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, skyboxFolder + "skybox2.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, skyboxFolder + "skybox1.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, skyboxFolder + "skybox2.jpg");
            skyBox.SkyEpsilon = 50f;
            skyBox.updateValues();


            //Centrar camara rotacional respecto a la canoa
            GuiController.Instance.RotCamera.Enable = true;
            GuiController.Instance.RotCamera.targetObject(barcoJugador.BoundingBox());
            
            barcoJugador.setEnemy(barcoIA);
            barcoJugador.setEffect(effect);
            barcoJugador.setTechnique("HeightScene");

            barcoIA.setEnemy(barcoJugador);
            barcoIA.setEffect(effect);
            barcoIA.setTechnique("HeightScene");

            GuiController.Instance.Modifiers.addFloat("alturaOlas", 5f, 30f, 10f);
            GuiController.Instance.Modifiers.addFloat("frecuenciaDeDisparo", 1f, 3f, 2f);
            GuiController.Instance.Modifiers.addFloat("frecuenciaOlas", 50f, 300f, 100f);
            GuiController.Instance.Modifiers.addFloat("velocidadMaxima", 10f, 400f, 100f);

            textPerlinNoise1 = TextureLoader.FromFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\perlin1.jpg");
            textPerlinNoise2 = TextureLoader.FromFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\perlin2.jpg");

            effect.SetValue("perlinNoise1", textPerlinNoise1);
            effect.SetValue("perlinNoise1", textPerlinNoise2);
            effect.SetValue("alpha", 0.8f);
            effect.SetValue("screen_dx", d3dDevice.PresentationParameters.BackBufferWidth);
            effect.SetValue("screen_dy", d3dDevice.PresentationParameters.BackBufferHeight);

            vecAux = new Vector3(0, 0, 0);
            ortogonal = new Vector3(0, 0, 0);

            sol = new Sol(effect); 
            //lluvia = new Lluvia();

        }

        public override void render(float elapsedTime)
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;
            
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;
            time += elapsedTime;

            alturaOlas = (float)GuiController.Instance.Modifiers["alturaOlas"];
            frecuenciaDeDisparo = (float)GuiController.Instance.Modifiers["frecuenciaDeDisparo"];
            frecuenciaOlas = (float)GuiController.Instance.Modifiers["frecuenciaOlas"];
            velocidadMaxima = (float)GuiController.Instance.Modifiers["velocidadMaxima"];


            sol.render();

            // Cargar variables de shader, por ejemplo el tiempo transcurrido.
            effect.SetValue("time", time);
            effect.SetValue("matWorldViewProj", device.Transform.World * device.Transform.View * device.Transform.Projection);
            effect.SetValue("amplitud", alturaOlas);
            effect.SetValue("frecuencia", frecuenciaOlas);



            //lluvia.render();

            effect.Technique = "RenderScene";
            
            terreno.render();

            effect.Technique = "HeightScene";

            barcoJugador.actualizarPosicionAnterior();

            //Render del barco del Jugador
            Plano planoBase = obtenerPlano(barcoJugador);

            setearVariablesBarcoShader(planoBase, barcoJugador.posicion(), effect);

            sentidoBarco = barcoJugador.calcularSentido();
            prodInterno = Vector3.Dot(planoBase.normal, sentidoBarco);
            cosAngulo = prodInterno;

            barcoJugador.aumentarAceleracionPorInclinacion(cosAngulo);
            barcoJugador.setFrecuenciaDeDisparos(frecuenciaDeDisparo);
            barcoJugador.setVelocidadMaxima(velocidadMaxima);
            barcoJugador.render(elapsedTime);


            //GuiController.Instance.Logger.log(barcoJugador.calcularSentido().ToString());
            //Render del barco con IA
            planoBase = obtenerPlano(barcoIA);

            setearVariablesBarcoShader(planoBase, barcoIA.posicion(), effect);

            sentidoBarco = barcoIA.calcularSentido();
            prodInterno = Vector3.Dot(planoBase.normal, sentidoBarco);
            cosAngulo = prodInterno;

            barcoIA.aumentarAceleracionPorInclinacion(cosAngulo);
            barcoIA.setFrecuenciaDeDisparos(frecuenciaDeDisparo);
            barcoIA.setVelocidadMaxima(velocidadMaxima);
            barcoIA.render(elapsedTime);


            // muevo el SkyBox para simular espacio infinito
            foreach (TgcMesh cara in skyBox.Faces)
            {
                cara.move(barcoJugador.posicion() - barcoJugador.getPosicionAnterior());
            }

            skyBox.render();

            //Actualizar posicion de cámara
            GuiController.Instance.RotCamera.targetObject(barcoJugador.BoundingBox());
            GuiController.Instance.CurrentCamera.updateCamera();
            

        }

        public override void close(){
            effect.Dispose();
            sol.dispose();
            barcoIA.dispose();
            barcoJugador.dispose();
            effect.Dispose();
            
            
        }

        public Vector3 aplicarTrigonometrica (Vector3 posicion, float radioY, float actualTime, float frecuencia){

            float X = posicion.X / frecuenciaOlas;
            float Z = posicion.Z / frecuenciaOlas;

            posicion.Y += radioY + (float)(Math.Sin(X + actualTime) * Math.Cos(Z + actualTime) + Math.Sin(Z + actualTime) + Math.Cos(X + actualTime)) * frecuencia;

            return posicion;
        }

        public Plano obtenerPlano (AlumnoEjemplos.BarbaAlpha.Barco.Barco barco)
        {
            float ancho;
            float largo;
            float radioEnY;
            Vector3 centroBase;
            Vector3 normalPlano;
            Vector3 puntoBase;
            Vector3 posicionLargo1;
            Vector3 posicionLargo2;
            Vector3 puntoLargo1;
            Vector3 puntoLargo2;
            Vector3 posicionAncho1;
            Vector3 puntoAncho1;
            Vector3 posicionAncho2;
            Vector3 puntoAncho2;
            Vector3 vector1;
            Vector3 vector2;

            Vector3 sentidoAux;

            radioEnY = barco.BoundingBox().calculateAxisRadius().Y;
            centroBase = barco.posicion() - new Vector3(0, radioEnY, 0);

            largo = barco.BoundingBox().calculateAxisRadius().X * 2;
            ancho = barco.BoundingBox().calculateAxisRadius().Z * 2;

            puntoBase = aplicarTrigonometrica(centroBase, radioEnY, time, alturaOlas);

            sentidoAux = barco.calcularSentido();

            posicionLargo1 = centroBase + new Vector3(largo / 2, 0, 0);
            //posicionLargo1 = centroBase + (largo / 2) * barco.calcularSentido();
            puntoLargo1 = aplicarTrigonometrica(posicionLargo1, radioEnY, time, alturaOlas);
            posicionLargo2 = centroBase + new Vector3(-largo / 2, 0, 0);
            //posicionLargo2 = centroBase - (largo / 2) * barco.calcularSentido();
            puntoLargo2 = aplicarTrigonometrica(posicionLargo2, radioEnY, time, alturaOlas);

            posicionAncho1 = centroBase + new Vector3(0, 0, ancho / 2);
            //posicionAncho1 = centroBase + (ancho / 2) * setearOrtogonal(barco.calcularSentido(), ortogonal);
            puntoAncho1 = aplicarTrigonometrica(posicionAncho1, radioEnY, time, alturaOlas);
            posicionAncho2 = centroBase + new Vector3(0, 0, -ancho / 2);
            //posicionAncho2 = centroBase - (ancho / 2) * setearOrtogonal(barco.calcularSentido(), ortogonal);
            puntoAncho2 = aplicarTrigonometrica(posicionAncho2, radioEnY, time, alturaOlas);

            vector1 = puntoLargo1 - puntoLargo2;
            vector2 = puntoAncho1 - puntoAncho2;
            normalPlano = Vector3.Normalize(Vector3.Cross(vector1, vector2));

            if (sentidoAux.Z > 0)
            {
                normalPlano.Y *= -1;
            }

            return new Plano(normalPlano, puntoBase);
          
        }

        public void setearVariablesBarcoShader(Plano plano, Vector3 posicionBarco, Microsoft.DirectX.Direct3D.Effect efecto)
        {
            efecto.SetValue("A", plano.normal.X);
            efecto.SetValue("B", plano.normal.Y);
            efecto.SetValue("C", plano.normal.Z);

            efecto.SetValue("xEnElPlano", plano.punto.X);
            efecto.SetValue("yEnElPlano", plano.punto.Y);
            efecto.SetValue("zEnElPlano", plano.punto.Z);

            efecto.SetValue("offsetX", posicionBarco.X);
            efecto.SetValue("offsetZ", posicionBarco.Z);
            efecto.SetValue("offsetY", posicionBarco.Y);
        }


        public Vector3 setearOrtogonal(Vector3 vector, Vector3 ortogonal)
        {
            if (vector == vecAux)
            {
                ortogonal.X = -1;
                ortogonal.Y = 0;
                ortogonal.Z = 0;
                return Vector3.Normalize(ortogonal);
            }

            ortogonal.X = -vector.Z;
            ortogonal.Y = 0;
            ortogonal.Z = vector.X;
            return Vector3.Normalize(ortogonal);

        }



        protected float[,] loadHeightMap(Microsoft.DirectX.Direct3D.Device d3dDevice, string path)
        {
            Bitmap bitmap = (Bitmap)Bitmap.FromFile(path);
            int width = bitmap.Size.Width;
            int length = bitmap.Size.Height;

            float[,] heightmap = new float[length, width];

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color pixel = bitmap.GetPixel(j, i);
                    float intensity = pixel.R * 0.299f + pixel.G * 0.587f + pixel.B * 0.114f;
                    heightmap[i, j] = intensity;
                }

            }
            bitmap.Dispose();
            return heightmap;
        }

        
    }

}
