using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TipewriterEffect : MonoBehaviour
{

    [SerializeField] private float typewriterSpeed = 50f;

    private bool isWritingDone;
    private Coroutine typeWriting;
    private string textToType;
    private TMP_Text textLabel;

    public bool IsWritingDone
    {
        get
        {
            return isWritingDone;
        }
    }

    private void Start()
    {
        isWritingDone = true;
    }
    private void OnDisable()
    {
        TextManager.OnEndTypeWriting -= EndTypewritingShowCompleteText;
    }

    private void OnEnable()
    {
        TextManager.OnEndTypeWriting += EndTypewritingShowCompleteText;
    }
    public void Run(string textToType, TMP_Text textLabel)
    {
        typeWriting=StartCoroutine(TypeText(textToType, textLabel));
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {
        isWritingDone = false;
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
        isWritingDone = true;
        Debug.Log(isWritingDone);
        textLabel.text = textToType;

    }
    public void ShowButtonTextImmediately(string textToType, TMP_Text textLabel)
    {

        textLabel.text = textToType;
    }


    private void EndTypewritingShowCompleteText()
    {
        if (typeWriting != null)
        {
            isWritingDone = true;
            StopCoroutine(typeWriting);
            textLabel.text = textToType;
        }
        

    }

}
