using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using System;

public class MarioController : MonoBehaviour
{

    #region Variables
    //Entradas
    private float InputX;
    private float InputZ;

    [Header("Velocidades y fuerzas para la fisica del personaje")]
    public float moveSpeed = 3f;

    [Header("Comprobaciones fisicas para el personaje")]
    public float gravity = -9.81f;
    private bool isGrounded;
    //Vector que utilizaremos para establecer la gravedad del personaje
    private Vector3 velocityGravity;

    [Header("Smoothness Zooms y Rotaciones")]
    [Range(0, 10)] public float rotateSpeed = 5f;

    [Header("Cinemachine reference")]
    public Cinemachine.CinemachineFreeLook cineCameraMario;
    public Cinemachine.CinemachineFreeLook cineCameraTarget;



    [Space]
    [Header("Mario hat mechanic")]
    public GameObject instanceMario;
    [Space]
    public Transform cap;
    public Transform capLanzar;
    public Transform pivotCap;
    public Transform padreCapCabeza;
    public Transform padreCapMano;
    public ParticleSystem particulasLanzamineto;
    public float rotateCapSpeed = 1.0f;
    public float duracionHoldCap = 6f;
    private bool isThrowed = false;
    private bool isSpinning = false;
    private bool vueltaGorroUpdate = false;
    private float timeMaximoVuelta;

    //Transicion a otro target
    private GameObject targetCambiar;
    private bool transicionOn;
    private bool transicionVolverMario = false;


    
    [Space]
    [Header("Materials disolve")]
    public Material disolveHatCabeza;
    public Material disolveEyesCabeza;
    public Material disolveRingCabeza;

    [Space]
    public Material disolveHatLanzar;
    public Material disolveEyesLanzar;
    public Material disolveRingLanzar;

    [Space]
    public Material disolveHatTarget;
    public Material disolverEyesTarget;
    public Material disolveRingTarget;

    [Space]
    public Material joinsMarioMaterial;
    public Material surfaceMarioMaterial;

    float cambioTransparente = 0f;
    float cambioOpaco = 100f;
    private bool cambiarMateriales = false;



    Vector3 rotationCapIncial;
    Vector3 positionCapIncial;




    //Time.deltatime
    private float delta;

    //Charactaer controller
    private CharacterController _cc;

    //Animator
    private Animator _animator;

    //Vector al cual vamos a querer que se mueva nuestro personaje
    private Vector3 desiredMovement;

    //Minimo para las rotaciones cuando nos movemos
    private float minimoRotacion = 0.1f;

    //Velocidad de nuestros inputs para saber si estamso en movimiento o no
    private float speedInputs;







    #endregion

    #region Init


    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _cc = this.GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        DOTween.Init();
        rotationCapIncial = cap.eulerAngles;
        positionCapIncial = cap.localPosition;


        //Materiales
        disolveHatCabeza.SetFloat("Vector1_5A2F4A5", 0);
        disolveEyesCabeza.SetFloat("Vector1_7527672C", 0);
        disolveRingCabeza.SetFloat("Vector1_7AE198B8", 0);

        disolveHatLanzar.SetFloat("Vector1_5A2F4A5", 1);
        disolveEyesLanzar.SetFloat("Vector1_7527672C", 1);
        disolveRingLanzar.SetFloat("Vector1_7AE198B8", 1);

        //Borrar el sombrero del enemigo
        disolveHatTarget.SetFloat("Vector1_5A2F4A5", 1);
        disolverEyesTarget.SetFloat("Vector1_7527672C", 1);
        disolveRingTarget.SetFloat("Vector1_7AE198B8", 1);

