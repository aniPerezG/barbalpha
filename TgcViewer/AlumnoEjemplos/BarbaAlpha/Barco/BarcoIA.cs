using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;

/*
namespace AlumnoEjemplos.BarbaAlpha.Barco
{
    class BarcoIA : Barco
    {
        private Vector3 direccion_normal;
        private Vector3 posicion_inicial;
        private Barco enemigo; // el enemigo es el barco del jugador

        public BarcoIA(Vector3 posicionInicial, marAbierto oceano, string pathEscena)
            : base(posicionInicial, oceano, pathEscena) { }

        public void moverYVirar(float elapsedTime)   {
            // El barco se mueve manteniendo una mínima distancia 'd' respecto de la posición del barco enemigo
            if (this.totalSpeed == 0) isMoving = false;
            isRotating = false;

            //Origen, y destino
            Vector3 origin = this.Position;
            if (iddle) { //(hasta no llegar ahí no cambia de destino)
                this.destination = this.enemy.Position;
                iddle = false;
            }

            //Direcciones origen y destino
            Vector3 direcOrg = this.forwardVector; 
            Vector3 direcDst = this.destination - this.Position;

            //Normalizar direcciones y sus ángulos
            direcOrg.Normalize(); 
            direcDst.Normalize();
            double angOrg = Math.Atan2(direcOrg.Z, direcOrg.X);
            double angDst = Math.Atan2(direcDst.Z, direcDst.X);
            if (angOrg < 0) angOrg += 2 * Math.PI;
            if (angDst < 0) angOrg += 2 * Math.PI;
            double distLeft = angOrg > angDst ? Math.PI * 2 - angOrg + angDst : angDst - angOrg;
            double distRight = angOrg < angDst ? Math.PI * 2 - angDst + angOrg : angOrg - angDst;
            if (!isInPosition(direcOrg, direcDst, THRESHOLD_DIR)) {
                if (distLeft < distRight) {
                    this.rotate(Direction.Left);
                } else {
                    this.rotate(Direction.Right);
                }   
            }
            this.acel(1);
            this.move(dir);
            this.doFriction();

            //Si este random se cumple, dispara
            if (new Utils.Randomizer(1, 500).getNext() > 495) {
                this.shoot();
                iddle = true; //para que tenga chances de esquivar el bache...
            }

            if (colliding) {
                if (!avoiding) {
                    recalc(); //REECALCULANDO...
                    timeAvoiding = 0;
                } else {
                    timeAvoiding += Shared.ElapsedTime;
                    if (timeAvoiding > 3) {
                        recalc();
                        timeAvoiding = 0;
                    }
                }
            }

            //Si llego al destino, lo marca como inactivo para buscar uno nuevo...
            if (isInPosition(Position, destination, THRESHOLD_POS)) {
                iddle = true;
                if (avoiding) avoiding = false;

    }
}
*/