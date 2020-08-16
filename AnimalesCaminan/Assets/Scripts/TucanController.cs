using UnityEngine;

public class TucanController : MonoBehaviour
{
    //Inputs
    private float inputHorizontal;
    private float inputVertical;

    [SerializeField]
    private float velocidadTucan;

    private Vector3 m_CurrentRotation;
    private bool m_AbleToGoesDown;
    private bool m_AbleToGoesStraight;

    [SerializeField]
    private LayerMask groundMask;

    private void Update()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");

        //-70 maximo 42 minimo x axis

        transform.position += transform.forward * Time.deltaTime * velocidadTucan;

        SetTucanRotation();
        DistanceEnvironment();


    }

    private void DistanceEnvironment()
    {
        //Raycast hacia el suelo
        Ray rayDown = new Ray(transform.position, Vector3.down);
        RaycastHit raycastHitDown;
        if (Physics.Raycast(rayDown, out raycastHitDown, 10f, groundMask))
            m_AbleToGoesDown = false;
        else
            m_AbleToGoesDown = true;

        Debug.Log("Can Down: "+m_AbleToGoesDown);

        //Raycast hacia delante
        Ray rayForward = new Ray(transform.position, Vector3.forward);
        RaycastHit raycastHitForward;
        if (Physics.Raycast(rayForward, out raycastHitForward, 15f, groundMask))
            m_AbleToGoesStraight = false;
        else
            m_AbleToGoesStraight = true;

        Debug.Log("Can Straight: "+m_AbleToGoesStraight);

    }

    private void SetTucanRotation()
    {
        m_CurrentRotation = transform.localRotation.eulerAngles;

        m_CurrentRotation.x = m_CurrentRotation.x > 180f ? -(360 - m_CurrentRotation.x) : m_CurrentRotation.x;
        m_CurrentRotation.z = m_CurrentRotation.z > 180f ? -(360 - m_CurrentRotation.z) : m_CurrentRotation.z;

        if (m_CurrentRotation.x <= -70f)
            m_CurrentRotation.x = -70f;
        else if (m_CurrentRotation.x > 42f)
            m_CurrentRotation.x = 42f;

        if (m_CurrentRotation.z <= -45f)
            m_CurrentRotation.z = -45f;
        else if (m_CurrentRotation.z > 45f)
            m_CurrentRotation.z = 45f;


        if(m_AbleToGoesDown && m_AbleToGoesStraight)
        {
            transform.localRotation = Quaternion.Euler(m_CurrentRotation);
            transform.Rotate(inputVertical + Mathf.Abs(inputHorizontal), 0f, -1f * inputHorizontal);
        }else if (!m_AbleToGoesDown)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(-10f, transform.rotation.y, 0f)), 5f * Time.deltaTime);
        }else if (!m_AbleToGoesStraight)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(-70f, transform.rotation.y, 0f)), 5f * Time.deltaTime);
        }


    }
}
