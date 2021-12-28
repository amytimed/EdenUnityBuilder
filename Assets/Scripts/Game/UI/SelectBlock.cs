using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectBlock : MonoBehaviour
{
    public BlockType Block;

    void Start()
    {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(Select);
    }

    public void Select()
    {
        BuildController.Instance.SelectedBlock = Block;
        BuildController.Instance.InventoryBlocks.SetActive(false);
        BuildController.Instance.UpdateBlockPreview();
    }
}