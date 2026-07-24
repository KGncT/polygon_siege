using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // Player transform

    [Header("Yükseklik ve Ofset")]
    [SerializeField] private float height = 15f;
    [SerializeField] private Vector2 planarOffset = Vector2.zero; // x/z ekseninde kaydırma gerekirse

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.2f;

    private Vector3 currentVelocity = Vector3.zero;

    private void Start()
    {
        if (target != null)
        {
            transform.position = GetDesiredPosition();
        }

        // Tam tepeden, sabit kuşbakışı açı
        transform.eulerAngles = new Vector3(65f, 0f, 0f);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = GetDesiredPosition();
        transform.position = Vector3.SmoothDamp(
            transform.position, desiredPosition, ref currentVelocity, smoothTime);
    }

    private Vector3 GetDesiredPosition()
    {
        return new Vector3(
            target.position.x + planarOffset.x,
            height,
            target.position.z + planarOffset.y);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}