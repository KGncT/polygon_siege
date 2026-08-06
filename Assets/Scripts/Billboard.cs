using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Kameranın rotasyonunu direkt kopyala (en stabil yöntem)
        transform.rotation = cam.rotation;
    }
}