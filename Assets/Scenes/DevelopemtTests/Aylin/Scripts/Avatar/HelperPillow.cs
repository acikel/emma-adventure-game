using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperPillow : ActionObject
{
    //sprite which should appear after player putting together the helper
    public Sprite spritePillowSloved;
    private SpriteRenderer spiteRendererForPillow;
    // Start is called before the first frame update
    void Start()
    {
        spiteRendererForPillow = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void actionOnDrop(string itemName)
    {
        //also set start position of helper
        AvatarManager.helperAvatar.gameObject.SetActive(true);
        if (spiteRendererForPillow != null)
            spiteRendererForPillow.sprite = spritePillowSloved;
        if (AvatarManager.helperAnimator != null)
            AvatarManager.helperAnimator.SetTrigger("Idle");

    }

}
