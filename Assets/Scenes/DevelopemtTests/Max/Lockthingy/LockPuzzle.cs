using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class LockPuzzle : MonoBehaviour, IPointerClickHandler
{

    public Button Triangle;
    public Button Square;
    public Button Circle;
    public Button Hexagon;
    public Button Octagon;
    public Button Pentagon;

    public GameObject Lock;
    public GameObject CheckPanel;
    public GameObject Window;

    public Sprite CheckPanelgreen;
    public Sprite CheckPanelred;

    Sprite Image;

    private string InputCode = "";
    private int InputCodenumber;
    public int Code = 3043;

    private int count;

    void Start()
    {
        //Debug.Log("Triangle: " + Triangle);
        //Debug.Log("onClick: "+ Triangle.onClick);
        
        
        Triangle.onClick.AddListener(OnClick3);
        Square.onClick.AddListener(OnClick4);
        Circle.onClick.AddListener(OnClick0);
        Hexagon.onClick.AddListener(OnClick6);
        Octagon.onClick.AddListener(OnClick8);
        Pentagon.onClick.AddListener(OnClick5);
        InputCode = "";
        
        CheckPanel.GetComponent<Image>().sprite = CheckPanelred;
        
        

    }

    private void Update()
    {
        //Debug.Log(InputCode);
        
    }
    void Check()
    {
        if (count == 4)
        {
            InputCodenumber = int.Parse(InputCode);
            if (InputCodenumber == Code)
            {
                CheckPanel.GetComponent<Image>().sprite = CheckPanelgreen;
                InputCode = "";
            }
            else
            {
                InputCode = "";
               
            }
        }
        
    }

    void OnClick0()
    {
        InputCode += "0";

        count = InputCode.Length;
        Check();
    }

    void OnClick3()
    {
        InputCode += "3";

        count = InputCode.Length;
        Check();
    }

    void OnClick4()
    {
        InputCode += "4";

        count = InputCode.Length;
        Check();
    }

    void OnClick5()
    {
        InputCode += "5";

        count = InputCode.Length;
        Check();
    }

    void OnClick6()
    {
        InputCode += "6";

        count = InputCode.Length;
        Check();
    }

    void OnClick8()
    {
        InputCode += "8";

        count = InputCode.Length;
        Check();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Window.SetActive(true);
        if (Window.activeSelf == true)
        {
            Window.SetActive(false);
            //Debug.Log("Window not here");
        }
        else
        {
            Window.SetActive(true);
            // Debug.Log("Window here");
        }

    }
}
