using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO_Visual : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 10;
    private Vector3 destination = Vector3.zero;
    
    public void SetDestination(Vector3 _destination)
    {
        destination = _destination + new Vector3(0,.6f,0);
    }
    private void Update()
    {
        if (this.transform.position != destination)
        {
            var step = fallSpeed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, destination, step);
        }
    }
}
