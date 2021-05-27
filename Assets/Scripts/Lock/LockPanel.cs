using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LockPanel : MonoBehaviour, IPointerEnterHandler
{
    [HideInInspector]
    public static string InputCode = "";
    private int InputCodenumber;
    public int Code = 3043;
    private int count;
    private string selectedObjectName;

    public Sprite CheckPanelSolved;
    public Sprite CheckPanelUnsolved;
    public Image CheckPanel;

    

    //subscribed by OpenLockDoor.cs to set open door active when code was solved.
    public delegate void HandleLockSolved();
    public static event HandleLockSolved OnLockSolved;

    //resume panel of which the OnPointerDown blocker should be deactivated if the popupwindow is launched on this resumeblocker. (otherwise two clicks are needed to close the popupwidnow via resumepanel)
    public ResumePanel imageResumePanel;

    // Start is called before the first frame update
    void Start()
    {
        InputCode = "";
    }

    private void OnEnable()
    {
        InputCode = "";
  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //if image pop ups was entered over this popUpResumeBlocker then there is no need for an onPointerDownStop of the ResumePanel through popUpWindowJustOpened (of ResumePanel)
    //As this popUpResumeBlocker is blocking the ResumePanel, OnPointerDown of the ResumePanel is never called in this case and therefore the popUpWindow is not closed right away.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            imageResumePanel.resetJustOpened();
        }
    }

    public void codeButtonClick()
    {
        //Debug.Log("codeButtonClick");
        checkPressedButtonAndAddNumber();
        count = InputCode.Length;
        Check();
    }

    private void Check()
    {
        if (count == 4)
        {
            InputCodenumber = int.Parse(InputCode);
            if (InputCodenumber == Code)
            {
                //code was solved
                CheckPanel.sprite = CheckPanelSolved;
                //close canvas put sprite back to red and open door.
                OnLockSolved();
            }
            InputCode = "";
        }

    }

    private void checkPressedButtonAndAddNumber()
    {
        selectedObjectName = EventSystem.current.currentSelectedGameObject.name;
        if (selectedObjectName.Contains("Button"))
        {
            switch(selectedObjectName)
            {
                case "CircleButton":
                    InputCode += "0";
                    break;
                case "HexagonButton":
                    InputCode += "6";
                    break;
                case "OctagonButton":
                    InputCode += "8";
                    break;
                case "PentagonButton":
                    InputCode += "5";
                    break;
                case "SquareButton":
                    InputCode += "4";
                    break;
                case "TriangleButton":
                    InputCode += "3";
                    break;
            }
        }
    }
}
