using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackToGame : MonoBehaviour, IPointerClickHandler
{
    public GameObject Window;



    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (Window.activeSelf == true)
        {
            Window.SetActive(false);
        }
    }
}
