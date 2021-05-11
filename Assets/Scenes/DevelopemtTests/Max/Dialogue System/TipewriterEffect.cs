using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TipewriterEffect : MonoBehaviour
{

    [SerializeField] private float typewriterSpeed = 50f;

    public bool written;

    public void Run(string textToType, TMP_Text textLabel)
    {
        StartCoroutine(TypeText(textToType, textLabel));
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {
        float t = 0;
        int charIndex = 0;

        while (charIndex < textToType.Length)
        {
                t += Time.deltaTime * typewriterSpeed;
                charIndex = Mathf.FloorToInt(t);
                charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

                textLabel.text = textToType.Substring(0, charIndex);
                yield return null;
        }
        written = true;
        Debug.Log(written);
        textLabel.text = textToType;

    }

}
