using UnityEngine;

// This script acts as a collection for all the
// individual Reactions that happen as a result
// of an interaction.
public class ReactionCollection : MonoBehaviour
{
    //public Reaction[] reactions = new Reaction[0];      // Array of all the Reactions to play when React is called.
    public Reaction[] reactions;
    private int lastReactionToPlay;
    private int lastReactionToPlayTmp;

    private void OnEnable()
    {
        if (reactions == null)
            return;

        //reactions = new Reaction[0];
        TextManager.OnNextTurn += React;
        // Go through all the Reactions and call their Init function.
        for (int i = 0; i < reactions.Length; i++)
        {
            // The DelayedReaction 'hides' the Reaction's Init function with it's own.
            // This means that we have to try to cast the Reaction to a DelayedReaction and then if it exists call it's Init function.
            // Note that this mainly done to demonstrate hiding and not especially for functionality.
            reactions[i].Init();
        }
        lastReactionToPlay = 0;
        lastReactionToPlayTmp = -1;
        SortReactions();
        assignGameObjectName();
        React();
    }

    private void printReactions()
    {
        for (int i=0; i< reactions.Length; i++)
        {
            Debug.Log("Reactions of:" + gameObject.name + "reaction nr"+i+ " turn: "+ reactions[i].reactionTurn );
        }
    }
    private void OnDisable()
    {
        TextManager.OnNextTurn -= React;
    }
    public void React()
    {
        Debug.Log("reactions" + gameObject.name + " : "+ reactions);
        if (reactions == null)
            return;

        //Debug.Log("inReact" +gameObject.name+"  " + "  reactions.Length: "+ reactions.Length);
        //Debug.Log("current turn:"+ TextManager.TurnCounter);
        for (int i = lastReactionToPlay; i < reactions.Length; i++)
        {
            //Debug.Log("inReact2 "+reactions[i].GameObjectName+ ": " + reactions[i].reactionTurn);
            if (TextManager.TurnCounter == reactions[i].reactionTurn)
            {
                Debug.Log("inReact3" + reactions[i].GameObjectName);
                lastReactionToPlay++;
                reactions[i].React(this);
            }
        }

        //Debug.Log("current index in " + gameObject.name  +" : "+ lastReactionToPlay);
    /*
        if (TextManager.TurnCounter==reactions[lastReactionToPlay].reactionTurn lastReactionToPlay < reactions.Length)
        {
            reactions[lastReactionToPlay].React(this); //Play text reaction
            if(reactions[lastReactionToPlay + 1].GetType() == typeof(AudioReaction))
                reactions[lastReactionToPlay+1].React(this); //Play Sound reaction
            lastReactionToPlay++;
        }
        */

        /*
        for (int i = 0; i < reactions.Length; i++)
        {
             reactions[i].React(this);
        }
        */
    }

    public bool reactionsFinished()
    {
        return lastReactionToPlay == reactions.Length;
    }

    private void assignGameObjectName()
    {
        foreach (Reaction r in reactions)
        {
            // The DelayedReaction 'hides' the Reaction's Init function with it's own.
            // This means that we have to try to cast the Reaction to a DelayedReaction and then if it exists call it's Init function.
            // Note that this mainly done to demonstrate hiding and not especially for functionality.
            r.setGameObjectName(gameObject.name);
        }
        
    }
    private void SortReactions()
    {
        // Go through all the instructions...
        for (int i = 0; i < reactions.Length; i++)
        {
            // ... and create a flag to determine if any reordering has been done.
            //bool swapped = false;

            // For each instruction, go through all the instructions...
            for (int j = 0; j < reactions.Length; j++)
            {
                // ... and compare the instruction from the outer loop with this one.
                // If the outer loop's instruction has a later start time, swap their positions and set the flag to true.
                
                if (reactions[i].reactionTurn < reactions[j].reactionTurn)
                {
                    Reaction temp = reactions[i];
                    reactions[i] = reactions[j];
                    reactions[j] = temp;

                    //swapped = true;
                }
                
            }

            // If for a single instruction, all other reaction turns are later then they are correctly ordered.
            //if (!swapped)
            //    break;
        }
    }
}
