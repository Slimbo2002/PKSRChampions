using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
public class RebindControls : MonoBehaviour
{
    [SerializeField]
    InputActionReference inputAction;

    [SerializeField]
    bool excludeMouse = true;
    [Range(0, 10)]
    [SerializeField]
    int selectedBinding;
    [SerializeField]
    InputBinding.DisplayStringOptions displayStringOptions;

    [Header("BindingInfo")]
    [SerializeField]
    InputBinding inputBinding;
    int bindingIndex;

    string actionName;

    [Header("UI")]
    [SerializeField]
    TextMeshProUGUI actionText;
    [SerializeField] 
    Button rebindButton;
    [SerializeField]
    TextMeshProUGUI rebindText;
    [SerializeField]
    Button resetButton;
    private void OnEnable()
    {
        rebindButton.onClick.AddListener(() => DoRebind());
        resetButton.onClick.AddListener(() => DoReset());

        if(inputAction != null)
        {
            GetBindingInfo();
            InputManager.LoadBindingOverride(actionName);
            UpdateUI();
        }

        InputManager.rebindComplete += UpdateUI;

        InputManager.rebindCancelled += UpdateUI;
    }
    private void OnDisable()
    {
        InputManager.rebindComplete -= UpdateUI;
        InputManager.rebindCancelled -= UpdateUI;
    }
    private void OnValidate()
    {
        if(inputAction == null)
        {
            return;
        }
        GetBindingInfo();
        UpdateUI();

    }
    void GetBindingInfo()
    {
        if(inputAction.action!=null)
        {
            actionName= inputAction.action.name;
        }
            
        if(inputAction.action.bindings.Count > selectedBinding)
        {
            inputBinding = inputAction.action.bindings[selectedBinding];
            bindingIndex = selectedBinding;
        }
    }
    private void UpdateUI()
    {
        if(actionText!= null)
        {
            actionText.text = actionName;
        }
        if (rebindText != null)
        {
            if(Application.isPlaying)
            {
                rebindText.text = InputManager.GetBindingName(actionName, bindingIndex);
            }
            else
            {
                rebindText.text = inputAction.action.GetBindingDisplayString(bindingIndex);
            }
        }
    }
    void DoRebind()
    {
        InputManager.StartRebind(actionName, bindingIndex, rebindText, excludeMouse);
    }
    void DoReset()
    {
        InputManager.ResetBinding(actionName, bindingIndex);
        UpdateUI();
    }
}
