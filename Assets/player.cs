using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class player : MonoBehaviour
{
    Vector3 pos1, pos2;
    public GameObject gameManager;
    public bool canTouch, player1;
    public int jumps;
    public Vector3 velocity;
    private void Update()
    {
        velocity = GetComponent<Rigidbody>().velocity;

        if (velocity.y > 10) velocity.y = 10;
        if (velocity.y < -10) velocity.y = -10;

        if (velocity.x > 10) velocity.x = 10;
        if (velocity.x < -10) velocity.x = -10;

        GetComponent<Rigidbody>().velocity = velocity;

        for (int i = 0; i < Input.touchCount; i++)
        {
            if (gameManager.GetComponent<gameManager>().endTimer != 0) continue;

            if (player1)
            {
                if (Input.GetTouch(i).position.x < Screen.width / 2) canTouch = true;
                else canTouch = false;
            }
            else
            {
                if (Input.GetTouch(i).position.x > Screen.width / 2) canTouch = true;
                else canTouch = false;
            }

            if (canTouch)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    pos1 = Input.GetTouch(i).position;
                }

                if (Input.GetTouch(i).phase == TouchPhase.Moved)
                {
                    pos2 = Input.GetTouch(i).position;
                }

                if (Input.GetTouch(i).phase == TouchPhase.Ended)
                {
                    Vector3 pos = (pos1 - pos2) / 25;
                    Debug.Log(pos);
                    if (pos.x > 10) pos.x = 10;
                    else if (pos.x < -10) pos.x = -10;

                    if (pos.y > 10) pos.y = 10;
                    else if (pos.y < -10) pos.y = -10;

                    if (jumps < 2)
                    {
                        GetComponent<Rigidbody>().AddForce(pos, ForceMode.Impulse);
                        jumps += 1;
                    }
                    Debug.Log(pos);
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        jumps = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            gameManager.GetComponent<gameManager>().generate(gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        jumps = 1;
    }
}
