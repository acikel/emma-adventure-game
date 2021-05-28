using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionSwitch : MonoBehaviour//, IPointerDownHandler
{
    public GameObject objectToSwitchTo;
    private InteractionSwitch interactionSwitcher;
    private SpriteRenderer objectToSwitchToSpriteRenderer;
    private SpriteRenderer spriteRenderer;
    public bool fadeTransition;
    public float fadeDuration = 1f;

    private Color colorTmp;
    private Inventory inventory;
    private float alphaTmp;
    private bool isFading;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        objectToSwitchToSpriteRenderer = objectToSwitchTo.GetComponent<SpriteRenderer>();
        interactionSwitcher = objectToSwitchTo.GetComponent<InteractionSwitch>();
        inventory = API.Inventory;
        isFading = false;
    }


    private void OnMouseDown()
    {
        if (spriteRenderer.color.a!=1 || interactionSwitcher.isSwitcherFading())
            return;

        inventory.InteractionWithUIActive = true;
        if (!fadeTransition)
        {
            objectToSwitchTo.SetActive(true);
            setAlphaOfRenderer(objectToSwitchToSpriteRenderer, 1);
            gameObject.SetActive(false);
            setAlphaOfRenderer(spriteRenderer, 0);
        }
        else
        {
            objectToSwitchTo.SetActive(true);
            StartCoroutine(FadeCaller(objectToSwitchToSpriteRenderer, 1));
            StartCoroutine(FadeAndDeactivateCaller(spriteRenderer, 0, gameObject));
        }

        /*
        if (!fadeTransition)
        {
            if (objectToSwitch1SpriteRenderer.color.a == 1)
            {

                setAlphaOfRenderer(objectToSwitch1SpriteRenderer, 0);
                setAlphaOfRenderer(spriteRenderer, 1);
                
            }
            else
            {
                setAlphaOfRenderer(objectToSwitch1SpriteRenderer, 1);
                setAlphaOfRenderer(spriteRenderer, 0);
            }
        }
        else
        {
            if (objectToSwitch1SpriteRenderer.color.a == 1)
            {
                StartCoroutine(Fade(objectToSwitch1SpriteRenderer, 0));
                StartCoroutine(Fade(spriteRenderer, 1));
            }
            else
            {
                StartCoroutine(Fade(objectToSwitch1SpriteRenderer, 1));
                StartCoroutine(Fade(spriteRenderer, 0));
            }
        }
        */

    }

    private void OnMouseUp()
    {
        //if (spriteRenderer.color.a != 1)
            //return;
        //inventory.InteractionWithUIActive = false;
    }

    private void setAlphaOfRenderer(SpriteRenderer renderer, float alpha)
    {
        colorTmp = renderer.color;
        colorTmp.a = alpha;
        renderer.color = colorTmp;
    }

    private IEnumerator Fade(SpriteRenderer renderer, float finalAlpha)
    {
        isFading = true;
        float fadeSpeed = Mathf.Abs(renderer.color.a - finalAlpha) / fadeDuration;
        while (!Mathf.Approximately(renderer.color.a, finalAlpha))
        {
            alphaTmp = Mathf.MoveTowards(renderer.color.a, finalAlpha,
                fadeSpeed * Time.deltaTime);

            setAlphaOfRenderer(renderer, alphaTmp);
            yield return null;
        }
        
    }

    private IEnumerator FadeCaller(SpriteRenderer renderer, float finalAlpha)
    {
        yield return StartCoroutine(Fade(renderer, finalAlpha));
        isFading = false;
        if (!interactionSwitcher.isSwitcherFading())
            inventory.InteractionWithUIActive = false;
        
    }

    private IEnumerator FadeAndDeactivateCaller(SpriteRenderer renderer, float finalAlpha, GameObject gameObject)
    {
        yield return StartCoroutine(FadeAndDeactivate(renderer, finalAlpha, gameObject));
        gameObject.SetActive(false);
        isFading = false;
        if (!interactionSwitcher.isSwitcherFading())
            inventory.InteractionWithUIActive = false;
        
    }
    private IEnumerator FadeAndDeactivate(SpriteRenderer renderer, float finalAlpha, GameObject gameObject)
    {
        isFading = true;
        float fadeSpeed = Mathf.Abs(renderer.color.a - finalAlpha) / fadeDuration;
        while (!Mathf.Approximately(renderer.color.a, finalAlpha))
        {
            alphaTmp = Mathf.MoveTowards(renderer.color.a, finalAlpha,
                fadeSpeed * Time.deltaTime);

            setAlphaOfRenderer(renderer, alphaTmp);
            yield return null;
        }
        
    }

    public bool isSwitcherFading()
    {
        return isFading;
    }
}
