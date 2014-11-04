using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer;

namespace AlumnoEjemplos.BarbaAlpha
{
    class Nube
    {
        private List<Lluvia> lluvias = new List<Lluvia>();
        private static Vector3 vertice_uno;
        private static Vector3 vertice_dos;
        private static Vector3 vertice_tres;
        private static Vector3 vertice_cuatro;

        public Nube(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int cantidadLluvias)
        {
            asignarVertices(v1, v2, v3, v4);
            crearLluvias(cantidadLluvias);
        }

        private static void asignarVertices(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            vertice_uno = v1;
            vertice_dos = v2;
            vertice_tres = v3;
            vertice_cuatro = v4;
        }

        private Vector3 obtenerPosRandom()
        {
            Random generador = new Random();
            int i = generador.Next();

            if (i % 2 == 0)
            {
                return vertice_dos;
            }
            if (i % 3 == 0)
            {
                return vertice_tres;
            }
            if (i % 4 == 0)
            {
                return vertice_cuatro;
            }
            else
            {
                return vertice_uno;
            }
        }

        private void crearLluvias(int cantidadLluvias)
        {
            for (int i = 0; i < cantidadLluvias; i++)
            {
                Lluvia lluvia = new Lluvia(obtenerPosRandom());
                lluvia.condensate(10);
                lluvias.Add(lluvia);
            }
        }

        public void render(float elapsedTime)
        {
            foreach (Lluvia lluvia in lluvias)
            {
                lluvia.render(elapsedTime);
            }
        }
    }
}
