using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public float camSpeed = 30f;
    public float xOffSet = 3;
    public float yOffSet = 20;
    public float zOffSet = -30;
    public bool playerCanMove = true;
    public bool playerHasMoved = false;
    public bool movingOnCoroutine = false;
    Vector3 velocity = Vector3.zero;

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
        DetectDoubleClick(); //If double clicking, let the camera auto-move again
        MovementManager();
    }

    private void MovementManager()
    {
        if (!playerCanMove)
            return;

        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        Vector3 newPos = transform.position;

        newPos.x += move.x * camSpeed * Time.deltaTime;
        newPos.z += move.y * camSpeed * Time.deltaTime;

        if (move != Vector2.zero)
            playerHasMoved = true;

        transform.position = newPos;
    }

    public void MoveToTarget(Vector3 target, float seconds = 0.5f)
    {
        // If the player has set the camera, leave it there
        if (playerHasMoved)
            return;

        if (!movingOnCoroutine)
            StartCoroutine(MoveTo(target, seconds));
    }

    private IEnumerator MoveTo(Vector3 target, float seconds)
    {
        movingOnCoroutine = true;

        Vector3 startPos = transform.position;

        // for easier clicking, offset a little to the side
        if (startPos.x - target.x < 0)
            target.x -= xOffSet;
        else
            target.x += xOffSet;

        // Never change camera Y ("zoom")
        target.y = startPos.y;

        target.z += zOffSet;

        float timePassed = 0;
        while (timePassed < seconds)
        {
            transform.position = Vector3.Lerp(startPos, target, (timePassed / seconds));
            //transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, seconds);
            timePassed += Time.deltaTime;
            yield return null;
        }
        transform.position = target;

        movingOnCoroutine = false;
        yield return null;
    }

    private bool clickedRecently = false;
    private float doubleClickSpan = 0.5f;
    float doublClickTimer = 0;
    private void DetectDoubleClick()
    {
        if (Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            if (clickedRecently)
            {
                playerHasMoved = false;
                clickedRecently = false;
            }
            clickedRecently = true;
            doublClickTimer = doubleClickSpan;
        }

        if(clickedRecently)
        {
            doublClickTimer -= Time.deltaTime;
            if (doublClickTimer < 0f)
            {
                clickedRecently = false;
            }
        }
    }

}
