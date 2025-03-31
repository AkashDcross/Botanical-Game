using UnityEngine;
using UnityEngine.UI;

public class ContextMenuManager : MonoBehaviour
{
    public static ContextMenuManager Instance;
    public GameObject contextMenu;
    public Button giveMedicineButton;

    private NPC selectedNPC;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        contextMenu.SetActive(false);

        if (giveMedicineButton != null)
        {
            giveMedicineButton.onClick.AddListener(OnGiveMedicineClicked);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                NPC npc = hit.collider.GetComponent<NPC>();
                if (npc != null)
                {
                    Debug.Log($"Right-clicked on NPC: {npc.npcData.name}");
                    ShowContextMenu(npc);
                }
            }
        }
    }

    public void ShowContextMenu(NPC npc)
    {
        selectedNPC = npc;
        contextMenu.transform.position = Input.mousePosition;
        contextMenu.SetActive(true);
        Debug.Log($"Context menu shown for {npc.npcData.name}.");
    }

    public void OnGiveMedicineClicked()
    {
        Debug.Log("Give Medicine button clicked.");

        PlantHotbarManager hotbarManager = FindObjectOfType<PlantHotbarManager>();
        PlantData selectedPlant = hotbarManager?.GetSelectedPlantData();

        if (selectedPlant != null && selectedNPC != null)
        {
            Debug.Log($"Selected plant: {selectedPlant.plantName}");
            Debug.Log($"Selected NPC: {selectedNPC.npcData.name}");

            bool success = selectedNPC.TreatWithPlant(selectedPlant);
            if (success)
            {
                hotbarManager.ReducePlantQuantity(selectedPlant);
            }
        }
        else
        {
            Debug.Log("No plant selected or no NPC selected.");
        }

        contextMenu.SetActive(false); // Hide the context menu after the interaction
        Debug.Log("Context menu hidden.");
    }
}
