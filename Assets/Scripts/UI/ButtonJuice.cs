using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonJuice : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Header("Audio")]
    [SerializeField] AudioClip enterSound;
    [SerializeField] AudioClip clickSound;

    [Header("Tweening")]
    [SerializeField] bool expandObject = false;
    [SerializeField] Ease expandEase;
    Vector3 startScale = Vector3.one;

    private void Start()
    {
        startScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GotSoundForClick())
            AudioHandler.PlaySoundEffect(clickSound);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GotSoundForEnter())
            AudioHandler.PlaySoundEffect(enterSound);

        if (expandObject)
        {
            transform.DOScale(startScale * 1.1f, 0.3f).SetEase(expandEase);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (expandObject)
        {
            transform.DOScale(startScale, 0.3f).SetEase(expandEase);
        }
    }

    private bool GotSoundForEnter()
    {
        if (enterSound != null)
            return true;

        enterSound = Resources.Load<AudioClip>("Music/buttonEnter");

        if (enterSound != null)
            return true;

        return false;
    }

    private bool GotSoundForClick()
    {
        if (clickSound != null)
            return true;

        clickSound = Resources.Load<AudioClip>("Music/buttonClick");

        if (clickSound != null)
            return true;

        return false;
    }
    public void OnDisable()
    {
        transform.DOKill();
    }
}
