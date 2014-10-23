using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;


namespace AlumnoEjemplos.BarbaAlpha.Barco
{
    public class Misil
    {

        public float tiempoDeVuelo = 0;
        private float inicialY;
        private float anteriorY;
        private float velocidad_inicial_vertical = 500f; // Vo respecto de Y
        private const int altura_canion = 5; // altura inicial desde donde se realiza el tiro oblicuo
        private const float gravedad = -1500f; // sólo afecta el desplazamiento respecto de Y
        private const float velocidad_inicial_horizontal = -600f; // Sobre X no hay gravedad, es constante
        private TgcMesh mesh; // malla del misil


        public Misil(Barco barco)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene escena = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Objetos\\BarrilPolvora\\BarrilPolvora-TgcScene.xml");
            float angulo = FastMath.PI / 2 + barco.getRotacionAcumulada();
            
            this.mesh = escena.Meshes[0];
            this.mesh.Position = new Vector3(barco.posicion().X, barco.posicion().Y + altura_canion, barco.posicion().Z);
            inicialY = anteriorY = this.mesh.Position.Y;
            this.rotateY(angulo);
        }

        public TgcBoundingBox BoundingBox()
        {
            return this.mesh.BoundingBox;
        }

        public void rotateY(float angulo)
        {
            this.mesh.rotateY(angulo);
        }

        public void move(float x, float y, float z)
        {
            this.mesh.move(x, y, z);
        }

        public void moveOrientedY(float movement)
        {
            this.mesh.moveOrientedY(movement);
        }

        private void volarHorizontal(float movement)
        {
            this.moveOrientedY(movement);
        }

        private void volarVertical()
        {
            float y = inicialY + this.velocidad_inicial_vertical * this.tiempoDeVuelo + 0.5f * gravedad * this.tiempoDeVuelo * this.tiempoDeVuelo;
            this.move(0, y - anteriorY, 0);
        }

        public void render(float elapsedTime)
        {
            this.tiempoDeVuelo += elapsedTime;
            this.volarVertical();
            this.volarHorizontal(velocidad_inicial_horizontal * elapsedTime);
            this.mesh.render();
            anteriorY = mesh.Position.Y;
        }

        public bool teHundisteEn(marAbierto oceano)
        {
            return this.mesh.Position.Y < 0;
        }

        public bool chocasteConBarco(Barco unBarco)
        {
            return TgcCollisionUtils.testAABBAABB(unBarco.BoundingBox(), this.BoundingBox()); 
        }

        public void dispose()
        {
            this.mesh.dispose();
        }
    }
}

