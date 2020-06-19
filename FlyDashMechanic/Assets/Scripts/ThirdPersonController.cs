using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UnityEngine.Rendering.Universal.Internal;

public class ThirdPersonController : MonoBehaviour
{

    #region Variables
    //Entradas
    private float InputX;
    private float InputZ;

    [Header("Velocidades y fuerzas para la fisica del personaje")]
    public float moveSpeed = 3f;

    [Header("Referencia Cinemachine Camara")]
    public Cinemachine.CinemachineFreeLook cinemachineCamera;
    private float fovStart;

    [Header("Comprobaciones fisicas para el personaje")]
    public float gravity = -9.81f;
    private bool isGrounded;
    //Vector que utilizaremos para establecer la gravedad del personaje
    private Vector3 velocityGravity;

    [Header("Smoothness Zooms y Rotaciones")]
    [Range(0, 10)] public float rotateSpeed = 5f;

    [Header("FlyDash mechanic")]
    public Transform targetFly;
    public Transform pivot;
    public Transform espadaPersonaje;
    public Material glowMaterial;
    public ParticleSystem trailPersonaje;
    private ParticleSystem estelaEspada;
    [Space]
    public List<Transform> targets = new List<Transform>();
    private Transform targetLanzar;
    private Transform targetProximo = null;
    private CinemachineImpulseSource impulse;
    private GameObject clonePlayer;
    private GameObject cloneEspada;
    private float flyDuration = 1f;
    private bool isFlying = false;
    private bool isDoingAttack = false;
    private bool couldAttack = false;
    private bool soltarEspadaGamepad = false;
    private bool cancelarLanzamiento = false;
    private float prevXcam;

    [Header("Canvas FlyDash")]
    public Image apuntar;
    public Image fijar;
    public Vector2 uiSeparacion;

    [Header("PostProcess Volume")]
    public UnityEngine.Rendering.VolumeProfile volumenPostProcess;
    private UnityEngine.Rendering.Universal.Vignette vignette;





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

    //Referencia a los controles de los mandos para nuestro personaje
    GamepadController controls;
    //Variables para los input de los mandos
    //Movimiento del jugador con el joystick izquierdo
    Vector2 movePlayer;
    //Movimeinto de la camara con el joystick derecho
    Vector2 moveCamara;






    #endregion

    #region Init

