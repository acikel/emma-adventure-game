using UnityEngine;

// This Reaction is used to play sounds through a given AudioSource.
// Since the AudioSource itself handles delay, this is a Reaction
// rather than an DelayedReaction.
public class AudioReaction : Reaction
{
    private void OnEnable()
    {
        TextManager.OnNextTurn += StopSound;
    }

    private void OnDisable()
    {
        TextManager.OnNextTurn -= StopSound;
    }
    private void StopSound()
    {

    }
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

    protected override void ImmediateReaction()
    {
        // Set the AudioSource's clip to the given one and play with the given delay.
        if (textAudio.Length!=0)
        {
            FMODUnity.RuntimeManager.PlayOneShot(textAudio);
        }
    }
    
    //------------------------------------------------- Implemenetation of Sounds WITH Fmod:  End-------------------------------------------------

}