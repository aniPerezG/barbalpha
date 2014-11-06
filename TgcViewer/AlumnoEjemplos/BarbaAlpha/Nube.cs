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

        public Nube(int cantidadLluvias)
        {
            cantidad_lluvias = cantidadLluvias;
            crearLluvias();
        }

        public void crearLluvias()
        {
            Lluvia nuevaLluvia;
            for (int i = 0; i < cantidad_lluvias; i++)
            {
                if (i % 2 == 0)
                {
                    nuevaLluvia = new Lluvia(true);
                }
                else
                {
                    nuevaLluvia = new Lluvia(false);
                }
                nuevaLluvia.condensate(3);
                lluvias.Add(nuevaLluvia);
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
