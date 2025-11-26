
using UnityEngine;
using System;

public class CannonController : MonoBehaviour
{
    [Header("Impact Prefabs")]
    public GameObject hitEffect;
    public GameObject missEffect;

    public void PlayShot(Vector3 worldPos, bool hit)
    {
        GameObject prefab = hit ? hitEffect : missEffect;
        if (prefab == null) return;

        var fx = Instantiate(prefab, worldPos, Quaternion.identity);
        Destroy(fx, 2f);
    }
}
