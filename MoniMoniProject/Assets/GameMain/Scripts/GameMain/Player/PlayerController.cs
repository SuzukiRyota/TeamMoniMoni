using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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
        is_pusheventkey = true;
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
        StartCoroutine(itemCoroutine());

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
                skillForcedEnd();
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
    MapChipController mapchip = null;

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
        //if (collision != null)
        //{
        //    if (state == State.SKILL)
        //    {
        //        skillHitWall();
        //        collision = null;
        //        switch (player_direction)
        //        {
        //            case PlayerDirection.UP:
        //                transform.Translate(new Vector3(0, -0.1f, 0));
        //                break;
        //            case PlayerDirection.DOWN:
        //                transform.Translate(new Vector3(0, 0.1f, 0));
        //                break;
        //            case PlayerDirection.RIGHT:
        //                transform.Translate(new Vector3(-0.1f, 0, 0));
        //                break;
        //            case PlayerDirection.LEFT:
        //                transform.Translate(new Vector3(0.1f, 0, 0));
        //                break;
        //        }
        //    }
        //}
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (state == State.SKILL &&
                animstate != AnimationState.ATTACKSKILL_HIT)
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
        if (state == State.SKILL)
        {
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
        else
        {
            if (animstate == AnimationState.ATTACKSKILL_START ||
                animstate == AnimationState.ATTACKSKILL)
            {
                animstate = AnimationState.ATTACKSKILLEND;
                dashcount = 0;
                vec = Vector2.zero;
            }
            if (animstate == AnimationState.ATTACKSKILLEND)
            {
                dashcount++;
                if (dashcount > dashend_anim_frame)
                {
                    animstate = AnimationState.IDLE;
                    dashcount = 0;
                    vec = Vector2.zero;
                }
            }
        }

    }

    /// <summary>
    /// スキルを強制終了させる関数
    /// </summary>
    void skillForcedEnd()
    {
        if (animstate == AnimationState.ATTACKSKILL_START ||
            animstate == AnimationState.ATTACKSKILL)
        {
            animstate = AnimationState.ATTACKSKILLEND;
            dashcount = 0;
            vec = Vector2.zero;
        }
    }

    /// <summary>
    /// 持っているアイテムの名前
    /// </summary>
    public string have_item_name = "";
    string current_have_item_name = "";
    public bool is_use_item = false;

    public string eventUseItem()
    {
        is_use_item = true;
        string item_name_temp = have_item_name;
        have_item_name = "Item";
        return item_name_temp;
    }
    public void useItem()
    {
        is_use_item = true;
        if (not_put_item.IndexOf(have_item_name) != -1)
        {
            return;
        }
        have_item_name = "Item";
    }


    [SerializeField]
    Image item_image;

    Dictionary<string, Sprite> items = new Dictionary<string, Sprite>();
    List<string> not_put_item = new List<string>();

    void itemsImageSetup()
    {
        Sprite[] loadsprite = Resources.LoadAll<Sprite>("Textures/Items");

        //　空
        items.Add("Item", System.Array.Find<Sprite>(
                            loadsprite, (sprite) => sprite.name.Equals(
                                "Item")));
        items.Add("Boarderaser", System.Array.Find<Sprite>(
                                    loadsprite, (sprite) => sprite.name.Equals(
                                        "Boarderaser")));
        items.Add("Statue", System.Array.Find<Sprite>(
                                    loadsprite, (sprite) => sprite.name.Equals(
                                        "Statue")));
        items.Add("Wheelchair", System.Array.Find<Sprite>(
                                      loadsprite, (sprite) => sprite.name.Equals(
                                          "Wheelchair")));
        items.Add("Easel", System.Array.Find<Sprite>(
                                      loadsprite, (sprite) => sprite.name.Equals(
                                          "Easel")));

        // 置けないアイテムを登録
        not_put_item.Add("Boarderaser");
    }

    private IEnumerator itemCoroutine()
    {
        itemsImageSetup();
        while (true)
        {
            if (current_have_item_name != have_item_name)
            {
                current_have_item_name = have_item_name;
                if (isHaveItem() == false)
                    item_image.sprite = items["Item"];
                else
                    item_image.sprite = items[have_item_name];
            }
            yield return null;
        }
    }

    public void setItem(string item_name)
    {
        if (isHaveItem() == false)
            have_item_name = item_name;
    }

    /// <summary>
    /// アイテムを持っているかどうか(持っていたらtrue)
    /// </summary>
    /// <returns></returns>
    public bool isHaveItem()
    {
        if (have_item_name == null ||
            have_item_name == "" ||
            have_item_name == "Item")
            return false;
        return true;
    }
}

