using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicBands : MonoBehaviour
{
    public static CinematicBands instance;
    private Animator animator;
    private GameObject BandUp;
    private GameObject BandDown;

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
        }

        instance = this;
    }

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        BandUp = this.transform.GetChild(0).gameObject;
        BandDown = this.transform.GetChild(1).gameObject;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            OpenBands();
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            CloseBands();
        }
    }

    public void OpenBands()
    {
        animator.Play("OpenBands");
    }
    public void CloseBands()
    {
        animator.Play("CloseBands");
    }
}