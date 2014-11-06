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
        private Vector3 velocidad_caida = new Vector3(0, 0, 0);
        private Vector3 velocidad_aux = new Vector3(0, 0, 0);
        private Vector3 posicion_inicial;
        private TgcSphere gotita;
        private Lluvia lluvia;
        private Random generador = new Random();

        public Gota(Vector3 posicion, float anchoGota, float altoGota, Lluvia tremendaLluvia) 
        {
            posicion_inicial = posicion;


            gotita = new TgcSphere();
            gotita.BasePoly = TgcSphere.eBasePoly.CUBE;
            gotita.Radius =  anchoGota / 2;
            gotita.Inflate = false;
            gotita.setColor(Color.AliceBlue);
            gotita.updateValues();

            velocidad_caida.Y = -generador.Next(20);

            lluvia = tremendaLluvia;
        }

        public TgcSphere getGotita()
        {
            return gotita;
        }

        public float getPositionY()
        {
            return gotita.Position.Y;
        }

        public void llovete()
        {
            gotita.move(velocidad_caida);
        }

        public void subiteDeNuevo()
        {
            this.gotita.Position = lluvia.getPosicion();
        }

        public void render(float elapsedTime)
        {
            gotita.render();
        }

    }
}
