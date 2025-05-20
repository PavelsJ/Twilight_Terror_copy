using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    public static UI_Inventory Instance { get; private set; }
    
    public Transform selectedSlotTransform;
    public Image[] itemSlots;
    
    private int currentSlotIndex = 0; 
    private int selectedSlotIndex = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (currentSlotIndex > 1)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SelectItem(Vector3.left);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                SelectItem(Vector3.right);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveItem();
        }
    }

    private void SelectItem(Vector3 direction)
    {
        if (currentSlotIndex == 0)
        {
            selectedSlotTransform.gameObject.SetActive(false);
            return;
        }

        if (direction == Vector3.left)
        {
            selectedSlotIndex = (selectedSlotIndex - 1 + currentSlotIndex) % currentSlotIndex;
        }
        else if (direction == Vector3.right)
        {
            selectedSlotIndex = (selectedSlotIndex + 1) % currentSlotIndex;
        }

        UpdateSelection();
    }
    
    public void AddItem(GameObject itemPrefab, int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            if (currentSlotIndex < itemSlots.Length)
            {
                Instantiate(itemPrefab, itemSlots[currentSlotIndex].transform);
                selectedSlotIndex = currentSlotIndex;
                currentSlotIndex++;

                UpdateSelection();
            }
            else
            {
                Debug.LogWarning("Недостаточно слотов для добавления предмета");
                break;
            }
        }
    }
    
    public void RemoveItem()
    {
        if (selectedSlotIndex < itemSlots.Length && itemSlots[selectedSlotIndex].transform.childCount > 0)
        {
            Music_Manager.instance.PlaySound(Music_Manager.SoundType.ItemExpire);
            
            GameObject child = itemSlots[selectedSlotIndex].transform.GetChild(0).gameObject;
            
            var item = child.GetComponent<UI_Item>();
            if (item != null)
            {
                item.UseItem();
            }

            Destroy(child);

            ShiftItems();

            UpdateSelection();
        }
        else
        {
            Debug.LogWarning("Выбранный слот пуст или некорректен");
        }
    }

    private void ShiftItems()
    {
        for (int i = selectedSlotIndex; i < currentSlotIndex - 1; i++)
        {
            if (itemSlots[i + 1].transform.childCount > 0)
            {
                Transform movingItem = itemSlots[i + 1].transform.GetChild(0);
                movingItem.SetParent(itemSlots[i].transform);
                movingItem.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            }
        }

        currentSlotIndex--;

        if (currentSlotIndex > 0)
        {
            selectedSlotIndex = Mathf.Clamp(selectedSlotIndex, 0, currentSlotIndex - 1);
        }
    }
    

    private void UpdateSelection()
    {
        if (currentSlotIndex > 0)
        {
            selectedSlotTransform.gameObject.SetActive(true);
            selectedSlotTransform.position = itemSlots[selectedSlotIndex].transform.position + new Vector3( 48f, 0);
        }
        else
        {
            selectedSlotTransform.gameObject.SetActive(false);
        }
    }

    public bool IsInventoryEmpty()
    {
        return currentSlotIndex == 0;
    }

    public bool IsInventoryFull()
    {
        return currentSlotIndex >= itemSlots.Length;
    }
}