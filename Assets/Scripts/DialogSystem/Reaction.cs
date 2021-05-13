using UnityEngine;

// This is the base class for all Reactions.
// There are arrays of inheriting Reactions on ReactionCollections.
public abstract class Reaction : ScriptableObject
{
    //turn of a reaction in a conversation beginning from 1.
    public int reactionTurn;
    private string gameObjectName;

    
    public string GameObjectName
    {
        get
        {
            return gameObjectName;
        }
    }

    // This is called from ReactionCollection.
    // This function contains everything that is required to be done for all
    // Reactions as well as call the SpecificInit of the inheriting Reaction.
    public void Init()
    {
        SpecificInit();
    }

    private void OnEnable()
    {
        gameObjectName = name;
    }
    // This function is virtual so that it can be overridden and used purely
    // for the needs of the inheriting class.
    protected virtual void SpecificInit()
    { }


    // This function is called from ReactionCollection.
    // It contains everything that is required for all for all Reactions as
    // well as the part of the Reaction which needs to happen immediately.
    public void React(MonoBehaviour monoBehaviour)
    {
        ImmediateReaction();
    }

    // This is the core of the Reaction and must be overridden to make things happpen.
    protected abstract void ImmediateReaction();
}
