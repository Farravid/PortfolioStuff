using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System;

public class PlayerController : MonoBehaviourPunCallbacks
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

    //Time.deltatime
    private float delta;

    //Charactaer controller
    private CharacterController _cc;

    //Vector al cual vamos a querer que se mueva nuestro personaje
    private Vector3 desiredMovement;

    //Minimo para las rotaciones cuando nos movemos
    private float minimoRotacion = 0.1f;

    //Velocidad de nuestros inputs para saber si estamso en movimiento o no
    private float speedInputs;

    //Animator
    private Animator _animator;


    //Photon things
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    [Tooltip("Camera de cada jugador")]
    public GameObject playerCamera;

    [SerializeField] private GameObject chatInGame;




    #endregion

    private void Awake()
    {
        if (photonView.IsMine)
        {
            PlayerController.LocalPlayerInstance = this.gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        //Para las camaras del cinemamachine de cada uno, la verdad es la hostia
        if (photonView.IsMine)
        {
            Debug.Log("Nombre del jugador: "+photonView.Owner.NickName);
            playerCamera.SetActive(true);
        }
        else{
            playerCamera.SetActive(false);
        }

        //Recogemos el chat
        chatInGame = GameObject.Find("ChatManager");
        chatInGame.SetActive(false);

    }

    void Update()
    {

        delta = Time.deltaTime;
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
            return;


        //Es decir solo permitiremos el movimiento cuando el chat no este activo
        if (!chatInGame.activeInHierarchy)
        {
            MagnitudInput();
            SetGravityGround();
        }
        ControlarChatPhoton();
    }


    #region PlayerMovementAndGravity

    public void MagnitudInput()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        speedInputs = new Vector2(InputX, InputZ).sqrMagnitude;
        if (speedInputs > minimoRotacion)
        {
            PlayerInput(true);
        }
        else
        {
            PlayerInput(false);
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

        _animator.SetFloat("velocidad", desiredMovement.magnitude);

        

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

    }


    #endregion

    #region Photon player

    private void ControlarChatPhoton()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            chatInGame.SetActive(true);
            _animator.SetFloat("velocidad", 0);

        }
        //MANEJAR EL ESCAPE ESTE TAMBIEN CON EL ESCAPE DEL MENU DE PAUSA
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            chatInGame.SetActive(false);
        }
    }

    #endregion

}
