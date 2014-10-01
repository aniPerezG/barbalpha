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

namespace AlumnoEjemplos.BarbaAlpha
{
    public class marAbierto : TgcExample
    {

        Effect effect;
        float time;
        TgcSimpleTerrain terreno;
        TgcMesh canoa;
        string heightmap;
        string textura;
        float scaleXZ;
        float scaleY;

        VertexBuffer bolsaDeVertices;


        CustomVertex.PositionColored[] data;
        CustomVertex.PositionColored[] data2;
        CustomVertex.PositionColored[] data3;

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
            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcSceneLoader loader = new TgcSceneLoader();

            canoa = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\Canoa\\Canoa-TgcScene.xml").Meshes[0];

            canoa.Position = new Vector3(0, 5, 0);

            string shaderFolder = GuiController.Instance.AlumnoEjemplosMediaDir +"\\shaders";
            time = 0;

            scaleXZ = 20f;
            scaleY = 1.3f;

            effect = TgcShaders.loadEffect(shaderFolder + "\\shaderLoco.fx");

            heightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "Heightmap\\" + "heightmap1.jpg";
            textura = GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Textures\\Liquidos" + "\\water_flow.jpg";
            

            terreno = new TgcSimpleTerrain();
            terreno.loadHeightmap(heightmap, scaleXZ, scaleY, new Vector3(50, 0, 50));
            terreno.loadTexture(textura);
            terreno.Effect = effect;
            terreno.Technique = "RenderScene";


            GuiController.Instance.RotCamera.Enable = true;
            GuiController.Instance.RotCamera.targetObject(canoa.BoundingBox);
        }

        public override void render(float elapsedTime)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            time += elapsedTime;

            GuiController.Instance.RotCamera.targetObject(canoa.BoundingBox);
            GuiController.Instance.CurrentCamera.updateCamera();


            // Cargar variables de shader, por ejemplo el tiempo transcurrido.
            
            effect.SetValue("time", time);
            terreno.render();
            canoa.render();


        }

        public override void close(){
            effect.Dispose();
        }

    }
}