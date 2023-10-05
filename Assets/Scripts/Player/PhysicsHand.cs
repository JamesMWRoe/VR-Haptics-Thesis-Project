using System;
using UnityEngine;

public class PhysicsHand : MonoBehaviour
{

    Rigidbody rigidBody;
    [SerializeField] Transform virtualHandTransform;
    [SerializeField] Rigidbody playerRigidBody;

    [SerializeField] HandHapticHandler hapticHandler;
    [SerializeField] HapticStreamSurfaceHandler hapticStreamHandler;

    float frequency;
    float damping = 1;

    float rotationFrequency = 100f;
    float rotationDamping = 0.9f;

    bool isHapticContactMade = false;

    public Vector3 GetVelocity()
    {   return rigidBody.velocity;   }

    void Awake()
    {
        frequency = 1/Time.fixedDeltaTime;
        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        PDMotion();
        PDRotation();
    }

    void OnCollisionEnter(Collision collision)
    {
        StartContactBasedHaptics(collision);
        StartStreamHaptics(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        StopContactBasedHaptics(collision);
        StopStreamHaptics(collision);
    }

    //based on a paper by David Wu back in 2003
    void PDMotion()
    {
        float springConstant = (6*frequency)*(6*frequency) * 0.25f;
        float damperConstant = 4.5f * frequency * damping;
        float g = 1 / (1 + damperConstant*Time.fixedDeltaTime + springConstant*Time.fixedDeltaTime*Time.fixedDeltaTime);
        float gSpringConstant = springConstant * g;
        float gDamperConstant = (damperConstant + springConstant*Time.fixedDeltaTime)*g;
        Vector3 forceToBeApplied = (virtualHandTransform.position - rigidBody.position)*gSpringConstant + ( - rigidBody.velocity)*gDamperConstant;

        rigidBody.AddForce(forceToBeApplied, ForceMode.Acceleration);
    }

    void PDRotation()
    {
        float springConstant = (6*rotationFrequency)*(6*rotationFrequency) * 0.25f;
        float damperConstant = 4.5f * rotationFrequency * rotationDamping;
        float g = 1 / (1 + damperConstant*Time.fixedDeltaTime + springConstant*Time.fixedDeltaTime*Time.fixedDeltaTime);
        float gSpringConstant = springConstant * g;
        float gDamperConstant = (damperConstant + springConstant*Time.fixedDeltaTime)*g;

        Quaternion rotationalError = virtualHandTransform.rotation*Quaternion.Inverse(transform.rotation);

        rotationalError.ToAngleAxis(out float angle, out Vector3 axis);
        axis.Normalize();
        axis *= Mathf.Deg2Rad;
        Vector3 torqueToBeApplied = gSpringConstant * axis * angle + (-rigidBody.angularVelocity)*gDamperConstant;

        rigidBody.AddTorque(torqueToBeApplied, ForceMode.Acceleration);
    }

    void StartContactBasedHaptics(Collision collision)
    {
        DateTime dateTimeAtBeginning = DateTime.Now;

        if (!IsCollisionWithHapticEnabledObject(collision))
        {   return;   }
        if (isHapticContactMade)
        {   return;   }

        isHapticContactMade = true;

        Debug.Log("Touched haptic enabled object");
        Transform objectToCheck = collision.transform;

        MaterialHandler materialHandler;
        VariableMaterialHandler variableMaterialHandler = null;
        while(!objectToCheck.TryGetComponent<MaterialHandler>(out materialHandler))
        {
            if (objectToCheck.TryGetComponent<VariableMaterialHandler>(out variableMaterialHandler))
            {
                break;
            }
            objectToCheck = objectToCheck.parent;
        }

        DateTime dateTimeAfterMaterialHandlerCheck = DateTime.Now;
        TimeSpan timeToCheckForMaterialHandler = dateTimeAfterMaterialHandlerCheck - dateTimeAtBeginning;
        Debug.Log("Time to check for material handler: " + timeToCheckForMaterialHandler.TotalMilliseconds);

        if(materialHandler != null)
        {   hapticHandler.StartHapticContact(materialHandler.hapticMaterial, collision.impulse.magnitude);   }
        else
        {   hapticHandler.StartHapticContact(variableMaterialHandler.hapticMaterial, collision.impulse.magnitude);   }

        DateTime dateTimeAfterStartingHapticContact = DateTime.Now;
        TimeSpan timeToStartHapticContact = dateTimeAfterStartingHapticContact - dateTimeAfterMaterialHandlerCheck;
        Debug.Log("Time to start haptic contact: " + timeToStartHapticContact.TotalMilliseconds);
    }

    void StopContactBasedHaptics(Collision collision)
    {
        if (!IsCollisionWithHapticEnabledObject(collision))
        {   return;  }

        if(!isHapticContactMade)
        {   return;   }

        isHapticContactMade = false;

        hapticHandler.StopHapticContact();
    }

    void StartStreamHaptics(Collision collision)
    {
        if (!IsCollisionWithHapticStreamObject(collision))
        {   return;  }

        if (isHapticContactMade)
        {   return;   }

        isHapticContactMade = true;

        ContactPoint contactPoint = collision.GetContact(0);

        HapticInteractable interactablesHapticInfo = collision.transform.GetComponent<HapticInteractable>();

        hapticStreamHandler.StartHapticStream(contactPoint.point, interactablesHapticInfo, rigidBody);
    }

    void StopStreamHaptics(Collision collision)
    {
        if (!IsCollisionWithHapticStreamObject(collision))
        {   return;  }

        if (!isHapticContactMade)
        {   return;   }

        isHapticContactMade = false;

        hapticStreamHandler.StopHapticStream();
    }

    bool IsCollisionWithHapticEnabledObject(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {    return true;    }

        return false;
    }

    bool IsCollisionWithHapticStreamObject(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {    return true;    }

        return false;
    }

}
