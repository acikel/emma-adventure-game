using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagePopUpPanel : MonoBehaviour
{
    //image to be changed if imagePopUpPanel is opened.
    [SerializeField]
    private Image popUpImagePanelImage;   

    
    public void setImageOfPopUpImagePanel(Sprite sprite)
    {
        popUpImagePanelImage.sprite = sprite;
    }

}
