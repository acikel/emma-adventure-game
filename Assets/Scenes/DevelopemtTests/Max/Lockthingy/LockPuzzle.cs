using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LockPuzzle : MonoBehaviour
{

    public Button Triangle;
    public Button Square;
    public Button Circle;
    public bool solved;
    public GameObject Lock;

    public List<int> InputCode = new List<int>();
    public List<int> Code;

    void Start()
    {
        Triangle.onClick.AddListener(OnClick1);
        Square.onClick.AddListener(OnClick2);
        Circle.onClick.AddListener(OnClick3);
        Code = new List<int>() { 3, 0, 4, 3 };

    }

    private void Update()
    {
        for (int i = 0; i < InputCode.Count; i++)
        {
            //Debug.Log(InputCode[i]);
            //Debug.Log(Code[i]);
        }
        Debug.Log(InputCode.Count);
        Debug.Log(solved);
        Check();
    }
    void Check()
    {
        if (InputCode.Count == 4)
        {
            for (int i = 0; i < InputCode.Count; i++)
            {
                if (InputCode[i] != Code[i])
                {
                    solved = false;
                    return;
                }
                solved = true;
                //What happens if the Code is solved
                Lock.SetActive(false);
                return;
            }
        }
    }

    void OnClick1()
    {
        InputCode.Add(3);
    }

    void OnClick2()
    {
        InputCode.Add(4);
    }

    void OnClick3()
    {
        InputCode.Add(0);
    }

}
