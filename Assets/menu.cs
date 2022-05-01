using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class menu : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        MobileAds.Initialize(initStatus => { });

        BannerView bannerView = new BannerView("ca-app-pub-8300493683714143/4773048360", AdSize.IABBanner, 0, -650);

        AdRequest request = new AdRequest.Builder().Build();
        
        bannerView.LoadAd(request);
    }

    public void pressed(string name)
    {
        Application.LoadLevel(name);
    }

    public void exit()
    {
        Application.Quit();
    }
}
