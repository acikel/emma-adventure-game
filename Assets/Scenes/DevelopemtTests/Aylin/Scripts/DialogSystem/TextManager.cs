using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;
using System.Collections;

// This class is used to manage the text that is
// displayed on screen.  In situations where many
// messages are triggered one after another it
// makes sure they are played in the correct order.
public class TextManager : MonoBehaviour
{

    // This struct encapsulates the messages that are
    // sent for organising.
    public struct Instruction
    {
        public string message;      // The body of the message.
        public Color textColor;     // The color the message should be displayed in.
        //public float startTime;     // The time the message should start being displayed based on when it is triggered and its delay.
        public string callerName;
    }


    public TMP_Text textEmma;                               // Reference to the Text component that will display the message for player.
    public TMP_Text[] textNPCs;
    //public Text[] textAnswerButton;
    public Button[] buttonAnswerOptions;


    //Subscribed by ReactionCollection to define which reaction will be invoked next. 
    public delegate void OnNextTurnHandler();
    public static event OnNextTurnHandler OnNextTurn;

    //list of all ReactionCollections in DialogScene to determine if all ended in update function.
    public List<ReactionCollection> listOfReactionColledtions;
    private SceneManager sceneManager;

    //Subscribed by Typeeriter to determine end of typewriting. And AudioReaction to stop sound when a text is skipped.
    public delegate void OnEndTypeWritingHandler();
    public static event OnEndTypeWritingHandler OnEndTypeWriting;

    private TipewriterEffect typeWriter;
    private InputManager inputManager;
    private TMP_Text currentTextBox;

    public float displayTimePerCharacter = 0.1f;    // The amount of time that each character in a message adds to the amount of time it is displayed for.
    public float additionalDisplayTime = 0.5f;      // The additional time that is added to the message is displayed for.


    private List<Instruction> instructions = new List<Instruction>();
    // Collection of instructions that are ordered by their startTime.
    private float clearTime;                        // The time at which there should no longer be any text on screen.

    //turn of a reaction in a conversation beginning from 1. Used by ReactionCollection class.
    private static int turnCounter=1;
    private bool newMouseClick = true;

    //Canvas to asign main camera to for render mode screen space camera, as the camera is another scene then the dialog system.
    public Canvas CanvasDialog;
    public static int TurnCounter
    {
        get
        {
            return turnCounter;
        }
    }

    private void Start()
    {
        CanvasDialog.worldCamera = Camera.main;
        //Set CanvasDialog.planeDistance to zero as mouse events (onclick) wont work on buttons otherwise as colliders/raycastblockers are overlapped.
        CanvasDialog.planeDistance = 0;
        inputManager = API.InputManager;
        sceneManager = API.SceneManager;
        typeWriter = GetComponent<TipewriterEffect>();
        turnCounter = 1;
        addOnClickListenerToButtons();
        setButtonsActive(false);

    }

    
    private void addOnClickListenerToButtons()
    {
        foreach(Button b in buttonAnswerOptions)
        {
            b.onClick.AddListener(OnClick);
        }
    }

    public void OnClick()
    {
        setButtonsActive(false);
        //Write.written = false;
        turnCounter++;

        OnNextTurn?.Invoke();
    }

    private void setButtonsActive(bool isActive)
    {
        foreach (Button b in buttonAnswerOptions)
        {
            b.gameObject.SetActive(isActive);
        }
    }

    //function is needed to wait before next dialog text is invoked. Without waiting 
    //(if OnNextTurn?.Invoke(); is invoked right after a text is cleared with typeWriter.clearPreviousText(); 
    //in the upadte function) the typewritting effect does not work somehow.
    //The longer the text the longer the function needs to wait.
    private IEnumerator invokeNextTypewrittenDialogText()
    {
        yield return new WaitForSeconds(0.3f);
        OnNextTurn?.Invoke();
    }

    private bool didAllReactionsEnd()
    {
        if (listOfReactionColledtions == null)
            return false;
        foreach(ReactionCollection reactionCollection in listOfReactionColledtions)
        {
            if (!reactionCollection.reactionsFinished())
                return false;
        }
        return true;
    }

    private void Update()
    {
        //Debug.Log("turn count:" + turnCounter + "newMouseClick"+ newMouseClick+ "inputManager.isMouseDown()"+ inputManager.isMouseDown()+ "!buttonAnswerOptions[0].IsActive()"+ !buttonAnswerOptions[0].IsActive());
        if (newMouseClick && inputManager.isMouseDown() && !buttonAnswerOptions[0].IsActive()/*&& type writer ist fertig und sound fertig sonst beenden in anderen methode und keine buttons activ da dann antwort gewaehlt werden muss*/)
        {
            //Debug.Log("Text Manager in next turn");
            if (didAllReactionsEnd())
            {
                //Debug.Log("Text Manager didAllReactionsEnd()");
                sceneManager.CurrentSequenceNummber++;
                //Disable for smooth fade to next scene
                CanvasDialog.enabled = false;
                sceneManager.unloadDialogSystemLoadNewSequenceUnlockPlayer("Sequence"+sceneManager.CurrentSequenceNummber+"Zone1");
            }
            newMouseClick = false;
            if (typeWriter.IsWritingDone)
            {
                //Debug.Log("turn count2:" + turnCounter);
                turnCounter++;
                if (currentTextBox != null)
                {
                    //Debug.Log("turn count3:" + turnCounter);
                    //currentTextBox.text = string.Empty;
                    typeWriter.clearPreviousText();
                    StartCoroutine(invokeNextTypewrittenDialogText());
                    //OnNextTurn?.Invoke();
                    
                }
                
            }
            
            
        }else
        if (!newMouseClick && inputManager.isMouseDown() && !typeWriter.IsWritingDone)
        {
            //Debug.Log("turn count4:" + turnCounter);
            OnEndTypeWriting?.Invoke();
        }else
        if (!inputManager.isMouseDown())
        {
            //Debug.Log("mouse new" + newMouseClick);
            newMouseClick = true;
        }
        /*
        // If there are instructions and the time is beyond the start time of the first instruction...
        if (instructions.Count > 0 )
        {
            // ... set the Text component to display the instruction's message in the correct color.
            textEmma.text = instructions[0].message;
            textEmma.color = instructions[0].textColor;

            // Then remove the instruction.
            instructions.RemoveAt(0);
        }
        // Otherwise, if the time is beyond the clear time, clear the text component's text.
        else if (Time.time >= clearTime)
        {
            textEmma.text = string.Empty;
        }
        */
    }


