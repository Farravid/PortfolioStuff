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

    [Header("Mario hat mechanic")]
    public Transform cap;
    public Transform pivotCap;
    public Transform padreCap;
    public float rotateCapSpeed = 1.0f;
    private bool isThrowed = false;
    private bool isSpinning = false;

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

    }


    void Update()
    {
        delta = Time.deltaTime;
        MagnitudInput();
        SetGravityGround();
        LanzarGorroAnim();
        SetRotacionGorro();
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
    public void LanzarGorroAnim()
    {
        if (Input.GetMouseButtonDown(0) && !isThrowed)
        {
            isThrowed = true;
            _animator.SetTrigger("lanzar");
        }

    }

    public void LanzarGorroMove()
    {
        cap.transform.parent = null;
        cap.transform.DOMove(pivotCap.position, 0.5f);
        isSpinning = true;
        cap.transform.DORotate(new Vector3(18f, 0f, 368f), 0.1f);
        Invoke("VueltaGorro", 3f);
    }

    public void SetRotacionGorro()
    {
        if (isSpinning)
        {
            cap.transform.Rotate(0f, rotateCapSpeed, 0f);
        }
    }

    public void VueltaGorro()
    {
        cap.parent = padreCap.transform;
        isSpinning = false;
        cap.transform.DOLocalMove(Vector3.zero, 0.5f);
        cap.transform.DOLocalRotate(Vector3.zero, 0.5f);
    }

    #endregion


}

