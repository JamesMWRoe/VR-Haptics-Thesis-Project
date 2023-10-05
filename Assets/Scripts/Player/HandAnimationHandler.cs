using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimationHandler : MonoBehaviour
{
    [SerializeField] Animator handAnimator;

    [SerializeField] InputActionProperty pinchTriggerAction;
    [SerializeField] InputActionProperty squeezeTriggerAction;

    void Update()
    {
        handAnimator.SetFloat("PinchFloat", pinchTriggerAction.action.ReadValue<float>());
        handAnimator.SetFloat("SqueezeFloat", squeezeTriggerAction.action.ReadValue<float>());
    }
}
