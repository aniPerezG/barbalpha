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
            posicionSol = new Vector3(0, 1000, 1000);
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

        public Vector3 getPosition()
        {
            return sol.Position;
        }

        public void render()
        {
            sol.render();
        }
        

        public void dispose()
        {
            sol.dispose();
        }
    }
}