    /*
    // This function is called from TextReactions in order to display a message to the screen.
    public void DisplayMessage(string message, Color textColor, float delay)
    {
        // The time when the message should start displaying is the current time offset by the delay.
        float startTime = Time.time + delay;

        // Calculate how long the message should be displayed for based on the number of characters.
        float displayDuration = message.Length * displayTimePerCharacter + additionalDisplayTime;

        // Create a new clear time...
        float newClearTime = startTime + displayDuration;

        // ... and if it is after the old clear time, replace the old clear time with the new.
        if (newClearTime > clearTime)
            clearTime = newClearTime;

        // Create a new instruction.
        Instruction newInstruction = new Instruction
        {
            message = message,
            textColor = textColor,
            startTime = startTime
        };

        // Add the new instruction to the collection.
        instructions.Add(newInstruction);

        // Order the instructions by their start time.
        SortInstructions();
    }
    */

    private void writeText(string message, string textObjectName)
    {
        typeWriter = GetComponent<TipewriterEffect>();
        if (textObjectName.Contains("Emma"))
        {
            //Debug.Log("reaction emma");
            typeWriter.Run(message, textEmma);
            currentTextBox = textEmma;
        }else if (textObjectName.Contains("NPC"))
        {
            //Debug.Log("reaction npc");
            int npcNumber;
            if((npcNumber=getObjectNumberFromName(textObjectName))!=-1 &&  npcNumber - 1< textNPCs.Length && 0 < npcNumber)
            {
                typeWriter.Run(message, textNPCs[npcNumber - 1]);
                currentTextBox = textNPCs[npcNumber - 1];
            }
                
        }
        else if (textObjectName.Contains("Button"))
        {
            //Debug.Log("reaction button");
            int buttonNumber;
            if ((buttonNumber = getObjectNumberFromName(textObjectName)) != -1 && buttonNumber - 1 < buttonAnswerOptions.Length && 0 < buttonNumber)
            {
                //Debug.Log("button number: "+ buttonNumber);
                currentTextBox = buttonAnswerOptions[buttonNumber - 1].GetComponentInChildren<TMP_Text>();
                typeWriter.ShowButtonTextImmediately(message, currentTextBox);//antworten werden direct angezeigt.
                setButtonActive(buttonAnswerOptions[buttonNumber - 1], true);
            }
                
        }


    }

    private void setButtonActive(Button answerButton, bool setActive)
    {
        answerButton.gameObject.SetActive(setActive);
    }
    private int getObjectNumberFromName(string textObjectName)
    {
        string b = string.Empty;
        int val;

        for (int i = 0; i < textObjectName.Length; i++)
        {
            if (Char.IsDigit(textObjectName[i]))
                b += textObjectName[i];
        }

        if (b.Length > 0)
            return val = int.Parse(b);

        return -1;
    }
    public void DisplayMessage(string message, Color textColor, string textObjectName)
    {
        //Debug.Log("in DisplayMessage from " + textObjectName);
        writeText(message, textObjectName);

        /*
        // The time when the message should start displaying is the current time offset by the delay.
        float startTime = Time.time;

        // Calculate how long the message should be displayed for based on the number of characters.
        float displayDuration = message.Length * displayTimePerCharacter + additionalDisplayTime;

        // Create a new clear time...
        float newClearTime = startTime + displayDuration;

        // ... and if it is after the old clear time, replace the old clear time with the new.
        if (newClearTime > clearTime)
            clearTime = newClearTime;

        // Create a new instruction.
        Instruction newInstruction = new Instruction
        {
            message = message,
            textColor = textColor,
            //startTime = startTime
            callerName = textObjectName
        };

        // Add the new instruction to the collection.
        instructions.Add(newInstruction);

        // Order the instructions by their start time.
        //SortInstructions();
        */
    }


    /*
    // This function orders the instructions by start time using a bubble sort.
    private void SortInstructions()
    {
        // Go through all the instructions...
        for (int i = 0; i < instructions.Count; i++)
        {
            // ... and create a flag to determine if any reordering has been done.
            bool swapped = false;

            // For each instruction, go through all the instructions...
            for (int j = 0; j < instructions.Count; j++)
            {
                // ... and compare the instruction from the outer loop with this one.
                // If the outer loop's instruction has a later start time, swap their positions and set the flag to true.
                
                if (instructions[i].startTime > instructions[j].startTime)
                {
                    Instruction temp = instructions[i];
                    instructions[i] = instructions[j];
                    instructions[j] = temp;

                    swapped = true;
                }
                
            }

            // If for a single instruction, all other instructions are later then they are correctly ordered.
            if (!swapped)
                break;
        }
    }
    */
}

