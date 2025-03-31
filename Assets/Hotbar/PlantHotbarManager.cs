using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlantHotbarManager : MonoBehaviour
{
    [Header("Hotbar Slots")]
    public Image[] hotbarSlots;
    public TextMeshProUGUI[] quantityTexts;
    public List<PlantData> allPlantData;

    private Dictionary<string, HotbarItem> plantInventory = new Dictionary<string, HotbarItem>(); // Store HotbarItem instead of int
    private int selectedSlot = -1;

    void Start()
    {
        foreach (var slot in hotbarSlots)
        {
            slot.gameObject.SetActive(false);
        }

        foreach (var text in quantityTexts)
        {
            text.text = "";
        }
    }

    void Update()
    {
        if (plantInventory.Count > 0)
        {
            HandleHotbarInput();
        }
    }

    void HandleHotbarInput()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSlot(i);
                return;
            }
        }
    }

    void SelectSlot(int slotIndex)
    {
        if (selectedSlot != -1 && hotbarSlots[selectedSlot].gameObject.activeSelf)
        {
            ResetSlotColor(hotbarSlots[selectedSlot]);
        }

        selectedSlot = slotIndex;

        if (hotbarSlots[selectedSlot].gameObject.activeSelf)
        {
            DarkenSlot(hotbarSlots[selectedSlot]);
        }
    }

    void DarkenSlot(Image slotImage)
    {
        slotImage.color = new Color(slotImage.color.r * 0.5f, slotImage.color.g * 0.5f, slotImage.color.b * 0.5f, slotImage.color.a);
    }

    void ResetSlotColor(Image slotImage)
    {
        slotImage.color = new Color(1f, 1f, 1f, slotImage.color.a);
    }

    public PlantData GetSelectedPlantData()
    {
        if (selectedSlot != -1 && hotbarSlots[selectedSlot].gameObject.activeSelf)
        {
            var hotbarItem = plantInventory.Values.FirstOrDefault(item => item.slotImage == hotbarSlots[selectedSlot]);
            Debug.Log($"Plant selected in slot {selectedSlot}: {hotbarItem?.plantData?.plantName}");
            return hotbarItem?.plantData;
        }

        Debug.LogWarning("No slot is selected or the selected slot is inactive.");
        return null;
    }

    public PlantData GetPlantDataForSlot(Transform slotTransform)
    {
        int slotIndex = System.Array.IndexOf(hotbarSlots, slotTransform.GetComponent<Image>());

        if (slotIndex != -1)
        {
            var hotbarItem = plantInventory.Values.FirstOrDefault(item => item.slotIndex == slotIndex);
            return hotbarItem?.plantData;
        }
        return null;
    }

    public void AddPlantToHotbar(PlantData plantData)
    {
        if (plantData.plantEffects.Contains("Poisonous"))
        {
            Debug.Log($"{plantData.plantName} is poisonous and will not be added to the hotbar.");
            return;
        }

        if (plantInventory.ContainsKey(plantData.plantName))
        {
            plantInventory[plantData.plantName].IncreaseQuantity();
        }
        else
        {
            for (int i = 0; i < hotbarSlots.Length; i++)
            {
                if (!hotbarSlots[i].gameObject.activeSelf)
                {
                    var hotbarItem = new HotbarItem(plantData, hotbarSlots[i], quantityTexts[i], i);
                    plantInventory[plantData.plantName] = hotbarItem;
                    UpdateHotbarDisplay();
                    return;
                }
            }

            Debug.LogWarning("No empty slot available in the hotbar!");
        }

        UpdateHotbarDisplay();
    }

    public void ReducePlantQuantity(PlantData plantData)
    {
        if (plantInventory.ContainsKey(plantData.plantName))
        {
            plantInventory[plantData.plantName].DecreaseQuantity();
            if (plantInventory[plantData.plantName].quantity <= 0)
            {
                plantInventory[plantData.plantName].ClearSlot();
                plantInventory.Remove(plantData.plantName);
            }
            UpdateHotbarDisplay();
        }
    }

    void UpdateHotbarDisplay()
    {
        foreach (var item in plantInventory.Values)
        {
            item.UpdateUI();
        }
    }
}
