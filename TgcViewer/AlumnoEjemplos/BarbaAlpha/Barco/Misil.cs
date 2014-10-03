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
    class Misil{

    public float tiempoDeVuelo { set; get; }
    private float velocidad_inicial_vertical = 10f;
    private const int altura_canion = 150;
    private const float gravedad = -0.2f;
    private const float velocidad_inicial_horizontal = -500f; // Sobre X no hay gravedad
    private TgcMesh mesh; // malla del misil
  
    public TgcBoundingBox BoundingBox
    {
        get { return this.mesh.BoundingBox; }
    }
    
    public Vector3 Position {
        get { return this.mesh.Position; }
        set { throw new NotImplementedException(); }
    }

    public Vector3 Rotation { get; set; }
    public Vector3 Scale { get; set; }
    
    public Misil(Vector3 posicionBarco, Vector3 rotacionRespectoDeBarco) {
        
        var loader = new TgcSceneLoader();
        var scene = loader.loadSceneFromFile("ESCENA"); // debiera ser la escena del Barco

        this.mesh = scene.Meshes[1];
        this.mesh.Position = new Vector3(posicionBarco.X, posicionBarco.Y + altura_canion, posicionBarco.Z);
        this.mesh.Rotation = new Vector3(rotacionRespectoDeBarco.X + 0.5f, rotacionRespectoDeBarco.Y, rotacionRespectoDeBarco.Z + 0.5f);
    }

    public void move(Vector3 v) {
        throw new NotImplementedException();
    }
    public void move(float x, float y, float z) {
        throw new NotImplementedException();
    }
    public void moveOrientedY(float movement) {
        this.mesh.moveOrientedY(movement);
    }
    public void moveVertically() {
        this.velocidad_inicial_vertical += gravedad * this.tiempoDeVuelo; // Vyi = g * (t-t0)
        this.mesh.move(0, this.velocidad_inicial_vertical * this.tiempoDeVuelo + 0.5f * gravedad * this.tiempoDeVuelo * this.tiempoDeVuelo, 0); // y = Vyi * (t-t0) + g/2 * (t-t0)^2 ; g < 0
    }
    public void getPosition(Vector3 pos) {
        throw new NotImplementedException();
    }
    public void rotateX(float angulo) {
        throw new NotImplementedException();
    }
    public void rotateY(float angulo){
        throw new NotImplementedException();
        
    }
    public void rotateZ(float angulo) {
        this.mesh.rotateZ(angulo);
    }

    public void render() {
        //this.tiempoDeVuelo += Shared.ElapsedTime;
        //this.moveOrientedY(velocidad_inicial_horizontal * Shared.ElapsedTime);
        this.moveVertically();
        this.rotateZ(0.1f);
        this.mesh.render();
    }

    public bool teHundisteEn(marAbierto oceano) {
        return false ; //oceano.getYValueFor(this.Position.X, this.Position.Z) >= (int) this.Position.Y;
    } 
        /*
    public bool chocasteConBarco(Barco unBarco) {
        return TgcCollisionUtils.testSphereAABB(unBarco.hitBoxSphere, this.mesh.BoundingBox); 
    }*/

    public void dispose() {
        this.mesh.dispose();
    }
}
    }

