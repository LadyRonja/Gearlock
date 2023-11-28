using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public float camSpeed = 5f;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion
    }

    private void Update()
    {
        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        Vector3 newPos = transform.position;

        newPos.x += move.x * camSpeed * Time.deltaTime;
        newPos.z += move.y * camSpeed * Time.deltaTime;

        transform.position = newPos;
    }
}
