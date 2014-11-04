using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;

namespace AlumnoEjemplos.BarbaAlpha
{
    class Gota
    {
        private Vector3 velocidad_caida = new Vector3(0, -10f, 0);
        private Vector3 velocidad_aux = new Vector3(0, 0, 0);
        private Vector3 posicion_inicial;
        private TgcCylinder gotita;
        private Lluvia lluvia;


        public Gota(Vector3 posicion, float anchoGota, float altoGota, Lluvia tremendaLluvia) 
        {
            posicion_inicial = posicion;

            gotita = new TgcCylinder(posicion_inicial, anchoGota, altoGota / 2);
            gotita.AlphaBlendEnable = true;
            gotita.Color = Color.Gray;
            gotita.updateValues();
            
            lluvia = tremendaLluvia;
        }

        public float getPositionY()
        {
            return gotita.Position.Y;
        }

        public void subiteDeNuevo()
        {
            this.gotita.Position = lluvia.getPosicion();
        }
        
        public void setVelocidadCaida(Vector3 velocidad)
        {
            // this.velocidad_caida = velocidad;
        }

        public void render(float elapsedTime)
        {
            //velocidad_aux = velocidad_caida;
            //velocidad_aux.Multiply(elapsedTime);

            gotita.move(velocidad_caida);
            gotita.updateValues();
            gotita.render();
        }

    }
}
