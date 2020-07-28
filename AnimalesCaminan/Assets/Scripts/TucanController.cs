using UnityEngine;

public class TucanController : MonoBehaviour
{
    //Inputs
    private float inputHorizontal;
    private float inputVertical;

    private void Update()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");

        //-70 maximo 42 minimo

        //transform.position += transform.forward * Time.deltaTime * 2f;

        Debug.Log(transform.rotation.x);

        if(transform.rotation.x >= -0.52f && transform.rotation.x <= 0.6f)
        {
            transform.Rotate(inputVertical, 0.0f, -1f * inputHorizontal);

        }
    }
}
