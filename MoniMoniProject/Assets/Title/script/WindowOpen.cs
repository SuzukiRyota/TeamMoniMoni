﻿using UnityEngine;
using System.Collections;

public class WindowOpen : MonoBehaviour
{
    private Vector3 size;
    private Vector3 size_2;
    private int local_time;
    private bool display_window;
    private float changesize_window;
    float alpha;
    bool check_path;
    // Use this for initialization
    void Start()
    {
        check_path = false;
           alpha = 0.0f;
        local_time = 55;
        display_window = false;
        changesize_window = 0.0f;
        size = transform.localScale;
        size_2 = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        local_time++;
        if (check_path == false&&local_time==1)
        {
            switch (display_window)
            {
                case false:
                    display_window = true;
                    break;
                case true:
                    display_window = false;
                    break;
            }
        }
        if (check_path == true && local_time == 2) check_path = false;
        if (display_window == true)
        {
            if (alpha < 1.00f)
            {
                alpha += 0.02f;
                this.GetComponent<CanvasGroup>().alpha = alpha;
            }
            if (changesize_window < 1.00f)
            {
                changesize_window += 0.02f;
                size_2.y = size.y * changesize_window;
                transform.localScale = size_2;
            }
            if (changesize_window > 1.00f)
            {
                changesize_window = 1.000f;
                size_2.y = size.y * changesize_window;
                transform.localScale = size_2;
            }
        }
        if (display_window == false)
        {
            if (alpha > 0.00f)
            {
                alpha -= 0.02f;
                this.GetComponent<CanvasGroup>().alpha = alpha;
            }
            if (changesize_window > 0.00f)
            {
                changesize_window -= 0.02f;
                size_2.y = size.y * changesize_window;
                transform.localScale = size_2;
            }
            if (changesize_window < 0.00f)
            {
                changesize_window = 0.000f;
                size_2.y = size.y * changesize_window;
                transform.localScale = size_2;
            }
        }

    }
    public void OnClick()
    {
        if (local_time >= 55) local_time = 0;
    }
    public void NeedNotOnClick()
    {
        if (local_time >= 55) { local_time = 0; check_path = true; }
    }
}
