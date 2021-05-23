using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Typewriter : MonoBehaviour
{

    [SerializeField] private float typewriterSpeed = 50f;

    public bool written = false;

    public void Run(string textToType, TMP_Text textLabel)
    {
        textLabel = GetComponent<TMP_Text>();
        StartCoroutine(TypeText(textToType, textLabel));
        Debug.Log(written);
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {
        foreach (char c in textToType)
        {
            if (textLabel.text == textToType)
            {
                written = true;
                break;
            }
            textLabel.text += c;
            yield return new WaitForSeconds(0.001f);
        }
    }
}
