using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer;

namespace AlumnoEjemplos.BarbaAlpha
{
    class Lluvia
    {
        private Vector3 posicion_lluvia;
        public List<Gota> gotas = new List<Gota>();


        public Lluvia(Vector3 posicion)
        {
            posicion_lluvia = posicion;
        }

        public Vector3 getPosicion()
        {
            return this.posicion_lluvia;
        }

        public void mover(Vector3 v)
        {
            posicion_lluvia += v;
        }

        public void condensate(int cantidadGotas)
        {
            for (int i = 0; i < cantidadGotas; i++)
            {
                Gota gota = new Gota(posicion_lluvia, 2, 4, this);
                gotas.Add(gota);
            }
        }

        public static void controlarGota(Gota gotita) 
        {
            if (gotita.getPositionY() <= 0) gotita.subiteDeNuevo();
        }

        public void render(float elapsedTime)
        {
            foreach (Gota gota in gotas)
            {
                controlarGota(gota);
                gota.render(elapsedTime);
            }
        }
    }
}
