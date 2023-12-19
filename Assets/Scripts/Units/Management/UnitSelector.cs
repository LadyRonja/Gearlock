using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class UnitSelector : MonoBehaviour
{
    public static UnitSelector Instance;
    public Unit selectedUnit;
    public bool playerCanSelectNewUnit = true;

    [Header("Selected Unit")]
    public GameObject tempUIPanel;
    public TMP_Text tempNameText;
    public TMP_Text tempHealthText;
    public Image tempHealthFill;
    public Image tempHealthFillWhite;
    public TMP_Text tempPowerText; 
    public List<GameObject> MovePointDark;
    public List<GameObject> MovePointLight;
    public Image portrait;
    int maxMovePoints = 4;

    [Header("Hover Info")]
    public TMP_Text tempNameTextMini;
    public TMP_Text tempHealthTextMini;
    public Image tempHealthFillMini;
    public Image tempHealthFillWhiteMini;
    //public TMP_Text tempPowerTextMini;
    //public List<GameObject> MovePointDarkMini;
    //public List<GameObject> MovePointLightMini;

    private Unit hoveredUnit; // Track the currently hovered unit


    [Header("All player Units")]
    public Transform gridParent;
    public GameObject displayPrefab;

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
        if(Input.GetMouseButtonDown((int)MouseButton.Right))
            DeselectUnit();

        UpdateUI();
        UpdatePlayerUnitUI();
    }

    //test elin
    private void UpdateMiniUI()
    {
        // Update the mini UI based on the hovered unit
        if (hoveredUnit != null)
        {
            tempNameTextMini.text = hoveredUnit.unitName;
            tempHealthTextMini.text = $"HP: {hoveredUnit.healthCur}/{hoveredUnit.healthMax}";
            tempHealthFillMini.fillAmount = (float)hoveredUnit.healthCur / (float)hoveredUnit.healthMax;
            tempHealthFillWhiteMini.fillAmount = (float)hoveredUnit.healthCur / (float)hoveredUnit.healthMax;
            // Update other mini UI elements as needed
        }
        else
        {
            // Reset mini UI when no unit is hovered
            tempNameTextMini.text = string.Empty;
            tempHealthTextMini.text = string.Empty;
            // Reset other mini UI elements as needed
        }
    }

        public void UpdateSelectedUnit(Unit unitToSelect, bool calledByAI)
    {
        // Determine if updating the selected units is legal
        if (!playerCanSelectNewUnit && !calledByAI)
            return;

        if(selectedUnit != null)
            selectedUnit.standingOn.UnHighlight();

        selectedUnit = unitToSelect;
        if(selectedUnit != null)
        {
            if (selectedUnit.playerBot)
                selectedUnit.standingOn.Highlight(Color.blue);
            else
                selectedUnit.standingOn.Highlight(Color.yellow);

            CameraController.Instance.MoveToTarget(selectedUnit.transform.position);
        }
        UpdateUI();
    }

    public void DeselectUnit()
    {
        if (!TurnManager.Instance.isPlayerTurn) return;
        if (ActiveCard.Instance.cardBeingPlayed != null) return;

        UpdateSelectedUnit(null);
    }

    public void UpdateSelectedUnit(Unit unitToSelect)
    {
        UpdateSelectedUnit(unitToSelect, false);
    }

    public void UpdateUI(bool damageApplication)
    {

        for (int i = 0; i < maxMovePoints;  i++)
        {
            MovePointDark[i].SetActive(false);
            MovePointLight[i].SetActive(false);
        }

        // TODO: Update UI properly
        if (tempUIPanel == null) return;

        if (selectedUnit == null)
        {
            tempUIPanel.SetActive(false);
        }
        else
        {
            tempUIPanel.SetActive(true);
            tempNameText.text = selectedUnit.unitName;
            tempNameTextMini.text = selectedUnit.unitName;
            tempPowerText.text = $"{selectedUnit.power}";
            tempHealthText.text = $"HP: {selectedUnit.healthCur}/{selectedUnit.healthMax}";
            tempHealthTextMini.text = $"HP: {selectedUnit.healthCur}/{selectedUnit.healthMax}";

            tempHealthFill.fillAmount = (float)selectedUnit.healthCur / (float)selectedUnit.healthMax;
            tempHealthFillMini.fillAmount = (float)selectedUnit.healthCur / (float)selectedUnit.healthMax;


            if (damageApplication)
                StartCoroutine(ReduceHealthbarOverTime(0.5f, selectedUnit));
            else
            {
                tempHealthFillWhite.fillAmount = (float)selectedUnit.healthCur / (float)selectedUnit.healthMax;
                tempHealthFillWhiteMini.fillAmount = (float)selectedUnit.healthCur / (float)selectedUnit.healthMax;
            }
                

            portrait.sprite = selectedUnit.portrait;

            for (int i = 0; i < selectedUnit.movePointsMax; i++)
            {
                MovePointDark[i].SetActive(true);
            }

            if (selectedUnit.movePointsCur < 0)
            {
                Debug.LogError("Units movepoint cur is negative, displaying as 0");
                for (int i = 0; i < selectedUnit.movePointsMax; i++)
                {
                    MovePointLight[i].SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < selectedUnit.movePointsCur; i++)
                {
                    MovePointLight[i].SetActive(true);
                }
            }

        }
    }

    public void UpdateUI()
    {
        UpdateUI(false);
    }

    // Inefficent way, but much easier to write and keep track of.
    public void UpdatePlayerUnitUI()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
            UnitStorage.Instance.playerPanels = new();
        }

        foreach (Unit u in UnitStorage.Instance.playerUnits)
        {
            GameObject unitMiniPanel = Instantiate(displayPrefab, gridParent);
            UnitMiniPanel ump = unitMiniPanel.GetComponent<UnitMiniPanel>();
            ump.SetInfo(u);
            ump.UnHighlight();
            UnitStorage.Instance.playerPanels.Add(ump);
        }
    }

    private IEnumerator ReduceHealthbarOverTime(float seconds, Unit healthOf)
    {
        float startValue = tempHealthFillWhite.fillAmount;
        float endValue = (float)healthOf.healthCur/(float)healthOf.healthMax;

        float timeToSet = 0.5f;
        float timePassed = 0;

        while (timePassed < timeToSet)
        {
            tempHealthFillWhite.fillAmount = Mathf.Lerp(startValue, endValue, (timePassed / timeToSet));

            timePassed += Time.deltaTime;
            yield return null;
        }
        tempHealthFillWhite.fillAmount = endValue;

        yield return null;
    }
}
