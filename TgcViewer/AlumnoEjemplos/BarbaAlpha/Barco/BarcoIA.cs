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
        private const float distancia_minima = 4;
        private const float frecuencia_disparo = 5;
        protected float tiempo = 0;
        private Vector3 direccion_normal = new Vector3(0, 0, 1);
        private Vector3 posicion_inicial;
        private Vector3 posicion_enemigo;
        private bool estasMuyCerca = false;
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
        public override void moveOrientedY(float movement) { }

        public BarcoIA(Vector3 posicionInicial, marAbierto oceano, string pathEscena)
            : base(posicionInicial, oceano, pathEscena)
        {
            var loader = new TgcSceneLoader();
            var escenaCanion = loader.loadSceneFromFile(pathEscena);
            this.barco = escenaCanion.Meshes[1];
            this.posicion_inicial = this.barco.Position = posicionInicial;
        }

        public void evaluarDistanciaDeEnemigo()
        {
            var vecAux = new Vector3(posicion_enemigo.X - this.barco.Position.X,
                                     posicion_enemigo.Y - this.barco.Position.Y,
                                     posicion_enemigo.Z - this.barco.Position.Z);
            if (Vector3.Length(vecAux) > distancia_minima) estasMuyCerca = false;
            else estasMuyCerca = true;
        }

        public void moverYVirar(float elapsedTime)
        {
            // El barco se mueve manteniendo una mínima distancia 'd' respecto de la posición del barco enemigo
            Vector3 posicionEnemigo = new Vector3(0, 0, 0);
            tiempo += elapsedTime;
            enemigo.getPosition(posicion_enemigo);
            evaluarDistanciaDeEnemigo();
            if(estasMuyCerca)
            {
                // Debo moverme para alejarme de mi enemigo
            }
            if ((tiempo % frecuencia_disparo) == 0)
            {
                this.disparar(elapsedTime); // disparo cada 5 segundos;
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
