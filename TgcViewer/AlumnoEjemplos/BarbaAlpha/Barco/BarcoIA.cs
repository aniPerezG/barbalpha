using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX.DirectInput;
using TgcViewer;

namespace AlumnoEjemplos.BarbaAlpha.Barco
{
    class BarcoIA : Barco
    {
        private const float distancia_maxima = 500;
        private const float frecuencia_disparo = 3;
        private Vector3 direccion_normal = new Vector3(0, 0, -1);
        private Vector3 posicionAnteriorEnemy;
        private bool estasMuyLejos = true;

        public BarcoIA(Vector3 posicionInicial, marAbierto oceano, string pathEscena)
            : base(posicionInicial, oceano, pathEscena) { this.direccion.haciaLaDerecha(); }

        public Vector3 posicionEnemigo()
        {
            return getEnemy().posicion();
        }

        public void rotate(Vector3 rotacion)
        {
            this.barco.rotateX(rotacion.X);
            this.barco.rotateY(rotacion.Y);
            this.barco.rotateZ(rotacion.Z);
        }

        private void virarHaciaDestino() 
        {   // Debería virar el barco hasta alinearse en la direccion propuesta
            direccion_normal.Normalize();
            obtenerDireccionAEnemigo().Normalize();

            double angOrg = Math.Atan2(direccion_normal.Z, direccion_normal.X) + 2 * Math.PI;
            double angDst = Math.Atan2(obtenerDireccionAEnemigo().Z, obtenerDireccionAEnemigo().X) + 2 * Math.PI;
            double distLeft = angOrg > angDst ? 2 * Math.PI - angOrg + angDst : angDst - angOrg;
            double distRight = angOrg < angDst ? 2 * Math.PI - angDst + angOrg : angOrg - angDst;

            if (distLeft < distRight) this.rotate(new Vector3(1, 0, 0));
            else this.rotate(new Vector3(-1, 0, 0));
        }

        private Vector3 obtenerDireccionAEnemigo()
        {   // Retorna el vector director de la recta que pasa por la posición de este barco y
            // por la posición del barco enemigo
            return (new Vector3(this.posicionEnemigo().X - this.posicion().X,
                               this.posicionEnemigo().Y - this.posicion().Y,
                               this.posicionEnemigo().Z - this.posicion().Z));
        }

        private void evaluarDistanciaDeEnemigo()
        {
            if (Vector3.Length(obtenerDireccionAEnemigo()) <= distancia_maxima) estasMuyLejos = false;
            else estasMuyLejos = true;
        }

        private void apuntarEnemigo(float elapsedTime)
        {
        }

        protected override void moverYVirar(float elapsedTime)
        {   // El barco se mueve manteniendo una mínima distancia 'd' respecto de la posición del barco enemigo
            this.evaluarDistanciaDeEnemigo();
            this.apuntarEnemigo(elapsedTime);
            if (estasMuyLejos)
            {   // Debo virar en dirección al destino, moverme hacia esa posición y virar en posicion de disparo*/
                this.acelerar(-1);
            }
            else this.disparar();
        }

        public override void render(float elapsedTime)
        {
            base.render(elapsedTime);
            posicionAnterior = this.posicion();
            posicionAnteriorEnemy = this.posicionEnemigo();
        }
    }
}
