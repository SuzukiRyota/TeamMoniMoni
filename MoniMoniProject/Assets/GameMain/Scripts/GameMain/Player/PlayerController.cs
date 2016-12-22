using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// プレイヤーの操作を持つクラス
/// </summary>
public class PlayerController : MonoBehaviour
{
    //[SerializeField]
    //StickController stick;

    [SerializeField]
    MoveButtonController movebutton;

    [SerializeField]
    CanvasController canvascontroller;

    // プレイヤーの向き
    public enum PlayerDirection
    {
        UP, DOWN, RIGHT, LEFT
    }

    public PlayerDirection player_direction = PlayerDirection.DOWN;

    // プレイヤーの状態
    public enum State
    {
        NORMAL,
        EVENT,
        TALK,
        SKILL,
    }

    public State state;
    public State current_state;

    public enum AnimationState
    {
        IDLE,
        WORK,
        ATTACKSKILL_START,
        ATTACKSKILL,
        ATTACKSKILLEND,
        ATTACKSKILL_HIT,
    }

    public AnimationState animstate;

    public Vector2 vec;

    [SerializeField]
    private float speed;

    // イベントキーが押されたかどうか
    bool is_pusheventkey;
    public void pushEventKey()
    {
        is_pusheventkey = true; ;
    }

    void Start()
    {
        player_direction = PlayerDirection.DOWN;
        state = State.NORMAL;
        transform.position = new Vector3(0, 0, -1.0f);

        vec = new Vector2(0, 0);
        is_pusheventkey = false;

        StartCoroutine(moveMethod());
        StartCoroutine(fieldCheck());
        StartCoroutine(stateCoroutine());
    }


    /// <summary>
    /// プレイヤーの状態が変わった時だけ通るif文があるコルーチン
    /// </summary>
    private IEnumerator stateCoroutine()
    {
        while (true)
        {
            if (current_state != state)
            {
                current_state = state;
                canvascontroller.playerStateChangeCanvas();
            }
            yield return null;
        }
    }

    /// <summary>
    /// フィールドを選択しているかどうかのフラグを変更するコルーチン
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator fieldCheck()
    {
        while (true)
        {
            // イベントキーが押されたとき
            if (is_pusheventkey)
                // 選択しているブロックにイベントがあるかどうか
                if (mapchip.checkEventExists())
                {
                    state = State.EVENT;
                    mapchip.isEventCompleted();
                }

            if (Input.GetKeyDown(KeyCode.Return))
                // 選択しているブロックにイベントがあるかどうか
                if (mapchip.checkEventExists())
                {
                    state = State.EVENT;
                }

            if (mapchip.isEventCompleted())
            {
                state = State.NORMAL;
            }

            is_pusheventkey = false;

            yield return null;
        }
    }

    [SerializeField]
    MapChipController mapchip = null; // ブロックサイズを取得するため

    /// <summary>
    /// playerの位置からcell番号(x, y)を返す
    /// </summary>
    /// <returns>cell番号</returns>
    public Vector2 retCell()
    {
        var pos = transform.position;
        Vector2 cell_f = pos / mapchip.chip_size;

        Vector2 cell_i = new Vector2(Mathf.RoundToInt(cell_f.x), Mathf.RoundToInt(cell_f.y));
        cell_i.x = Mathf.Abs(cell_i.x);
        cell_i.y = Mathf.Abs(cell_i.y);

        return cell_i;
    }


    /// <summary>
    /// プレイヤー移動メソッド
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator moveMethod()
    {
        while (true)
        {

            move();

            directionChange();
            yield return null;
        }
    }

    /// <summary>
    /// プレイヤー移動関数
    /// </summary>
    private void move()
    {
        vec = Vector2.zero;

        vecChange();
        skillUpdate();

        Vector3 vec_ = new Vector3(vec.x * 0.05f, vec.y * 0.05f, 0);
        transform.Translate(vec_);
        //var rigidbody = GetComponent<Rigidbody2D>();
        //rigidbody.velocity = vec;


    }

