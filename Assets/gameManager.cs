using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using GoogleMobileAds.Api;

public class gameManager : MonoBehaviour
{
    public GameObject player1, player2, hunter, hunterText, obstacle;
    public float timer, endTimer;
    bool end;
    public int player1Score, player2Score;
    public TextMeshPro scoreText, timerText, stateText;

    InterstitialAd interstitial;

    private void Start()
    {
        if (UnityEngine.Random.Range(0, 2) == 0) hunter = player1;
        else hunter = player2;
        
        interstitial = new InterstitialAd("ca-app-pub-8300493683714143/2965142678");
        AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);
        GenerateObstacles();

    }
    private void FixedUpdate()
    {
        float min = 0, sec;
        sec = Convert.ToSingle(Math.Round(timer / 5, 0));
        while (sec >= 60)
        {
            sec -= 60;
            min += 1;
        }
        if (timer > 0) timer -= 0.1f;
        if (timer <= 0)
        {
            end = true;
        }

        if (end)
        {
            if (endTimer == 0.1f)
            {
                player1.GetComponent<Rigidbody>().useGravity = false;
                player2.GetComponent<Rigidbody>().useGravity = false;
                Time.timeScale = 0.5f;
            }
            endTimer += 0.1f;
            if (endTimer >= 5)
            {
                player1.transform.position = new Vector3(-7, -2, -5);
                player2.transform.position = new Vector3(7, -2, -5);

                if (hunter == player1)
                {
                    hunter = player2;
                    if (timer > 0) player1Score += 1;
                    else player2Score += 1;
                }
                else
                {
                    hunter = player1;
                    if (timer > 0) player2Score += 1;
                    else player1Score += 1;
                }
                endTimer = 0;
                Time.timeScale = 1f;
                player1.GetComponent<Rigidbody>().useGravity = true;
                player2.GetComponent<Rigidbody>().useGravity = true;
                timer = 300;
                end = false; 
                GenerateObstacles();
            }
        }

        string secText = "";
        if (sec < 10) secText = "0" + sec;
        else secText = "" + sec;
        timerText.text = "time: 0" + min + ":" + secText;
        scoreText.text = player1Score + ":" + player2Score;

        hunterText.transform.position = hunter.transform.position + transform.up;

        if (player1Score > player2Score) stateText.text = "green is leading";
        else if (player2Score > player1Score) stateText.text = "red is leading";
        else stateText.text = "draw";
    }

    public void generate(GameObject playerSending = null)
    {
        if (playerSending == hunter)
        {
            end = true;
        }
    }

    public void GenerateObstacles()
    {
        Debug.Log((player1Score + player2Score) % 4);
        if ((player1Score + player2Score) % 4 == 0 && player1Score + player2Score > 0)
        {
            interstitial = new InterstitialAd("ca-app-pub-8300493683714143/2965142678");
            AdRequest request = new AdRequest.Builder().Build();
            interstitial.LoadAd(request);
            interstitial.Show();
        }

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("obstacle")) Destroy(obj);
        int i = 0;
        while (i < 5)
        {
            Instantiate(obstacle, new Vector3(UnityEngine.Random.Range(-8, 8), UnityEngine.Random.Range(-3, 2), -3), Quaternion.identity);
            i += 1;
        }
    }

    public void exit()
    {
        Application.LoadLevel("menu");
    }
}
