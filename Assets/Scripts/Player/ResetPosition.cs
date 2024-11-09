using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    public Transform spawnPos;

    bool resetting;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -50f)
        {
            ResetPos();
        }

        if (UserInputs.inputREF.resetInput)
        {
            ResetPos();
        }
    }

    public void ResetPos()
    {
        SetPos();

        // Restart the timer
        GameManager.Instance.RestartTimer();

        StartCoroutine(GameManager.Instance.timing.StartCountdown());

    }

    public void SetPos()
    {
        
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero; // Also reset angular velocity

        // Temporarily disable physics
        rb.isKinematic = true;

        // Stop swinging logic
        if (GetComponent<Swinging>().isActiveAndEnabled)
        {
            GetComponent<Swinging>().StopSwinging();
        }


        // Reset position using Rigidbody
        rb.position = spawnPos.position;

        // Reset rotation
        rb.rotation = spawnPos.rotation;

        // Reset camera rotation
        GetComponent<PlayerMovement>().playerCamTransform.gameObject.transform.rotation = spawnPos.rotation;

        // Re-enable physics
        rb.isKinematic = false;
    }
}
