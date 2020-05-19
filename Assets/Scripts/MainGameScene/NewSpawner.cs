﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSpawner : MonoBehaviour
{
    public GameObject coolDownDisplayGO;
    public Image fillImage;

    public float currentCoolDown;
    public float coolDown;

    public AtomEnum atomType;
    public GameObject atomPrefab;
    public MainGrid mainGrid;
    public List<SubGrid> spawnableSubGridList;
    public AtomController atomController;

    public GameEvent OnClick;
    public GameEvent OnSpawnNewAtom;

    private void Update()
    {
        currentCoolDown = Mathf.MoveTowards(currentCoolDown, 0f, Time.deltaTime);
        coolDownDisplayGO.SetActive(currentCoolDown > 0f);
        fillImage.fillAmount = (coolDown - currentCoolDown) / coolDown;
    }

    public void OnMouseUpAsButton()
    {
        if (!enabled)
            return;
        if (currentCoolDown > 0f)
            return;
        OnClick.Invoke(this);
    }

    public virtual void SpawnAtom()
    {
        for (var i = 0; i < spawnableSubGridList.Count; i++)
        {
            var subGrid = spawnableSubGridList[i];
            if (subGrid != null && subGrid.isEmpty)
            {
                var newAtomGO = Instantiate(atomPrefab, subGrid.transform);
                newAtomGO.transform.position = transform.position;
                var newAtom = newAtomGO.GetComponent<Atom>();
                newAtom.Setup(atomType, mainGrid);
                newAtom.SubGridLinked(subGrid);
                subGrid.AtomLinked(newAtom);
                newAtom.AtomComplete();
                atomController.SpawnNewAtom(newAtom);
                OnSpawnNewAtom.Invoke(newAtom);
            }
        }
    }
}
