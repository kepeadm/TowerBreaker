using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject inventory;
    public void OpenInventory()
    {
        buttons.SetActive(false);
        inventory.SetActive(true);
    }
    public void CloseInventory()
    {
        inventory.SetActive(false);
        buttons.SetActive(true);
    }
}