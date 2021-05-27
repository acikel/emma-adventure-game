using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionSwitch : MonoBehaviour//, IPointerDownHandler
{
    public SpriteRenderer objectToSwitch1;
    public SpriteRenderer objectToSwitch2;

    private Color colorTmp;
    private Inventory inventory;

    private void Start()
    {
        inventory = API.Inventory;
    }


    private void OnMouseDown()
    {
        inventory.InteractionWithUIActive = true;
        if (objectToSwitch1.color.a==1)
        {
            setAlphaOfRenderer(objectToSwitch1,0);
            setAlphaOfRenderer(objectToSwitch2, 1);
        }
        else
        {
            setAlphaOfRenderer(objectToSwitch1, 1);
            setAlphaOfRenderer(objectToSwitch2, 0);
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
}
