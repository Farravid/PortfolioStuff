using System;
using UnityEngine;

public class TucanController : MonoBehaviour
{
    //Inputs
    private float inputHorizontal;
    private float inputVertical;

    [SerializeField]
    private float velocidadTucan;
    [SerializeField]
    private float smoothResetRotation;

    private Vector3 m_CurrentRotation;
    private bool m_AbleToGoesDown;
    private bool m_AbleToGoesStraight;

    [SerializeField]
    private LayerMask limiteTucanLayer;

    private void Update()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");

        //Debug.Log(transform.forward.y);

        Debug.Log(velocidadTucan);
        transform.position += transform.forward * Time.deltaTime * velocidadTucan;

        VelocidadHuracan();
        SetTucanRotation();
        DistanceGround();


    }

    private void VelocidadHuracan()
    {
        //if(Time.time %2 == 0)
        velocidadTucan -= transform.forward.y * velocidadTucan * Time.deltaTime;
        if (velocidadTucan > 20f)
            velocidadTucan = 20f;
        if (m_CurrentRotation.x < 0f && velocidadTucan < 1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(50f, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z)), smoothResetRotation * Time.deltaTime);
    }

    private void DistanceGround()
    {
        //Raycast hacia el suelo
        Ray rayDown = new Ray(transform.position, Vector3.down);
        RaycastHit raycastHitDown;
        if (Physics.Raycast(rayDown, out raycastHitDown, 10f, limiteTucanLayer))
            m_AbleToGoesDown = false;
        else
            m_AbleToGoesDown = true;
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


        if (m_AbleToGoesDown)
        {
            transform.localRotation = Quaternion.Euler(m_CurrentRotation);
            if(inputHorizontal != 0)
                transform.Rotate(inputVertical, inputHorizontal/3f, -1f * inputHorizontal);
            else
                transform.Rotate(inputVertical, 0f, -1f * inputHorizontal);
        }
        else if (!m_AbleToGoesDown)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(-10f, transform.localRotation.eulerAngles.y, 0f)), smoothResetRotation * Time.deltaTime);
        }
    }
}
