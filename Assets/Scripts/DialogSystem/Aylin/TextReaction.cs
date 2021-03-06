using UnityEngine;

// This Reaction has a delay but is not a DelayedReaction.
// This is because the TextManager component handles the
// delay instead of the Reaction.
public class TextReaction : Reaction
{
    public string message;                      // The text to be displayed to the screen.
    public Color textColor = Color.white;       // The color of the text when it's displayed (different colors for different characters talking).
    //public float delay;                         // How long after the React function is called before the text is displayed.


    private TextManager textManager;            // Reference to the component to display the text.

    private void OnEnable()
    {
        //textManager = FindObjectOfType<TextManager>();
    }
    protected override void SpecificInit()
    {
    }


    protected override void ImmediateReaction()
    {
        textManager = API.TextManager;
        //Debug.Log("reactionname: " + GameObjectName + "reaction message: "+ message);
        //textManager.DisplayMessage(message, textColor, delay);
        textManager.DisplayMessage(message, textColor, GameObjectName);
    }
}