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
    public abstract class Barco {

        public float tiempo = 0;
        public float velocidad = 0;
        public float velocidad_maxima = 100;
        public float rotacion_acumulada = 0;
        public Barco enemy { set; get; } // barco enemigo al que se ataca o del que se defiende
        public Direccion direccion = new Direccion();
        public TgcMesh barco; // malla del barco
        private bool canionListo;
        private int puntaje; // contador de disparos exitosos
        private float tiempo_entre_disparos = 0;
        private float frecuencia_disparo = 2;
        private float friccion = 10000f;
        private float velocidadAbsolutaRotacion = 40f;
        private marAbierto agua; // terreno sobre el que se navega
        private List<Misil> misilesAEliminar = new List<Misil>(); // misiles a remover de la escena
        private List<Misil> misilesDisparados = new List<Misil>(); // misiles ya en el aire

        
        public Barco(Vector3 posicionInicial, marAbierto oceano, string pathEscena) {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene escenaCanion = loader.loadSceneFromFile(pathEscena); // escena del cañon
            this.barco = escenaCanion.Meshes[0];
            this.setPosicion(posicionInicial);
            this.setAgua(oceano);
        }

        protected abstract void moverYVirar(float elapsedTime);

        public float getRotacionAcumulada()
        {
            return rotacion_acumulada;
        }

        public Barco getEnemy()
        {
            return enemy;
        }
        
        public TgcBoundingBox BoundingBox()
        {
            return barco.BoundingBox;
        }

        public Vector3 posicion()
        {
            return barco.Position;
        }

        public void setEnemy(Barco enemigo)
        {
            this.enemy = enemigo;
        }

        public void setVelocidadMaxima(float velocidadMaxima)
        {
            this.velocidad_maxima = velocidadMaxima;
        }
        
        public void move(Vector3 v)
        {
            this.barco.move(v);
        }
        
        public void rotarSobreY(float angulo)
        {
            this.rotacion_acumulada += angulo;
            this.barco.rotateY(angulo);
        }

        public void setAgua(marAbierto oceano)
        {
            agua = oceano;
        }

        public void setFrecuenciaDeDisparos(float frecuencia)
        {
            this.frecuencia_disparo = frecuencia;
        }

        public void setEffect(Microsoft.DirectX.Direct3D.Effect efecto)
        {
            barco.Effect = efecto;
        }

        public void setPosicion(Vector3 posicion)
        {
            this.barco.Position = posicion;
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
            float velocidad = this.calcularVelocidadDeRotacion(direccion);
            float rotAngle = Geometry.DegreeToRadian(velocidad * tiempo);
            this.rotarSobreY(rotAngle);
        }

        private void prepararCanion(float elapsedTime)
        {
            tiempo_entre_disparos += elapsedTime;
            if (tiempo_entre_disparos > 1 && FastMath.Floor(tiempo_entre_disparos) % frecuencia_disparo == 0)
            {
                tiempo_entre_disparos = 0;
                canionListo = true;
            }
            
        }

        private void verificarCanion(float elapsedTime)
        {
            if (!canionListo)
            {
                this.prepararCanion(elapsedTime);
            }
        }
        
        protected void disparar() 
        {
            if (canionListo)
            {
                Misil nuevoMisil = new Misil(this);
                misilesDisparados.Add(nuevoMisil);
                canionListo = false;
            }
        }

        private void eliminarMisiles()
        {
            misilesAEliminar.ForEach((misil) => misilesAEliminar.Remove(misil));
        }

        private void verificarDisparos(float elapsedTime)
        {
            foreach (Misil misil in misilesDisparados)
            {
                if (misil.teHundisteEn(this.agua))
                {
                    misilesAEliminar.Add(misil);
                    continue;
                }
                else if (misil.chocasteConBarco(this.getEnemy()))
                {
                    misilesAEliminar.Add(misil);
                    leDisteA(this.enemy);
                    continue;
                }
                misil.render(elapsedTime);
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
                if (velocidad * friccion > 0) friccion *= (-1);
                velocidad += friccion * elapsedTime * elapsedTime / 2;
            }
        }

        protected void controlarVelocidadMaxima() 
        {
            int signo = Math.Sign(velocidad);
            if (Math.Abs(velocidad) > velocidad_maxima) velocidad = velocidad_maxima * signo;
        }
       
        public virtual void render(float elapsedTime)    
        {
            tiempo += elapsedTime;
            this.barco.render();
            this.aplicarFriccion(elapsedTime);
            this.moverYVirar(elapsedTime);
            this.controlarVelocidadMaxima();
            this.verificarCanion(elapsedTime);
            this.verificarDisparos(elapsedTime); // evalúa el estado de los misiles disparados
            this.eliminarMisiles(); // elimina aquellos misiles que terminaron su trayectoria
        }

        
        public virtual void close()
        {

        }
    }
}

