using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public Vector3 rotationDegrees = new Vector3(0, 120, 0);

    public UnityEvent onRotationComplete;
    public UnityEvent onDesRotationComplete;

    public void RotateBy()
    {
        StopAllCoroutines();
        StartCoroutine(RotateRoutine(rotationDegrees, false));
    }

    public void UndoRotation()
    {
        StopAllCoroutines();
        StartCoroutine(RotateRoutine(-rotationDegrees, true));
    }

    private IEnumerator RotateRoutine(Vector3 rotationDeg, bool isUndo)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(rotationDeg);

        float angle = Quaternion.Angle(startRotation, targetRotation);
        float t = 0f;

        if (angle < 0.0001f)
        {
            transform.rotation = targetRotation;

            if (isUndo)
                onDesRotationComplete?.Invoke();
            else
                onRotationComplete?.Invoke();

            yield break;
        }

        while (t < 1f)
        {
            t += Time.deltaTime * (rotationSpeed / angle);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;

        if (isUndo)
            onDesRotationComplete?.Invoke();
        else
            onRotationComplete?.Invoke();
    }
}
