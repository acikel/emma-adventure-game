using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImagePopUpPanel : MonoBehaviour, IPointerEnterHandler
{
    //resume panel of which the OnPointerDown blocker should be deactivated if the popupwindow is launched on this resumeblocker. (otherwise two clicks are needed to close the popupwidnow via resumepanel)
    public ResumePanel imageResumePanel;
    //image to be changed if imagePopUpPanel is opened.
    [SerializeField]
    private Image popUpImagePanelImage;   

    
    public void setImageOfPopUpImagePanel(Sprite sprite)
    {
        popUpImagePanelImage.sprite = sprite;
    }


    //deactive 

    //if image pop ups was entered over this popUpResumeBlocker then there is no need for an onPointerDownStop of the ResumePanel through popUpWindowJustOpened (of ResumePanel)
    //As this popUpResumeBlocker is blocking the ResumePanel, OnPointerDown of the ResumePanel is never called in this case and therefore the popUpWindow is not closed right away.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            imageResumePanel.resetJustOpened();
        }
    }
}
