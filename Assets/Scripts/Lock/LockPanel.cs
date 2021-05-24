using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LockPanel : MonoBehaviour
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
    public void codeButtonClick()
    {
        //Debug.Log("codeButtonClick");
        checkPressedButtonAndAddNumber();
        count = InputCode.Length;
        Check();
    }
}
