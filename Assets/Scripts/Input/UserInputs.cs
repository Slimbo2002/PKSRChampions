using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class UserInputs : MonoBehaviour
{
    public static UserInputs inputREF;

    public InputActions actions;

    InputAction moveAction, lookAction, jumpAction, sprintAction, crouchAction, swingAction, resetAction, pauseAction, anyAction;
    InputAction UINextTab, UIPrevTab;

    public Vector2 moveInput { get; private set; }
    public Vector2 lookInput { get; private set; }
    public bool jumpInput { get; private set; }
    public bool resetInput { get; private set; }
    public bool sprintInput { get; private set; }
    public bool crouchInput { get; private set; }
    public bool notCrouchInput { get; private set; }
    public bool swingInput { get; private set; }
    public bool notSwingInput { get; private set; }
    public bool pauseInput { get; private set; }
    public bool anyInput { get; private set; }


    public float jumpFloat { get; private set; }
    public float crouchFloat { get; private set; }


    public bool nextTabInput;
    public bool prevTabInput;


    private void Awake()
    {
        if (InputManager.inputActions == null)
        {
            InputManager.inputActions = new InputActions(); // Ensure it's initialized
        }

        if (inputREF == null)
        {
            inputREF = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        actions = InputManager.inputActions;

        if (actions == null)
        {
            Debug.LogError("InputActions not initialized in InputManager");
            return;
        }

        SetUpInputActions();
    }

    private void Update()
    {
        UpdateInputs();
    }

    void SetUpInputActions()
    {
        actions.Player.Enable();
        actions.UI.Enable();

        moveAction = actions.Player.WASD;
        lookAction = actions.Player.Look;
        jumpAction = actions.Player.Jump;
        sprintAction = actions.Player.Sprint;
        crouchAction = actions.Player.Crouch;
        swingAction = actions.Player.Swing;
        resetAction = actions.Player.Reset;
        pauseAction = actions.Player.Pause;
        anyAction = actions.Player.Any;

        UINextTab = actions.UI.NextTab;
        UIPrevTab = actions.UI.PrevTab;
    }

    void UpdateInputs()
    {

        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();
        jumpFloat = jumpAction.ReadValue<float>();
        crouchFloat = crouchAction.ReadValue<float>();
        jumpInput = jumpAction.WasPressedThisFrame();
        sprintInput = sprintAction.WasPressedThisFrame();
        crouchInput = crouchAction.WasPressedThisFrame();
        notCrouchInput = crouchAction.WasReleasedThisFrame();
        swingInput = swingAction.WasPressedThisFrame();
        notSwingInput = swingAction.WasReleasedThisFrame();
        resetInput = resetAction.WasPressedThisFrame();
        pauseInput = pauseAction.WasPressedThisFrame();
        anyInput= anyAction.WasPressedThisFrame();

        nextTabInput = UINextTab.WasPressedThisFrame();
        prevTabInput = UIPrevTab.WasPressedThisFrame();
    }
}
