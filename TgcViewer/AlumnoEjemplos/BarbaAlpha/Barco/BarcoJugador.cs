using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.BarbaAlpha.Barco
{
    class BarcoJugador : Barco  {

        public TgcMesh barco;
        public TgcBoundingBox BoundingBox;
        public float time = 0;

        public BarcoJugador(Vector3 posicion_inicial, marAbierto oceano, string pathEscena) : base (posicion_inicial, oceano, pathEscena) {
            var loader = new TgcSceneLoader();
            var escenaCanion = loader.loadSceneFromFile(pathEscena); // escena del cañon
            this.barco = escenaCanion.Meshes[0];
            this.BoundingBox = this.barco.BoundingBox;
        }

        public override void setEffect(Microsoft.DirectX.Direct3D.Effect efecto)
        {
            barco.Effect = efecto;
        }

        public override void setTechnique(string tecnica)
        {
            barco.Technique = tecnica;
        }

        public override Vector3 posicion()
        {
            return this.barco.Position;
        }

        public override void moveOrientedY(float desplazamientoEnY)  {
            barco.moveOrientedY(desplazamientoEnY);
        }

        public void rotarSobreY(float angulo)   {
            barco.rotateY(angulo);
        }

        protected override void virar(Direccion direccion, float tiempo)
        {
            var velocidad = base.calcularVelocidadDeRotacion(direccion);
            var rotAngle = Geometry.DegreeToRadian(velocidad * tiempo);
            barco.rotateY(rotAngle);
        }

        protected virtual void moverYVirar(float elapsedTime)
        {
            this.time += elapsedTime;
            TgcD3dInput input = GuiController.Instance.D3dInput;

            if (input.keyDown(Key.Up))
                this.moveOrientedY(-1);
            if (input.keyDown(Key.Down))
                this.moveOrientedY(1);
            if (input.keyDown(Key.Right))
            {
                direccion.haciaLaDerecha();
                this.virar(direccion, elapsedTime);
            }
            if (input.keyDown(Key.Left))
            {
                direccion.haciaLaIzquierda();
                this.virar(direccion, elapsedTime);
            }
            if (input.keyDown(Key.Space))
                this.disparar(elapsedTime);

            float posY = 3 * (float)Math.Cos(time + 100);
            this.barco.move(new Vector3(0, ((posY) - barco.Position.Y), 0));
        }

        public override void render(float elapsedTime)   {
            base.render(elapsedTime);
            this.moverYVirar(elapsedTime);
            this.barco.render();
        }

        public void dispose()      {
            this.barco.dispose();
        }
    }
}
