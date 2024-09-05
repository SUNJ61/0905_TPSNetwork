using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCtrl : MonoBehaviour
{
    private CanvasGroup invenGroup;
    void Start()
    {
        invenGroup = GetComponent<CanvasGroup>();
        invenGroup.alpha = 0.0f;
        invenGroup.blocksRaycasts = false;
    }
    public void OnInventory()
    {
        GameManager.G_instance.isInvenopen = !GameManager.G_instance.isInvenopen;
        invenGroup.alpha = (GameManager.G_instance.isInvenopen) ? 1f : 0f;
        invenGroup.blocksRaycasts = GameManager.G_instance.isInvenopen;
        Time.timeScale = (GameManager.G_instance.isInvenopen) ? 0f : 1f;
    }
}
