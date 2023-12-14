using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitMiniPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Image potrait;
    public TMP_Text nameText;
    public Image healthFill;
    public Image highligther;
    
    public Unit myUnit;

    public void SetInfo(Unit unit)
    {
        potrait.sprite = unit.portrait;
        nameText.text = unit.unitName;
        healthFill.fillAmount = (float)unit.healthCur / (float)unit.healthMax;

        myUnit = unit;
    }

    public void Highlight()
    {
        if (highligther == null)
            return;

        highligther.gameObject.SetActive(true);
    }

    public void UnHighlight()
    {
        if (highligther == null)
            return;

        highligther.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(ActiveCard.Instance.cardBeingPlayed == null)
            UnitSelector.Instance.UpdateSelectedUnit(myUnit);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverManager.HoverTileEnter(myUnit.standingOn);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverManager.HoverTileExit(myUnit.standingOn);
    }
}
