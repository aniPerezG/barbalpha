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

    public float tiempoDeVuelo { set; get; } // cuánto hace que esta volando
    private float velocidad_inicial_vertical = 3f; // Vo respecto de Y
    private const int altura_canion = 5; // altura inicial desde donde se realiza el tiro oblicuo
    private const float gravedad = -0.2f; // sólo afecta el desplazamiento respecto de Y
    private const float velocidad_inicial_horizontal = -1000f; // Sobre X no hay gravedad, es constante
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
        TgcSceneLoader loader = new TgcSceneLoader();
        var escena = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Objetos\\BarrilPolvora\\BarrilPolvora-TgcScene.xml");
        this.mesh = escena.Meshes[0];
        this.mesh.Position = new Vector3(posicionBarco.X, posicionBarco.Y + altura_canion, posicionBarco.Z);
        this.mesh.Rotation = new Vector3(rotacionRespectoDeBarco.X + 0.5f, rotacionRespectoDeBarco.Y, rotacionRespectoDeBarco.Z + 0.5f);
    }

    public void move(Vector3 v) {
        throw new NotImplementedException();
    }
    public void move(float x, float y, float z) {
        throw new NotImplementedException();
    }
    public void volarHorizontal(float movement) {
        this.mesh.moveOrientedY(movement);
    }
    public void volarVertical() {
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
        throw new NotImplementedException();
    }

    public void render(float elapsedTime) {
        this.tiempoDeVuelo += elapsedTime;
        this.volarHorizontal(velocidad_inicial_horizontal * elapsedTime);
        this.volarVertical();
        this.mesh.render();
    }

    public bool teHundisteEn(marAbierto oceano) {
        return (int) oceano.CalcularAlturaRespectoDe(this.Position.X, this.Position.Z) >= (int) this.Position.Y;
    } 
      
    public bool chocasteConBarco(Barco unBarco) {
        // return TgcCollisionUtils.testAABBAABB(unBarco.BoundingBox, this.mesh.BoundingBox); 
        return false;
    }

    public void dispose() {
        this.mesh.dispose();
    }
}
    }

