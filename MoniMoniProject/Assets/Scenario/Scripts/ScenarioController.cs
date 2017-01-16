using UnityEngine;
using System.Collections;

public class ScenarioController : MonoBehaviour
{
    [SerializeField]
    GameObject openingcanvas;

    [SerializeField]
    GameObject talkcanvas;

    [SerializeField]
    StagingController staging;

    [SerializeField]
    TalkManager talkmanager;

    int openingstep;

    void Start()
    {
        openingstep = 0;
    }

    void Update()
    {
        
        if (openingstep == 0)
            if (openingcanvas.GetComponent<OpeningTextController>().is_talknow == false)
            {
                if (staging.fadeOutBlack())
                {
                    openingcanvas.SetActive(false);
                    talkcanvas.SetActive(true);
                    talkmanager.talkStart();
                    openingstep++;
                }
            }
        if (openingstep == 1)
        {
            if (staging.fadeInBlack())
            {
                talkmanager.is_talknow = true;
                openingstep++;
            }
        }
    }
}
