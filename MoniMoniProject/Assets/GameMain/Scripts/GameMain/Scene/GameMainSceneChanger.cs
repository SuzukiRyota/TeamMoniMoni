using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameMainSceneChanger : MonoBehaviour
{

    [SerializeField]
    EnemyManager enemymanager;

    [SerializeField]
    StagingController staging;

    [SerializeField]
    EventsCreate eventcreate;

    void Start()
    {

    }


    void Update()
    {
        if (enemymanager.is_bosshit)
        {
            if (staging.flushStart())
            {
                eventcreate.returnHouse();
                SceneManager.LoadScene("Hunting");
            }
        }

        if (enemymanager.is_enemy_hit)
        {
            if (staging.flushStart())
            {
                eventcreate.stateSave();
                SceneManager.LoadScene("Hunting");
            }
        }
    }
}
