using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid_Cell : MonoBehaviour
{
    [SerializeField] private Vector2Int Grid_Location;

    [SerializeField] private Letter_Block contained_Letter_Block = null;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color highlightColor;
    private bool isHighlighted = false;

    public Vector2Int grid_Location { get => Grid_Location; set => Grid_Location = value; }
    public Letter_Block Contained_Letter_Block { get => contained_Letter_Block; set => contained_Letter_Block = value; }
    public bool IsHighlighted { get => isHighlighted; }

    private void Awake()
    {
        this.gameObject.GetComponent<SpriteRenderer>().color = defaultColor;
    }
    public void SetLetterBlock(Letter_Block lb)
    {
        contained_Letter_Block = lb;
        contained_Letter_Block.GetComponent<Letter_Block>().SetDestination(this.transform.position);
        //contained_Letter_Block.transform.position = this.transform.position;
    }

    public void HighlightSpace(bool isBottom)
    {
        this.gameObject.GetComponent<SpriteRenderer>().color = highlightColor;
        isHighlighted = true;
    }
    public void UnHighlightSpace()
    {
        this.gameObject.GetComponent<SpriteRenderer>().color = defaultColor;
        isHighlighted = false;
    }
}
