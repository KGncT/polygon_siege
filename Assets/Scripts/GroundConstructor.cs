using UnityEngine;

public class GroundConstructor : MonoBehaviour
{
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private Material[] groundMaterials;

    public void ConstructGround()
    {
        int rndMaterialIndex = Random.Range(0, groundMaterials.Length);
        groundPrefab.GetComponent<MeshRenderer>().material = groundMaterials[rndMaterialIndex];

        for(int i = -4; i < 5; i++)
        {
            for(int j = -4; j < 5; j++)
            {
                GameObject tile = Instantiate(groundPrefab, new Vector3(i*10f, 0f, j*10f), Quaternion.identity);
                tile.name = $"Tile_{i}_{j}";
                tile.transform.SetParent(transform);
            }
        }
    }
}
