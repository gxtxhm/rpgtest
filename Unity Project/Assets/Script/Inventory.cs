using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory instance;
    private void Awake()
    {
        if (instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    #endregion

    public delegate void OnChangeItem();
    public OnChangeItem onChangeItem;

    public List<Item> items = new List <Item>();
    private void Start()
    {
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
        if (collision.CompareTag("FieldItem")) 
        {
                FieldItem fieldItem = collision.GetComponent<FieldItem>();
                if (AddItem(fieldItem.GetItem())) 
                {
                    fieldItem.DestroyItem();
                }
            }
    }

}
