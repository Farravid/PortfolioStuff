/*using FIMSpace.FLook;
using FIMSpace.GroundFitter;
using UnityEngine;

public class ComodoMovement : FGroundFitter_MovementLook
{
    [Tooltip("Layer mask con la que el ray de la cabeza para los obstaculos va a colisionar")]
    public LayerMask lookLayerMask = 1<<0;
    private Vector3 targetLookPos;
    private float currentTargetLookRotationOffset;

    public Transform boneInicioRayCastObstaculoHead;

    private FLookAnimator lookAnimator;

    protected override void Start()
    {
        base.Start();
        lookAnimator = GetComponent<FLookAnimator>();
        currentTargetLookRotationOffset = lookAnimator.RotationOffset.x;
    }

    protected override void Update()
    {
        base.Update();

        if (targetOfLook)
        {
            targetOfLook.position = Vector3.Lerp(targetOfLook.position, targetLookPos, Time.deltaTime * 8f);
        }
        //90 cabeza agachada - 35 cabeza para arriba en el rotation offset de la x

        RaycastHit lookhit;
        Ray lookRay = new Ray(boneInicioRayCastObstaculoHead.position, Quaternion.Euler(0f,fitter.UpAxisRotation, 0f) * Vector3.forward);
        //Debemos calcular la distancia para colisionar del rayo mas o menos a ojo desde el inspector
        //39.9 40.2 el rayo es de una longitud de 0,3
        Physics.Raycast(lookRay, out lookhit, 1.5f ,lookLayerMask, QueryTriggerInteraction.Ignore);
        if (lookhit.transform)
        {
            float dist = Vector3.Distance(lookRay.origin, lookhit.point);
            currentTargetLookRotationOffset = Mathf.Lerp(currentTargetLookRotationOffset , Mathf.Lerp(95f, 50f, Mathf.InverseLerp(1.5f, 0.75f, dist)), Time.deltaTime * 4f);
        }
        else
        {
            currentTargetLookRotationOffset = Mathf.Lerp(currentTargetLookRotationOffset, 95f, Time.deltaTime * 2.5f);
        }

        lookAnimator.RotationOffset = new Vector3(currentTargetLookRotationOffset, 0f, 0f);

    }

    protected override void SetLookAtPosition(Vector3 tPos)
    {
        targetLookPos = tPos;
    }
}*/
