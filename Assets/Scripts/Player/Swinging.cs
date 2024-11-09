using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("References")]
    PlayerMovement playerScript;
    Rigidbody rb;
    LineRenderer lineRenderer;
    Transform gunTip;
    Transform cam;
    [SerializeField]
    Transform player;

    [SerializeField]
    public LayerMask grappleable;

    [Header("Prediction")]
    RaycastHit predictionHit;
    float predictionSphereCastRadius;
    public Transform predictionPoint;

    [Header("Swinging")]
    float swingDistance = 30f;
    Vector3 swingPoint;
    SpringJoint joint;
    float swingCooldown = 0.25f;
    float swingTimer;

    void Awake()
    {
        playerScript = GetComponent<PlayerMovement>();
        cam = playerScript.playerCamTransform;
        rb = GetComponent<Rigidbody>();

        lineRenderer = GetComponent<LineRenderer>();
        gunTip = playerScript.orientation;
    }

    void Update()
    {
        if (swingTimer > 0)
        {
            swingTimer -= Time.deltaTime;
        }

        if (UserInputs.inputREF.swingInput && swingTimer <= 0 && !GameManager.Instance.waitingForCountdown)
        {
            StartSwinging();
        }

        if (UserInputs.inputREF.notSwingInput)
        {
            StopSwinging();
        }

        CheckForSwingPoints();
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartSwinging()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, swingDistance, grappleable))
        {
            if (predictionHit.point == Vector3.zero) return;

            swingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float distFromPoint = Vector3.Distance(player.position, swingPoint);

            joint.maxDistance = distFromPoint * 0.8f;
            joint.minDistance = distFromPoint * 0.25f;

            joint.spring = 4f;
            joint.damper = 4f;
            joint.massScale = 4.5f;

            lineRenderer.positionCount = 2;
            currentGrapplePos = swingPoint;

            Vector3 directionToSwingPoint = (swingPoint - player.position).normalized;
            float angleToSwingPoint = Vector3.Angle(rb.velocity, directionToSwingPoint);

            float forceMultiplier = Mathf.Clamp01(1 - angleToSwingPoint / 90f);
            rb.AddForce(playerScript.orientation.transform.forward * 20f * forceMultiplier, ForceMode.Impulse);

            // Set the cooldown timer
            swingTimer = swingCooldown;
        }
    }

    public void StopSwinging()
    {
        lineRenderer.positionCount = 0;
        Destroy(joint);
    }

    private void CheckForSwingPoints()
    {
        if (joint != null) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out sphereCastHit, swingDistance, grappleable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward, out raycastHit, swingDistance, grappleable);

        Vector3 realHitPoint = raycastHit.point != Vector3.zero ? raycastHit.point : sphereCastHit.point;

        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    Vector3 currentGrapplePos;

    void DrawRope()
    {
        if (!joint) return;

        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, currentGrapplePos);
    }
}
