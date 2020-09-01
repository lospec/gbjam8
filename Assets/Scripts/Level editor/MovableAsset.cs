using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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
            if (collider.Equals(overlapped) && !prevDragging && !editor.isDrawing)
            {
                Select();
                Drag();
            }
        }
        else if (prevDragging)
        {
            Drop();
        }

        if (overlapped != null && overlapped.Equals(collider) && !editor.isDragging)
        {
            editor.canDraw = false;
        }
        else
        {
            editor.canDraw = true;
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

    private void Select()
    {
        editor.selectedIcon.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        editor.selectedIcon.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 0.5f);
        editor.SetSelected(this.gameObject);

        isSelected = true;
    }

    private void Deselect()
    {
        isSelected = false;
        editor.ResetSelected(this.gameObject);
        editor.selectedIcon.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 0);
    }

    private void Drag()
    {
        editor.isDragging = true;
        isDragging = true;
        editor.SetBrushPreview(this.gameObject);
    }

    private void Drop()
    {
        editor.isDragging = false;
        isDragging = false;
        editor.ResetBrushPreview();
    }
}
