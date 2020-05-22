﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FTUE2NeightbourScene : MonoBehaviour
{
    public FTUE2SellableSData targetSellable;

    public GameObject apple;
    public GameObject tomato;
    public GameObject aloe;
    public GameObject claimButton;
    public FTUE2Header header;

    public AssetReference backAR;

    void Start()
    {
        apple.SetActive(false);
        tomato.SetActive(false);
        aloe.SetActive(false);

        if (GameDataManager.instance.gameData.sellableList.Count == 0)
        {
            claimButton.SetActive(false);
            tomato.SetActive(true);
            return;
        }

        claimButton.SetActive(true);
        targetSellable = GameDataManager.instance.gameData.sellableList.First();

        switch (targetSellable.atomEnum)
        {
            case AtomEnum.apple_sapling:
                apple.SetActive(true);
                break;
            case AtomEnum.tomato_sapling:
                tomato.SetActive(true);
                break;
            case AtomEnum.aloe_sapling:
                aloe.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void Claim()
    {
        header.AddCoin(targetSellable.sellPrice);
        GameDataManager.instance.gameData.sellableList.Remove(targetSellable);
        claimButton.SetActive(false);
    }

    public void Back()
    {
        backAR.LoadSceneAsync();
    }
}
