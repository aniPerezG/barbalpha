﻿using System;
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
        float time;
        TgcSimpleTerrain terreno;
        Texture renderTarget;
        BarcoJugador barcoJugador;
        BarcoIA barcoIA;
        string heightmap;
        string textura;
        float scaleXZ;
        float scaleY;
        TgcMesh canoa;
        TgcScene scene;

        
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
            barcoIA = new BarcoIA(new Vector3(4, 0, 4), this, GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\Canoa\\Canoa-TgcScene.xml", barcoJugador);
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


            //Centrar camara rotacional respecto a la canoa
            GuiController.Instance.RotCamera.Enable = true;
            GuiController.Instance.RotCamera.targetObject(barcoJugador.BoundingBox);

            barcoJugador.setEffect(effect);
            barcoJugador.setTechnique("HeightScene");

            barcoIA.setEffect(effect);
            barcoIA.setTechnique("HeightScene");

            GuiController.Instance.Modifiers.addFloat("alturaOlas", 5f, 30f, 10f);
            GuiController.Instance.Modifiers.addFloat("frecuenciaOlas", 50f, 300f, 100f);

        }

        public override void render(float elapsedTime)
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;
            
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;
            time += elapsedTime;

            float alturaOlas = (float)GuiController.Instance.Modifiers["alturaOlas"];
            float frecuenciaOlas = (float)GuiController.Instance.Modifiers["frecuenciaOlas"];

            // Cargar variables de shader, por ejemplo el tiempo transcurrido.
            effect.SetValue("time", time);
            effect.SetValue("matWorldViewProj", device.Transform.World * device.Transform.View * device.Transform.Projection);
            effect.SetValue("amplitud", alturaOlas);
            effect.SetValue("frecuencia", frecuenciaOlas);
            //effect.SetValue("centroBarco", barcoJugador.centro);


            effect.Technique = "RenderScene";
            terreno.render();

            effect.Technique = "HeightScene";
            barcoJugador.render(elapsedTime);
            //barcoIA.render(elapsedTime);

            //Actualizar posicion de cámara
            GuiController.Instance.RotCamera.targetObject(barcoJugador.BoundingBox);
            GuiController.Instance.CurrentCamera.updateCamera();

        }

        public override void close(){
            effect.Dispose();
        }
    }
}