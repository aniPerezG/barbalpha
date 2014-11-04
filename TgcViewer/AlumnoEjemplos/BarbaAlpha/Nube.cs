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
        private int cantidad_lluvias;
        private float longitud;
        private Vector3 centro_nube;
        private static Vector3 vertice_uno;
        private static Vector3 vertice_dos;
        private static Vector3 vertice_tres;
        private static Vector3 vertice_cuatro;


        public Nube(Vector3 centroNube, float longitudPlano, int cantidadLluvias)
        {
            centro_nube = centroNube;
            longitud = longitudPlano;
            cantidad_lluvias = cantidadLluvias;
        }

        public void mover(Vector3 v) 
        {
            foreach (Lluvia lluvia in lluvias)
            {
                lluvia.mover(v);
            }
        }

        public void setCentro(Vector3 v)
        {
            centro_nube = v;
        }

        public void armarLimitesLluvia()
        {
            vertice_uno = centro_nube + new Vector3(-longitud / 2, 0, -longitud / 2);
            vertice_dos = centro_nube + new Vector3(longitud / 2, 0, -longitud / 2);
            vertice_tres = centro_nube + new Vector3(longitud / 2, 0, longitud / 2);
            vertice_cuatro = centro_nube + new Vector3(-longitud / 2, 0, longitud / 2);

            Lluvia lluviaUno = new Lluvia(vertice_uno);
            Lluvia lluviaDos = new Lluvia(vertice_dos);
            Lluvia lluviaTres = new Lluvia(vertice_tres);
            Lluvia lluviaCuatro = new Lluvia(vertice_cuatro);

            lluviaUno.condensate(1);
            lluviaDos.condensate(1);
            lluviaTres.condensate(1);
            lluviaCuatro.condensate(1);

            lluvias.Add(lluviaUno);
            lluvias.Add(lluviaDos);
            lluvias.Add(lluviaTres);
            lluvias.Add(lluviaCuatro);

            crearLluvias(vertice_uno, vertice_dos, false);
            crearLluvias(vertice_dos, vertice_tres, false);
            crearLluvias(vertice_tres, vertice_cuatro, true);
            crearLluvias(vertice_cuatro, vertice_uno, true);
        }

        public void crearLluvias(Vector3 p1, Vector3 p2, Boolean hack)
        {
            Vector3 vecAux = p2 - p1;

            float modulo = Vector3.Length(vecAux);
            float distanciaEntreLluvias = modulo / (cantidad_lluvias + 1);

            if (hack) distanciaEntreLluvias *= -1;

            if (vecAux.X == 0) vecAux = new Vector3(0, 0, distanciaEntreLluvias); // los puntos están alineados en X, muevo en Z
            else vecAux = new Vector3(distanciaEntreLluvias, 0, 0); // los puntos están alineados en Z, muevo en X

            for (int i = 0; i < cantidad_lluvias; i++)
            {
                Lluvia nuevaLluvia = new Lluvia(p1 + vecAux);
                nuevaLluvia.condensate(1);
                lluvias.Add(nuevaLluvia);
                if (vecAux.X == 0) vecAux.Z += distanciaEntreLluvias;
                else vecAux.X += distanciaEntreLluvias;
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
