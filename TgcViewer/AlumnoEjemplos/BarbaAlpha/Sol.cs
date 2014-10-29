using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.BarbaAlpha
{
    class Sol
    {
        TgcSphere sol;
        Vector3 posicionSol;
        Vector3 CameraPos;
        Effect effect;

        public Sol(Effect efecto)
        {
            posicionSol = new Vector3(0, 1500, 0);
            sol = new TgcSphere();
            sol.setColor(Color.Yellow);
            sol.Radius = 100;
            sol.BasePoly = TgcSphere.eBasePoly.CUBE;
            sol.Position = posicionSol;
            effect = efecto;
            //sol.Effect = efecto;
            //sol.Technique = "LightTechnique";
            sol.updateValues();

            CameraPos = GuiController.Instance.CurrentCamera.getPosition();
        }

        public void render()
        {
            //cargar variables para la iluminacion
            this.setearEfecto();

            sol.render();
        }

        public void setearEfecto()
        {
            CameraPos = GuiController.Instance.CurrentCamera.getPosition();

            effect.SetValue("CameraPosX", CameraPos.X);
            effect.SetValue("CameraPosY", CameraPos.Y);
            effect.SetValue("CameraPosZ", CameraPos.Z);

            effect.SetValue("LightPositionX", this.posicionSol.X);
            effect.SetValue("LightPositionY", this.posicionSol.Y);
            effect.SetValue("LightPositionZ", this.posicionSol.Z);

            effect.SetValue("LightDiffuseColorX", Color.White.R);
            effect.SetValue("LightDiffuseColorY", Color.White.G);
            effect.SetValue("LightDiffuseColorZ", Color.White.B);

            effect.SetValue("LightSpecularColorX", Color.White.R);
            effect.SetValue("LightSpecularColorY", Color.White.G);
            effect.SetValue("LightSpecularColorZ", Color.White.B);

            effect.SetValue("DiffuseColorX", Color.White.R);
            effect.SetValue("DiffuseColorY", Color.White.G);
            effect.SetValue("DiffuseColorZ", Color.White.B);

            effect.SetValue("AmbientLightColorX", Color.White.R);
            effect.SetValue("AmbientLightColorY", Color.White.G);
            effect.SetValue("AmbientLightColorZ", Color.White.B);

            effect.SetValue("EmissiveColorX", Color.White.R);
            effect.SetValue("EmissiveColorY", Color.White.G);
            effect.SetValue("EmissiveColorZ", Color.White.B);

            effect.SetValue("SpecularColorX", Color.Yellow.R);
            effect.SetValue("SpecularColorY", Color.Yellow.G);
            effect.SetValue("SpecularColorZ", Color.Yellow.B);

            effect.SetValue("LightDistanceSquared", 10);
            effect.SetValue("SpecularPower", 10);
        }
    }
}
