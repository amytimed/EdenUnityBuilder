using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Block highlight controller. For example, when you place or remove a block this shows preview of block
/// </summary>
public class HighlighterController : MonoBehaviour
{

    public GameObject HighlighterPrefab;

    public Mesh HighlightMesh;

    public Material AddBlockMaterial;

    public Material DestroyBlockMaterial;

    public List<MultiTouchHighlight> Highlighters = new List<MultiTouchHighlight>();

    public class MultiTouchHighlight
    {
        public GameObject HighlighterInstance;
        public Touch HightlightTouch;

        public MultiTouchHighlight(GameObject instance)
        {
            HighlighterInstance = instance;
        }
    }

    /// <summary>
    /// Adds new block hightlight
    /// </summary>
    public void AddHightlight(Vector3 pos, bool isRemoving)
    {
        GameObject highlight = Instantiate(HighlighterPrefab);
        highlight.transform.position = pos;
        if (!isRemoving)
        {
            highlight.GetComponentInChildren<MeshRenderer>().material = AddBlockMaterial;
        }
        else
        {
            highlight.GetComponentInChildren<MeshRenderer>().material = DestroyBlockMaterial;
        }
        highlight.GetComponentInChildren<MeshFilter>().sharedMesh = BlockPreview.MeshBlock;
        Highlighters.Add(new MultiTouchHighlight(highlight));
    }

    /// <summary>
    /// Setup all hightlighters
    /// </summary>
    public void SetPositions(Vector3[] positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            Highlighters[i].HighlighterInstance.transform.position = positions[i];
        }
    }

    public void RemoveHighlights()
    {
        for (int i = 0; i < Highlighters.Count; i++)
        {
            Destroy(Highlighters[i].HighlighterInstance);
        }
        Highlighters.Clear();
    }

    public void RemoveHighlight(Touch t)
    {
        for (int i = 0; i < Highlighters.Count; i++)
        {
            if (t.fingerId == Highlighters[i].HightlightTouch.fingerId)
            {
                Destroy(Highlighters[i].HighlighterInstance);
                Highlighters.RemoveAt(i);
            }
        }
    }

    public Vector3Int GetHighlightPosition(Touch t)
    {
        for (int i = 0; i < Highlighters.Count; i++)
        {
            if (Highlighters[i] != null && t.fingerId == Highlighters[i].HightlightTouch.fingerId)
            {
                return new Vector3Int((int)Highlighters[i].HighlighterInstance.transform.position.x, (int)Highlighters[i].HighlighterInstance.transform.position.y, (int)Highlighters[i].HighlighterInstance.transform.position.z);
            }
        }
        return Vector3Int.zero;
    }
}
