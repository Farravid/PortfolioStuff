using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    protected static PlayerInput s_Instance;
    public static PlayerInput Instance => s_Instance;


    [HideInInspector]
    public bool playerControllerInputBlocked;

    protected Vector2 m_Movimiento;
    protected bool m_Jump;
    protected bool m_Run;

    public Vector2 MoveInput
    {
        get
        {
            if (playerControllerInputBlocked)
                return Vector2.zero;
            return m_Movimiento;
        }
    }

    public bool JumpInput => m_Jump && !playerControllerInputBlocked;
    public bool RunInput => m_Run && !playerControllerInputBlocked;

    private void Awake()
    {
        if (s_Instance == null)
            s_Instance = this;
        else if (s_Instance != this)
            throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " + s_Instance.name + " and " + name + ".");
    }

    private void Update()
    {
        m_Movimiento.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        m_Jump = Input.GetKeyDown(KeyCode.Space);
        m_Run = Input.GetKeyDown(KeyCode.LeftShift);
    }


}
