using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using System.Drawing;


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
        private Barco barco;
        private TgcSphere mesh; // malla del misil

        public Misil(Barco barco)
        {
            //habria que inyectarlo por parametro
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            TgcSceneLoader loader = new TgcSceneLoader();
            //TgcScene escena = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Objetos\\BarrilPolvora\\BarrilPolvora-TgcScene.xml");
            //this.mesh = escena.Meshes[0];

            mesh = new TgcSphere();
            mesh.BasePoly = TgcSphere.eBasePoly.CUBE;
            mesh.setColor(Color.Black);
            mesh.Radius = 10;
            mesh.Position = new Vector3(0, 2, 0);
            mesh.Inflate = true;
            mesh.LevelOfDetail = 4;
            mesh.updateValues();

            string texturePath = (string)GuiController.Instance.AlumnoEjemplosMediaDir + "\\Textures\\metalOscuro.jpg";
            mesh.setTexture(TgcTexture.createTexture(d3dDevice, texturePath));

            this.barco = barco;
        }

        public TgcBoundingSphere BoundingSphere()
        {
            return this.mesh.BoundingSphere;
        }

        public void setearMisil()
        {
            float angulo = FastMath.PI / 2 + barco.getRotacionAcumulada();
            this.tiempoDeVuelo = 0;
            this.mesh.Position = new Vector3(barco.posicion().X, barco.posicion().Y + altura_canion, barco.posicion().Z);
            this.inicialY = this.anteriorY = this.mesh.Position.Y;
            if (this.mesh.Rotation.Y != angulo )
            {
                this.rotateY(angulo - this.mesh.Rotation.Y);
            }
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
            return TgcCollisionUtils.testSphereAABB(this.BoundingSphere(), unBarco.BoundingBox()); 
        }

        public void dispose()
        {
            this.mesh.dispose();
        }
    }
}

