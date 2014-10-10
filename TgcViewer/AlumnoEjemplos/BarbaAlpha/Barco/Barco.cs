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
        public float velocidad = 0;
        public float friccion = 0;
        public Barco enemy { set; get; } // barco enemigo al que se ataca o del que se defiende
        public Direccion direccion;
        public TgcScene escena; //escena donde existe el barco
        public TgcMesh barco; // malla del barco
        private List<Misil> misilesDisparados = new List<Misil>(); // misiles ya en el aire
        private List<Misil> misilesAEliminar = new List<Misil>(); // misiles a remover de la escena
        private const float intervalo_entre_misiles = 2.5f; //tiempo entre cada disparo
        private const float velocidadAbsolutaRotacion = 40f;
        private readonly marAbierto agua; // terreno sobre el que se navega
        protected Vector3 direccion_disparos;


        public Vector3 Scale { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Position { get; set; }
        public Matrix Transform { get; set; }
        public bool AutoTransformEnable { get; set; }

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
        
        public Barco(Vector3 posicionInicial, marAbierto oceano, string pathEscena) {
            var loader = new TgcSceneLoader();
            this.direccion = new Direccion();
            this.escena = loader.loadSceneFromFile(pathEscena);
            this.agua = oceano;
            this.friccion = 10000f;
        }

        public void move(Vector3 v)
        {
            barco.move(v);
        }

        public TgcBoundingBox BoundingBox()
        {
            return barco.BoundingBox;
        }

        public void rotarSobreY(float angulo)
        {
            barco.rotateY(angulo);
        }

        public Vector3 posicion()
        {
            return barco.Position;
        }
        
        public void setEffect(Microsoft.DirectX.Direct3D.Effect efecto)
        {
            barco.Effect = efecto;
        }

        public void setTechnique(string tecnica)
        {
            barco.Technique = tecnica;
        }
       
        public void moveOrientedY(float movement)
        {
            barco.moveOrientedY(movement); 
        }

        protected void virar(Direccion direccion, float tiempo)
        {
            var velocidad = this.calcularVelocidadDeRotacion(direccion);
            var rotAngle = Geometry.DegreeToRadian(velocidad * tiempo);
            barco.rotateY(rotAngle);
        }
        
        protected void disparar(float elapsedTime) {
            var nuevoMisil = new Misil(this.posicion(), vectorNormalA(this.posicion()));
            misilesDisparados.Add(nuevoMisil);
        }

        public Vector3 vectorNormalA(Vector3 vector)
        {   // FIXME deberia depender de la rotacion del barco y no del desplazamiento
            float offset = 1;
            var vecAux = new Vector3(vector.X, vector.Y, vector.Z + offset);
            vecAux = Vector3.Cross(vector, vecAux);
            return vecAux;
        }

        private void eliminarMisiles()
        {
            misilesAEliminar.ForEach((misil) => misilesAEliminar.Remove(misil));
        }

        private void verificarDisparos(float elapsedTime)
        {
            foreach (var misil in this.misilesDisparados)
            {
                if (misil.teHundisteEn(this.agua))
                {
                    misilesAEliminar.Add(misil);
                }
                else if (misil.chocasteConBarco(this.enemy))
                {
                    misilesAEliminar.Add(misil);
                    leDisteA(this.enemy);
                }
                else
                {
                    misil.render(elapsedTime);
                }
            }
        }

        protected void leDisteA(Barco enemigo)  {
            if (++this.puntaje == 5) this.close();
        }

        protected float calcularVelocidadDeRotacion(Direccion direccion){
            if (direccion.esDerecha()) return velocidadAbsolutaRotacion;
            else return velocidadAbsolutaRotacion * (-1);
        }

        protected void acelerar(float aceleracion)
        {
            velocidad += aceleracion;
        }

        protected void aplicarFriccion(float elapsedTime)
        {
            if (velocidad != 0)
            {

                if (velocidad * friccion > 0)
                {
                    friccion *= (-1);
                }

                velocidad += friccion * elapsedTime * elapsedTime / 2;
            }
        }

        public virtual void render(float elapsedTime)    
        {
            this.verificarDisparos(elapsedTime); // evalúa el estado de los misiles disparados
            this.eliminarMisiles(); // elimina aquellos misiles que terminaron su trayectoria
        }
        public virtual void close()
        {

        }
    }
}

