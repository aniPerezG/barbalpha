using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.Particles;

namespace AlumnoEjemplos.BarbaAlpha
{
    class Lluvia
    {
  
        public List<ParticleEmitter> Emisores = new List<ParticleEmitter>();
        public void init()
        {
            for (int i = 0; i < 70; i++)
            {
                ParticleEmitter Emisor = new ParticleEmitter(GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\gotaSola.png", 100);
                Emisor.Speed = new Vector3(0, -150, 0);
                Emisor.Dispersion = 100;
                Emisor.MinSizeParticle = 5;
                Emisor.MaxSizeParticle = 10;
                Emisor.CreationFrecuency = 0.3f;
                Emisor.ParticleTimeToLive = 6;
                Emisores.Add(Emisor);
            }
        }

        public void render()
        {

            Vector3 Posicion = GuiController.Instance.CurrentCamera.getPosition();
            Vector3 LookAt = GuiController.Instance.CurrentCamera.getLookAt();
            Posicion.Y += 90;
            foreach (var Emisor in Emisores)
            {
                Posicion.X = LookAt.X ;
                Posicion.Z = LookAt.Z ;
                Emisor.Position = Posicion;
                Emisor.render();
            }
        }
    }
}
