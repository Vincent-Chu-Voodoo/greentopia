﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlantStageEnum
{
    OnCard, NeedNutrition, Growing, Grown, Collected
}

public class FTUE2Plant : MonoBehaviour
{
    public int plantID;
    public int plantStage;
    public PlantStageEnum plantStageEnum;
    public PlantSData plantSData;
    public FTUE2PlantDisplay plantDisplay;
    public Canvas canvas;
    public BoxCollider boxCollider;

    public GameObject hourGlassGO;
    public GameObject freeGO;
    public GameObject twentyDiamondGO;

    public GameObject nutritionGO;
    public GameObject progressionGO;
    public GameObject tapGO;
    public FTUEPlantProgress fTUEPlantProgress;

    public List<IngredientData> nutrientRequiredList;

    public GameEvent OnPlanted;

    public void ResetBoxSize()
    {
        boxCollider.center = plantDisplay.transform.GetChild(0).localPosition;
        boxCollider.size = plantDisplay.spriteRenderer.bounds.size;
    }

    public void Setup(int _id, PlantSData _plantSData, int _plantStage, PlantStageEnum _plantStageEnum)
    {
        plantID = _id;
        plantSData = _plantSData;
        plantStage = _plantStage;
        plantStageEnum = _plantStageEnum;
        plantDisplay.Setup(_plantSData, _plantStage, this);
        nutrientRequiredList = _plantSData.nutrientRequiredList;
        fTUEPlantProgress.totalTime = plantSData.growningTimeInSecond;
        fTUEPlantProgress.OnGrown.AddListener(Grow);
        canvas.worldCamera = Camera.main;
        ConfigurePlant(_plantStageEnum);

        if (plantSData.sellable.atomEnum == AtomEnum.aloe_sapling)
        {
            freeGO.SetActive(false);
            twentyDiamondGO.SetActive(true);
        }
        else
        {
            freeGO.SetActive(true);
            twentyDiamondGO.SetActive(false);
        }
    }

    public void ConfigurePlant(PlantStageEnum _plantStageEnum)
    {
        plantStageEnum = _plantStageEnum;
        switch (plantStageEnum)
        {
            case PlantStageEnum.OnCard:
                gameObject.AddComponent<FTUE2PlantOnCardBehaviour>();
                break;
            case PlantStageEnum.NeedNutrition:
                nutritionGO.SetActive(true);
                break;
            case PlantStageEnum.Growing:
                tapGO.SetActive(true);
                hourGlassGO.SetActive(true);
                break;
            case PlantStageEnum.Grown:
                tapGO.SetActive(false);
                plantDisplay.Setup(plantSData, 10, this);
                for (var i = 0; i < plantSData.sellableCapacity; i++)
                    plantDisplay.SpawnSellable(plantSData);
                break;
            case PlantStageEnum.Collected:
                plantDisplay.Setup(plantSData, 10, this);
                break;
            default:
                break;
        }
    }

    private void OnMouseUpAsButton()
    {
        //var pointerEventData = new PointerEventData(eventSystem)
        //{
        //    position = Input.mousePosition
        //};
        //var results = new List<RaycastResult>();
        //graphicRaycaster.Raycast(pointerEventData, results);
        //if (results.Count > 0)
        //    return;

        switch (plantStageEnum)
        {
            case PlantStageEnum.OnCard:
                break;
            case PlantStageEnum.NeedNutrition:
                break;
            case PlantStageEnum.Growing:
                tapGO.SetActive(progressionGO.activeSelf);
                hourGlassGO.SetActive(progressionGO.activeSelf);
                progressionGO.SetActive(!progressionGO.activeSelf);
                break;
            case PlantStageEnum.Grown:
                Collect();
                break;
            default:
                break;
        }
    }

    public void Collect()
    {
        GameDataManager.instance.gameData.sellableList.Add(plantSData.sellable);
        plantDisplay.CollectSellable();
        ConfigurePlant(PlantStageEnum.Collected);
    }

