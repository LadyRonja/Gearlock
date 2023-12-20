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

    Vector3 mouseStartPos = Vector3.zero;
    public bool inverseMouseControls = false;

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
        KeyBoardMovement();
        MouseMovement();

        if(Input.GetKeyDown(KeyCode.I))
            inverseMouseControls = !inverseMouseControls;
    }

    private void KeyBoardMovement()
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

    private void MouseMovement()
    {
        if (Input.GetMouseButtonDown((int)MouseButton.Middle))
        {
            mouseStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButton((int)MouseButton.Middle))
        {
            Vector3 dir = (mouseStartPos - Input.mousePosition);
            dir.Normalize();

            if(!inverseMouseControls)
            {
                dir *= -1f;
            }

            if (dir != Vector3.zero)
                playerHasMoved = true;

            Vector3 newPos = transform.position;
            newPos.x += dir.x * camSpeed * 1.15f * Time.deltaTime;
            newPos.z += dir.y * camSpeed * 1.15f * Time.deltaTime;

            transform.position = newPos;
        }
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

                Vector3 mousePosition = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider == null)
                        return;

                    // Check for unit
                    if (hit.collider.gameObject.TryGetComponent<Unit>(out Unit u))
                    {
                        UnitSelector.Instance.UpdateSelectedUnit(u);
                    }
                    else if (hit.collider.gameObject.TryGetComponent<Tile>(out Tile t))
                    {
                        if(t.occupant != null)
                            UnitSelector.Instance.UpdateSelectedUnit(t.occupant);

                    }
                }
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
