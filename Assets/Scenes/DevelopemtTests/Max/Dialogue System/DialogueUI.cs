using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textLabel;

    private void Start()
    {
        textLabel.text = "”I don't want to leave you, I don't want to be alone again, I want everything to go back to how it was! How can I even see you?";
    }
}
