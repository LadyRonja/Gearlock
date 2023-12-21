using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CameraController : MonoBehaviour
{
    [Header("General")]
    public static CameraController Instance;
    public float camSpeed = 30f;
    public float xOffSet = 3;
    public float yOffSet = 20;
    public float zOffSet = -30;
    [Space]
    public bool playerCanMove = true;
    [HideInInspector] public bool playerHasMoved = false;
    [HideInInspector] public bool movingOnCoroutine = false;
    Vector3 velocity = Vector3.zero;

    Vector3 mouseStartPos = Vector3.zero;
    public bool inverseMouseControls = false;

    [Header("Double clicker")]
    private bool clickedRecently = false;
    private float doubleClickSpan = 0.5f;
    float doublClickTimer = 0;

    [Header("Clamping")]
    bool clampsFound = false;
    [SerializeField] float clampBufferHorizontal = 0f;
    [SerializeField] float clampBufferVertical = 30f;
    Vector2 horizontalClamps = Vector2.zero;
    Vector2 verticalClamps = Vector2.zero;


    private void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion
    }

    private void Start()
    {
        FindClamps();
    }

    private void Update()
    {
        DetectDoubleClick(); //If double clicking, let the camera auto-move again
        KeyBoardMovement();
        MouseMovement();
        ClampCamera();

        if (Input.GetKeyDown(KeyCode.I))
            inverseMouseControls = !inverseMouseControls;
    }

    private void FindClamps()
    {
        if (GridManager.Instance == null) return;
        if (GridManager.Instance.tiles == null) return;

        clampsFound = true;

        float minX = GridManager.Instance.tiles[0,0].transform.position.x;
        float maxX = GridManager.Instance.tiles[GridManager.Instance.tiles.GetLength(0) - 1, 0].transform.position.x;
        minX -= clampBufferHorizontal;
        maxX += clampBufferHorizontal;
        horizontalClamps = new Vector2(minX, maxX);

        float minZ = GridManager.Instance.tiles[0, 0].transform.position.z;
        float maxZ = GridManager.Instance.tiles[0, GridManager.Instance.tiles.GetLength(1) - 1].transform.position.z;
        minZ -= clampBufferVertical;
        maxZ -= clampBufferVertical;
        verticalClamps = new Vector2(minZ, maxZ);
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
            //dir.Normalize();

            if(!inverseMouseControls)
            {
                dir *= -1f;
            }

            if (dir != Vector3.zero)
                playerHasMoved = true;

            float dampener = 0.004f;
            Vector3 newPos = transform.position;
            newPos.x += dir.x * camSpeed * 1.15f * dampener * Time.deltaTime;
            newPos.z += dir.y * camSpeed * 1.15f * dampener * Time.deltaTime;

            transform.position = newPos;
        }
    }

    private void ClampCamera()
    {
        if (!clampsFound)
            return;

        float clampedX = Mathf.Clamp(transform.position.x, horizontalClamps.x, horizontalClamps.y);
        float clampedZ = Mathf.Clamp(transform.position.z, verticalClamps.x, verticalClamps.y);
        Vector3 clampedPos = transform.position;
        clampedPos.x = clampedX;
        clampedPos.z = clampedZ;

        transform.position = clampedPos;
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
