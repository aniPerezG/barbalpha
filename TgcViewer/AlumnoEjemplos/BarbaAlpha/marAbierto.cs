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

        //variables necesarias para calculo del plano
        float ancho;
        float largo;
        float radioEnY;
        Vector3 centroBase;
        Vector3 normalPlano;
        Vector3 punto1;
        Vector3 posicion2;
        Vector3 punto2;
        Vector3 posicion3;
        Vector3 punto3;
        Vector3 vector1;
        Vector3 vector2;

        //variables necesarias para "Infinitud" del terreno
        Vector3 posicionAnterior;
        
        // Buffers
        public static CustomVertex.PositionNormalTextured[] _vertices;
        public static VertexBuffer _vertexBuffer;


        public override string getCategory()
        {
            return "Pirate Ship";
        }

        public override string getName()
        {
            return "Mar Abierto";
        }

        public override string getDescription()
        {
            return "MiIdea - Descripcion de la idea";
        }

        public override void init()
        {
            //Device de DirectX para crear primitivas
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcSceneLoader loader = new TgcSceneLoader();

            barcoJugador = new BarcoJugador(new Vector3(0, 0, 0), this, GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\Canoa\\Canoa-TgcScene.xml");
            barcoIA = new BarcoIA(new Vector3(4, 0, 4), this, GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\Canoa\\Canoa-TgcScene.xml");
            //canoa = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\Canoa\\Canoa-TgcScene.xml").Meshes[0];

            string shaderFolder = GuiController.Instance.AlumnoEjemplosMediaDir +"\\shaders";
            time = 0;

            scaleXZ = 20f;
            scaleY = 1.3f;

            effect = TgcShaders.loadEffect(shaderFolder + "\\shaderLoco.fx");

            heightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "Heightmap\\" + "heightmap11.jpg";
            textura = GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Textures\\Liquidos" + "\\water_flow.jpg";

            terreno = new TgcSimpleTerrain();
            terreno.loadHeightmap(heightmap, scaleXZ, scaleY, new Vector3(0, 0, 0));
            terreno.loadTexture(textura);
            terreno.Effect = effect;
            terreno.Technique = "RenderScene";

            // Creo SkyBox
            string texturesPath = GuiController.Instance.ExamplesMediaDir + "Texturas\\Quake\\SkyBox LostAtSeaDay\\";
            
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 0, 0);
            skyBox.Size = new Vector3(8000, 8000, 8000);
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "lostatseaday_up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "lostatseaday_dn.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "lostatseaday_lf.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "lostatseaday_rt.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "lostatseaday_ft.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "lostatseaday_bk.jpg");
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

            // Cargar variables de shader, por ejemplo el tiempo transcurrido.
            effect.SetValue("time", time);
            effect.SetValue("matWorldViewProj", device.Transform.World * device.Transform.View * device.Transform.Projection);
            effect.SetValue("amplitud", alturaOlas);
            effect.SetValue("frecuencia", frecuenciaOlas);


            effect.Technique = "RenderScene";
            
            terreno.render();

            effect.Technique = "HeightScene";

            //-----------------------------------------------------
            // calculo la ecucacion del plano que esta formada por los puntos cercanos al centro de la base del barco
            radioEnY = barcoJugador.BoundingBox().calculateAxisRadius().Y;
            centroBase = barcoJugador.posicion() - new Vector3(0, radioEnY , 0);

            largo = barcoJugador.BoundingBox().calculateAxisRadius().X * 2;
            ancho = barcoJugador.BoundingBox().calculateAxisRadius().Z * 2;
            punto1 = aplicarTrigonometrica(barcoJugador.posicion(), radioEnY, time, alturaOlas);
            posicion2 = barcoJugador.posicion() + new Vector3(largo/3, 0, 0);
            punto2 = aplicarTrigonometrica(posicion2, radioEnY, time, alturaOlas);
            posicion3 = barcoJugador.posicion() + new Vector3(0, 0, ancho/3);
            punto3 = aplicarTrigonometrica(posicion3, radioEnY, time, alturaOlas);

            vector1 = punto2 - punto1;
            vector2 = punto3 - punto1;
            normalPlano = Vector3.Cross(vector1, vector2);

            posicionAnterior = barcoJugador.posicion();
            barcoJugador.setPosicion(new Vector3(barcoJugador.posicion().X, aplicarTrigonometrica(barcoJugador.posicion(), 0, time, alturaOlas).Y - barcoJugador.posicion().Y, barcoJugador.posicion().Z));
            //barcoJugador.setPosicion(aplicarTrigonometrica(barcoJugador.posicion(), radioEnY, t.ime));

            effect.SetValue("A", normalPlano.X);
            effect.SetValue("B", normalPlano.Y);
            effect.SetValue("C", normalPlano.Z);

            effect.SetValue("xEnElPlano", centroBase.X);
            effect.SetValue("yEnElPlano", centroBase.Y);
            effect.SetValue("zEnElPlano", centroBase.Z);

            //------------------------------------------------------
            

            effect.SetValue("offsetX", barcoJugador.posicion().X);
            effect.SetValue("offsetZ", barcoJugador.posicion().Z);
            effect.SetValue("offsetY", barcoJugador.posicion().Y);

            barcoJugador.setFrecuenciaDeDisparos(frecuenciaDeDisparo);
            barcoJugador.setVelocidadMaxima(velocidadMaxima);
            barcoJugador.render(elapsedTime);

            effect.SetValue("offsetX", barcoIA.posicion().X);
            effect.SetValue("offsetZ", barcoIA.posicion().Z);
            effect.SetValue("offsetY", barcoIA.posicion().Y);

            barcoIA.setFrecuenciaDeDisparos(frecuenciaDeDisparo);
            barcoIA.setVelocidadMaxima(velocidadMaxima);
            barcoIA.render(elapsedTime);

            // muevo el SkyBox para simular espacio infinito
            foreach (TgcMesh cara in skyBox.Faces)
            {
                cara.move(barcoJugador.posicion() - posicionAnterior);
            }

            skyBox.render();

            //Actualizar posicion de cámara
            GuiController.Instance.RotCamera.targetObject(barcoJugador.BoundingBox());
            GuiController.Instance.CurrentCamera.updateCamera();

        }

        public override void close(){
            effect.Dispose();
        }

        public Vector3 aplicarTrigonometrica (Vector3 posicion, float radioY, float actualTime, float frecuencia){

            float X = posicion.X / frecuenciaOlas;
            float Z = posicion.Z / frecuenciaOlas;

            posicion.Y += radioY + (float)(Math.Sin(X + actualTime) * Math.Cos(Z + actualTime) + Math.Sin(Z + actualTime) + Math.Cos(X + actualTime)) * frecuencia;

            return posicion;
        }



    }

}