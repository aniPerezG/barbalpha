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
        // private Direction dir = Direction.Forward;
        Vector3 destino;
        bool ocioso = true; // booleano que indica si el barco estaba sin nada que hacer
        bool avoiding = false;
        float timeAvoiding = 0;
        const double THRESHOLD_POS = 200;
        const double THRESHOLD_DIR = 0.5;

        
        public BarcoIA(Vector3 posicionInicial, marAbierto oceano, string directorioEscena) : base (initialPosition, terrain, directorioEscena);
    
        protected override void moverYRotar() {
            //IA del enemigo:
            //El chabon se mueve siempre para adelante, persiguiendo a la posición del tanque jugador
            //Para llegar a una determinada posición, primero debería rotar en la dirección adecuada
            //Y darle para adelante.

            //Origen, y destino
            Vector3 origen = this.Position;
            if (ocioso) { 
                this.destino = this.enemy.Position;
                ocioso = false;
            }

            //Direcciones origen y destino
            Vector3 direccionOrigen = this.forwardVector; 
            Vector3 direccionDestino = this.destino - this.Position;

            //Normalizar direcciones y sus ángulos
            direccionOrigen.Normalize(); 
            direccionDestino.Normalize();
            double anguloOrigen = Math.Atan2(direccionOrigen.Z, direccionOrigen.X);
            double anguloDestino = Math.Atan2(direccionOrigen.Z, direccionOrigen.X);
            if (anguloOrigen < 0) anguloOrigen += 2 * Math.PI;
            if (anguloDestino < 0) anguloOrigen += 2 * Math.PI;
            double distLeft = anguloOrigen > anguloDestino ? Math.PI * 2 - anguloOrigen + anguloDestino : anguloDestino - anguloOrigen;
            double distRight = anguloOrigen < anguloDestino ? Math.PI * 2 - anguloDestino + anguloOrigen : anguloOrigen - anguloDestino;
            if (!isInPosition(direccionOrigen, direccionOrigen, THRESHOLD_DIR)) {
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

            base.processMovement();
        }
        
        
        
        }
