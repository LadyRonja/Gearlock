using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelector : MonoBehaviour
{
    public static UnitSelector Instance;
    public Unit selectedUnit;
    public GameObject tempUIPanel;
    public TMP_Text tempNameText;
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

    public void UpdateSelectedUnit(Unit unitToSelect)
    {
        selectedUnit = unitToSelect;
        UpdateUI();
    }

    public void UpdateUI()
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
            tempMovePointsText.text = $"MovementPoints: {selectedUnit.movePointsCur}/{selectedUnit.movePointsMax}";
        }
    }
}
