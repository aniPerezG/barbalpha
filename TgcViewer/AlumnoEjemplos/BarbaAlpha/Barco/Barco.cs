using AlumnoEjemplos.BarbaAlpha.Barco;
using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.BarbaAlpha.Barco
{
    public abstract class Barco : ITransformObject {

        public int puntaje; // contador de disparos exitosos
        public TgcBoundingSphere hitBoxSphere; // boundingBox para colisiones del barco
        public Barco enemy { set; get; } // barco enemigo al que se ataca o del que se defiende
        public TgcMesh malla; // malla del barco
        private List<Misil> misilesDisparados = new List<Misil>();
        private List<Misil> misilesAEliminar = new List<Misil>();
        private const float escala = 3; //escalado del barco
        private const float intervalo_entre_misiles = 2.5f; //tiempo entre cada disparo
        private readonly marAbierto agua; // terreno sobre el que se navega
        protected const float velocidad_max = 300; // velocidad máxima del barco
        protected float tiempo = 0;
        protected Vector3 rotacion_inicial_canion;


        public Vector3 Position
        {
            get { return malla.Position; }
            set { malla.Position = value; }
        }

        protected void disparar() {

            var tiempoDeVueloDeUltimoMisil = 0f;

            foreach (var misil in misilesDisparados)
            {
                if (tiempoDeVueloDeUltimoMisil == 0f || tiempoDeVueloDeUltimoMisil < misil.tiempoDeVuelo)
                    tiempoDeVueloDeUltimoMisil = misil.tiempoDeVuelo;
            }

            if (intervalo_entre_misiles <= tiempoDeVueloDeUltimoMisil || this.nuncaSeDisparo())
            {
                this.setInicioRotacionCanion();
                var nuevoMisil = new Misil(this.posicionReal(), this.rotacion_inicial_canion);
                misilesDisparados.Add(nuevoMisil);
            }
        }

        private bool nuncaSeDisparo()
        {
            return misilesDisparados.Count == 0;
        }

        private Vector3 posicionReal()  {
            return crearPosicionHeightMapDePosicionDeBarco(Position);
        }

        public virtual void moverOrientadoY(float movement)     {
            malla.moveOrientedY(movement);
        }

        public virtual void rotarSobreY(float angle)    {
            malla.rotateY(angle);
        }

        private Vector3 crearPosicionHeightMapDePosicionDeBarco(Vector3 posicionDeBarco)     {
            return new Vector3(posicionDeBarco.X, /*agua.getYValueFor(posicionDeBarco.X, posicionDeBarco.Z) * agua.ScaleY */ 0, posicionDeBarco.Z); // actualizar Y respecto de marea
        }

        public abstract void setInicioRotacionCanion();

        private void eliminarMisiles()
        {
            misilesAEliminar.ForEach((misil) => misilesAEliminar.Remove(misil));
        }

        private void verificarDisparos()
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
                    misil.render();
                }
            }
        }

        protected void leDisteA(Barco enemigo) 
        {
            if (++this.puntaje == 5)
            {
                this.close();
            }
            enemigo.teDieron(); // notifica al enemigo que fue atacado

        }

        protected abstract void teDieron()
        {
            throw new NotImplementedException();
        }

        public virtual void render()    {

            this.malla.render(); // renderiza la malla del barco
            this.verificarDisparos(); // evalúa el estado de los misiles disparados
            this.eliminarMisiles(); // elimina aquellos misiles que terminaron su trayectoria

        }
        
        private void close()
        {
            this.malla.dispose();
        }
    }
}

