using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;

public class playerOnline : MonoBehaviourPunCallbacks
{
    Vector3 pos1, pos2;
    public GameObject gameManager;
    public int jumps;
    public Vector3 velocity;

    private void Start()
    {
        gameManager = GameObject.Find("gameManager");
    }
    private void Update()
    {
        if (!this.photonView.IsMine) return;
        velocity = GetComponent<Rigidbody>().velocity;

        if (velocity.y > 10) velocity.y = 10;
        if (velocity.y < -10) velocity.y = -10;

        if (velocity.x > 10) velocity.x = 10;
        if (velocity.x < -10) velocity.x = -10;

        GetComponent<Rigidbody>().velocity = velocity;

        for (int i = 0; i < Input.touchCount; i++)
        {
            if (gameManager.GetComponent<gameManagerOnline>().endTimer != 0) continue;

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

    private void OnCollisionStay(Collision collision)
    {
        jumps = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            this.photonView.RPC("generate", RpcTarget.All, collision.collider.GetComponent<PhotonView>().Owner.NickName);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        jumps = 1;
    }

    [PunRPC]
    public void generate(string playerHit)
    {
        gameManager.GetComponent<gameManagerOnline>().sendMethodRPC("generate", new object[2] { this.photonView.Owner.NickName, playerHit });
    }
}
