using UnityEngine;

public class GroundConstructor : MonoBehaviour
{
    [SerializeField] private GameObject groundPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = Vector3.zero;
        ConstructGround();
    }

    private void ConstructGround()
    {
        for(int i = -2; i < 3; i++)
        {
            for(int j = -2; j < 3; j++)
            {
                GameObject tile = Instantiate(groundPrefab, new Vector3(i*10f, 0f, j*10f), Quaternion.identity);
                tile.name = $"Tile_{i}_{j}";
                tile.transform.SetParent(transform);
            }
        }
    }
}
