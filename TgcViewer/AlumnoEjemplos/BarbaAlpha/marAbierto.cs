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


namespace AlumnoEjemplos.BarbaAlpha
{
    public class marAbierto : TgcExample
    {

        Microsoft.DirectX.Direct3D.Effect effect;
        float time;
        TgcSimpleTerrain terreno;

        TgcMesh canoa;
        string heightmap;
        string textura;
        float scaleXZ;
        float scaleY;
        TgcElipsoid canoaEliptica;

        TgcScene scene;
       
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

            canoa = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\Canoa\\Canoa-TgcScene.xml").Meshes[0];

            canoa.Position = new Vector3(0, 5, 0);
            //canoa.AutoUpdateBoundingBox = false;

            canoaEliptica = new TgcElipsoid(canoa.BoundingBox.calculateBoxCenter(), new Vector3(5, 5, 20));

           
            string shaderFolder = GuiController.Instance.AlumnoEjemplosMediaDir +"\\shaders";
            time = 0;

            scaleXZ = 20f;
            scaleY = 1.3f;

            effect = TgcShaders.loadEffect(shaderFolder + "\\shaderLoco.fx");

            heightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "Heightmap\\" + "heightmap1.jpg";
            textura = GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Textures\\Liquidos" + "\\water_flow.jpg";
            

            terreno = new TgcSimpleTerrain();
            terreno.loadHeightmap(heightmap, scaleXZ, scaleY, new Vector3(0, 0, 0));
            terreno.loadTexture(textura);
            terreno.Effect = effect;
            terreno.Technique = "RenderScene";


            //Centrar camara rotacional respecto a la canoa
            GuiController.Instance.RotCamera.Enable = true;
            GuiController.Instance.RotCamera.targetObject(canoa.BoundingBox);
        }

        public override void render(float elapsedTime)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            time += elapsedTime;

            // Cargar variables de shader, por ejemplo el tiempo transcurrido.
            effect.SetValue("time", time);


            //Capturar las teclas del teclado
            TgcD3dInput input = GuiController.Instance.D3dInput;
            Vector3 movement = new Vector3(0, 0, 0);
            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                movement.X = 1;
            }
            else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                movement.X = -1;
            }
            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                movement.Z = -1;
            }
            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                movement.Z = 1;
            }


            //Guardar la posicion anterior antes de cambiarla
            Vector3 originalPos = canoa.Position;

            //Adaptar la canoa a la altura del agua
            /*float x = originalPos.X;
            float z = originalPos.Z;

            float largo = scaleXZ * 64;
            float pos_i = 64f * (0.5f + x / largo);
            float pos_j = 64f * (0.5f + z / largo);

            int pi = (int)x;
            float fracc_i = pos_i - pi;
            int pj = (int)z;
            float fracc_j = pos_j - pj;

            int pi1 = pi + 1;
            int pj1 = pj + 1;


            float H0 = terreno.HeightmapData[pi, pj] * scaleY;
            float H1 = terreno.HeightmapData[pi1, pj] * scaleY;
            float H2 = terreno.HeightmapData[pi, pj1] * scaleY;
            float H3 = terreno.HeightmapData[pi1, pj1] * scaleY;
            float H = (H0 * (1 - fracc_i) + H1 * fracc_i) * (1 - fracc_j) +
                      (H2 * (1 - fracc_i) + H3 * fracc_i) * fracc_j;

            */
            //Aplicar movimiento
            canoa.move(movement);
            /*
            Vector3 posicion = canoa.Position;
            canoa.Position = posicion + new Vector3(0, H, 0);
            */
            //Actualizar posicion de cámara
            GuiController.Instance.RotCamera.targetObject(canoa.BoundingBox);
            GuiController.Instance.CurrentCamera.updateCamera();

            terreno.render();
            canoa.render();


        }

        public override void close(){
            effect.Dispose();
        }

    }
}