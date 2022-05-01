using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class gameManagerOnline : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab, hunter, hit, hunterText, obstacle;
    public GameObject[] players;
    public int[] scores;
    public float timer, endTimer;
    bool end, gameStarted;
    public int player1Score, player2Score;
    public TextMeshPro scoreText, timerText, stateText;

    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        Vector3 pos = new Vector3(-7, -2, -5);
        if (players.Length == 1) pos = new Vector3(7, -2, -5);
        else if (players.Length == 2) pos = new Vector3(-7, 2, -5);
        else if (players.Length == 3) pos = new Vector3(7, 2, -5);
        PhotonNetwork.Instantiate(playerPrefab.name, pos, Quaternion.identity);

        scores = new int[System.Convert.ToInt32(PhotonNetwork.CurrentRoom.CustomProperties["MaxPlayers"])];

        for (int i = 0; i < System.Convert.ToInt32(PhotonNetwork.CurrentRoom.CustomProperties["MaxPlayers"]); i++)
        {
            scores[i] = 0;
        }
        GenerateObstacles();
    }
    private void FixedUpdate()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        if (!gameStarted)
        {
            timerText.text = players.Length + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
            scoreText.text = "room name: " + PhotonNetwork.CurrentRoom.Name;
            stateText.text = "waiting...";
            if (players.Length == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                if (PhotonNetwork.IsMasterClient) startGame();
            }

            return;
        }
        float min = 0, sec;
        sec = Convert.ToSingle(Math.Round(timer / 5, 0));
        while (sec >= 60)
        {
            sec -= 60;
            min += 1;
        }
        if (timer > 0 && players.Length > 1) timer -= 0.1f;
        if (timer <= 0)
        {
            end = true;
        }

        if (end)
        {
            if (endTimer == 0.1f)
            {
                foreach(GameObject player in players) if (player.GetComponent<PhotonView>().IsMine) player.GetComponent<Rigidbody>().useGravity = false;
                Time.timeScale = 0.5f;
            }
            endTimer += 0.1f;
            if (endTimer >= 5)
            {
                if (gameObject == players[0]) transform.position = new Vector3(-7, -2, -5);
                else if (gameObject == players[1]) transform.position = new Vector3(7, -2, -5);
                else if (gameObject == players[2]) transform.position = new Vector3(-7, 2, -5);
                else if (gameObject == players[3]) transform.position = new Vector3(7, 2, -5);

                hunter = hit;
                if (timer > 0)
                {
                    scores[Array.IndexOf(players, hunter)] += 1;
                }
                else
                {
                    foreach (int score in scores)
                    {
                        if (Array.IndexOf(scores, score) != Array.IndexOf(players, hunter)) scores[Array.IndexOf(scores, score)] += 1;
                    }
                }

                endTimer = 0;
                Time.timeScale = 1f;
                foreach (GameObject player in players) if (player.GetComponent<PhotonView>().IsMine) player.GetComponent<Rigidbody>().useGravity = true;
                timer = 300;
                end = false;
                GenerateObstacles();
            }
        }

        string secText = "";
        if (sec < 10) secText = "0" + sec;
        else secText = "" + sec;
        timerText.text = "time: 0" + min + ":" + secText;
        string scoresText = "";

        foreach (int score in scores)
        {
            scoresText += ":" + score;
        }

        hunterText.transform.position = hunter.transform.position + transform.up;

        if (players.Length == 2)
        {
            if (scores[0] > scores[1]) stateText.text = "green is leading";
            else if (scores[1] > scores[0]) stateText.text = "red is leading";
            else stateText.text = "draw";
        }
        else if (players.Length == 3)
        {
            if (scores[0] == scores[1] && scores[1] == scores[2]) stateText.text = "draw";
            else if (scores[0] == scores.Min()) stateText.text = "green is losing";
            else if (scores[1] == scores.Min()) stateText.text = "red is losing";
            else if (scores[2] == scores.Min()) stateText.text = "blue is losing";
        }
        else if (players.Length == 4)
        {
            if (scores[0] == scores[1] && scores[1] == scores[2]) stateText.text = "draw";
            else if (scores[0] == scores.Min()) stateText.text = "green is losing";
            else if (scores[1] == scores.Min()) stateText.text = "red is losing";
            else if (scores[2] == scores.Min()) stateText.text = "blue is losing";
            else if (scores[3] == scores.Min()) stateText.text = "yellow is losing";
        }
        
    }

    [PunRPC]
    public void generate(string playerSending = null, string playerHit = null)
    {
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().Owner.NickName == playerSending) hunter = player;
            else if (player.GetComponent<PhotonView>().Owner.NickName == playerHit) hit = player;
        }
    }

    public void GenerateObstacles()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!gameStarted) return;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("obstacle")) Destroy(obj);
        int i = 0;
        while (i < 5)
        {
            PhotonNetwork.Instantiate(obstacle.name, new Vector3(UnityEngine.Random.Range(-8, 8), UnityEngine.Random.Range(-3, 2), -3), Quaternion.identity);
            i += 1;

        }
    }

    public void sendMethodRPC(string name, object[] addParams = null)
    {
        this.photonView.RPC(name, RpcTarget.All, addParams);
    }
    public void startGame()
    {
        string hunterName = PhotonNetwork.CurrentRoom.Players[UnityEngine.Random.Range(0, PhotonNetwork.CurrentRoom.Players.Count)].UserId;
        this.photonView.RPC("startingGame", RpcTarget.AllBuffered, hunterName);
    }

    [PunRPC]
    public void startingGame(string hunterName)
    {
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().Owner.UserId == hunterName)
            {
                hunter = player;
                break;
            }
        }
        gameStarted = true;

        if (gameObject == players[0]) transform.position = new Vector3(-7, -2, -5);
        else if (gameObject == players[1]) transform.position = new Vector3(7, -2, -5);
        else if (gameObject == players[2]) transform.position = new Vector3(-7, 2, -5);
        else if (gameObject == players[3]) transform.position = new Vector3(7, 2, -5);
    }
}
