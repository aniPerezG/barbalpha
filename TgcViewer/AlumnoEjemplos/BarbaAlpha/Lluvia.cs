using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.BarbaAlpha
{
    class Lluvia
    {
        private Vector3 posicion_lluvia;
        private Random generador = new Random();
        public List<Gota> gotas = new List<Gota>();


        public Lluvia(Boolean hack)
        {
            int aux = 1;
            if (hack)
            {
                aux *= -1;
            }
            posicion_lluvia.X = generador.Next(1500) * aux;
            posicion_lluvia.Y = 700;
            posicion_lluvia.Z = generador.Next(1500) * aux;
        }

        public Vector3 getPosicion()
        {
            return posicion_lluvia;
        }

        public void condensate(int cantidadGotas)
        {
            for (int i = 0; i < cantidadGotas; i++)
            {
                Gota gota = new Gota(posicion_lluvia, 2f, 3f, this);
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
                gota.llovete();
                gota.getGotita().updateValues();
                TgcCollisionUtils.FrustumResult resultado = TgcCollisionUtils.classifyFrustumSphere(GuiController.Instance.Frustum, gota.getGotita().BoundingSphere);
                if (resultado == TgcCollisionUtils.FrustumResult.INSIDE || resultado == TgcCollisionUtils.FrustumResult.INTERSECT)
                {
                    gota.render(elapsedTime);
                }
            }
        }
    }
}
