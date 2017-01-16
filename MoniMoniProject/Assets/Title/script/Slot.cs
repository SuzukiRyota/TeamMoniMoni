using UnityEngine;
using System.Collections;

public class Slot : MonoBehaviour
{
    public GameObject slot_1;
    public GameObject slot_2;
    public GameObject slot_3;
    public GameObject slot_4;
    Vector3 pos_1;
    Vector3 pos_2;
    Vector3 pos_3;
    Vector3 pos_4;
    Vector3 pos_1_base;
    Vector3 pos_2_base;
    Vector3 pos_3_base;
    Vector3 pos_4_base;
    public int time;
    public bool open;
    float alpha;
    // Use this for initialization
    void Start()
    {
        alpha = 0.0f;
        pos_1 = slot_1.transform.localPosition;
        pos_2 = slot_2.transform.localPosition;
        pos_3 = slot_3.transform.localPosition;
        pos_4 = slot_4.transform.localPosition;
        pos_1_base = slot_1.transform.localPosition;
        pos_2_base = slot_2.transform.localPosition;
        pos_3_base = slot_3.transform.localPosition;
        pos_4_base = slot_4.transform.localPosition;
        time = 0;
        open = false;
        
    }

    // Update is called once per frame
    void Update()
    {

        if (open == true)
        {
            time++;
            if (alpha < 1.00f)
            {
                alpha += 0.02f;
                this.GetComponent<CanvasGroup>().alpha = alpha;
            }
        }
        else {
            if (alpha > 0.00f)
            {
                alpha -= 0.02f;
                this.GetComponent<CanvasGroup>().alpha = alpha;
            }
        }
        if (time >= 60 && time < 90)
        {
            pos_1.y += 0.0075f;
            slot_1.transform.localPosition = pos_1;
        }
        if (time >= 90 && time < 120)
        {
            pos_1.y -= 0.0075f;
            slot_1.transform.localPosition = pos_1;
            pos_2.y += 0.0075f;
            slot_2.transform.localPosition = pos_2;
        }
        if (time >= 120 && time < 150)
        {
            pos_2.y -= 0.0075f;
            slot_2.transform.localPosition = pos_2;
            pos_3.y += 0.0075f;
            slot_3.transform.localPosition = pos_3;
        }
        if (time >= 150 && time < 180)
        {
            pos_3.y -= 0.0075f;
            slot_3.transform.localPosition = pos_3;
            pos_4.y += 0.0075f;
            slot_4.transform.localPosition = pos_4;
        }
        if (time >= 180 && time < 210)
        {
            pos_4.y -= 0.0075f;
            slot_4.transform.localPosition = pos_4;
            pos_1.y += 0.0075f;
            slot_1.transform.localPosition = pos_1;
        }
        if (time == 209) time = 89;
    }
    public void OnClick()
    {
        switch (open)
        {
            case false:
               open = true;
                break;
            case true:
                if (time >= 55)
                {
                    open = false;
                    time = 0;
                    
                    slot_1.transform.localPosition = pos_1_base;
                    slot_2.transform.localPosition = pos_2_base;
                    slot_3.transform.localPosition = pos_3_base;
                    slot_4.transform.localPosition = pos_4_base;
                    pos_1 = pos_1_base;
                    pos_2 = pos_2_base;
                    pos_3 = pos_3_base;
                    pos_4 = pos_4_base;
                }
                break;
        }
    }
}
