using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    KeyCode openInventory = KeyCode.E;
    public GameObject[] inventoryObjects;
    Sprite[] sprites;
    int selectedIndex;
    bool isSelected;
    Image[] images;
    string[] imageNames;
    public int inventoryObjectCount = 0;
    public int maxObjectCount = 10;
    bool isOpen;
    Interaction interaction;
    private void Awake()
    {
        selectedIndex = 9999;
        inventoryObjects = new GameObject[maxObjectCount];
        images = new Image[transform.GetChild(0).childCount];
        sprites = new Sprite[maxObjectCount];
        imageNames = new string[maxObjectCount];
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            images[i] = transform.GetChild(0).GetChild(i).GetComponent<Image>();
        }
    }
    private void Update()
    {
        for (int i = 0; i < inventoryObjectCount; i++)
        {
            images[i].sprite = sprites[i];
        }
        if (Input.GetKeyDown(openInventory))
        {
            transform.GetChild(0).gameObject.SetActive(!isOpen);
            isOpen = !isOpen;
        }
        else { }
        InventoryInteraction();
    }

    private void InventoryInteraction()
    {
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown(i.ToString()) && !isSelected && images[i].color.a != 0)
            {
                images[i].color = Color.gray;
                selectedIndex = i - 1;
                isSelected = !isSelected;
            }
            else if (Input.GetKeyDown(i.ToString()) && selectedIndex == i && isSelected && images[i].color.a != 0)
            {
                images[i].color = Color.white;
                selectedIndex = 9999;
                isSelected = !isSelected;
            }
        }
        if (selectedIndex < 10)
        {
            if (interaction.InventoryInteract() && interaction.InventoryInteractObject().name == imageNames[selectedIndex] && Input.GetMouseButtonDown(0) & isSelected)
            {
                Transform newTransform = interaction.InventoryInteractObject().transform.parent;
                Destroy(interaction.InventoryInteractObject());
                GameObject placedObject = Instantiate(inventoryObjects[selectedIndex], newTransform);
                placedObject.SetActive(true);
                placedObject.layer = 0;
                Destroy(inventoryObjects[selectedIndex]);
            }
        }
    }

    public void NewObject(GameObject newObject, Sprite inventoryImage, Interaction _interaction)
    {
        interaction = _interaction;
        inventoryObjects[inventoryObjectCount] = newObject;
        sprites[inventoryObjectCount] = inventoryImage;
        imageNames[inventoryObjectCount] = newObject.name;
        images[inventoryObjectCount].color = Color.white;
        inventoryObjectCount++;
    }
}
