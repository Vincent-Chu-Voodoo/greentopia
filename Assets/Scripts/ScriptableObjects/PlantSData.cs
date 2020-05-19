﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "PlantSData", menuName = "SData/PlantSData")]
public class PlantSData : ScriptableObject
{
    public string plantName;
    public float prestigePoint;
    public float prestigeLevelRequirement;
    public float growningTimeInSecond;
    public float speedUpDiamond;
    public int sellableCapacity;
    public FTUE2SellableSData sellable;
    public float sellableSpawnTime;
    public List<IngredientData> nutrientRequiredList;
    public List<IngredientData> ingredientList;
}
