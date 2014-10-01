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

namespace AlumnoEjemplos.BarbaAlpha
{
    public class marAbierto : TgcExample
    {

        Effect effect;
        float time;

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

            string shaderFolder = GuiController.Instance.AlumnoEjemplosMediaDir +"\\shaders";

            time = 0;

            
            data = new CustomVertex.PositionColored[3];
            data2 = new CustomVertex.PositionColored[3];
            data3 = new CustomVertex.PositionColored[3];

            data[0] = new CustomVertex.PositionColored(0, 1, 0, Color.Red.ToArgb());
            data[1] = new CustomVertex.PositionColored(3, 1, 0, Color.Red.ToArgb());
            data[2] = new CustomVertex.PositionColored(3, 1, 4, Color.Red.ToArgb());


            data2[0] = new CustomVertex.PositionColored(3, 1, 4, Color.Blue.ToArgb());
            data2[1] = new CustomVertex.PositionColored(6, 1, 4, Color.Blue.ToArgb());
            data2[2] = new CustomVertex.PositionColored(3, 1, 0, Color.Blue.ToArgb());

            data3[0] = new CustomVertex.PositionColored(3, 1, 0, Color.Green.ToArgb());
            data3[1] = new CustomVertex.PositionColored(6, 1, 0, Color.Green.ToArgb());
            data3[2] = new CustomVertex.PositionColored(6, 1, 4, Color.Green.ToArgb());


            effect = TgcShaders.loadEffect(shaderFolder + "\\shaderLoco.fx");

           

        }

        public override void render(float elapsedTime)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            d3dDevice.VertexFormat = CustomVertex.PositionColored.Format;

            time += elapsedTime;
            effect.SetValue("time", time);

            
            
            effect.Begin(FX.None);
            effect.BeginPass(0);


            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleList, 1, data);
            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleList, 1, data2);
            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleList, 1, data3);


            effect.EndPass();
            effect.End();
        }

        public override void close(){

        }

    }
}