﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _firstSelected;

    private void Awake()
    {
        DisableMenu();
    }

    public virtual GameObject EnableMenu()
    {
        foreach (UnityEngine.UI.Selectable button in transform.GetComponentsInChildren<UnityEngine.UI.Button>())
            button.interactable = true;

        return _firstSelected;
    }

    public virtual void DisableMenu()
    {
        foreach (UnityEngine.UI.Selectable button in transform.GetComponentsInChildren<UnityEngine.UI.Button>())
            button.interactable = false;
    }

    public GameObject FirstSelected
    {
        get { return _firstSelected; }
    }
}
