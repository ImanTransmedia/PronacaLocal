using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public Vector3 rotationDegrees = new Vector3(0, 120, 0);

    public UnityEvent onRotationComplete;

    public void RotateBy()
    {
        StopAllCoroutines();
        StartCoroutine(RotateRoutine(rotationDegrees));
    }

    private IEnumerator RotateRoutine(Vector3 rotationDeg)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(rotationDeg);

        float angle = Quaternion.Angle(startRotation, targetRotation);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * (rotationSpeed / angle);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
        onRotationComplete?.Invoke();
    }
}
