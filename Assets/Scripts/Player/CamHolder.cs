using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class CamHolder : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    Transform orientation;
    PlayerMovement playerScript;

    //Rotation and look
    private float xRotation;
    private float sensitivity = 1f;
    private float sensMultiplier = 2f;
    private float desiredX;

    float rotate;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = player.GetComponent<PlayerMovement>();
        orientation = playerScript.orientation;
    }

    // Update is called once per frame  
    void Update()
    {
        transform.position = playerScript.playerHead.transform.position;
        if (!GameManager.Instance.isPaused && !GameManager.Instance.waitingForCountdown)
        {
            Look();
        }
        
    }

    private void Look()
    {
        float mouseX = GetPlayerLook().x * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = GetPlayerLook().y * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        

        //Find current look rotation
        Vector3 rot = playerScript.playerCamTransform.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerScript.playerCamTransform.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
        player.transform.localRotation = Quaternion.Euler(0, desiredX, 0);

    }
    public Vector2 GetPlayerLook()
    {
        return UserInputs.inputREF.lookInput;
    }
    public void DoFov(int endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }
    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }

}
