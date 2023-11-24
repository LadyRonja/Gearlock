using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelector : MonoBehaviour
{
    public static UnitSelector Instance;
    public Unit selectedUnit;
    public bool playerCanSelectNewUnit = true;
    public GameObject tempUIPanel;
    public TMP_Text tempNameText;
    public TMP_Text tempHealthText;
    public Image tempHealthFill;
    public Image tempHealthFillWhite;
    public TMP_Text tempMovePointsText;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        #endregion

        UpdateUI();
    }

    public void UpdateSelectedUnit(Unit unitToSelect, bool calledByAI)
    {
        if (!playerCanSelectNewUnit && !calledByAI)
            return;

        selectedUnit = unitToSelect;
        UpdateUI();
    }

    public void UpdateSelectedUnit(Unit unitToSelect)
    {
        UpdateSelectedUnit(unitToSelect, false);
    }

    public void UpdateUI(bool damageApplication)
    {
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
            tempHealthText.text = $"HP: {selectedUnit.healthCur}/{selectedUnit.healthMax}";
            tempHealthFill.fillAmount = (float)selectedUnit.healthCur / (float)selectedUnit.healthMax;
            if(damageApplication)
                StartCoroutine(ReduceHealthbarOverTime(0.5f, selectedUnit));
            else
                tempHealthFillWhite.fillAmount = (float)selectedUnit.healthCur / (float)selectedUnit.healthMax;

            if(selectedUnit.movePointsCur < 0)
            {
                Debug.LogError("Units movepoint cur is negative, displaying as 0");
                tempMovePointsText.text = $"MovementPoints: 0/{selectedUnit.movePointsMax}";
            }
            else
            {
                tempMovePointsText.text = $"MovementPoints: {selectedUnit.movePointsCur}/{selectedUnit.movePointsMax}";
            }

        }
    }

    public void UpdateUI()
    {
        UpdateUI(false);
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

        yield return null;
    }
}
