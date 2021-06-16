using System.Collections;
using UnityEngine;

// This Reaction is used to play sounds through a given AudioSource.
// Since the AudioSource itself handles delay, this is a Reaction
// rather than an DelayedReaction.
public class AudioReaction : Reaction
{
    //------------------------------------------------- Implemenetation of Sounds WITHOUT Fmod:  Start-------------------------------------------------
    /*
    public AudioSource audioSource;     // The AudioSource to play the clip.
    public AudioClip audioClip;         // The AudioClip to be played.
    

    protected override void ImmediateReaction()
    {
        // Set the AudioSource's clip to the given one and play with the given delay.
        if (audioSource != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
    */
    //------------------------------------------------- Implemenetation of Sounds WITHOUT Fmod:  End-------------------------------------------------




    //------------------------------------------------- Implemenetation of Sounds WITH Fmod:  Start-------------------------------------------------

    [FMODUnity.EventRef]
    public string textAudio; // Reference to the AudioClip to be played.
    private FMOD.Studio.EventInstance audioEvent;
    //private FMOD.Studio.EventInstance audioEvent2;
    private FMOD.Studio.EventDescription eventDescription;

    private void OnEnable()
    {
        if (textAudio.Equals("") || textAudio == null)
            return;

        eventDescription = FMODUnity.RuntimeManager.GetEventDescription(textAudio);
        eventDescription.createInstance(out audioEvent);

        TextManager.OnEndTypeWriting += stopCurrentSound;
    }
    private void OnDisable()
    {
        if (textAudio == null)
            return;

        TextManager.OnEndTypeWriting -= stopCurrentSound;
        releaseSound(audioEvent);
    }

    protected override void ImmediateReaction()
    {
        //Debug.Log("start sound1: "+ textAudio);

        if (textAudio.Length!=0)
        {

            //This is only not working for the first loaded voice in the dream scene. If one should be played with the first reaction
            //Thats way the first audio reaction need to be in the scene as seperate objecgt with a fmod emiiter event and an collider
            //which is not on the player (to not collide with it) and above all colliders (z=100) and play on start (stop on mouse down)
            //This way the first sound gets activated right away and stops when the text is stopped.
            //To see the object look at SoundToPlayOnLoad game object of scene Sequence1DialogSystem which is in the folder Assets/Scenes/DevelopmentTests/Aylin/DialogSystem
            //FMODUnity.RuntimeManager.PlayOneShot(textAudio); would work for the first sound but then it cant be stopped when wanting to skip the text.
            audioEvent.start();
        }
    }


    private void stopSound(FMOD.Studio.EventInstance instance)
    {
        //Debug.Log("stop sound");
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void releaseSound(FMOD.Studio.EventInstance instance)
    {
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
        instance.clearHandle();
    }

    private void stopCurrentSound()
    {
        stopSound(audioEvent);
    }
    //------------------------------------------------- Implemenetation of Sounds WITH Fmod:  End-------------------------------------------------

}