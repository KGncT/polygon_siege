using UnityEngine;
using UnityEngine.UI;

public class HeartsActions : MonoBehaviour
{
    [SerializeField] private Image heartIconPrefab;    

    public void UpdateHearts(int lives)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        for(int i=0; i < lives; i++)
        {
            Instantiate(heartIconPrefab, transform);
        }
    }
}
