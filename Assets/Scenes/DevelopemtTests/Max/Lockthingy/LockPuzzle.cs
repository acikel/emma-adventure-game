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

    public List<int> InputCode = new List<int>();
    public List<int> Code = new List<int>() { 3, 0, 4, 3 };

    void Start()
    {
        Triangle.onClick.AddListener(OnClick1);
        Square.onClick.AddListener(OnClick2);
        Circle.onClick.AddListener(OnClick3);

    }

    private void Update()
    {
        Debug.Log(InputCode.Count);
        solved = Check(InputCode, Code);
        if (InputCode.Count == 4)
        {
            if (solved == true)
            {
                Debug.Log("yes");
            }
            if (solved == false)
            {
                Debug.Log("false");
            }
        }
    }

    private bool Check(List<int> list1, List<int> list2)
    {
        var areListsEqual = false;

        if (list1.Count != list2.Count)
            return false;

        for (var i = 0; i < list1.Count; i++)
        {
            if (list2[i] == list1[i])
            {
                areListsEqual = true;
            }
        }

        return areListsEqual;
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