    /// <summary>
    /// プレイヤーのベクトルをGUIからもらう関数
    /// </summary>
    private void vecChange()
    {
        if (state != State.NORMAL) return;

        vec = speed * movebutton.getButtonPushVec();

        if (vec.x < 0.11f && vec.x > -0.11f)
            vec.x = 0;
        if (vec.y < 0.11f && vec.y > -0.11f)
            vec.y = 0;

        if (vec.x != 0 || vec.y != 0)
            animstate = AnimationState.WORK;
        else
            animstate = AnimationState.IDLE;
    }

    /// <summary>
    /// プレイヤーのアニメーションの向きを変える関数
    /// </summary>
    void directionChange()
    {
        if (vec.y > 0.0f)
        {
            player_direction = PlayerDirection.UP;
        }
        if (vec.y < 0.0f)
        {
            player_direction = PlayerDirection.DOWN;
        }
        if (vec.x > 0.0f)
        {
            player_direction = PlayerDirection.RIGHT;
        }
        if (vec.x < 0.0f)
        {
            player_direction = PlayerDirection.LEFT;
        }
    }

    public void pushSkillKey()
    {
        state = State.SKILL;
        animstate = AnimationState.ATTACKSKILL_START;
    }
    public void skillHitWall()
    {
        animstate = AnimationState.ATTACKSKILL_HIT;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (state == State.SKILL)
            {
                skillHitWall();
                collision = null;
                switch (player_direction)
                {
                    case PlayerDirection.UP:
                        transform.Translate(new Vector3(0, -0.1f, 0));
                        break;
                    case PlayerDirection.DOWN:
                        transform.Translate(new Vector3(0, 0.1f, 0));
                        break;
                    case PlayerDirection.RIGHT:
                        transform.Translate(new Vector3(-0.1f, 0, 0));
                        break;
                    case PlayerDirection.LEFT:
                        transform.Translate(new Vector3(0.1f, 0, 0));
                        break;
                }
            }
        }
    }

    // ダッシュの速さ
    [SerializeField]
    private int dashSpeed;

    // ダッシュするための準備時間
    [SerializeField]
    int dashstart_anim_frame = 0;

    // ダッシュしてる合計時間
    [SerializeField]
    int dash_anim_frame = 0;

    // ダッシュしてる最中の時間
    [SerializeField]
    int dashend_anim_frame = 0;

    // ダッシュ中に障害物に当たった時に発生するアニメーションの時間
    [SerializeField]
    int dashhit_frame = 0;

    // ダッシュ中に増える値
    int dashcount = 0;

    // 今のアニメーション状況を保存
    AnimationState currentanimstate;

    /// <summary>
    /// ダッシュスキルのアニメーションとダッシュの処理をする関数
    /// </summary>
    void skillUpdate()
    {
        if (state != State.SKILL) return;

        if (animstate != currentanimstate)
        {
            currentanimstate = animstate;
            if (animstate == AnimationState.ATTACKSKILL_HIT)
            {
                state = State.SKILL;
                vec = Vector2.zero;
                dashcount = 0;
            }
        }

        if (animstate != AnimationState.ATTACKSKILL_HIT)
        {
            int skillendtime = dashstart_anim_frame + dash_anim_frame + dashend_anim_frame;
            dashcount++;
            if (dashcount > skillendtime)
            {
                state = State.NORMAL;
                animstate = AnimationState.IDLE;
                dashcount = 0;
                vec = Vector2.zero;
            }

            // スキル発動アニメーション
            if (dashcount < dashstart_anim_frame)
            {
                animstate = AnimationState.ATTACKSKILL_START;
                return;
            }

            // スキル発動して移動するアニメーション
            if (dashcount < dash_anim_frame + dashstart_anim_frame)
            {
                animstate = AnimationState.ATTACKSKILL;

                switch (player_direction)
                {
                    case PlayerDirection.UP:
                        vec.y = speed;
                        break;
                    case PlayerDirection.DOWN:
                        vec.y = -speed;
                        break;
                    case PlayerDirection.RIGHT:
                        vec.x = speed;
                        break;
                    case PlayerDirection.LEFT:
                        vec.x = -speed;
                        break;
                }
                vec.x *= dashSpeed;
                vec.y *= dashSpeed;

                return;
            }

            animstate = AnimationState.ATTACKSKILLEND;
            return;
        }
        else
        {
            dashcount++;
            if (dashcount > dashhit_frame)
            {
                dashcount = 0;
                state = State.NORMAL;
                animstate = AnimationState.IDLE;
            }
            return;
        }
    }

}

