using UnityEngine;

public class GameObjectSpriteSaver : Saver
{
    public SpriteRenderer spriteRendererToSaveSpriteOf;     // Reference to the GameObject that will have its sprite saved from and loaded to.
    private Texture2D tex;


    protected override string SetKey()
    {
        // Here the key will be based on the name of the gameobject, the gameobject's type and a unique identifier.
        return spriteRendererToSaveSpriteOf.name + spriteRendererToSaveSpriteOf.GetType().FullName + uniqueIdentifier;
    }


    protected override void Save()
    {
        //Debug.Log("saved");
        saveData.Save(key, spriteRendererToSaveSpriteOf.sprite);
    }


    protected override void Load()
    {
        tex = new Texture2D(128, 128);
        //Debug.Log("loaded");
        // Create a variable to be passed by reference to the Load function.
        Sprite spriteState = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        // If the load function returns true then the activity can be set.
        if (saveData.Load(key, ref spriteState))
            spriteRendererToSaveSpriteOf.sprite=spriteState;
    }
}
