using UnityEngine;

public class WeaponPositioner : MonoBehaviour
{
    [SerializeField] private Transform weapon;
    private Transform weaponPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // weaponPoint = transform.Find("weapon-position");
        weaponPoint = FindDeepChild(transform, "weapon-position");
        if(weaponPoint == null)
        {
            Debug.LogError("WeaponPosition child not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(weapon != null && weaponPoint != null)
        {
            weapon.position = weaponPoint.position;
            weapon.rotation = weaponPoint.rotation;
        }
    }


    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}
