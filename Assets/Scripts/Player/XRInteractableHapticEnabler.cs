using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRInteractableHapticEnabler : MonoBehaviour
{

    XRGrabInteractable objectGrabInteractable;

    HandHapticHandler hapticHandler;

    [SerializeField] TextAsset hapticClip;

    void Start()
    {
        objectGrabInteractable = GetComponent<XRGrabInteractable>();
        objectGrabInteractable.selectEntered.AddListener(BeginHapticsOfInteractor);
        objectGrabInteractable.selectExited.AddListener(EndHapticsOfInteractor);
    }

    void BeginHapticsOfInteractor(SelectEnterEventArgs enterEventArgs)
    {
        XRBaseControllerInteractor interactor = (XRBaseControllerInteractor)enterEventArgs.interactorObject;

        if(interactor.xrController.transform.TryGetComponent<HandHapticHandler>(out hapticHandler))
        {
            hapticHandler.StartInteractionBasedHaptics(hapticClip);
        }
        else
        {
            Debug.Log("Could not find haptic handler. Please check a haptic handler is attached to the controller.");
        }
    }

    void EndHapticsOfInteractor(SelectExitEventArgs exitEventArgs)
    {
        if (hapticHandler != null)
        {
            hapticHandler.StopInteractionBasedHaptics();

            hapticHandler = null;
        }
    }
}
