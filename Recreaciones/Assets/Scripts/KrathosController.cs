using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class KrathosController : MonoBehaviour
{
    #region Variables
    //Variables de entrada
    private float InputX;
    private float InputZ;

    //Camara Principal
    private Camera mainCamera;

    //Character controller del personaje
    private CharacterController _cc;

    //Animator del personaje y cosas de las animaciones
    private Animator _anim;

    //Direccion a la que queremos qsue el jugadoer se mueva en todo momento, la utilizamos para movernos ademas de para rotar al personaje
    Vector3 movimientoDeseado;




    [Header("Velocidades Movimientos y Rotaciones")]
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float rotateSpeed = 5f;
    //Vector 3 que utilizamos para la gravedad y sus funciones
    private Vector3 velocity;


    [Header("Comprobaciones fisicas")]
    public Transform groundCheck;
    public LayerMask groundMask;
    //Lo utilizamos para girar al personaje hacia donde esta mirando la camara cuando apunta con el hacha
    public Transform pivotAim;
    //Comprueba si el jugador esta en el suelo
    private bool isGrounded;
    //Controla si el jugador esta apuntando o no
    private bool aiming = false;

    [Header("Comprobaciones visuales")]
    public GameObject canvasMirilla;


    [Header("Referencia WeaponThrow Script")]
    public WeaponThrow weaponThrow;


    //Velocidada minima a la que tiene que ir el personaje para que se de la rotacion
    private float permitirRotacion = 0.1f;

    //Recogemos el Time deltaTime en esta variable para que nos sea mas comodo utilizarla
    private float delta;

    //Varibale que recogera si nos estamos moviendo o no
    private float speed;

    [Header("Camara Cinemamachine")]
    //Camara cinemachine que utilizamos para hacer la similitud de movimiento cuando apunta
    public Cinemachine.CinemachineFreeLook cinemaCamera;

    #endregion

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        canvasMirilla.gameObject.SetActive(false);
        _anim = this.GetComponent<Animator>();
        mainCamera = Camera.main;
        _cc = this.GetComponent<CharacterController>();
    }

    void Update()
    {
        delta = Time.deltaTime;
        magnitudInputs();
        girarPivot();
        movimientosAdicionales();
        setGravity();

    }

    /// <summary>
    /// Gira el pivote que tiene asignado el player para posteriormente cuando el jugador quiera apuntar con el arma para disparar que apunte en la direccion que esta la camara
    /// de esta
    /// </summary>
    public void girarPivot()
    {
        Vector3 eulerAnglesAim = new Vector3(this.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, this.transform.eulerAngles.z);
        pivotAim.rotation = Quaternion.Euler(eulerAnglesAim);
    }

    /// <summary>
    /// Los diferntes movimientos que realiza el personaje a parte del tipico de movimiento
    /// </summary>
    public void movimientosAdicionales()
    {
        apuntarTirar();
        recogerArma();
    }

    /// <summary>
    /// Se encarga de ajustar las animaciones de recogida del arma
    /// El comportamiento fisico de esta y demas efectos los trata en el script weapongThrow
    /// </summary>
    public void recogerArma()
    {
        if (weaponThrow.getReturning())
        {
            _anim.SetBool("volviendoArma",true);
        }
        else
        {
            _anim.SetBool("volviendoArma", false);

        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && weaponThrow.getThrow())
        {
            _anim.SetTrigger("recoger");
        }
    }

    /// <summary>
    /// Este metodo se encarga de controlar las animaciones y los elementos del hud del player cuando apunta y cuando no esta apuntando, ademas de cuando tira el arma
    /// Por otra parte tambien llama a las funciones de zoom para controlarlo cuando apunta
    /// Basicamente es un controlador de animaciones y de la rotacion del personaje, las fisicas del lanzamiento se encarga el script WeaponThrow
    /// </summary>
    public void apuntarTirar()
    {
        if (speed < 0.1f)
        {
            //Si el jugador y el hacha no ha sido lanzada
            if (Input.GetKey(KeyCode.Mouse1) && !weaponThrow.getThrow())
            {
                aiming = true;
                //Camera Zoom
                cameraZoom();
                //Vamos a hacer ademas que el jugador mire a donde esta mirando el forward de la camara, utilizamos quaternion
                Quaternion newRotation = Quaternion.LookRotation(pivotAim.transform.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotateSpeed * 2f * delta);
                
                //Lanza el hacha
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    //Ocultamos canvas
                    canvasMirilla.gameObject.SetActive(false);
                    _anim.SetTrigger("throw");
                }
            }
            else
            {
                //Ocultamos canvas
                canvasMirilla.gameObject.SetActive(false);              
                aiming = false;
                //Camera unZoom
                cameraUnZoom();
            }
            _anim.SetBool("apuntando", aiming);
        }
    }

    /// <summary>
    /// Estos metodos se encargan de realizar la animacion de un zoom de la camara manipulando el field of view de la camara del cinemamachine mediante una interpolacion matematica de dos valores que sucede en un espacio de tiempo (ultimo parametro)
    /// </summary>
    public void cameraZoom(){
        cinemaCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemaCamera.m_Lens.FieldOfView,60,delta * 5);
    }
    public void cameraUnZoom(){
        cinemaCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemaCamera.m_Lens.FieldOfView, 80, delta * 5);
    }

    /// <summary>
    /// Metodo que se invoca desde el Animator en cierto frame de la animacion de apuntar para que se muestre el canvas con nuestra mirilla y sea mas acertado y a tiempo
    /// </summary>
    public void sacarCanvas()
    {
        //Activamos el canvas de la mira
        canvasMirilla.gameObject.SetActive(true);
    }

    /// <summary>
    /// movimientoJugador()
    /// Este metodo mueve al jugador en funcion de los inputs axis horizontal y vertical ademas de rotarlo en funcion a la camara que estemos utilizando, siempre sera la main camera ya que tiene ligado el cinemamchine
    /// Rotaremos y nos moveremos siempre al mismo vector
    /// </summary>
    void movimientoJugador()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        Vector3 camaraForward = mainCamera.transform.forward;
        Vector3 camaraRight = mainCamera.transform.right;

        camaraForward.y = 0f;
        camaraRight.y = 0f;

        camaraForward.Normalize();
        camaraRight.Normalize();

        movimientoDeseado = camaraForward * InputZ + camaraRight * InputX;

        _anim.SetFloat("velocidadPlayer", (movimientoDeseado * moveSpeed).sqrMagnitude);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movimientoDeseado), rotateSpeed * delta);
        _cc.Move(movimientoDeseado * moveSpeed * delta);

    }

    /// <summary>
    /// magnitudInputs()
    /// Este metodo controla cuando el personaje es apto para rotar y para moverse, de tal manera que si el personaje se va a parar no se ejecutara el Quaternion del movinmiento del jugador
    /// evitando asi que el jugador siempre gire para la misma posicion cuando se para y se quede mirando al ultimo vector3 que se ha solicitado
    /// </summary>
    void magnitudInputs()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");


        //Calculate the Input Magnitude
        if(!aiming)
        speed = new Vector2(InputX, InputZ).sqrMagnitude;

        if (speed > permitirRotacion && !aiming)
        {
            movimientoJugador();
        }
        else if (speed < permitirRotacion)
        {
             _anim.SetFloat("velocidadPlayer",0.1f);
            
        }
    }


    /// <summary>
    /// setGravity()
    /// Metodo que asigna la gravedad al personaje.
    /// Siempre que este en el suelo nos aseguraremos de que se encuentra en el suelo diciendole que su velocidad.y en vez de ser 0 será -2f
    /// Si está en el aire  le aplicamos una aceleración para que cada vez caiga más rapido
    /// </summary>
    void setGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundMask);
        _anim.SetBool("isGrounded", isGrounded);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        //La aceleracion hay que multiplicar el tiempo al cuadrado;
        _cc.Move(velocity * Time.deltaTime);
    }


    /// <summary>
    /// Este metodo la sphere que utilizamos para saber si el personaje colisiona con los objetos que se encuentren en la layer ground
    /// por lo tanto si colisiona significara que esta en el suelo el personaje.
    /// Es puramente orientativo y visual, como tal no hace nada
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, 0.1f);
    }
}
