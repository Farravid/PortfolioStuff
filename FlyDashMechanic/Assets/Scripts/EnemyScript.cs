using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    ThirdPersonController third;

    void Start()
    {
        third = FindObjectOfType<ThirdPersonController>();
    }

    private void OnBecameVisible()
    {
        if (!third.targets.Contains(transform))
            third.targets.Add(transform);
    }

    private void OnBecameInvisible()
    {
        if (third.targets.Contains(transform))
            third.targets.Remove(transform);
    }
}