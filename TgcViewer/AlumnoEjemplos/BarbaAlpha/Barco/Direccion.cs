using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.BarbaAlpha.Barco
{
    public class Direccion     {
        public bool derecha;

        public void haciaLaDerecha()
        {
            derecha = true;
        }

        public void haciaLaIzquierda()
        {
            derecha = false;
        }
        public bool esDerecha() {
            return derecha;
        }
    }
}
