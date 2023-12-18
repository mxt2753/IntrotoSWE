using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : GridObject
{
    GameObject itemPicture;
    GameObject originalItemPicture;

    private bool pickedUp = false;
    private string itemObjectName;

    public Item(Vector3 position, float cellSize, int x, int y, string printName, string itemObjectName) : base(position, cellSize, x, y, printName)
    {
        originalItemPicture = GameObject.Find(itemObjectName);
        itemPicture = GameObject.Instantiate(originalItemPicture, position + new Vector3(0, 0, -1), Quaternion.identity);
    }

    public void PickUp()
    {
        if((Player.Instance.GetInventory().Count < 3) && (this is SatchelItem))
        {
            pickedUp = true;
            GameObject.Destroy(itemPicture);
        }
        else 
        {
            pickedUp = true;
            GameObject.Destroy(itemPicture);
        }
    }
}

public class GoalItem : Item
{
    public GoalItem(Vector3 position, float cellSize, int x, int y, string printName, string itemObjectName) : base(position, cellSize, x, y, printName, itemObjectName)
    {

    }
}

public class SatchelItem : Item
{
    GameObject originalIcon;
    GameObject itemIcon;

    public SatchelItem(Vector3 position, float cellSize, int x, int y, string printName, string itemObjectName) : base(position, cellSize, x, y, printName, itemObjectName)
    {
        originalIcon = GameObject.Find(itemObjectName + "Icon");
        itemIcon = GameObject.Instantiate(originalIcon, new Vector3(-100, -100, -100), Quaternion.identity);
    }

    public GameObject GetPicture()
    {
        return itemIcon;
    }

    public void UpdateIconParent(Transform parentTransform)
    {
        itemIcon.transform.SetParent(parentTransform);
    }

    public void SetItemPosition(Vector3 newPosition)
    {
        itemIcon.transform.position = newPosition;
    }
}

public class HealthPotion : SatchelItem
{
    public HealthPotion(Vector3 position, float cellSize, int x, int y, string printName, string itemObjectName) : base(position, cellSize, x, y, printName, itemObjectName)
    {

    }
}
