using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarItem
{
    public PlantData plantData { get; private set; }
    public Image slotImage { get; private set; }
    public TextMeshProUGUI quantityText { get; private set; }
    public int quantity { get; private set; }
    public int slotIndex { get; private set; }

    public HotbarItem(PlantData data, Image slot, TextMeshProUGUI text, int index)
    {
        plantData = data;
        slotImage = slot;
        quantityText = text;
        quantity = 1;
        slotIndex = index;

        slotImage.gameObject.SetActive(true);
        slotImage.sprite = plantData.plantImage;
        UpdateUI();
    }

    public void IncreaseQuantity()
    {
        quantity++;
        UpdateUI();
    }

    public void DecreaseQuantity()
    {
        quantity--;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (quantity <= 0)
        {
            ClearSlot();
        }
        else
        {
            quantityText.text = quantity.ToString();
        }
    }

    public void ClearSlot()
    {
        slotImage.gameObject.SetActive(false);
        quantityText.text = "";
    }
}
