using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dirt : MonoBehaviour
{
    public Tile myTile;
    public MeshRenderer gfx;
    public SpriteRenderer highligther;
    public AudioClip getMinedSound;


    [Header("Highlighter Bounce")]
    protected float highlighterYOffSet = 0;
    protected Vector3 highlighterStartPos = Vector3.zero;
    protected AnimationCurve highligtherCurve;

    private void Start()
    {
        UnHighlight();

        if (highligther != null)
        {
            highlighterStartPos = highligther.transform.position;
            highligtherCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
            highligtherCurve.preWrapMode = WrapMode.PingPong;
            highligtherCurve.postWrapMode = WrapMode.PingPong;
        }
    }

    private void Update()
    {
        if (highligther == null)
            return;

        highlighterYOffSet = highligtherCurve.Evaluate(Time.time % highligtherCurve.length);
        Vector3 currentPos = highligther.transform.position;
        highligther.transform.position = new Vector3(currentPos.x, highlighterStartPos.y + highlighterYOffSet, currentPos.z);
    }

    public void Highlight()
    {
        if (highligther != null)
            highligther.gameObject.SetActive(true);
    }
    public void UnHighlight()
    {
        if(highligther != null)
            highligther.gameObject.SetActive(false);
    } 
}
