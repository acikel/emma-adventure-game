using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionSwitch : MonoBehaviour//, IPointerDownHandler
{
    public SpriteRenderer objectToSwitch1;
    public SpriteRenderer objectToSwitch2;
    public bool fadeTransition;
    public float fadeDuration = 1f;

    private Color colorTmp;
    private Inventory inventory;
    private float alphaTmp;

    private void Start()
    {
        inventory = API.Inventory;
    }


    private void OnMouseDown()
    {
        inventory.InteractionWithUIActive = true;

        if (!fadeTransition)
        {
            if (objectToSwitch1.color.a == 1)
            {
                setAlphaOfRenderer(objectToSwitch1, 0);
                setAlphaOfRenderer(objectToSwitch2, 1);
            }
            else
            {
                setAlphaOfRenderer(objectToSwitch1, 1);
                setAlphaOfRenderer(objectToSwitch2, 0);
            }
        }
        else
        {
            if (objectToSwitch1.color.a == 1)
            {
                StartCoroutine(Fade(objectToSwitch1, 0));
                StartCoroutine(Fade(objectToSwitch2, 1));
            }
            else
            {
                StartCoroutine(Fade(objectToSwitch1, 1));
                StartCoroutine(Fade(objectToSwitch2, 0));
            }
        }
        
    }

    private void OnMouseUp()
    {
        inventory.InteractionWithUIActive = false;
    }

    private void setAlphaOfRenderer(SpriteRenderer renderer, float alpha)
    {
        colorTmp = renderer.color;
        colorTmp.a = alpha;
        renderer.color = colorTmp;
    }

    private IEnumerator Fade(SpriteRenderer renderer, float finalAlpha)
    {
        float fadeSpeed = Mathf.Abs(renderer.color.a - finalAlpha) / fadeDuration;
        while (!Mathf.Approximately(renderer.color.a, finalAlpha))
        {
            alphaTmp = Mathf.MoveTowards(renderer.color.a, finalAlpha,
                fadeSpeed * Time.deltaTime);

            setAlphaOfRenderer(renderer, alphaTmp);
            yield return null;
        }
    }

    private IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
