using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    private Vector3 originalPosition;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            originalPosition = transform.position;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Shake(float duration, float intensity)
    {
        originalPosition = transform.position;
        StartCoroutine(ShakeCoroutine(duration, intensity));
    }

    private IEnumerator ShakeCoroutine(float duration, float intensity)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = originalPosition + Random.insideUnitSphere * intensity;

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition;
    }
}
