using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;

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

    [Header("Comprobaciones fisicas para el personaje")]
    public float gravity = -9.81f;
    private bool isGrounded;
    //Vector que utilizaremos para establecer la gravedad del personaje
    private Vector3 velocityGravity;

    [Header("Smoothness Zooms y Rotaciones")]
    [Range(0, 10)] public float rotateSpeed = 5f;

    [Header("FlyDash mechanic")]
    public Transform target;
    [Range(0.5f,1.5f)] public float flyDuration = 1f;


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


    //Para las animaciones y la camara y demas
    private bool isFlying = false;



    #endregion

    #region Init

    private void Awake()
    {

        controls = new GamepadController();
        controls.Gameplay.MovePlayer.performed += ctx => movePlayer = ctx.ReadValue<Vector2>();
        controls.Gameplay.MovePlayer.canceled += ctx => movePlayer = Vector2.zero;
        controls.Gameplay.MoveCamera.performed += ctx => moveCamara = ctx.ReadValue<Vector2>();
        controls.Gameplay.MoveCamera.canceled += ctx => moveCamara = Vector2.zero;
        controls.Gameplay.FlyDash.performed += ctx => FlyDashAnim();

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
        _animator = this.GetComponent<Animator>();
        _cc = this.GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        DOTween.Init();
    }


    void Update()
    {
        delta = Time.deltaTime;
        MagnitudInput();
        SetGravityGround();
        CamaraGamepad();
        if (Input.GetMouseButtonDown(0))
        {
            FlyDashAnim();
        }
        SetCameraFly();
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
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Slash")) { 
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
            velocityGravity.y = -2f;
        }

        velocityGravity.y += gravity * Time.deltaTime;
        //La aceleracion hay que multiplicar el tiempo al cuadrado;
        _cc.Move(velocityGravity * Time.deltaTime);

        //En el animator la variable va a ser la misma a isGrounded de la clase
        _animator.SetBool("isSuelo", isGrounded);

    }


    #endregion


    #region FlyDash

    public void FlyDashAnim()
    {
        _animator.SetTrigger("flyDash");
    }

    public void DoFlyDash()
    {
        transform.DOMove(target.position, flyDuration);
        Invoke("SlowMotionFly", flyDuration);
        isFlying = true;

    }

    public void SetCameraFly()
    {
        if (isFlying)
        {
            cinemachineCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.m_Lens.FieldOfView, 50f, delta *2f);
            cinemachineCamera.m_YAxis.Value = Mathf.Lerp(cinemachineCamera.m_YAxis.Value, 0.75f, delta * delta *2f);
            cinemachineCamera.m_XAxis.Value = Mathf.Lerp(cinemachineCamera.m_XAxis.Value, 0f, delta *2f);
        }
    }

    public void SlowMotionFly()
    {
        Time.timeScale = 1f;
        _animator.speed = 0.05f;
    }

    #endregion


}
