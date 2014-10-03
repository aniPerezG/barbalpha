using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;

namespace AlumnoEjemplos.BarbaAlpha.Barco
{
    class BarcoJugador : Barco  {

        public TgcMesh canion;
        private float rotacion_canion = 0.5f; // en cuánto comienza rotado respecto de la dirección del barco
        private float angulo_canion_y = 0.5f; // ángulo respecto del plano XZ (apertura del cañón)

        public BarcoJugador(Vector3 posicion_inicial, marAbierto oceano, string pathEscena) : base (posicion_inicial, oceano, pathEscena) {
            var loader = new TgcSceneLoader();
            var escenaCanion = loader.loadSceneFromFile(pathEscena); // escena del cañon

            this.canion = escenaCanion.Meshes[0];         
            this.canion.AutoUpdateBoundingBox = this.canion.AutoTransformEnable = false;
        }

        public override void setInicioRotacionCanion()   {
            this.rotacion_inicial_canion = this.canion.Rotation;
        }
        
        public override void moveOrientedY(float desplazamientoEnY)  {
            base.moveOrientedY(desplazamientoEnY);
            canion.moveOrientedY(desplazamientoEnY);
        }

        public override void rotarSobreY(float angulo)   {
            base.rotarSobreY(angulo);
            canion.rotateY(angulo);
            GuiController.Instance.ThirdPersonCamera.rotateY(angulo); // rotar cámara dependiendo de la orientación del barco
        }

        public override void teDieron() {
        }

        public override void render(float elapsedTime)   {
            base.render(elapsedTime);
            this.canion.render();
        }

        public void dispose()      {
            this.canion.dispose();
            //base.dispose();
        }
    }
}
