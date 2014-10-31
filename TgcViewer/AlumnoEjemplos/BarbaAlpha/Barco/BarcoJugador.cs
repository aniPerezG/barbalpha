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

        public BarcoJugador(Vector3 posicionInicial, marAbierto oceano, string pathEscena)
            : base(posicionInicial, oceano, pathEscena) {}

        protected override void moverYVirar(float elapsedTime)
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;

            if (input.keyDown(Key.Up))
            {
                this.acelerar(-1);
                if (estoyYendoParaAtras)
                {
                    this.invertirSentido();
                    estoyYendoParaAtras = false;
                }
            }

            if (input.keyDown(Key.Down))
            {
                this.acelerar(1);
                if (!estoyYendoParaAtras)
                {
                    this.invertirSentido();
                    estoyYendoParaAtras = true;
                }
            }
            if (input.keyDown(Key.Space)) this.disparar();
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
            this.moveOrientedY(velocidad * elapsedTime);
        }

        public override void render(float elapsedTime)   {
            base.render(elapsedTime);
        }

        public void dispose()      {
            this.barco.dispose();
        }
    }
}
