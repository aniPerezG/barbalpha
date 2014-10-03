using AlumnoEjemplos.BarbaAlpha.Barco;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.BarbaAlpha.Barco
{
    public abstract class Barco : ITransformObject {

        public int puntaje; // contador de disparos exitosos
        public Barco enemy { set; get; } // barco enemigo al que se ataca o del que se defiende
        public Direccion direccion;
        public TgcMesh malla; // malla del barco
        public TgcScene escena; //escena donde existe el barco
        public TgcBoundingBox BoundingBox; // boundingBox para colisiones del barco
        private List<Misil> misilesDisparados = new List<Misil>(); // misiles ya en el aire
        private List<Misil> misilesAEliminar = new List<Misil>(); // misiles a remover de la escena
        private const float intervalo_entre_misiles = 2.5f; //tiempo entre cada disparo
        private const float velocidadAbsolutaRotacion = 40f;
        private readonly marAbierto agua; // terreno sobre el que se navega
        protected float tiempo = 0;
        protected Vector3 direccion_normal = new Vector3(0, 0, 1); //dirección en que se desplaza "derecho"
        protected Vector3 direccion_disparos;

        public Vector3 Rotation
        {
            get { return malla.Rotation; }
            set { throw new NotImplementedException(); }
        }

        public Vector3 Scale { get; set; }
        public void move(Vector3 v)  {
            throw new NotImplementedException();
        }
        public void move(float x, float y, float z)  {
            throw new NotImplementedException();
        }
        public void getPosition(Vector3 pos)    {
            throw new NotImplementedException();
        }
        public void rotateX(float angle)    {
            throw new NotImplementedException();
        }
        public void rotateY(float angle)
        {
            throw new NotImplementedException();
        }
        public void rotateZ(float angle)
        {
            throw new NotImplementedException();
        }
        public Matrix Transform { get; set; }
        public bool AutoTransformEnable { get; set; }
        public Vector3 Position     {
            get { return malla.Position; }
            set { malla.Position = value; }
        }

        public Barco(Vector3 posicionInicial, marAbierto oceano, string pathEscena) {
            var loader = new TgcSceneLoader();

            this.escena = loader.loadSceneFromFile(pathEscena); 
            this.direccion = new Direccion();
            this.malla = escena.Meshes[0];
            this.agua = oceano;
            this.BoundingBox = malla.BoundingBox;
            this.Position = posicionInicial;
        }

        public abstract void setEffect(Microsoft.DirectX.Direct3D.Effect efecto);
        public abstract void setTechnique(string tecnica);

        protected void disparar(float elapsedTime) {
            var nuevoMisil = new Misil(this.posicionReal(), vectorNormalA(this.posicionReal()));
            misilesDisparados.Add(nuevoMisil);
        }

        public Vector3 vectorNormalA(Vector3 vector)
        {
            float offset = 10;
            var vecAux = new Vector3(vector.X, vector.Y, vector.Z + offset);
            vecAux = Vector3.Cross(vector, vecAux);
            return vecAux;
        }

        private bool nuncaSeDisparo()   {
            return misilesDisparados.Count == 0;
        }

        private Vector3 posicionReal()  {
            return crearPosicionHeightMapDePosicionDeBarco(Position);
        }

        public virtual void moveOrientedY(float movement)     {
            this.malla.moveOrientedY(movement);
        }

        public virtual void rotarSobreY(float angle)    {
            this.malla.rotateY(angle);
        }

        private Vector3 crearPosicionHeightMapDePosicionDeBarco(Vector3 posicionDeBarco)     {
            //return new Vector3(posicionDeBarco.X, /*agua.getYValueFor(posicionDeBarco.X, posicionDeBarco.Z) * agua.ScaleY */ 0, posicionDeBarco.Z); // actualizar Y respecto de marea
            return this.Position;
        }

        public abstract void setInicioRotacionCanion();

        private void eliminarMisiles()
        {
            misilesAEliminar.ForEach((misil) => misilesAEliminar.Remove(misil));
        }

        private void verificarDisparos(float elapsedTime)
        {
            foreach (var misil in this.misilesDisparados)
            {
                if (misil.teHundisteEn(agua))
                {
                    misilesAEliminar.Add(misil);
                }
                else if (misil.chocasteConBarco(this.enemy))
                {
                    this.leDisteA(this.enemy);
                    misilesAEliminar.Add(misil);
                }
                else
                {
                    misil.render(elapsedTime);
                }
            }
        }

        protected void leDisteA(Barco enemigo)  {
            if (++this.puntaje == 5)    {
                this.close();
            }
          //  enemigo.teDieron(); // notifica al enemigo que fue atacado
        }

        public abstract void teDieron();

        private float calcularVelocidadDeRotacion(Direccion direccion){
            if (direccion.esDerecha())
            {
                return velocidadAbsolutaRotacion;
            }
            else
            {
                return velocidadAbsolutaRotacion * (-1);
            }
            
        }

        private void virar(Direccion direccion, float tiempo) {
            var velocidad = this.calcularVelocidadDeRotacion(direccion);
            var rotAngle = Geometry.DegreeToRadian(velocidad * tiempo);
            this.rotarSobreY(rotAngle);
        }

        protected virtual void moverYVirar(float elapsedTime)      {
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
        }

        public virtual void render(float elapsedTime)    {
            this.moverYVirar(elapsedTime);
            this.verificarDisparos(elapsedTime); // evalúa el estado de los misiles disparados
            this.eliminarMisiles(); // elimina aquellos misiles que terminaron su trayectoria
        }
        
        private void close()
        {
            this.malla.dispose();
        }
    }
}

