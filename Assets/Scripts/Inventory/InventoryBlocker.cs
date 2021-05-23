using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Dieses script muss als erstes gameobject von den UI elementen sein (in der Reihenfolge im Canvas) und kein Child object von einem Canvas was
//nicht geblockt werden soll. Sonst schaltet dieses script die functionalität (onDrag, OnDrop usw) aller game objects oberhalb von ihm aus.
//Ausserdem muss das gameobject welches dieses script enthält ein image mit raycast target und gewähltem surce image entahlten. Sonst functioniert es nicht. (dafür ist kein collider nötig).
public class InventoryBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
{
    private Inventory inventory;
    private SceneManager sceneManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        
        sceneManager = API.SceneManager;
        if (!sceneManager.IsFading)
        {
            //Debug.Log("Entered");
            inventory = API.Inventory;
            inventory.InteractionWithUIActive = false;
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Entered2");
        inventory = API.Inventory;
        inventory.InteractionWithUIActive = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        sceneManager = API.SceneManager;
        if (!sceneManager.IsFading)
        {
            //Debug.Log("Entered3");
            inventory = API.Inventory;
            inventory.InteractionWithUIActive = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Entered4");
        inventory = API.Inventory;
        inventory.InteractionWithUIActive = true;
    }
}
