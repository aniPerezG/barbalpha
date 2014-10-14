using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;


namespace AlumnoEjemplos.BarbaAlpha.Barco
{
    class BarcoIA : Barco
    {
        private const float distancia_minima = 500;
        private const float frecuencia_disparo = 3;
        private Vector3 direccion_normal = new Vector3(0, 0, -1);
        private Vector3 posicion_anterior;
        private Vector3 posicion_enemigo;
        private bool estasMuyCerca = true;
        private Barco enemigo; // el enemigo es el barco del jugador

        public BarcoIA(Vector3 posicionInicial, marAbierto oceano, string pathEscena, BarcoJugador enemigo)
            : base(posicionInicial, oceano, pathEscena)
        {
            var loader = new TgcSceneLoader();
            var escenaCanion = loader.loadSceneFromFile(pathEscena);
            this.barco = escenaCanion.Meshes[0];
            this.barco.Position = posicionInicial;
            this.enemigo = enemigo;
            this.direccion.haciaLaDerecha();
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
            return new Vector3(posicion_enemigo.X - this.barco.Position.X,
                               posicion_enemigo.Y - this.barco.Position.Y,
                               posicion_enemigo.Z - this.barco.Position.Z);
        }

        private Vector3 obtenerDestino()
        {   // Retorna un punto en el espacio tomando un múltiplo del vector director 
            // sumando el punto independiente de la recta
            return new Vector3(2 * obtenerDireccionAEnemigo().X + this.barco.Position.X,
                               2 * obtenerDireccionAEnemigo().Y + this.barco.Position.Y,
                               2 * obtenerDireccionAEnemigo().Z + this.barco.Position.Z);
        }

        private Vector3 obtenerMovimiento()
        {   // Retorna el vector diferencia entre el destino del barco y la posición actual
            return new Vector3(obtenerDestino().X - this.barco.Position.X,
                               obtenerDestino().Y - this.barco.Position.Y,
                               obtenerDestino().Z - this.barco.Position.Z);
        }

        private Vector3 obtenerDireccionNormal()
        {
            var normal = new Vector3(this.posicion().X - this.posicion_anterior.X,
                                     this.posicion().Y - this.posicion_anterior.Y,
                                     this.posicion().Z - this.posicion_anterior.Z); 
            normal.Normalize();
            return normal;
        }

        private void evaluarDistanciaDeEnemigo()
        {
            if (Vector3.Length(obtenerDireccionAEnemigo()) > distancia_minima) estasMuyCerca = false;
            else estasMuyCerca = true;
        }

        private void apuntarAEnemigo()
        {   // Dado que estoy paralelo a la dirección del enemigo, roto 90° para apuntarle
            this.barco.rotateY(FastMath.PI_HALF);
        }

        private void atacarA(Barco enemigo, float elapsedTime)
        {
            if ((Math.Floor(tiempo) % frecuencia_disparo) == 0)
            {
                this.disparar(elapsedTime); // disparo cada 'frecuencia_disparo' segundos;
            }
        }

        private void navegar(float elapsedTime)
        {   // El barco se desplaza formando una circunferencia cuyo centro es la posicion del enemigo.
            // Siempre está apuntando al enemigo, salvo que esté muy cerca.

            this.acelerar(1);
            this.virar(direccion, elapsedTime/5);
            this.atacarA(enemigo, elapsedTime);
        }

        protected override void moverYVirar(float elapsedTime)
        {   // El barco se mueve manteniendo una mínima distancia 'd' respecto de la posición del barco enemigo
            
            tiempo += elapsedTime;
            this.posicion_anterior = this.barco.Position;
            this.posicion_enemigo = enemigo.posicion();
            this.evaluarDistanciaDeEnemigo();

            if (estasMuyCerca)
            {   // Debo virar en dirección al destino, moverme hacia esa posición y virar en posicion de disparo*/
                this.acelerar(-1);
                this.moveOrientedY(velocidad * elapsedTime);
            }
            else this.navegar(elapsedTime);
        }

        public override void render(float elapsedTime)
        {
            base.render(elapsedTime);
        }
    }
}
