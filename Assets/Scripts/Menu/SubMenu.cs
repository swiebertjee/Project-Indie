﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _firstSelected;

    private void Start()
    {
        DisableMenu();
    }

    public GameObject EnableMenu()
    {
        foreach (UnityEngine.UI.Button button in transform.GetComponentsInChildren<UnityEngine.UI.Button>())
            button.interactable = true;

        return _firstSelected;
    }

    public void DisableMenu()
    {
        foreach (UnityEngine.UI.Button button in transform.GetComponentsInChildren<UnityEngine.UI.Button>())
            button.interactable = false;
    }
}
