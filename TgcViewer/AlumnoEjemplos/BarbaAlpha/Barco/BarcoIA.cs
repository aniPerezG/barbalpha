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
        protected float tiempo = 0;
        private Vector3 direccion_normal = new Vector3(0, 0, -1);
        private Vector3 posicion_inicial;
        private Vector3 posicion_enemigo = new Vector3(0, 0, 0);
        private bool estasMuyCerca = true;
        private Barco enemigo; // el enemigo es el barco del jugador
        private TgcMesh barco; // la malla de este barco

        public override Vector3 posicion()
        {
            throw new NotImplementedException();
        }
        protected override void virar(Direccion direccion, float tiempo)
        {
            throw new NotImplementedException();
        }
        public override void setTechnique(string tecnica) { }
        public override void setEffect(Microsoft.DirectX.Direct3D.Effect efecto) { }
        public override void moveOrientedY(float movement) 
        {
            this.barco.moveOrientedY(movement);
        }

        public BarcoIA(Vector3 posicionInicial, marAbierto oceano, string pathEscena, BarcoJugador enemigo)
            : base(posicionInicial, oceano, pathEscena)
        {
            var loader = new TgcSceneLoader();
            var escenaCanion = loader.loadSceneFromFile(pathEscena);
            this.barco = escenaCanion.Meshes[0];
            this.posicion_inicial = this.barco.Position = posicionInicial;
            this.enemigo = enemigo;
        }

        public void rotate(Vector3 rotacion)
        {
            this.barco.rotateX(rotacion.X);
            this.barco.rotateY(rotacion.Y);
            this.barco.rotateZ(rotacion.Z);
        }

        public void virarHaciaDestino() 
        {   // Debería virar el barco hasta alinearse en la direccion propuesta
            direccion_normal.Normalize();
            calcularDireccion().Normalize();

            double angOrg = Math.Atan2(direccion_normal.Z, direccion_normal.X) + 2 * Math.PI;
            double angDst = Math.Atan2(calcularDireccion().Z, calcularDireccion().X) + 2 * Math.PI;
            double distLeft = angOrg > angDst ? 2 * Math.PI - angOrg + angDst : angDst - angOrg;
            double distRight = angOrg < angDst ? 2 * Math.PI - angDst + angOrg : angOrg - angDst;

            if (distLeft < distRight) this.rotate(new Vector3(1, 0, 0));
            else this.rotate(new Vector3(-1, 0, 0));
        }

        public void move(Vector3 movimiento)
        {   // Muevo el barco segun indica el vector movimiento
            this.barco.move(movimiento);
        }

        private Vector3 calcularDireccion()
        {   // Retorna el vector director de la recta que pasa por la posición de este barco y
            // por la posición del barco enemigo
            return new Vector3(posicion_enemigo.X - this.barco.Position.X,
                               posicion_enemigo.Y - this.barco.Position.Y,
                               posicion_enemigo.Z - this.barco.Position.Z);
        }

        private Vector3 obtenerDestino()
        {   // Retorna un punto en el espacio tomando un múltiplo del vector director 
            // sumando el punto independiente de la recta
            return new Vector3(2 * calcularDireccion().X + this.barco.Position.X,
                               2 * calcularDireccion().Y + this.barco.Position.Y,
                               2 * calcularDireccion().Z + this.barco.Position.Z);
        }

        public Vector3 obtenerMovimiento()
        {   // Retorna el vector diferencia entre el destino del barco y la posición actual
            return new Vector3(obtenerDestino().X - this.barco.Position.X,
                               obtenerDestino().Y - this.barco.Position.Y,
                               obtenerDestino().Z - this.barco.Position.Z);
        }

        public void evaluarDistanciaDeEnemigo()
        {
            if (Vector3.Length(calcularDireccion()) > distancia_minima) estasMuyCerca = false;
            else estasMuyCerca = true;
        }

        public void apuntarAEnemigo()
        {   // Dado que estoy paralelo a la dirección del enemigo, roto 90° para apuntarle
            this.barco.rotateY(FastMath.PI_HALF);
        }

        public void moverYVirar(float elapsedTime)
        {   // El barco se mueve manteniendo una mínima distancia 'd' respecto de la posición del barco enemigo
            tiempo += elapsedTime;
            
            posicion_enemigo = enemigo.posicion();
            this.evaluarDistanciaDeEnemigo();
            //base.aplicarFriccion(elapsedTime);
            
            if (estasMuyCerca)
            {   // Debo virar en dirección al destino, moverme hacia esa posición y virar en posicion de disparo*/
                /*this.virarHaciaDestino();
                this.move(this.obtenerMovimiento());
                this.apuntarAEnemigo();*/ 
                this.acelerar(-1);
                this.moveOrientedY(velocidad * elapsedTime); //velocidad * elapsedTime);
            }
            if ((tiempo % frecuencia_disparo) == 0)
            {   
                this.disparar(elapsedTime); // disparo cada 'frecuencia_disparo' segundos;
            }
        }

        public override void render(float elapsedTime)
        {
            base.render(elapsedTime);
            this.moverYVirar(elapsedTime);
            this.barco.render();
        }
    }
}
