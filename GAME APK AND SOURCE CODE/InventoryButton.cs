using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;


public class InventoryButton : MonoBehaviour
{
    [SerializeField] GameObject InventoryMenu;
    [SerializeField] GameObject satchelItems;

    public void Awake()
    {
        InventoryMenu.SetActive(false);
    }

    public void Inventory()
    {
        if(InventoryMenu.activeSelf)
        {
            InventoryMenu.SetActive(false);
            Time.timeScale=1f;
        }
        else if(!InventoryMenu.activeSelf)
        {
            InventoryMenu.SetActive(true);
            Time.timeScale=0f;
            GetItems();
        }
    }

    public List<Transform> GetSatchelChildren()
    {
        List<Transform> children = new List<Transform>();
        foreach(Transform child in satchelItems.transform)
        {
            children.Add(child);
            Debug.Log(child);
        }
        return children;
    }

    public void GetItems()
    {
        List<SatchelItem> inventory = Player.Instance.GetInventory();
        List<Transform> children = GetSatchelChildren();
        for(int x = 0; x < inventory.Count; x++)
        {
            inventory.ElementAt(x).SetItemPosition(children.ElementAt(x).position);
            inventory.ElementAt(x).UpdateIconParent(satchelItems.transform);
        }
    }

}
