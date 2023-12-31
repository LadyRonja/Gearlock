using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DynomiteProjectile : Projectile
{
    [Header("Dynamite Specifics")]
    public int explosionRadius = 2;
    public int explosionDamage = 3;
    [Space]
    public GameObject explosionPrefab;
    [SerializeField] AudioClip dynamiteTssSound;

    private void Start()
    {
        AudioSource mySource = GetComponent<AudioSource>();
        mySource.clip = dynamiteTssSound;
        mySource.loop = true;
        if (Scenehandler.Instance != null)
            mySource.volume = Scenehandler.Instance.effectVolume;
        else
            mySource.volume = 1f;
        mySource.Play();
    }

    public override void OnArrival()
    {
        List<Tile> allTiles = new();
        allTiles.AddRange(GridManager.Instance.tiles);
        List<Tile> tilesToExplodeOn = allTiles.Where(t => Pathfinding.GetDistance(targetTile, t) <= explosionRadius).ToList();

        StartCoroutine(ExplosionDelays(tilesToExplodeOn));
    }

    private IEnumerator ExplosionDelays(List<Tile> tilesToExplodeOn)
    {
        CameraController.Instance.ExplosionEffect();
        foreach (Tile t in tilesToExplodeOn)
        {
            Vector3 spawnPos = t.transform.position;
            spawnPos.y += 5f;
            GameObject explosion = Instantiate(explosionPrefab, spawnPos, Quaternion.identity);

            if (t.containsDirt)
            {
                t.RemoveDirt();
                GameStats.Instance.IncreaseRocksMined();
            }   

            if (t.occupied)
                t.occupant.TakeDamage(explosionDamage);

            yield return new WaitForSeconds(0.2f);
        }

        ConfirmArrival();
        Destroy(this.gameObject);

        yield return null;
    }

    public override void ConfirmArrival()
    {
        activator.ConfirmCardExecuted();
    }

}
