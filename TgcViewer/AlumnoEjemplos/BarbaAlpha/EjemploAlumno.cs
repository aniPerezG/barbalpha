using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Shaders;
using AlumnoEjemplos.BarbaAlpha;

namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class EjemploAlumno : TgcExample
    {

        TgcMesh mesh;
        TgcMesh canoa;
        Effect effect;
        float time;
       
        
        

        /// <summary>
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el árbol de la derecha de la pantalla.
        /// </summary>
        public override string getCategory()
        {
            return "Pirate Ship";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "Principal";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "MiIdea - Descripcion de la idea";
        }

        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            //GuiController.Instance: acceso principal a todas las herramientas del Framework

            //Device de DirectX para crear primitivas
            Device d3dDevice = GuiController.Instance.D3dDevice;

            
            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            string alumnoShaderFolder = alumnoMediaFolder + "\\shaders";
            string exampleShaderFolder = GuiController.Instance.ExamplesDir + "\\Shaders\\WorkshopShaders\\Shaders\\";
            string exampleMediaFolder = GuiController.Instance.ExamplesMediaDir;

            //Cargar modelo estatico
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(alumnoMediaFolder + "aguita-TgcScene.xml");
            mesh = scene.Meshes[0];
            mesh.Scale = new Vector3(5.0f, 5.0f, 5.0f);
            mesh.Position = new Vector3(-250f, 0f, 250f);

           
            canoa = loader.loadSceneFromFile(exampleMediaFolder + "MeshCreator\\Meshes\\Vehiculos\\Canoa\\Canoa-TgcScene.xml").Meshes[0];

            canoa.Position = new Vector3(0f, 60f, 0f);


            //Cargar Shader personalizado
            effect = TgcShaders.loadEffect(alumnoShaderFolder + "\\shaderLoco.fx");

            // le asigno el efecto a la malla 
            mesh.Effect = effect;

            //Configurar Technique dentro del shader
            mesh.Technique = "RenderScene";
            
            
            //Centrar camara rotacional respecto a este mesh
            GuiController.Instance.RotCamera.Enable = true;
            GuiController.Instance.RotCamera.targetObject(mesh.BoundingBox);

            time = 0;

            //Alejar camara rotacional segun tamaño del BoundingBox del objeto
           // GuiController.Instance.RotCamera.targetObject(mesh.BoundingBox);



            ///////////////USER VARS//////////////////

            //Crear una UserVar
            GuiController.Instance.UserVars.addVar("variablePrueba");

            //Cargar valor en UserVar
            GuiController.Instance.UserVars.setValue("variablePrueba", 5451);



            ///////////////MODIFIERS//////////////////

            //Crear un modifier para un valor FLOAT
            GuiController.Instance.Modifiers.addFloat("valorFloat", -50f, 200f, 0f);

            //Crear un modifier para un ComboBox con opciones
            string[] opciones = new string[]{"opcion1", "opcion2", "opcion3"};
            GuiController.Instance.Modifiers.addInterval("valorIntervalo", opciones, 0);

            //Crear un modifier para modificar un vértice
            GuiController.Instance.Modifiers.addVertex3f("valorVertice", new Vector3(-100, -100, -100), new Vector3(50, 50, 50), new Vector3(0, 0, 0));



            ///////////////CONFIGURAR CAMARA ROTACIONAL//////////////////
            //Es la camara que viene por default, asi que no hace falta hacerlo siempre
            //GuiController.Instance.RotCamera.Enable = true;
            //Configurar centro al que se mira y distancia desde la que se mira
            //GuiController.Instance.RotCamera.setCamera(new Vector3(0, 0, 0), 100);


            /*
            ///////////////CONFIGURAR CAMARA PRIMERA PERSONA//////////////////
            //Camara en primera persona, tipo videojuego FPS
            //Solo puede haber una camara habilitada a la vez. Al habilitar la camara FPS se deshabilita la camara rotacional
            //Por default la camara FPS viene desactivada
            GuiController.Instance.FpsCamera.Enable = true;
            //Configurar posicion y hacia donde se mira
            GuiController.Instance.FpsCamera.setCamera(new Vector3(0, 0, -20), new Vector3(0, 0, 0));
            */



            ///////////////LISTAS EN C#//////////////////
            //crear
            List<string> lista = new List<string>();

            //agregar elementos
            lista.Add("elemento1");
            lista.Add("elemento2");

            //obtener elementos
            string elemento1 = lista[0];

            //bucle foreach
            foreach (string elemento in lista)
            {
                //Loggear por consola del Framework
                GuiController.Instance.Logger.log(elemento);
            }

            //bucle for
            for (int i = 0; i < lista.Count; i++)
            {
                string element = lista[i];
            }


        }


        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        /// 

       
        public override void render(float elapsedTime)
        {
            //Device de DirectX para renderizar
            Device device = GuiController.Instance.D3dDevice;
            time += elapsedTime;

            GuiController.Instance.RotCamera.targetObject(mesh.BoundingBox);
            GuiController.Instance.CurrentCamera.updateCamera();
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            // Cargar variables de shader, por ejemplo el tiempo transcurrido.
            effect.SetValue("time", time);


            //Guardar posicion original antes de cambiarla
            Vector3 originalPos = canoa.Position;


            
            bool collisionFound = false;
           // foreach (TgcMesh mesh in scene.Meshes)
            //{


                //Los dos BoundingBox que vamos a testear
                TgcBoundingBox canoaBoundingBox = canoa.BoundingBox;
                TgcBoundingBox sceneMeshBoundingBox = mesh.BoundingBox;

                

                //Ejecutar algoritmo de detección de colisiones
                TgcCollisionUtils.BoxBoxResult collisionResult = TgcCollisionUtils.classifyBoxBox(canoaBoundingBox, sceneMeshBoundingBox);

                //Hubo colisión con un objeto. Guardar resultado y abortar loop.
                if (collisionResult != TgcCollisionUtils.BoxBoxResult.Afuera)
                {
                    collisionFound = true;
                }

                if (collisionFound)
                {
                    canoa.Position = new Vector3(0,0,0);
                }
            
            //Renderizar modelo
            mesh.render();

            canoa.render();

            //Obtener valor de UserVar (hay que castear)
            int valor = (int)GuiController.Instance.UserVars.getValue("variablePrueba");

            //Obtener valores de Modifiers
            float valorFloat = (float)GuiController.Instance.Modifiers["valorFloat"];
            string opcionElegida = (string)GuiController.Instance.Modifiers["valorIntervalo"];
            Vector3 valorVertice = (Vector3)GuiController.Instance.Modifiers["valorVertice"];


            ///////////////INPUT//////////////////
            //conviene deshabilitar ambas camaras para que no haya interferencia

            //Capturar Input teclado 
            if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.F))
            {
                //Tecla F apretada
            }

            //Capturar Input Mouse
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Boton izq apretado
            }

        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        /// 

        public override void close()
        {
            effect.Dispose();
                       
        }

    }
}
