using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text Emma;
    [SerializeField] private TMP_Text Other;
    [SerializeField] private TMP_Text Answer1;
    [SerializeField] private TMP_Text Answer2;
    [SerializeField] private TMP_Text Answer3;

    public int Click;
    TipewriterEffect Write;
    public bool go;

    public Button Button1;
    public Button Button2;
    public Button Button3;
    public GameObject Button1Obj;
    public GameObject Button2Obj;
    public GameObject Button3Obj;

    bool buttonON = false;

    private void Start()
    {
        Write = GetComponent<TipewriterEffect>();
        Button1.onClick.AddListener(OnClick);
        Button2.onClick.AddListener(OnClick);
        Button3.onClick.AddListener(OnClick);

        Write.Run("Oooh, is that you Emma? Oh you've gotten so tall! Still skinny though. Always so picky about dinner…. ", Other);
    }
    private void Update()
    {
        Debug.Log(Click);
        go = Write.written;
        InputMouse();
        if (go == false)
        {
            if (Click == 1)
            {
                Write.Run(" noise of crying ", Emma);
                Other.text = string.Empty;
            }
            if (Click == 2)
            {
                Write.Run("Oh don't fret dear, look who it is! (Helper) was my friend when I was your age, and when his time came, I put his bones together on my own. I just couldn't say goodbye... I’m glad he's here to watch over you.", Other);
                Emma.text = string.Empty;
            }
            if (Click == 3)
            {
                Write.Run("Grandmama, what happened? Why did you leave me alone? what happened to you and everything around?", Emma);
                Other.text = string.Empty;
            }
            if (Click == 4)
            {
                Write.Run("Oooh, I know many many things, but this is not one of them. Perhaps the professor knows more?", Other);
                Emma.text = string.Empty;
            }
            if (Click == 5)
            {
                Button1Obj.SetActive(true);
                Button2Obj.SetActive(true);
                Button3Obj.SetActive(true);
                buttonON = true;
                Write.Run("What proffesor what are you talking about?", Answer1);
                Write.Run("How do you not know..?", Answer2);
                Write.Run("I haven't seen you in so long and this is all you can say to me?", Answer3);
                Other.text = string.Empty;
            }
            if (Click == 6)
            {
                Write.Run(" I think he was once a famous scientist, but then he lost his job and moved here with his wife. He did seem rather worried last time I saw him. Never liked his wife though. Dreadful woman. high pitched voice. You remember him? He's our closest neighbor. Maybe you should go visit him?", Other);
                Answer1.text = string.Empty;
                Answer2.text = string.Empty;
                Answer3.text = string.Empty;
            }
            if (Click == 7)
            {
                Write.Run("I don't want to leave you, I don't want to be alone again, I want everything to go back to how it was! How can I even see you?", Answer1);
                Write.Run("But what if there's no one? I don't want to be around this dead silence… How did you manage to come back..?", Answer2);
                Other.text = string.Empty;

            }
            if (Click == 8)
            {
                Write.Run("Who knows, maybe it's because I wished to make sure you ate enough. That I wished to see you and let you know that you'll be alright. Maybe it was your wish for a real goodbye. some things we'll never know. But if im here now, I'm sure others will be", Other);
                Answer1.text = string.Empty;
                Answer2.text = string.Empty;
            }
            if (Click == 9)
            {
                Write.Run("I was alone for so long, it was so quiet...", Answer1);
                Write.Run("I don't think i can do this alone.", Answer2);
                Other.text = string.Empty;

            }
            if (Click == 8)
            {
                Write.Run("You're not alone. Not anymore..  (helper) is with you my dear, after all, as long as there are bones, we can't be forgotten. Now, don't be afraid, you're a brave girl and I know you can find the answers that you’re looking for.", Other);
                Answer1.text = string.Empty;
                Answer2.text = string.Empty;
            }
            if (Click == 9)
            {
                Write.Run("(Emma wipes her tears and takes a deep breath, and gives grandma a shaky smile)", Emma);
                Other.text = string.Empty;
            }
            if (Click == 10)
            {
                Write.Run("Off you go, don't dilly dally, I'm going to stay here in my chair- I just got comfortable, and these plants won't water themselves", Other);
                Emma.text = string.Empty;
            }
            if (Click == 11)
            {
                Write.Run("(emma leaves)/n(grandma tries to turn the wheel again)/nWhy didn't I oil this while I was alive?", Other);
            }
        }
    }

    void OnClick()
    {
        Button1Obj.SetActive(false);
        Button2Obj.SetActive(false);
        Button3Obj.SetActive(false);
        Write.written = false;
        Click += 1;
    }

    void InputMouse()
    {
        if (Input.GetMouseButtonDown(0) && go == true && buttonON == false)
        {
            Write.written = false;
            Click += 1;

        }
    }
}
