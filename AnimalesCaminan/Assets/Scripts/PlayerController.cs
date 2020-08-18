using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]


public class PlayerController : MonoBehaviour
{
    private Vector3 m_VectorVertical;
    private float m_AnglesDifToTurn;
    private Vector3 m_DesiredMovement;
    private bool m_IsGrounded;
    private float m_CurrentSpeed;
    [SerializeField]
    private float gravity;
    [SerializeField]
    private float maxSpeed;

    [SerializeField]
    private LayerMask groundMask;


    private CharacterController m_CharControl;
    private Animator m_Animator;
    private PlayerInput m_PlayerInput;

    readonly int m_HashIsGrounded = Animator.StringToHash("IsGrounded");
    readonly int m_HashMovementSpeed = Animator.StringToHash("MovementSpeed");
    readonly int m_HashAnguloGiro = Animator.StringToHash("AnguloGiro");




    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        m_CharControl = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
        m_PlayerInput = GetComponent<PlayerInput>();

        gravity = -9.81f;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        SetGravityAndGround();
        CalcularMovimiento();
        AdicionalMovements();
        UpdateAnimator();
    }

    private void AdicionalMovements()
    {
        Run();
    }

    private void Run()
    {
        if (m_PlayerInput.RunInput && maxSpeed == 3f)
            maxSpeed = 6f;
        else if (m_PlayerInput.RunInput && maxSpeed == 6f)
            maxSpeed = 3f;
    }

    private void UpdateAnimator()
    {
        m_Animator.SetBool(m_HashIsGrounded, m_IsGrounded);
        m_Animator.SetFloat(m_HashMovementSpeed, m_CurrentSpeed);
        m_Animator.SetFloat(m_HashAnguloGiro, m_AnglesDifToTurn);
    }

    private void CalcularMovimiento()
    {
        if (m_PlayerInput.MoveInput.sqrMagnitude > 1f)
            m_PlayerInput.MoveInput.Normalize();

        if (IsMoveInput)
            m_CurrentSpeed = Mathf.SmoothStep(m_CurrentSpeed, maxSpeed, 12f * Time.deltaTime);
        else
            m_CurrentSpeed = Mathf.SmoothStep(m_CurrentSpeed, 0f, 12f * Time.deltaTime);
    }

    private void SetGravityAndGround()
    {
        if (!m_IsGrounded)
            m_VectorVertical.y += gravity * Time.deltaTime;
        else
            m_VectorVertical.y = -2f;

        m_CharControl.Move(m_VectorVertical * Time.deltaTime);
    }

    private void PlayerMovement()
    {
        bool rotationAllowed = (m_PlayerInput.MoveInput.sqrMagnitude > 0.1f) ? true : false;

        //Clmaping magnitude
        Vector3 playerInput;
        playerInput = Vector3.ClampMagnitude(new Vector3(m_PlayerInput.MoveInput.x, 0f, m_PlayerInput.MoveInput.y), 1);

        Vector3 camForw = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForw.y = 0f;
        camRight.y = 0f;

        camForw.Normalize();
        camRight.Normalize();

        m_DesiredMovement = playerInput.x * camRight + playerInput.z * camForw;
        m_DesiredMovement = m_DesiredMovement * m_CurrentSpeed;

        //Calculamos el angulo de giro para activar o no la animacion.
        m_AnglesDifToTurn = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(m_DesiredMovement));

        if (Math.Abs(m_PlayerInput.MoveInput.x) > Math.Abs(m_PlayerInput.MoveInput.y))
            m_AnglesDifToTurn = m_PlayerInput.MoveInput.x > 0 ? m_AnglesDifToTurn * 1 : m_AnglesDifToTurn * -1;
        else
            m_AnglesDifToTurn = m_PlayerInput.MoveInput.y > 0 ? m_AnglesDifToTurn * 1 : m_AnglesDifToTurn * -1;

        if (!IsMoveInput)
            m_AnglesDifToTurn = 0f;

        if(Mathf.Abs(m_AnglesDifToTurn) < 120 && rotationAllowed)
            transform.rotation = Quaternion.Slerp(transform.rotation, (Quaternion.LookRotation(m_DesiredMovement)), 5f * Time.deltaTime);

    }

    // Detecta si el jugador esta moviendose o no
    private bool IsMoveInput => !Mathf.Approximately(m_PlayerInput.MoveInput.sqrMagnitude, 0);
}
