using UnityEngine;

public class CameraRelativeInput : MonoBehaviour
{
    private Transform cameraTransform;

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }


    public Vector3 Convert(Vector2 input)
    {
        if (cameraTransform == null)
            return new Vector3(input.x, 0f, input.y);

        Vector3 fwd = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        fwd.y = 0f;
        right.y = 0f;
        fwd.Normalize();
        right.Normalize();

        return right * input.x + fwd * input.y;
    }
}