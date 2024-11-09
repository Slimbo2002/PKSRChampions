using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] Animator anim;
    PlayerMovement movement;

    private bool lastSprinting;
    private bool lastJumping;
    private bool lastSliding;
    private bool lastCrouching;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        UpdateAnimatorState("Sprinting", movement.sprinting, ref lastSprinting);
        UpdateAnimatorState("Slide", movement.isSliding, ref lastSliding);
        UpdateAnimatorState("Crouch", movement.isCrouching, ref lastCrouching);
    }

    // Helper method to avoid repetitive SetBool calls and check for state changes
    private void UpdateAnimatorState(string parameter, bool currentState, ref bool lastState)
    {
        if (currentState != lastState)
        {
            anim.SetBool(parameter, currentState);
            lastState = currentState;
        }
    }
}
