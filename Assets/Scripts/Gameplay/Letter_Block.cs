using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Letter_Block : MonoBehaviour
{
    [SerializeField] private string letter = "A";
    [SerializeField] private float fallSpeed = 10;
    [SerializeField] private Color highlightColor;

    private Vector3 destination = new Vector3(0,0,0);
    private Grid_Cell ParentCell = null;

    public string Letter { get => letter; }
    public Grid_Cell parentCell { get => ParentCell;}

    public bool isHighlighted = false;

    public void SetLetter(string _letter)
    {
        letter = _letter.ToUpper();
        this.transform.GetChild(0).GetComponent<TextMeshPro>().text = _letter;
    }
    public void SetDestination(Grid_Cell grid_cell)
    {
        destination = grid_cell.transform.position;
        ParentCell = grid_cell;
    }
    public void SetDestination(Vector3 _destination)
    {
        destination = _destination;
    }
    private void Update()
    {
        if(this.transform.position != destination)
        {
            var step = fallSpeed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, destination, step);
        }
    }
    private void OnMouseDown()
    {
        Manager_Game.LetterClickedEvent.Invoke(this);
    }
    public void HighlightLetter()
    {
        GetComponent<SpriteRenderer>().color = highlightColor;
    }
    //[SerializeField] private current cell?
}
