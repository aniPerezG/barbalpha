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
        private bool estasMuyLejos = true;

        public BarcoIA(Vector3 posicionInicial, marAbierto oceano, string pathEscena)
            : base(posicionInicial, oceano, pathEscena) { this.direccion.haciaLaDerecha(); }

        public Vector3 posicionEnemigo()
        {
            return getEnemy().posicion();
        }

        private Vector3 obtenerDireccionAEnemigo()
        {   // Retorna el vector director de la recta que pasa por la posición de este barco y
            // por la posición del barco enemigo
            return Vector3.Normalize(this.posicionEnemigo() - this.posicion());
        }

        private void evaluarDistanciaDeEnemigo()
        {
            if (Vector3.Length(obtenerDireccionAEnemigo()) <= distancia_maxima) estasMuyLejos = false;
            else estasMuyLejos = true;
        }

        private Boolean estoyApuntandoAEnemigo()
        {
            return obtenerDireccionAEnemigo() == getSentido();
        }

        private float gradosARotar()
        {
            return FastMath.Acos(Vector3.Dot(this.obtenerDireccionAEnemigo(), this.getSentido()));
        }

        private float radianesARotar()
        {
            return Geometry.DegreeToRadian(this.gradosARotar());
        }

        private void apuntarEnemigo()
        {
            //GuiController.Instance.Logger.log(this.getSentido().ToString());
            if (!estoyApuntandoAEnemigo())
            {
                this.rotarSobreY(radianesARotar());
            }
        }

        protected override void moverYVirar(float elapsedTime)
        {
            this.evaluarDistanciaDeEnemigo();
            this.apuntarEnemigo();
            if (estasMuyLejos)
            {
                this.acelerar(-1);
            }
            else this.disparar();
        }

        public override void render(float elapsedTime)
        {
            base.render(elapsedTime);
        }
    }
}
