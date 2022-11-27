using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    
    public GameObject inventoryPanel;
    bool activeInventory = false;

    public delegate void OnChangeItem();
    public OnChangeItem onChangeItem;

    List<Item> items = new List <Item>();
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) 
        {
            activeInventory = !activeInventory;
            inventoryPanel.SetActive(activeInventory);
        }
    }

    public bool AddItem(Item _item) 
    {
        if (items.Count < 9) 
        {
            items.Add(_item);
            if (onChangeItem != null)
              onChangeItem.Invoke();
            return true;
        }
        return false;
    }
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Get1");
        if (collision.CompareTag("FieldItem")) 
        {
                Debug.Log("Get2");
                FieldItem fieldItem = collision.GetComponent<FieldItem>();
                if (AddItem(fieldItem.GetItem())) 
                {
                    fieldItem.DestroyItem();
                }
            }
    }
}
