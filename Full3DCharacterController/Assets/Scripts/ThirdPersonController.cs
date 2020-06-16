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


    //Variables movimiento
    [Header("Velocidades y fuerzas para la fisica del personaje")]
    public float moveSpeed = 3f;
    private float currentSpeed;
    public float jumpForce = 2f;

    [Header("Comprobaciones fisicas para el personaje")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float gravity = -9.81f;
    //Vector que utilizaremos para establecer la gravedad del personaje
    private Vector3 velocityGravity;
    private bool isGrounded;
    private bool isAgachado = false;
    private bool isRunning = false;
    private bool isFalling = false;

    /*#region Variables FootIK
    private Vector3 leftFootPosition, rightFootPosition, rightFootIkPosition, leftFootIkPosition;
    private Quaternion leftFootIkRotation, rightFootIkRotation;
    private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;
    [Header("Foot IK controller")]
    public bool enableFootIk = true;
    [Range(0, 2)][SerializeField] private float distanceFloorIK = 1.14f;
    [Range(0,2)][SerializeField] private float raycastDownDistance = 1.5f;
    //Necesitamos utilizar tambien el campo del layermask que sea nuestro escenario
    [SerializeField] private float pelvisOffSet = 0f;
    [Range (0,1)][SerializeField] private float pelvisSpeed = 0.3f;
    [Range(0, 1)] [SerializeField] private float ikPositionSpeed = 0.5f;

    public string leftFootAnimVariableName = "LeftFootCurve";
    public string rightFootAnimVariableName = "RightFootCurve";

    public bool useIkPro = false;
    public bool showIkDebug = true;
    #endregion*/









    [Header("Referencia Cinemachine Camara")]
    public Cinemachine.CinemachineFreeLook cinemachineCamera;

    [Header("Smoothness Zooms y Rotaciones")]
    [Range(0,10)]public float rotateSpeed = 5f;
    [Range(0, 10)] public float zoomCorrerSpeed = 5f;
    [Range(0, 10)] public float zoomAgachadoSpeed = 2f;





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
    PlayerControls controls;


    //Variables para los input de los mandos
    //Movimiento del jugador con el joystick izquierdo
    Vector2 movePlayer;
    //Movimeinto de la camara con el joystick derecho
    Vector2 moveCamara;

    //Boolean que controla si el jugador con el gamepad esta corriendo
    private bool isRunningGamepad;

    //Caracteristicas iniciales del cc
    private float ccHeightStart;
    private float ccCenterStart;



    #endregion

    #region InitGameplay

    private void Awake()
    {
        //Gamepad del player
        controls = new PlayerControls();
        //Lo que hace esto es llamar al metodo que tenga el nombre que le hemos asignado, en nuestro caso sera Jump
        //Performed se refiere a cuando la accion esta ocurriendo, estan otros atributos como son started y canceled
        controls.Gameplay.Jump.performed += ctx => Jumps();
        controls.Gameplay.Agacharse.performed += ctx => Agacharse();
        //Esto de aqui arriba es una Lambda expresion la cual hay que investigar
        controls.Gameplay.MovePlayer.performed += ctx => movePlayer = ctx.ReadValue<Vector2>();
        controls.Gameplay.MovePlayer.canceled += ctx => movePlayer = Vector2.zero;
        controls.Gameplay.MoveCamera.performed += ctx => moveCamara = ctx.ReadValue<Vector2>();
        controls.Gameplay.MoveCamera.canceled += ctx => moveCamara = Vector2.zero;

        //Si el click izquierdo esta pulsado haremos el metodo de correr
        controls.Gameplay.RunButton.started += ctx => isRunningGamepad = true;
        controls.Gameplay.RunButton.performed += ctx => isRunningGamepad = true;
        controls.Gameplay.RunButton.canceled += ctx => isRunningGamepad = false;



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
        Time.timeScale = 0.2f;
        _animator = this.GetComponent<Animator>();
        _cc = this.GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = moveSpeed;
        DOTween.Init();
        ccHeightStart = _cc.height;
        ccCenterStart = _cc.center.y;

    }

    // Update is called once per frame
    void Update()
    {
        delta = Time.deltaTime;
        MagnitudInput();
        AdicionalMovements();
        SetGravityGround();
        CamaraGamepad();
    }

    #endregion

    /// <summary>
    /// Esta funcion detecta si el hay algun movimiento en el joystick derecho del gamepad y en ese caso la camara rota en relacion a este.
    /// Si no hay ningun valor en el gamepad funcionara con el movimiento del raton 
    /// </summary>
    public void CamaraGamepad()
    {
        if(moveCamara.sqrMagnitude > 0.0f)
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


    public void AdicionalMovements()
    {
        //Correr con teclado
        if (Input.GetKey(KeyCode.LeftShift) && !isAgachado && speedInputs > 0.1f)
        {
            Correr();
        }
        else
        {
            currentSpeed = moveSpeed;
            isRunning = false;
            UnZoomCorrer();
        }

        //Correr con gamepad
        if (isRunningGamepad && !isAgachado && speedInputs > 0.1f)
        {
            currentSpeed = moveSpeed * 2;
            ZoomCorrer();
            isRunning = true;
        }
        else if(!Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = moveSpeed;
            UnZoomCorrer();
            isRunning = false;
        }

        //Agacharse con control
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Agacharse();
        }
        EstablecerHeightAgachado();


        //Saltos
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jumps();
        }

        Falling();
        EstablecerHeightFalling();
        

    }

    public void Falling()
    {
        if (!isGrounded && !_animator.GetCurrentAnimatorStateInfo(0).IsName("JumpIdle"))
        {
            isFalling = true;
        }
        else {
            isFalling = false;
        }

        _animator.SetBool("isFalling", isFalling);
    }

    public void EstablecerHeightFalling()
    {
        if (!isFalling)
        {
            _cc.height = 1.7f;
        }
        else
        {
            _cc.height = Mathf.Lerp(_cc.height, 0.6f, delta * 10f);
            _cc.center = new Vector3(0f, Mathf.Lerp(_cc.center.y, 1.2f, delta * 10f), 0f);

        }
    }


    public void Jumps()
    {
        if(isGrounded && speedInputs < 0.1 && !isAgachado)
        {
            //Salto estatico
            _animator.SetTrigger("jumpIdle");
        }else if (isGrounded && speedInputs > 0.1 && !isAgachado)
        {
            //Salto en movimiento
        }
    }





    /// <summary>
    /// Este metodo duplica la velocidad del jugador, ademas de establecer la variable isRunning a true y realizar un zoom para dar la sensacion al usuario de velocidad
    /// </summary>
    public void Correr()
    {
        currentSpeed = moveSpeed * 2;
        ZoomCorrer();
        isRunning = true;
    }


    /// <summary>
    /// Establece el estado de isAgachado dependiendo lo que haga el usuario ya activa la animacion de agacharse
    /// </summary>
    public void Agacharse()
    {
        if (!isRunning && isGrounded)
        {
            Debug.Log("entroTRO");
             isAgachado = !isAgachado;
            _animator.SetBool("isAgachado",isAgachado);
        }
       
    }


    /// <summary>
    /// Establece mediante interpolacion la altura del collider del character controller para que se ajuste al movimiento mediante interpolacion dependiendo si esta agachado o no
    /// </summary>
    public void EstablecerHeightAgachado()
    {
        if (isGrounded)
        {
            //Si no esta agachado vamos mediante interpolacion a establecer el heig
            if (!isAgachado)
            {
                //_cc.height = Mathf.Lerp(_cc.height, ccHeightStart, delta * 3f);
                //_cc.center = new Vector3(0f, Mathf.Lerp(_cc.center.y, ccCenterStart, delta * 3f), 0f);
                UnZoomAgachado();
            }
            else
            {
                _cc.height = Mathf.Lerp(_cc.height, 1.2f, delta * 3f);
                _cc.center = new Vector3(0f, Mathf.Lerp(_cc.center.y, 0.65f, delta * 3f), 0f);
                ZoomAgachado();

            }
        }

    }



    #region ZoomCameras
    public void ZoomCorrer()
    {
        if(!isAgachado)
        cinemachineCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.m_Lens.FieldOfView, 30, delta * zoomCorrerSpeed);
    }

    public void UnZoomCorrer()
    {
        if(!isAgachado)
        cinemachineCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.m_Lens.FieldOfView, 40, delta * zoomCorrerSpeed);
    }

    public void ZoomAgachado()
    {
        cinemachineCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.m_Lens.FieldOfView, 60, delta * zoomAgachadoSpeed);

    }
    public void UnZoomAgachado()
    {
        cinemachineCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.m_Lens.FieldOfView, 40, delta * zoomAgachadoSpeed);
    }
    #endregion

    #region PlayerSimpleMovement
    public void PlayerInput(bool rot)
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        //Hacemos este vector3 para que la velocidad diagonal como la estandar no se multipliquen y por lo tanto la velocidad maxima sea 1 en cualquier direccion
        Vector3 playerInput;
        //Estamos controlando el movimiento entre el gamepad y el movimiento de telcado y raton
        if (movePlayer.sqrMagnitude > minimoRotacion && speedInputs < minimoRotacion) {
            playerInput = Vector3.ClampMagnitude(new Vector3(movePlayer.x, 0f, movePlayer.y), 1);
        }else if(movePlayer.sqrMagnitude < minimoRotacion && speedInputs > minimoRotacion){
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
        desiredMovement = desiredMovement * currentSpeed;

        _animator.SetFloat("velocidadPlayer", desiredMovement.sqrMagnitude);
        //Si rot es true se rotara si no no, es para controlar la animacion
        if(rot)
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMovement), rotateSpeed * delta);
        _cc.Move(desiredMovement * delta);

    }

    public void MagnitudInput()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        speedInputs = new Vector2(InputX, InputZ).sqrMagnitude;
        //El primer if se basa en algunas de las animaciones que no permiten que haya movimiento
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("JumpIdle") && !_animator.GetCurrentAnimatorStateInfo(0).IsName("LandFalling")) {
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

    /// <summary>
    /// Establecemos la gravedad del personaje ademas de detectar si el personaje esta en el suelo mediante una esfera no visible a los pies del personaje
    /// </summary>
    public void SetGravityGround()
    {
        isGrounded = Physics.Raycast(_cc.bounds.min, Vector3.down, 0.1f);


        Debug.Log("isGrounded es: " + isGrounded);
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

    /*#region FootIKSystem

    private void FixedUpdate()
    {
        if (!enableFootIk) { return; }
        if(_animator == null) { return; }

        //Ajustamos siempre la posicion de los pies
        AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

        //Lanzamos un rayo desde la posicion  de cada uno de los pies al suelo para encontrar las posiciones de las cosas
        FeetPositionSolver(rightFootPosition,ref rightFootIkPosition,ref rightFootIkRotation);
        FeetPositionSolver(leftFootPosition, ref leftFootIkPosition, ref leftFootIkRotation);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!enableFootIk) { return; }
        if (_animator == null) { return; }

        MovePelvisHeight();

        //Posicion ik y rotacion del pie derecho - utilizando las pro features
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

        if (useIkPro)
        {
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _animator.GetFloat(rightFootAnimVariableName));
        }
        MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIkPosition, rightFootIkRotation, ref lastRightFootPositionY);

        //Posicion ik y rotacion del pie izquierdo - utilizando las pro features
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

        if (useIkPro)
        {
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _animator.GetFloat(leftFootAnimVariableName));
        }
        MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIkPosition, leftFootIkRotation, ref lastLeftFootPositionY);
    }

    #region FootIkCalculos
        
    void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 positionIkHolder, Quaternion rotationIkHolder, ref float lastFootPositionY)
    {
        Vector3 targetIkPosition = _animator.GetIKPosition(foot);
        if(positionIkHolder != Vector3.zero)
        {
            targetIkPosition = this.transform.InverseTransformPoint(targetIkPosition);
            positionIkHolder = transform.InverseTransformPoint(positionIkHolder);

            float yVariableFoot = Mathf.Lerp(lastFootPositionY, positionIkHolder.y, ikPositionSpeed);
            targetIkPosition.y += yVariableFoot;

            lastFootPositionY = yVariableFoot;

            targetIkPosition = transform.TransformPoint(targetIkPosition);
            _animator.SetIKRotation(foot, rotationIkHolder);
        }

        _animator.SetIKPosition(foot, targetIkPosition);
    }

    private void MovePelvisHeight()
    {
        if (rightFootIkPosition == Vector3.zero || leftFootIkPosition == Vector3.zero || lastPelvisPositionY == 0)
        {
            lastPelvisPositionY = _animator.bodyPosition.y;
            return;
        }

        float leftOffsetPos = leftFootIkPosition.y - transform.position.y;
        float rightOffsetPos = rightFootIkPosition.y - transform.position.y;

        float totalOffset = (leftOffsetPos < rightOffsetPos) ? leftOffsetPos : rightOffsetPos;

        Vector3 newPelvisPosition = _animator.bodyPosition + Vector3.up * totalOffset;

        newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisSpeed);

        _animator.bodyPosition = newPelvisPosition;

        lastPelvisPositionY = _animator.bodyPosition.y;
    }

    private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIkPositions, ref Quaternion feetIkRotations)
    {
        //raycast handling section
        RaycastHit feetOutHit;
        if (showIkDebug)
            Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (raycastDownDistance + distanceFloorIK), Color.yellow);

        if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, raycastDownDistance + distanceFloorIK, groundMask))
        {
            feetIkPositions = fromSkyPosition;
            feetIkPositions.y = feetOutHit.point.y + pelvisOffSet;
            feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

            return;
        }

        feetIkPositions = Vector3.zero;
    }

    private void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
    {
        feetPositions = _animator.GetBoneTransform(foot).position;
        feetPositions.y = transform.position.y + distanceFloorIK;
    }

    #endregion

    #endregion*/


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(_cc.bounds.max, 0.2f);
    }

}
