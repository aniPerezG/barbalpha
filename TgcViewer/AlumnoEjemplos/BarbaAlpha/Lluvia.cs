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
        public List<Gota> gotas = new List<Gota>();


        public Lluvia(Vector3 posicion)
        {
            posicion_lluvia = posicion;
        }
        public void setPosicion(Vector3 v)
        {
            this.posicion_lluvia = v;
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
                Gota gota = new Gota(posicion_lluvia, 1f, 3f, this);
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
                TgcCollisionUtils.FrustumResult resultado = TgcCollisionUtils.classifyFrustumSphere(GuiController.Instance.Frustum, gota.getGotita().BoundingSphere);
                if (resultado == TgcCollisionUtils.FrustumResult.INSIDE || resultado == TgcCollisionUtils.FrustumResult.INTERSECT)
                {
                    gota.render(elapsedTime);
                }
            }
        }
    }
}
