using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperPillow : ActionObject
{
    //sprite which should appear after player putting together the helper
    public Sprite spritePillowSloved;
    private SpriteRenderer spriteRendererForPillow;
    private Collider2D colliderOfPillow;
    // Start is called before the first frame update
    void Start()
    {
        spriteRendererForPillow = GetComponent<SpriteRenderer>();
        colliderOfPillow = GetComponent<Collider2D>();
        if (spriteRendererForPillow.sprite.Equals(spritePillowSloved))
            colliderOfPillow.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void actionOnDrop(string itemName)
    {
        //also set start position of helper
        if (colliderOfPillow != null)
            colliderOfPillow.enabled = false;
        AvatarManager.helperAvatar.gameObject.SetActive(true);
        //PlayerControler.scaleAvatar(AvatarManager.helperAvatar);
        if (spriteRendererForPillow != null)
        {
            spriteRendererForPillow.sprite = spritePillowSloved;
            spriteRendererForPillow.sortingOrder = 2;
        }
        if (AvatarManager.helperAnimator != null)
            AvatarManager.helperAnimator.SetTrigger("Idle");
    }

}