    private void Awake()
    {

        controls = new GamepadController();
        controls.Gameplay.MovePlayer.performed += ctx => movePlayer = ctx.ReadValue<Vector2>();
        controls.Gameplay.MovePlayer.canceled += ctx => movePlayer = Vector2.zero;
        controls.Gameplay.MoveCamera.performed += ctx => moveCamara = ctx.ReadValue<Vector2>();
        controls.Gameplay.MoveCamera.canceled += ctx => moveCamara = Vector2.zero;

        controls.Gameplay.FlyDash.performed += ctx => {
            if (!isFlying)
                FlyDashAnim();
            else
                soltarEspadaGamepad = true;
                };

    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    void Start()
    {
        //Time.timeScale = 0.2f;
        impulse = cinemachineCamera.GetComponent<CinemachineImpulseSource>();
        _animator = this.GetComponent<Animator>();
        _cc = this.GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        fovStart = cinemachineCamera.m_Lens.FieldOfView;
        trailPersonaje.Stop();
        DOTween.Init();
        //apuntar.color = Color.clear;

    }


    void Update()
    {
        delta = Time.deltaTime;
        MagnitudInput();
        SetGravityGround();
        CamaraGamepad();
        if (targets.Count < 1)
            return;
        girarPivot();
        if (Input.GetMouseButtonDown(0) && !isFlying)
        {
            FlyDashAnim();
        }
        if (!isFlying)
            targetLanzar = targets[targetIndex()];
        SetCameraFly();
        FlyDashAttack();
        targetIndex();
        HudSetUp();
    }

    #endregion

    #region PlayerMovementAndGravity

    public void MagnitudInput()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        speedInputs = new Vector2(InputX, InputZ).sqrMagnitude;
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Slash"))
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
        //Estamos controlando el movimiento entre el gamepad y el movimiento de telcado y raton
        if (movePlayer.sqrMagnitude > minimoRotacion && speedInputs < minimoRotacion){
            playerInput = Vector3.ClampMagnitude(new Vector3(movePlayer.x, 0f, movePlayer.y), 1);
        }else if (movePlayer.sqrMagnitude < minimoRotacion && speedInputs > minimoRotacion){
            playerInput = Vector3.ClampMagnitude(new Vector3(InputX, 0f, InputZ), 1);
        }else{
            playerInput = Vector3.ClampMagnitude(new Vector3(InputX, 0f, InputZ), 1);
        }

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

    /// <summary>
    /// Esta funcion detecta si el hay algun movimiento en el joystick derecho del gamepad y en ese caso la camara rota en relacion a este.
    /// Si no hay ningun valor en el gamepad funcionara con el movimiento del raton 
    /// </summary>
    public void CamaraGamepad()
    {
        if (!isFlying) { 
            if (moveCamara.sqrMagnitude > 0.0f)
            {
                cinemachineCamera.m_XAxis.m_InputAxisValue = moveCamara.x;
                cinemachineCamera.m_XAxis.m_MaxSpeed = 200f;
                cinemachineCamera.m_YAxis.m_InputAxisValue = moveCamara.y;
                cinemachineCamera.m_YAxis.m_MaxSpeed = 1f;

            }
            else
            {
                float inputXMouse = Input.GetAxis("Mouse X");
                float inputYMouse = Input.GetAxis("Mouse Y");

                cinemachineCamera.m_XAxis.m_InputAxisValue = inputXMouse;
                cinemachineCamera.m_YAxis.m_InputAxisValue = inputYMouse;
            }
        }
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

    /// <summary>
    /// Gira el pivote que tiene asignado el player para posteriormente cuando el jugador quiera apuntar con el arma para disparar que apunte en la direccion que esta la camara
    /// de esta
    /// </summary>
    public void girarPivot()
    {
        Vector3 eulerAnglesAim = new Vector3(this.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, this.transform.eulerAngles.z);
        pivot.rotation = Quaternion.Euler(eulerAnglesAim);
        if (isDoingAttack)
        {
            //Apuntamos hacia la direccion que esta mirando la camara, para ello utilizamos el pivot que lleva el personaje
            Quaternion newRotation = Quaternion.LookRotation(pivot.transform.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotateSpeed * 2f * delta);
        }
    }


    #endregion


    #region FlyDash

    public void FlyDashAnim()
    {
        if (isGrounded)
        {
            gravity = 0f;
            //Boolean que controla si hay que girar el personaje a cierto sitio
            isDoingAttack = true;
            _animator.SetBool("doingAttack", isDoingAttack);
            //Activamos la animacion
            _animator.SetTrigger("flyDash");
            //El personaje estara en el aire por lo tanto usamos esto para saberlo
            isFlying = true;
            //Recogemos en una variable el ultimo valor del x axis en cinemachine
            prevXcam = cinemachineCamera.m_XAxis.Value;
        }

    }

    public void DoFlyDash()
    {
        //Vamos a simular como que el personaje esta subiendo por lo tanto vamos a hacer como unas sombras
        clonePlayer = Instantiate(gameObject, transform.position, transform.rotation);
        Destroy(clonePlayer.GetComponent<Animator>());
        Destroy(clonePlayer.GetComponent<ThirdPersonController>());
        Destroy(clonePlayer.GetComponent<CharacterController>());

        impulse.GenerateImpulse(Vector3.right);

        SkinnedMeshRenderer[] meshChildren = clonePlayer.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer sk in meshChildren)
        {
            sk.material = glowMaterial;
        }


        //Justo cuando salte el animation event borramos al personaje y la espada
        MostrarMesh(false);
        //Movemos al personaje hacia el gameobject empty que tiene en la cabeza siempre en una duracion determinada
        transform.DOMove(targetFly.position, flyDuration).OnComplete(() => StartFlyDashAttack());
        //Añadimos algun efecto de particulas
        trailPersonaje.Play();

    }

    public void StartFlyDashAttack()
    {
        //Estaremos en el aire ya por lo tanto listos para el ataque por lo tanto hacemos visible al personaje y la espada
        MostrarMesh(true);
        //Y ademas ponemos que el personaje pueda atacar
        couldAttack = true;
        _animator.speed = 0.01f;
        trailPersonaje.Stop();

        //Postprocessado
        if(volumenPostProcess.TryGet(out vignette))
        {
            vignette.active = true;
        }

        apuntar.transform.DORotate(Vector3.forward * 90, .15f, RotateMode.LocalAxisAdd);
        fijar.DOFade(1f, 0.15f);

    }

    public void SetCancelarLanzamiento()
    {
        cancelarLanzamiento = true;
    }

    public void FlyDashAttack()
    {
        if((Input.GetMouseButtonDown(0) || soltarEspadaGamepad)  && couldAttack)
        {
            _animator.speed = 0.8f;

            Invoke("LanzarEspadas", 0.15f);

            gravity = -9.81f;
            isDoingAttack = false;
            _animator.SetBool("doingAttack", isDoingAttack);
            Destroy(clonePlayer.gameObject);
            couldAttack = false;
            Invoke("EndFlyDashAttack", 1.1f);
            fijar.DOFade(0f, 0.15f);
            //Postprocessado
            if (volumenPostProcess.TryGet(out vignette))
            {
                vignette.active = false;
            }
            soltarEspadaGamepad = false;
        }else if (cancelarLanzamiento)
        {
            _animator.speed = 0.8f;
            gravity = -9.81f;
            isDoingAttack = false;
            _animator.SetBool("doingAttack", isDoingAttack);
            Destroy(clonePlayer.gameObject);
            couldAttack = false;
            Invoke("EndFlyDashAttack", 1.1f);
            //Postprocessado
            if (volumenPostProcess.TryGet(out vignette))
            {
                vignette.active = false;
            }
            cancelarLanzamiento = false;
        }
    }




    public void LanzarEspadas()
    {
        cloneEspada = Instantiate(espadaPersonaje.gameObject, espadaPersonaje.transform.position, espadaPersonaje.transform.rotation);
        estelaEspada = cloneEspada.GetComponentInChildren<ParticleSystem>();
        MeshRenderer meshEspada = cloneEspada.GetComponentInChildren<MeshRenderer>();
        meshEspada.material = glowMaterial;
        cloneEspada.transform.parent = null;
        //Cuando se complete se lanzara a los dos enemigos mas cercanos en un cierto rango si estan
        cloneEspada.transform.DOMove(targetLanzar.position + new Vector3(0f, 1.2f, 0f), flyDuration / 2f).SetEase(Ease.InExpo).OnComplete(() => ReboteEspada1());
        estelaEspada.Play();

        cloneEspada.transform.DOLookAt(targetLanzar.position, 0.15f);
        apuntar.color = Color.clear;
    }

    public void ReboteEspada1()
    {
        estelaEspada.Stop();
        targetLanzar.parent.gameObject.GetComponent<Animator>().SetTrigger("hit");

        float distanceInicial = Mathf.Infinity;

        for (int i = 0; i < targets.Count; i++)
        {
            //Si el nuevo target no es el target al cual primero le hemos tirado la espada
            if (targetLanzar != targets[i])
            {

                float distanceTarget = Vector3.Distance(targetLanzar.position, targets[i].position);

                if(distanceTarget < distanceInicial)
                {
                    distanceInicial = distanceTarget;
                    targetProximo = targets[i];
                }
            }
        }


        if(targetProximo != null)
        {
            cloneEspada.transform.DOMove(targetProximo.position + new Vector3(0f, 1.2f, 0f), flyDuration / 2f).SetEase(Ease.InExpo).OnComplete(() => ReboteEspada2());
            estelaEspada.Play();
            cloneEspada.transform.DOLookAt(targetProximo.position, 0.1f);
        }
        else
        {
            Destroy(cloneEspada, 2f);
        }

    }

    public void ReboteEspada2()
    {
        targetProximo.parent.gameObject.GetComponent<Animator>().SetTrigger("hit");
        estelaEspada.Stop();
        bool change = false;
       float distanceInicial = Mathf.Infinity;

        for (int i = 0; i < targets.Count; i++)
        {
            if (targetProximo != targets[i] && targetLanzar != targets[i])
            {
                float distanceTarget = Vector3.Distance(targetProximo.position, targets[i].position);
                if (distanceTarget < distanceInicial)
                {
                    change = true;
                    distanceInicial = distanceTarget;
                    targetProximo = targets[i];
                }
            }
        }
        if (change)
        {
            cloneEspada.transform.DOMove((targetProximo.position + new Vector3(0f, 1.2f, 0f)), flyDuration / 2f).SetEase(Ease.InExpo).OnComplete(() => { Destroy(cloneEspada, 2f); targetProximo.parent.gameObject.GetComponent<Animator>().SetTrigger("hit"); });
            estelaEspada.Play();
            cloneEspada.transform.DOLookAt(targetProximo.position, 0.1f);
        }
        else
        {
            Destroy(cloneEspada, 2f);
        }
    }

    public void EndFlyDashAttack()
    {
        impulse.GenerateImpulse(Vector3.right);
        trailPersonaje.Stop();
        isFlying = false;
        apuntar.color = Color.white;
    }

    public void SetCameraFly()
    {
        if (isFlying)
        {
            cinemachineCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.m_Lens.FieldOfView, 80f, delta / 1f);
            cinemachineCamera.m_YAxis.Value = Mathf.Lerp(cinemachineCamera.m_YAxis.Value, 0.75f, delta / 0.5f);
            cinemachineCamera.m_XAxis.m_MaxSpeed = 0f;
        }
        else
        {
            cinemachineCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.m_Lens.FieldOfView, fovStart, delta / 0.1f);
            cinemachineCamera.m_XAxis.m_MaxSpeed = 300f;
        }
    }

    public void MostrarMesh(bool estado)
    {
        SkinnedMeshRenderer[] meshChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
        MeshRenderer[] meshEspada = GetComponentsInChildren<MeshRenderer>();
        foreach(SkinnedMeshRenderer sk in meshChildren)
        {
            sk.enabled = estado;
        }
        foreach (MeshRenderer es in meshEspada)
        {
            es.enabled = estado;
        }
    }

    public int targetIndex()
    {
        float[] distances = new float[targets.Count];

        for (int i = 0; i < targets.Count; i++)
        {
            distances[i] = Vector2.Distance(Camera.main.WorldToScreenPoint(targets[i].position), new Vector2(Screen.width / 2, Screen.height / 2));
        }

        float minDistance = Mathf.Min(distances);
        int index = 0;

        for (int i = 0; i < distances.Length; i++)
        {
            if (minDistance == distances[i])
                index = i;
        }

        return index;
    }

    public void HudSetUp()
    {
       apuntar.transform.position = Camera.main.WorldToScreenPoint(targetLanzar.position + (Vector3)uiSeparacion);
       if (isFlying)
            return;

       Color c = targets.Count < 1 ? Color.clear : Color.white;
       apuntar.color = c;
    }

    #endregion


}
