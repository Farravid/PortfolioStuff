using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformadoresCap : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }
}
