using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [Header("Target")]
    [SerializeField] private Transform target; // Player transform

    [Header("Yükseklik ve Ofset")]
    [SerializeField] private float height = 15f;
    [SerializeField] private Vector2 planarOffset = Vector2.zero; // x/z ekseninde kaydırma gerekirse

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.2f;

    [Header("Zoom")]
    [SerializeField] private float zoomOutStepAmount = 2f;
    [SerializeField] private float zoomDuration = 1f;

    private Coroutine zoomRoutine;
    private Vector3 currentVelocity = Vector3.zero;

    private void Awake()
    {
        Instance = this;
    }

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

    public void ZoomOutStep()
    {
        float targetPlanarOffsetX = planarOffset.x - 1f; // x ekseninde kaydırma gerekirse
        float targetHeight = height + zoomOutStepAmount;
        float targetPlanarOffsetY = planarOffset.y - 1.5f;

        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);

        zoomRoutine = StartCoroutine(ZoomRoutine(new Vector3(targetPlanarOffsetX, targetHeight, targetPlanarOffsetY)));
    }

    private IEnumerator ZoomRoutine(Vector3 targetPoint)
    {
        float startHeight = height;
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / zoomDuration;
            height = Mathf.Lerp(startHeight, targetPoint.y, t);
            planarOffset.x = Mathf.Lerp(planarOffset.x, targetPoint.x, t);
            planarOffset.y = Mathf.Lerp(planarOffset.y, targetPoint.z, t);
            yield return null;
        }

        height = targetPoint.y;
        planarOffset.x = targetPoint.x;
        planarOffset.y = targetPoint.z;
        zoomRoutine = null;
    }
}