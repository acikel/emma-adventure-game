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
        this.textLabel = textLabel;
        this.textToType = textToType;
        //typeWriting = StartCoroutine(PlayText(textToType, textLabel));
        typeWriting = StartCoroutine(TypeText(textToType, textLabel));
    }

    IEnumerator PlayText(string textToType, TMP_Text textLabel)
    {
        isWritingDone = false;
        foreach (char c in textToType)
        {
            textLabel.text += c;
            yield return new WaitForSeconds(0.125f);
        }
        isWritingDone = true;
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {
        isWritingDone = false;
        float t = 0;
        int charIndex = 0;
        //Debug.Log("TypeText with effect  "+ t);
        //Debug.Log("writer Done0:" + isWritingDone);
        while (charIndex < textToType.Length)
        {
            isWritingDone = false;
            t += Time.deltaTime * typewriterSpeed;
                charIndex = Mathf.FloorToInt(t);
            //Debug.Log("TypeText with effect1:" + charIndex);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            //Debug.Log("TypeText with effect2:" + charIndex + "  "+ textToType.Length);
            //Debug.Log("writer Done1:"+isWritingDone);
            textLabel.text = textToType.Substring(0, charIndex);
            //Debug.Log("writer Done1:"+isWritingDone);
            yield return null;
        }
        
        isWritingDone = true;
        //Debug.Log("writer Done3:" + isWritingDone);
        textLabel.text = textToType;

    }

    public void ShowButtonTextImmediately(string textToType, TMP_Text textLabel)
    {
        //Debug.Log("TypeText Ímmediately");
        textLabel.text = textToType;
    }

    public void clearPreviousText()
    {
        //Debug.Log("TypeText Ímmediately");
        textLabel.text = "";
    }

    private void EndTypewritingShowCompleteText()
    {
        //Debug.Log("end typing1");
        if (typeWriting != null)
        {
            //Debug.Log("end typing2");
            isWritingDone = true;
            StopCoroutine(typeWriting);
            textLabel.text = textToType;
            typeWriting = null;


        }
        

    }

}