    public void Grow(object obj)
    {
        progressionGO.SetActive(false);
        ConfigurePlant(PlantStageEnum.Grown);
        GameObject.FindGameObjectWithTag(TagEnum.Header.ToString()).GetComponent<FTUE2Header>().AddXp(plantSData.prestigePoint);
        GameObject.FindGameObjectWithTag(TagEnum.LevelUpAnimation.ToString()).GetComponent<FTUE2LevelUpProxy>().Play(transform);

        foreach (var ingredient in plantSData.nutrientRequiredList)
        {
            var remainCount = ingredient.count;
            var allIngredientList = GameDataManager.instance.gameData.nurserySessionData.gridDataList.FindAll(
                i => !i.isDusty && i.atomEnum == ingredient.atomEnum && i.atomLevel == ingredient.level
                ).ToList();
            while (allIngredientList.Count > 0 && remainCount > 0)
            {
                GameDataManager.instance.gameData.nurserySessionData.gridDataList.Remove(allIngredientList[0]);
                allIngredientList.RemoveAt(0);
                remainCount--;
            }

            allIngredientList = GameDataManager.instance.gameData.toolShedSessionData.gridDataList.FindAll(
                i => !i.isDusty && i.atomEnum == ingredient.atomEnum && i.atomLevel == ingredient.level
                );
            while (allIngredientList.Count > 0 && remainCount > 0)
            {
                GameDataManager.instance.gameData.toolShedSessionData.gridDataList.Remove(allIngredientList[0]);
                allIngredientList.RemoveAt(0);
                remainCount--;
            }
        }
    }

    public void SpeedUp()
    {
        GameObject.FindGameObjectWithTag(TagEnum.Header.ToString()).GetComponent<FTUE2Header>().AddDiamond(-plantSData.speedUpDiamond);
    }

    public void FedNutritionClicked()
    {
        nutritionGO.SetActive(false);
        ConfigurePlant(PlantStageEnum.Growing);
        var mi = GameObject.FindGameObjectWithTag(TagEnum.NutritionAnimation.ToString());
        mi.GetComponent<FTUE2NutritionAnimationProxy>().Play(transform);
        //mi.SetActive(true);
        //mi.GetComponent<FTUEGardenNutritionAnimation>().targetAnchor = transform;
    }

    public void Planted(object obj)
    {
        foreach (var ingredient in plantSData.ingredientList)
        {
            var remainCount = ingredient.count;
            var allIngredientList = GameDataManager.instance.gameData.nurserySessionData.gridDataList.FindAll(
                i => !i.isDusty && i.atomEnum == ingredient.atomEnum && i.atomLevel == ingredient.level
                ).ToList();
            while (allIngredientList.Count > 0 && remainCount > 0)
            {
                GameDataManager.instance.gameData.nurserySessionData.gridDataList.Remove(allIngredientList[0]);
                allIngredientList.RemoveAt(0);
                remainCount--;
            }

            allIngredientList = GameDataManager.instance.gameData.toolShedSessionData.gridDataList.FindAll(
                i => !i.isDusty && i.atomEnum == ingredient.atomEnum && i.atomLevel == ingredient.level
                );
            while (allIngredientList.Count > 0 && remainCount > 0)
            {
                GameDataManager.instance.gameData.toolShedSessionData.gridDataList.Remove(allIngredientList[0]);
                allIngredientList.RemoveAt(0);
                remainCount--;
            }
        }

        GameDataManager.instance.gameData.gardentPlantList.Add(
            new GardenPlantData()
            {
                id = plantID,
                plantData = new PlantData()
                {
                    plantName = plantSData.plantName,
                    count = 1
                },
                plantStageEnum = plantStageEnum,
                plantStage = plantStage,
                localPosition = transform.localPosition,
                localScale = transform.localScale
            }
        );

        Destroy(gameObject.GetComponent<FTUE2PlantOnCardBehaviour>());
        ConfigurePlant(PlantStageEnum.NeedNutrition);

        OnPlanted.Invoke(this);
    }
}
