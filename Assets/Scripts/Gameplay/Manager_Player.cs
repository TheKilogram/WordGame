using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Player : MonoBehaviour
{
    private float timer = 0;
    private float time = 0.2f;
    private void Update()
    {
        if(Manager_Game.gameOver == false)
        {
            timer += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                //Debug.Log("Right");
                Manager_Game.MoveLetterEvent.Invoke(1);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                //Debug.Log("Left");
                Manager_Game.MoveLetterEvent.Invoke(-1);
                //MoveBlock(Vector2.left);
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {

                if (timer >= time)
                {
                    Manager_Game.DropLetterEvent.Invoke();
                    timer = 0;
                }

            }
            else if (Input.GetKeyUp(KeyCode.Return))
            {
                Manager_Game.DeleteAllWordsEvent.Invoke();
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                Manager_Game.RestartGameEvent.Invoke();
            }
        }
        

        //code for player input
        //code for moving the letter block left and right locking to columns.??
        //maybe needs to be in a differnt class but this calls it
        
    }
}
