using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bicycle : ActionObject
{
    private SceneManager sceneManager;
    private string outroSceneNameToLoad = "Sequence2Zone1Outro";
    private Inventory inventory;

    public Sprite spriteBicycleRepaired;
    private SpriteRenderer spriteRendererForBicycle;


    private void Start()
    {
        spriteRendererForBicycle = GetComponent<SpriteRenderer>();
        sceneManager = API.SceneManager;
        inventory = API.Inventory;
    }
    // Update is called once per frame
    void Update()
    {

    }

    public override void actionOnDrop(string itemName)
    {
        //Debug.Log("actions drop name " + itemName);
        if (itemName.Contains("Wheel"))
        {
            spriteRendererForBicycle.sprite = spriteBicycleRepaired;
            StartCoroutine(waitAndLoadDialogSystem(1.5f));
        }
    }

    private IEnumerator waitAndLoadDialogSystem(float seconds)
    {
        //Debug.Log("waitAndLoadDialogSystem1");
        inventory.InteractionWithUIActive = true;
        yield return new WaitForSeconds(seconds);
        //Debug.Log("waitAndLoadDialogSystem2");
        sceneManager.loadNextSceneHideInventoryAndAvatars(outroSceneNameToLoad);
    }
}
