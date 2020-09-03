using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/*
 * TODO:
 * - update position when ending drag
 * - delete key, value when deleting asset
 */

public class MovableAsset : MonoBehaviour
{
    private LevelEditor editor;
    private PolygonCollider2D collider;

    private bool isDragging;
    private bool prevDragging;
    private bool isSelected;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<PolygonCollider2D>();
        editor = LevelEditor.Instance;

        isDragging = false;
        prevDragging = false;
    }

    private void Update()
    {
        Vector3 mousePos = FrequentlyAccessed.Instance.camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Collider2D overlapped = Physics2D.OverlapPoint(mousePos);

        if (Mouse.current.leftButton.isPressed)
        {
            if ((collider.transform.GetComponentInParent<PolygonCollider2D>().Equals(overlapped) 
                || collider.transform.GetComponent<PolygonCollider2D>().Equals(overlapped))
                && !prevDragging && !editor.isDrawing && !editor.isDragging)
            {
                Select();
                Drag();
            }
        }
        else if (prevDragging)
        {
            Drop();
        }

        if (isSelected)
        {
            if (editor.isDrawing)
            {
                Deselect();
            }
            editor.selectedIcon.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

        prevDragging = isDragging;
    }

    public void Select()
    {
        editor.SetSelected(this.gameObject);
        editor.selectedIcon.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        editor.selectedIcon.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 0.5f);

        isSelected = true;
    }

    public void Deselect()
    {
        isSelected = false;
        editor.ResetSelected(this.gameObject);
        editor.selectedIcon.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 0);
    }

    private void Drag()
    {
        editor.RemoveFromAssetList(this.gameObject);

        editor.isDragging = true;
        isDragging = true;
        editor.SetBrushPreview(this.gameObject);
    }

    private void Drop()
    {
        editor.AddToAssetList(this.gameObject);

        editor.isDragging = false;
        isDragging = false;
        editor.ResetBrushPreview();
    }
}
