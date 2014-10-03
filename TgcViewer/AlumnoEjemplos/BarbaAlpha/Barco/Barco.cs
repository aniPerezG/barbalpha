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
        public TgcScene escena; //escena donde existe el barco
        private List<Misil> misilesDisparados = new List<Misil>(); // misiles ya en el aire
        private List<Misil> misilesAEliminar = new List<Misil>(); // misiles a remover de la escena
        private const float intervalo_entre_misiles = 2.5f; //tiempo entre cada disparo
        private const float velocidadAbsolutaRotacion = 40f;
        private readonly marAbierto agua; // terreno sobre el que se navega
        protected float tiempo = 0;
        protected Vector3 direccion_normal = new Vector3(0, 0, 1); //dirección en que se desplaza "derecho"
        protected Vector3 direccion_disparos;

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
        public Vector3 Rotation {get; set;}
        public Vector3 Position{get; set;}
        public abstract void moveOrientedY(float movement);

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

        public Barco(Vector3 posicionInicial, marAbierto oceano, string pathEscena) {
            var loader = new TgcSceneLoader();

            this.escena = loader.loadSceneFromFile(pathEscena); 
            this.direccion = new Direccion();
            this.agua = oceano;
        }

        public abstract void setEffect(Microsoft.DirectX.Direct3D.Effect efecto);
        public abstract void setTechnique(string tecnica);
        public abstract Vector3 posicion();

        protected void disparar(float elapsedTime) {
            var nuevoMisil = new Misil(this.posicion(), vectorNormalA(this.posicion()));
            misilesDisparados.Add(nuevoMisil);
        }

        public Vector3 vectorNormalA(Vector3 vector)
        {
            float offset = 1;
            var vecAux = new Vector3(vector.X, vector.Y, vector.Z + offset);
            vecAux = Vector3.Cross(vector, vecAux);
            return vecAux;
        }

        private bool nuncaSeDisparo()   {
            return misilesDisparados.Count == 0;
        }

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

        protected float calcularVelocidadDeRotacion(Direccion direccion){
            if (direccion.esDerecha())
            {
                return velocidadAbsolutaRotacion;
            }
            else
            {
                return velocidadAbsolutaRotacion * (-1);
            }
        }

        protected void virar(Direccion direccion, float tiempo) {
            var velocidad = this.calcularVelocidadDeRotacion(direccion);
            var rotAngle = Geometry.DegreeToRadian(velocidad * tiempo);
        }

        public virtual void render(float elapsedTime)    {
            this.verificarDisparos(elapsedTime); // evalúa el estado de los misiles disparados
            this.eliminarMisiles(); // elimina aquellos misiles que terminaron su trayectoria
        }
        public virtual void close()
        {

        }
    }
}

