using AlumnoEjemplos.BarbaAlpha.Barco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;

namespace AlumnoEjemplos.BarbaAlpha.Barco
{
    class BarcoIA : Barco
    {
        private const float distancia_maxima = 500;
        private const float distancia_minima = 200;
        private const float frecuencia_disparo = 3;
        private Vector3 direccion_normal = new Vector3(0, 0, -1);
        private bool estasMuyLejos = true;
        private bool estasMuyCerca = false;
        private bool tengoQueEscaparme = false;

        public BarcoIA(Vector3 posicionInicial, marAbierto oceano, string pathEscena)
            : base(posicionInicial, oceano, pathEscena) { this.direccion.haciaLaDerecha(); }

        public Vector3 posicionEnemigo()
        {
            return getEnemy().posicion();
        }

        private Vector3 distanciaAEnemigo()
        {
            return this.posicionEnemigo() - this.posicion();
        }

        private Vector3 obtenerDireccionAEnemigo()
        {   // Retorna el vector director de la recta que pasa por la posición de este barco y
            // por la posición del barco enemigo
            return Vector3.Normalize(this.posicionEnemigo() - this.posicion());
        }

        private void evaluarDistanciaDeEnemigo()
        {
            if (Vector3.Length(this.distanciaAEnemigo()) <= distancia_maxima)
            {
                estasMuyLejos = false;
            }
            else estasMuyLejos = true;
            if (Vector3.Length(this.distanciaAEnemigo()) < distancia_minima)
            {
                estasMuyCerca = true;
            }
            else estasMuyCerca = false;

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

            if (estasMuyCerca)
            {
                this.acelerar(1);
            }

        }

        public override void teDieron(Misil misil)
        {
            this.escaparme();
            base.teDieron(misil);
        }

        public override void render(float elapsedTime)
        {
            //en caso de que lo bombardeen se mueve asi se aleja
            if (tengoQueEscaparme && !estasMuyLejos)
            {
                acelerar(-1);
            }
            else
            {
                tengoQueEscaparme = false;
            }
            
            base.render(elapsedTime);
            
        }



        override protected void finalizar()
        {
            agua.ganaste();
        }

        private void escaparme()
        {
            tengoQueEscaparme = true;
        }
    }
}
