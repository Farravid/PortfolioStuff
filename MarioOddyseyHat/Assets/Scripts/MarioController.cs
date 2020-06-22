﻿using System.Collections;
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
    public float rotateCapSpeed = 1.0f;
    public float duracionHoldCap = 6f;
    private bool isThrowed = false;
    private bool isSpinning = false;
    private bool vueltaGorroUpdate = false;
    private float timeMaximoVuelta;

    //Transicion a otro target
    private GameObject targetCambiar;
    private bool transicionOn;



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

        disolveHatTarget.SetFloat("Vector1_5A2F4A5", 1);
        disolverEyesTarget.SetFloat("Vector1_7527672C", 1);
        disolveRingTarget.SetFloat("Vector1_7AE198B8", 1);


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
            //Ponemos los materiales para que el gorro de la sensacion de que se disuelve
            cambiarMateriales = true;
            padreCapMano.gameObject.SetActive(true);
        }

    }

    public void AlphaMateriales()
    {

        if (cambiarMateriales)
        {
            //El gorro de la cabeza
            cambioTransparente += 0.7f;
            disolveHatCabeza.SetFloat("Vector1_5A2F4A5", cambioTransparente/100f);
            disolveEyesCabeza.SetFloat("Vector1_7527672C", cambioTransparente / 100f);
            disolveRingCabeza.SetFloat("Vector1_7AE198B8", cambioTransparente / 100f);

            //El de lanzar
            cambioOpaco -= 1f;
            disolveHatLanzar.SetFloat("Vector1_5A2F4A5", cambioOpaco / 100f);
            disolveEyesLanzar.SetFloat("Vector1_7527672C", cambioOpaco / 100f);
            disolveRingLanzar.SetFloat("Vector1_7AE198B8", cambioOpaco / 100f);

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

    }

    public void SetRotacionGorro()
    {
        if (isSpinning)
        {
            capLanzar.transform.Rotate(0f, rotateCapSpeed, 0f);
        }
    }

    public void VueltaGorro()
    {
        vueltaGorroUpdate = true;
        if (!Input.GetMouseButton(0))
        {
            capLanzar.GetComponent<SphereCollider>().enabled = false;
            capLanzar.parent = padreCapCabeza.transform;
            isSpinning = false;
            capLanzar.transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() => ResetLanzamientoSombrero());
            capLanzar.transform.DOLocalRotate(Vector3.zero, 0.5f);
            vueltaGorroUpdate = false;
        }
        else if(Time.time > timeMaximoVuelta){
            capLanzar.GetComponent<SphereCollider>().enabled = false;
            capLanzar.parent = padreCapCabeza.transform;
            isSpinning = false;
            capLanzar.transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() => ResetLanzamientoSombrero());
            capLanzar.transform.DOLocalRotate(Vector3.zero, 0.5f);
            vueltaGorroUpdate = false;
        }

        

    }

    public void ResetLanzamientoSombrero()
    {
        //Reseteamos los colores del material
        disolveHatCabeza.SetFloat("Vector1_5A2F4A5",0f);
        disolveEyesCabeza.SetFloat("Vector1_7527672C", 0f);
        disolveRingCabeza.SetFloat("Vector1_7AE198B8",0f);


        capLanzar.transform.parent = padreCapMano;
        capLanzar.transform.parent.gameObject.SetActive(false);
        capLanzar.transform.localPosition = Vector3.zero;
        capLanzar.transform.eulerAngles = Vector3.zero;
        isThrowed = false;
    }

    public void JumpVoltereta()
    {
        velocityGravity.y = Mathf.Sqrt(1.8f * -2f * gravity);
        capLanzar.GetComponent<SphereCollider>().enabled = false;
        capLanzar.parent = padreCapCabeza.transform;
        isSpinning = false;
        capLanzar.transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() => ResetLanzamientoSombrero());
        capLanzar.transform.DOLocalRotate(Vector3.zero, 0.5f);
        vueltaGorroUpdate = false;
    }

    public void TransicionTransformarse()
    {
        if (transicionOn)
        {
            //El gorro de lanzar
            cambioTransparente += 1f;
            disolveHatLanzar.SetFloat("Vector1_5A2F4A5", cambioTransparente / 100f);
            disolveEyesLanzar.SetFloat("Vector1_7527672C", cambioTransparente / 100f);
            disolveRingLanzar.SetFloat("Vector1_7AE198B8", cambioTransparente / 100f);

            cineCameraMario.Priority = 0;
            cineCameraTarget.Priority = 1;

            //El de cada target
            cambioOpaco -= 0.5f;
            disolveHatTarget.SetFloat("Vector1_5A2F4A5", cambioOpaco / 100f);
            disolverEyesTarget.SetFloat("Vector1_7527672C", cambioOpaco / 100f);
            disolveRingTarget.SetFloat("Vector1_7AE198B8", cambioOpaco / 100f);


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

    #endregion


}

