using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Letter_Block : MonoBehaviour
{
    [SerializeField] private string letter = "A";
    [SerializeField] private float fallSpeed = 10;

    private Vector3 destination = new Vector3(0,0,0);

    public string Letter { get => letter; }

    public void SetLetter(string _letter)
    {
        letter = _letter.ToUpper();
        this.transform.GetChild(0).GetComponent<TextMeshPro>().text = _letter;
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
    //[SerializeField] private current cell?
}
