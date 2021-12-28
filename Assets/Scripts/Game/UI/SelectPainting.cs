using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPainting : MonoBehaviour
{
    public Paintings Painting;

    void Start()
    {

    }

    public void Init(Paintings p)
    {
        Painting = p;
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(Select);
        Image img = gameObject.GetComponent<Image>();
        Color c = new Color(0, 0, 0);
        PaintController.PaintingDictonary.TryGetValue(Painting, out c);
        c.a = 1f;
        img.color = c;
    }

    public void Select()
    {
        BuildController.Instance.SelectedColor = Painting;
        BuildController.Instance.InventoryColors.SetActive(false);
    }
}