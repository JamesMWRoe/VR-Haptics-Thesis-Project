using UnityEngine;

public class HapticInteractable : MonoBehaviour
{
    public float GetHapticValue(Vector3 contactPoint)
    {
        Debug.Log("Contact point: " + contactPoint);
        Vector3 contactPointInLocalSpace = transform.InverseTransformPoint(contactPoint);

        float hapticValue = contactPointInLocalSpace.z + 0.5f;

        return hapticValue;
    }

    public float GetHapticSpeed(Vector3 interactorVelocity)
    {

        Vector3 localVelocity = transform.InverseTransformVector(interactorVelocity);

        float hapticSpeed = localVelocity.z;


        return hapticSpeed;
    }
}
