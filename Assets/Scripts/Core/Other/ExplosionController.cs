using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    [Header("Dirt,Generic,Glass,Lava,Leaves,Stone,Water,Wood")]
    public AudioClip[] ExplosionVariants;

    public GameObject ExplosionPrefab;

    public static ExplosionController Instance;

    void Start()
    {
        Instance = this;
    }

    public void SpawnExplosionEffect(int x, int y, int z, Constants.Blocks explosionType)
    {
        GameObject explosion = Instantiate(ExplosionPrefab, new Vector3(x, y, z), Quaternion.identity);
        explosion.GetComponent<AudioSource>().PlayOneShot(GetExplosionType(explosionType));
        Destroy(explosion, 2f);
    }

    public AudioClip GetExplosionType(Constants.Blocks explosionType)
    {
        return ExplosionVariants[(int)explosionType];
    }

    void Update()
    {

    }
}
