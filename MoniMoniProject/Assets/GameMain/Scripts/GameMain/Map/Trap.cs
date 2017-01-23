using UnityEngine;
using System.Collections;

/// <summary>
/// トラップのプレハブにつけるクラス
/// </summary>
public class Trap : MonoBehaviour
{
    public enum TrapDirection
    {
        UP, DOWN, RIGHT, LEFT
    }

    public TrapDirection direction = TrapDirection.DOWN;

    Vector2 search_pos;
    public float search_range;

    // エネミーの情報を得るため
    EnemyManager enemymanager;

    public int trap_active_time;

    bool is_trapstart;
    // 再発動時間
    public int trap_delay_count;
    int trap_count;

    void Start()
    {
        enemymanager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        search_pos = new Vector2();
        switch (direction)
        {
            case TrapDirection.UP:
                search_pos = transform.position + new Vector3(0, search_range, 0);
                break;
            case TrapDirection.DOWN:
                search_pos = transform.position + new Vector3(0, -search_range, 0);
                break;
            case TrapDirection.RIGHT:
                search_pos = transform.position + new Vector3(search_range, 0, 0);
                break;
            case TrapDirection.LEFT:
                search_pos = transform.position + new Vector3(-search_range, 0, 0);
                break;
        }
        is_trapstart = false;
        trap_count = 0;
    }

    void Update()
    {
        if (is_trapstart)
        {
            trap_count++;
            if (trap_count > trap_delay_count)
            {
                is_trapstart = false;
                trap_count = 0;
            }
        }
        else
        {
            searchEnemy(transform.position);
            searchEnemy(search_pos);
        }
    }

    /// <summary>
    /// エネミーが罠の範囲に入ったら罠にかける関数
    /// </summary>
    void searchEnemy(Vector2 search_pos_)
    {
        for (int i = 0; i < enemymanager.enemy_num; i++)
        {
            if (pointToCenterBoxRect(
                enemymanager.enemys[i].transform.position,
                search_pos_,
                new Vector2(search_range, search_range)))
            {
                enemymanager.enemyInTrap(i, trap_active_time);
                is_trapstart = true;
            }
        }
    }

    // 点と矩形（真ん中）
    public bool pointToCenterBoxRect(Vector2 pointpos_, Vector2 boxpos_, Vector2 boxsize_)
    {
        return (
            pointpos_.x > boxpos_.x - boxsize_.x / 2 &&
            pointpos_.x < boxpos_.x + boxsize_.x / 2 &&
            pointpos_.y > boxpos_.y - boxsize_.y / 2 &&
            pointpos_.y < boxpos_.y + boxsize_.y / 2);
    }
}