        //Poner entero al personaje al personaje
        joinsMarioMaterial.SetFloat("Vector1_5A2F4A5", 0);
        surfaceMarioMaterial.SetFloat("Vector1_7527672C", 0);


    }


    void Update()
    {
        delta = Time.deltaTime;
        MagnitudInput();
        SetGravityGround();
        LanzarGorroAnim();
        SetRotacionGorro();
        if (vueltaGorroUpdate)
            VueltaGorro();
        AlphaMateriales();
        TransicionTransformarse();
        InsideTarget();
        TransicionAMario();
    }

    #endregion

    #region PlayerMovementAndGravity

    public void MagnitudInput()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        speedInputs = new Vector2(InputX, InputZ).sqrMagnitude;
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("LanzarGorro"))
        {
            if (speedInputs > minimoRotacion)
            {
                PlayerInput(true);
            }
            else
            {
                PlayerInput(false);
            }
        }
    }
    public void PlayerInput(bool rot)
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        //Hacemos este vector3 para que la velocidad diagonal como la estandar no se multipliquen y por lo tanto la velocidad maxima sea 1 en cualquier direccion
        Vector3 playerInput;
        playerInput = Vector3.ClampMagnitude(new Vector3(InputX, 0f, InputZ), 1);

        Vector3 camForw = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForw.y = 0f;
        camRight.y = 0f;

        camForw.Normalize();
        camRight.Normalize();

        desiredMovement = playerInput.x * camRight + playerInput.z * camForw;
        desiredMovement = desiredMovement * moveSpeed;

        _animator.SetFloat("velocidadPlayer", desiredMovement.sqrMagnitude);
        //Si rot es true se rotara si no no, es para controlar la animacion
        if (rot)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMovement), rotateSpeed * delta);
        _cc.Move(desiredMovement * delta);

    }
    public void SetGravityGround()
    {
        isGrounded = Physics.Raycast(_cc.bounds.min, Vector3.down, 0.1f);


        if (isGrounded && velocityGravity.y < 0)
        {
            velocityGravity.y = 0f;
        }

        velocityGravity.y += gravity * Time.deltaTime;
        //La aceleracion hay que multiplicar el tiempo al cuadrado;
        _cc.Move(velocityGravity * Time.deltaTime);

        //En el animator la variable va a ser la misma a isGrounded de la clase
        _animator.SetBool("isSuelo", isGrounded);

    }


    #endregion

    #region MarioHat
    //Mecanica lanzar el gorro y que vuelva junto a la voltereta
    public void LanzarGorroAnim()
    {
        if (Input.GetMouseButtonDown(0) && !isThrowed)
        {
            isThrowed = true;
            _animator.SetTrigger("lanzar");
            //Ponemos los materiales para que el gorro de la sensacion de que se disuelve
            cambiarMateriales = true;
            padreCapMano.gameObject.SetActive(true);
        }

    }

    public void AlphaMateriales()
    {
        if (cambiarMateriales)
        {
            Debug.Log("Me meto en cambiar los materiales del principio");
            //El gorro de la cabeza
            cambioTransparente += 2f;
            if(cambioTransparente <= 100)
            {
                disolveHatCabeza.SetFloat("Vector1_5A2F4A5", cambioTransparente / 100f);
                disolveEyesCabeza.SetFloat("Vector1_7527672C", cambioTransparente / 100f);
                disolveRingCabeza.SetFloat("Vector1_7AE198B8", cambioTransparente / 100f);
            }

            //El de lanzar
            cambioOpaco -= 2f;
            if (cambioOpaco >= 0)
            {
                disolveHatLanzar.SetFloat("Vector1_5A2F4A5", cambioOpaco / 100f);
                disolveEyesLanzar.SetFloat("Vector1_7527672C", cambioOpaco / 100f);
                disolveRingLanzar.SetFloat("Vector1_7AE198B8", cambioOpaco / 100f);
            }

        }
    }

    public void LanzarGorroMove()
    {
        //Para que los materiales se reseten bien
        cambiarMateriales = false;
        cambioTransparente = 0f;
        cambioOpaco = 100f;

        padreCapMano.gameObject.SetActive(false);
        capLanzar.transform.parent = null;
        capLanzar.transform.DOMove(pivotCap.position, 0.5f);
        isSpinning = true;
        capLanzar.transform.DORotate(new Vector3(18f, 0f, 368f), 0.1f);
        Invoke("VueltaGorro", 2f);
        timeMaximoVuelta = Time.time + duracionHoldCap;
        capLanzar.GetComponent<SphereCollider>().enabled = true;
        particulasLanzamineto.Play();

    }

    public void SetRotacionGorro()
    {
        if (isSpinning && !transicionOn)
        {
            capLanzar.transform.Rotate(0f, rotateCapSpeed, 0f);
        }
    }

    public void VueltaGorro()
    {
        if (!transicionOn)
        {
            vueltaGorroUpdate = true;
            if (!Input.GetMouseButton(0))
            {
                capLanzar.GetComponent<SphereCollider>().enabled = false;
                capLanzar.parent = padreCapCabeza.transform;
                isSpinning = false;
                capLanzar.transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() => ResetLanzamientoSombrero(true));
                capLanzar.transform.DOLocalRotate(Vector3.zero, 0.5f);
                vueltaGorroUpdate = false;
            }
            else if (Time.time > timeMaximoVuelta)
            {
                capLanzar.GetComponent<SphereCollider>().enabled = false;
                capLanzar.parent = padreCapCabeza.transform;
                isSpinning = false;
                capLanzar.transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() => ResetLanzamientoSombrero(true));
                capLanzar.transform.DOLocalRotate(Vector3.zero, 0.5f);
                vueltaGorroUpdate = false;
            }
        }
    }

    public void ResetLanzamientoSombrero(bool reset)
    {
        Debug.Log("Me meto pq no hag transiciones");
        disolveHatCabeza.SetFloat("Vector1_5A2F4A5",0);
        disolveEyesCabeza.SetFloat("Vector1_7527672C",0);
        disolveRingCabeza.SetFloat("Vector1_7AE198B8",0);

        particulasLanzamineto.Stop();

        capLanzar.transform.parent = padreCapMano;
        capLanzar.transform.parent.gameObject.SetActive(false);
        capLanzar.transform.localPosition = Vector3.zero;
        capLanzar.transform.eulerAngles = Vector3.zero;
        
        if(reset)
            isThrowed = false;
    }

    public void JumpVoltereta()
    {
        velocityGravity.y = Mathf.Sqrt(1.8f * -2f * gravity);
        capLanzar.GetComponent<SphereCollider>().enabled = false;
        capLanzar.parent = padreCapCabeza.transform;
        isSpinning = false;
        capLanzar.transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() => ResetLanzamientoSombrero(true));
        capLanzar.transform.DOLocalRotate(Vector3.zero, 0.5f);
        vueltaGorroUpdate = false;
    }
    //Mecanica de convertirse en los demas
    public void TransicionTransformarse()
    {
        if (transicionOn)
        {
            particulasLanzamineto.Stop();

            moveSpeed = 2f;
            //El gorro de lanzar
            cambioTransparente += 1.5f;
            if (cambioTransparente <= 100)
            {
                disolveHatLanzar.SetFloat("Vector1_5A2F4A5", cambioTransparente / 100f);
                disolveEyesLanzar.SetFloat("Vector1_7527672C", cambioTransparente / 100f);
                disolveRingLanzar.SetFloat("Vector1_7AE198B8", cambioTransparente / 100f);
            }

            cineCameraTarget.LookAt = targetCambiar.transform;
            cineCameraTarget.Follow = targetCambiar.transform;


            cineCameraMario.Priority = 0;
            cineCameraTarget.Priority = 1;

            //El de cada target
            cambioOpaco -= 1.5f;
            if (cambioOpaco >= 0)
            {
                disolveHatTarget.SetFloat("Vector1_5A2F4A5", cambioOpaco / 100f);
                disolverEyesTarget.SetFloat("Vector1_7527672C", cambioOpaco / 100f);
                disolveRingTarget.SetFloat("Vector1_7AE198B8", cambioOpaco / 100f);

                //Disolver al personaje
                joinsMarioMaterial.SetFloat("Vector1_5A2F4A5", cambioTransparente / 100f);
                surfaceMarioMaterial.SetFloat("Vector1_7527672C", cambioTransparente / 100f);
            }
        }
    }

    public bool getTransicion()
    {
        return transicionOn;
    }
    public void setTransicion(bool t, GameObject target)
    {
        transicionOn = t;
        if (target != null)
            targetCambiar = target;

    }

    public void InsideTarget()
    {
        if(transicionOn && Input.GetKeyDown(KeyCode.F))
        {
            particulasLanzamineto.Play();

            //Prioridad de las camaras
            cineCameraMario.Priority = 1;
            cineCameraTarget.Priority = 0;

            //Variables de alpha
            cambioTransparente = 0f;
            cambioOpaco = 100f;

            //Rigidbody
            targetCambiar.GetComponent<TargetController>().enabled = false;

            //Variable que activa la transicion a Mario
            transicionVolverMario = true;

            transicionOn = false;

            //Reseteamos como si estuviera volviendo sin transicion
            capLanzar.GetComponent<SphereCollider>().enabled = false;
            capLanzar.parent = padreCapCabeza.transform;
            isSpinning = false;
            capLanzar.transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() => ResetLanzamientoSombrero(false));
            capLanzar.transform.DOLocalRotate(Vector3.zero, 0.5f);
            vueltaGorroUpdate = false;

            Invoke("ResetIsThrow", 3f);
        }
    }

    public void ResetIsThrow()
    {
        isThrowed = false;
    }

    public void setTargetMario(GameObject t)
    {
        if(t!=null)
            targetCambiar = t;
    }

    public void TransicionAMario()
    {
        if (transicionVolverMario)
        {
            //El de cada target
            cambioTransparente += 1f;
            if (cambioTransparente <= 100)
            {
                disolveHatTarget.SetFloat("Vector1_5A2F4A5", cambioTransparente / 100f);
                disolverEyesTarget.SetFloat("Vector1_7527672C", cambioTransparente / 100f);
                disolveRingTarget.SetFloat("Vector1_7AE198B8", cambioTransparente / 100f);
            }



            //Volvemos a mostrar al personaje
            cambioOpaco -= 1f;

            if (cambioOpaco >= 0)
            {
                joinsMarioMaterial.SetFloat("Vector1_5A2F4A5", cambioOpaco / 100f);
                surfaceMarioMaterial.SetFloat("Vector1_7527672C", cambioOpaco / 100f);

                //Y lo mismo con el gorro de mario
                disolveHatCabeza.SetFloat("Vector1_5A2F4A5", cambioOpaco / 100f);
                disolveEyesCabeza.SetFloat("Vector1_7527672C", cambioOpaco / 100f);
                disolveRingCabeza.SetFloat("Vector1_7AE198B8", cambioOpaco / 100f);
            }

            moveSpeed = 5f;
            particulasLanzamineto.Stop();



            Invoke("FinalizarTransicionAMario",1.5f);
            Invoke("ResetearAlphas",0.5f);
        }
    }

    public void FinalizarTransicionAMario() {
        transicionVolverMario = false;
        cambioTransparente = 0f;
        cambioOpaco = 100f;

        targetCambiar.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ResetearAlphas()
    {
        //El gorro de lanzar estara invisible
        disolveHatCabeza.SetFloat("Vector1_5A2F4A5", 0);
        disolveEyesCabeza.SetFloat("Vector1_7527672C", 0);
        disolveRingCabeza.SetFloat("Vector1_7AE198B8", 0);
        //El gorro en la cabeza estara full visible
        disolveHatLanzar.SetFloat("Vector1_5A2F4A5", 1);
        disolveEyesLanzar.SetFloat("Vector1_7527672C", 1);
        disolveRingLanzar.SetFloat("Vector1_7AE198B8", 1);

    }

    #endregion


}

