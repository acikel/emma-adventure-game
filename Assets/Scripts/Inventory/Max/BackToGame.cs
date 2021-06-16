using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackToGame : MonoBehaviour, IPointerClickHandler
{
    public GameObject Panel;



    public void OnPointerClick(PointerEventData pointerEventData)
    {
        
            
        if (Panel.activeSelf == true){
            Panel.SetActive(false);
            //Debug.Log("Window not here");
        } else{
            Panel.SetActive(true);
           // Debug.Log("Window here");
        }
        
        

    }
}
