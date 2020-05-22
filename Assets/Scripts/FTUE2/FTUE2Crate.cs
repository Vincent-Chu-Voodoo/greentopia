﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FTUE2Crate : MonoBehaviour
{
    public float price;
    public float cooldown;
    public bool canClaimFree
    {
        get { return currentCoolDown < 0.01f; }
    }

    public FTUE2BoardEnum targetBoard;
    public GameObject purchaseGO;
    public GameObject freeClaimGreenGO;
    public GameObject freeClaimGrayGO;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI coolDownText;

    public float currentCoolDown
    {
        get
        {
            if (targetBoard == FTUE2BoardEnum.NurseryBoard)
                return nurseryCoolDown;
            else if (targetBoard == FTUE2BoardEnum.ToolShedBoard)
                return toolShedCoolDown;
            else
                throw new System.Exception();
        }
        set
        {
            if (targetBoard == FTUE2BoardEnum.NurseryBoard)
                nurseryCoolDown = value;
            else if (targetBoard == FTUE2BoardEnum.ToolShedBoard)
                toolShedCoolDown = value;
            else
                throw new System.Exception();
        }
    }
    public static float nurseryCoolDown;
    public static float toolShedCoolDown;

    public float previousTime
    {
        get
        {
            if (targetBoard == FTUE2BoardEnum.NurseryBoard)
                return nurseryPreviousTime;
            else if (targetBoard == FTUE2BoardEnum.ToolShedBoard)
                return toolShedPreviousTime;
            else
                throw new System.Exception();
        }
        set
        {
            if (targetBoard == FTUE2BoardEnum.NurseryBoard)
                nurseryPreviousTime = value;
            else if (targetBoard == FTUE2BoardEnum.ToolShedBoard)
                toolShedPreviousTime = value;
            else
                throw new System.Exception();
        }
    }
    public static float nurseryPreviousTime;
    public static float toolShedPreviousTime;

    public FTUE2CrateSData fTUE2CrateSData;

    public GameEvent OnCliam;

    private void Start()
    {
        price = fTUE2CrateSData.purchasePrice;
        cooldown = fTUE2CrateSData.cooldownInSecond;
        purchaseGO.GetComponent<Button>().interactable = GameDataManager.instance.gameData.coin >= price;
        priceText.SetText($"{price}");
    }

    private void Update()
    {
        var timeDelta = Time.realtimeSinceStartup - previousTime;
        previousTime = Time.realtimeSinceStartup;
        currentCoolDown = Mathf.MoveTowards(currentCoolDown, 0f, timeDelta);
        coolDownText.SetText($"{(int)currentCoolDown / 3600:0}h {((int)currentCoolDown / 60) % 60:0}m");
        if (currentCoolDown > 0f)
        {
            freeClaimGreenGO.SetActive(false);
        }
        else
        {
            freeClaimGreenGO.SetActive(true);
            purchaseGO.SetActive(false);
            freeClaimGrayGO.SetActive(true);
        }
    }

    public void Claim()
    {
        if (!canClaimFree && GameDataManager.instance.gameData.coin < price)
            return;
        currentCoolDown = cooldown;
        //if (!canClaimFree)
            //GameDataManager.instance.gameData.coin -= price;
        GameDataManager.instance.gameData.crateList.Add(fTUE2CrateSData);
        freeClaimGreenGO.SetActive(false);
        GetComponent<Animator>().SetTrigger("claim");
        OnCliam.Invoke(this);
    }

    public void ClaimEnd()
    {
        purchaseGO.SetActive(true);
        freeClaimGrayGO.SetActive(false);
    }
}
