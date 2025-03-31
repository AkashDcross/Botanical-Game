using UnityEngine;
using UnityEngine.EventSystems;

public class HotbarItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private PlantHotbarManager hotbarManager;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        hotbarManager = FindObjectOfType<PlantHotbarManager>(); // Ensure you have a reference to the PlantHotbarManager
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root); // Move to the top of the hierarchy to follow the mouse
        canvasGroup.blocksRaycasts = false; // Prevent the item from blocking raycasts during drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition; // Follow the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject droppedOn = eventData.pointerEnter; // The object the item is dropped on

        if (droppedOn != null)
        {
            Debug.Log($"Dropped on: {droppedOn.name}");

            if (droppedOn.CompareTag("NPC"))
            {
                Debug.Log("Dropped on an NPC.");
                NPC npc = droppedOn.GetComponent<NPC>();
                if (npc != null && hotbarManager != null)
                {
                    PlantData plantData = hotbarManager.GetPlantDataForSlot(originalParent); // Get the corresponding plant data
                    if (plantData != null && npc.TreatWithPlant(plantData))
                    {
                        // Reduce the stack count in the hotbar
                        hotbarManager.ReducePlantQuantity(plantData);
                        Debug.Log($"{plantData.plantName} used on {npc.name}.");
                    }
                }
            }
        }

        // Return item to its original place if not dropped on a valid NPC
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
        canvasGroup.blocksRaycasts = true;
    }

}
