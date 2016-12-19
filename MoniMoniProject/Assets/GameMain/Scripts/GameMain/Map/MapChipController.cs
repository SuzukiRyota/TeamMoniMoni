using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;


/// <summary>
/// マップチップを管理するクラス
/// </summary>
public class MapChipController : MonoBehaviour
{

    [SerializeField]
    GameObject player = null;

    [SerializeField]
    PlayerController player_controller = null;

    [SerializeField]
    CameraController cameracontroller;

    public float chip_size;
    public float chip_scale;

    public int chip_num_x;
    public int chip_num_y;

    const int eventlayer = (int)LayerController.Layer.EVENT;

    public List<List<List<GameObject>>> blocks;

    public List<List<List<Block>>> blockcomponents;

    public string select_map_name;

    void Awake()
    {
        blocks = new List<List<List<GameObject>>>();

        // FIXME: スプライトまとめて読む感じに失敗・・・修正予定
        // SpriteLoader loader = new SpriteLoader();
        // loader.Load("Textures/samplechip");

        select_map_name = "school1";

        loadMap(select_map_name);
        //loadMap("test");

        chipsIsActive();

        for (int y = 0; y < chip_num_y; y++)
        {
            for (int x = 0; x < chip_num_x; x++)
            {
                if (blockcomponents[eventlayer][y][x].number != -1)
                {
                    for (int i = 0; i < (int)LayerController.Layer.LAYER_MAX; i++)
                    {
                        blocks[i][y][x].GetComponent<SpriteRenderer>().material.color = Color.red;
                    }
                }
            }
        }
    }

    void Start()
    {
        playerPop();

        // StartCoroutine
        StartCoroutine(playerSelectBlock());
        StartCoroutine(blockEventUpdate());
        StartCoroutine(blockDrawController());

        is_eventstart = false;
    }

    /// <summary>
    /// プレイヤーをpopさせる位置を決める関数
    /// </summary>
    private void playerPop()
    {
        for (int y = 0; y < chip_num_y; y++)
        {
            for (int x = 0; x < chip_num_x; x++)
            {
                var block = blockcomponents[(int)LayerController.Layer.EVENT][y][x];
                if (block.number == 0)
                {
                    var pop = blocks[(int)LayerController.Layer.EVENT][y][x].transform.position;
                    pop.z += -0.5f;
                    player.transform.position = pop;
                }

            }
        }
    }

    // プレイヤーが選んでいるブロック
    private int select_cell_x;
    private int select_cell_y;
    // プレイヤーのセル
    private int player_cell_x;
    private int player_cell_y;


    /// <summary>
    /// プレイヤーがブロックを選ぶコルーチン
    /// </summary>
    private IEnumerator playerSelectBlock()
    {
        while (true)
        {

            player_cell_x = (int)player_controller.retCell().x;
            player_cell_y = (int)player_controller.retCell().y;
            select_cell_x = player_cell_x;
            select_cell_y = player_cell_y;
            switch (player_controller.player_direction)
            {
                case PlayerController.PlayerDirection.UP:
                    select_cell_y -= 1;
                    break;
                case PlayerController.PlayerDirection.DOWN:
                    select_cell_y += 1;
                    break;
                case PlayerController.PlayerDirection.RIGHT:
                    select_cell_x += 1;
                    break;
                case PlayerController.PlayerDirection.LEFT:
                    select_cell_x -= 1;
                    break;
                default:
                    break;
            }

            select_cell_x = Mathf.Clamp(select_cell_x, 0, chip_num_x - 1);
            select_cell_y = Mathf.Clamp(select_cell_y, 0, chip_num_y - 1);
            player_cell_x = Mathf.Clamp(player_cell_x, 0, chip_num_x - 1);
            player_cell_y = Mathf.Clamp(player_cell_y, 0, chip_num_y - 1);

            //   blocks[0][select_cell_y][select_cell_x]
            //.GetComponent<SpriteRenderer>().material.color = Color.red;

            yield return null;
        }
    }

    // プレイヤーでイベントが起こった時に選んでいるブロック
    private int eventselect_cell_x;
    private int eventselect_cell_y;
    // プレイヤーでイベントが起こった時のセル
    private int eventplayer_cell_x;
    private int eventplayer_cell_y;

    bool is_eventstart;

    /// <summary>
    /// ブロックのイベントのアップデートコルーチン
    /// </summary>
    private IEnumerator blockEventUpdate()
    {
        while (true)
        {
            // イベントが起こる前に通る処理
            if (!is_eventstart)
            {
                if (player_controller.state == PlayerController.State.EVENT)
                {
                    var eventblock = blockcomponents[eventlayer][select_cell_y][select_cell_x];
                    if (eventblock.number != -1)
                    {
                        if (checkEventExists(select_cell_x, select_cell_y))
                        {
                            eventselect_cell_x = select_cell_x;
                            eventselect_cell_y = select_cell_y;
                            is_eventstart = eventblock.event_manager.eventExecution();
                        }
                    }
                }


                var block = blockcomponents[eventlayer]
                    [player_cell_y]
                    [player_cell_x];

                if (block.number != -1)
                {
                    if (overLapEventExists(player_cell_x, player_cell_y))
                    {
                        eventplayer_cell_x = player_cell_x;
                        eventplayer_cell_y = player_cell_y;
                        is_eventstart = block.event_manager.eventExecution();
                        player_controller.state = PlayerController.State.EVENT;
                    }
                }
            }

            // イベントが続行しているときに通る処理
            // イベントが原因でプレイヤーの位置がずれたとき用に処理を分ける必要があったため。
            if (is_eventstart)
            {
                var eventblock = blockcomponents[eventlayer][eventselect_cell_y][eventselect_cell_x];
                if (eventblock.number != -1)
                {
                    if (checkEventExists(eventselect_cell_x, eventselect_cell_y))
                    {
                        is_eventstart = eventblock.event_manager.eventExecution();
                    }
                }

                var block = blockcomponents[eventlayer]
                    [eventplayer_cell_y]
                    [eventplayer_cell_x];

                if (block.number != -1)
                {
                    if (overLapEventExists(eventplayer_cell_x, eventplayer_cell_y))
                    {
                        is_eventstart = block.event_manager.eventExecution();
                    }
                }

                if (is_eventstart == false)
                {
                    player_controller.state = PlayerController.State.NORMAL;
                }
            }

            yield return null;
        }
    }

    /// <summary>
    /// 調べる系のイベントの有無をプレイヤーに返す関数
    /// </summary>
    public bool checkEventExists()
    {
        var eventmanager = blockcomponents[eventlayer][select_cell_y][select_cell_x]
                .event_manager;
        if (eventmanager.eventExists(eventmanager.event_stage) == false)
            return false;
        if (eventmanager.trigger_type != EventRepository.EventTriggerType.CHECK)
            return false;
        return true;
    }

    /// <summary>
    /// 調べる系のイベントの有無を返す関数
    /// </summary>
    public bool checkEventExists(int cellx_, int celly_)
    {
        var eventmanager = blockcomponents[eventlayer][celly_][cellx_]
                .event_manager;
        if (eventmanager.eventExists(eventmanager.event_stage) == false)
            return false;
        if (eventmanager.trigger_type != EventRepository.EventTriggerType.CHECK)
            return false;
        return true;
    }

    /// <summary>
    /// 通過系のイベントの有無を返す関数
    /// </summary>
    /// <param name="cellx_"></param>
    /// <param name="celly_"></param>
    /// <returns></returns>
    public bool overLapEventExists(int cellx_, int celly_)
    {
        var eventmanager = blockcomponents[eventlayer][celly_][cellx_]
        .event_manager;
        if (eventmanager.eventExists(eventmanager.event_stage) == false)
            return false;
        if (eventmanager.trigger_type != EventRepository.EventTriggerType.OVERLAP)
            return false;
        return true;
    }

    /// <summary>
    /// イベントが終了したかどうかを返す関数
    /// </summary>
    /// <returns>イベントが終了したかどうか</returns>
    public bool isEventCompleted()
    {
        if (player_controller.state == PlayerController.State.EVENT ||
            player_controller.state == PlayerController.State.TALK)
        {
            var is_completed = blockcomponents[eventlayer][eventselect_cell_y][eventselect_cell_x]
                .event_manager.IsEventCompleted;
            if (is_completed)
            {
                blockcomponents[eventlayer][eventselect_cell_y][eventselect_cell_x]
                .event_manager.IsEventCompleted = false;
                return true;
            }
        }
        if (player_controller.state == PlayerController.State.EVENT ||
            player_controller.state == PlayerController.State.TALK)
        {
            var is_completed = blockcomponents[eventlayer][eventplayer_cell_y][eventplayer_cell_x]
              .event_manager.IsEventCompleted;
            if (is_completed)
            {
                blockcomponents[eventlayer][eventplayer_cell_y][eventplayer_cell_x]
                .event_manager.IsEventCompleted = false;
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// ブロックをオンオフするコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator blockDrawController()
    {
        while (true)
        {
            chipsIsActive();
            yield return null;
        }
    }


    int framecount = 0;
    int drawcount = 0;
    Vector2 prev_camerapos;
    /// <summary>
    /// カメラに映っているブロックのアクティブを制御する関数
    /// </summary>
    public void chipsIsActive()
    {
        drawcount = 8;
        framecount++;
        Vector2 camerapos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, cameracontroller.camera_follow_z));

        if (prev_camerapos == camerapos)
            return;
        if (framecount % drawcount != 0)
            return;
        prev_camerapos = camerapos;

        Vector2 camerahalfsize = camerapos - new Vector2(Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cameracontroller.camera_follow_z)).x,
            Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cameracontroller.camera_follow_z)).y);

        Vector2 camerasize = new Vector2(Mathf.Abs(camerahalfsize.x * 2), Mathf.Abs(camerahalfsize.y * 2));

        camerapos = camerapos + (camerasize / 2);
        camerasize += new Vector2(2, 2);

        for (int i = 0; i < (int)LayerController.Layer.LAYER_MAX; i++)
        {
            for (int y = 0; y < chip_num_y; y++)
            {
                for (int x = 0; x < chip_num_x; x++)
                {
                    if (pointToCenterBox(blocks[i][y][x].transform.position, camerapos, camerasize))
                    {
                        blocks[i][y][x].SetActive(true);
                    }
                    else
                    {
                        blocks[i][y][x].SetActive(false);
                    }
                }
            }
        }
    }

    bool pointToBottomLeftBox(Vector2 pointpos_, Vector2 boxpos_, Vector2 boxsize_)
    {
        return (
            pointpos_.x > boxpos_.x &&
            pointpos_.x < boxpos_.x + boxsize_.x &&
            pointpos_.y > boxpos_.y &&
            pointpos_.y < boxpos_.y + boxsize_.y);
    }
    bool pointToCenterBox(Vector2 pointpos_, Vector2 boxpos_, Vector2 boxsize_)
    {
        return (
            pointpos_.x > boxpos_.x - boxsize_.x / 2 &&
            pointpos_.x < boxpos_.x + boxsize_.x / 2 &&
            pointpos_.y > boxpos_.y - boxsize_.y / 2 &&
            pointpos_.y < boxpos_.y + boxsize_.y / 2);
    }

    /// <summary>
    /// 渡された座標が範囲外かどうか返す関数
    /// </summary>
    public bool isOutOfRange(int x_, int y_)
    {
        if (x_ < 0 || y_ < 0)
            return true;
        if (x_ > chip_num_x - 1 || y_ > chip_num_y - 1)
            return true;
        return false;
    }

    /// <summary>
    /// マップを読み込む関数
    /// </summary>
    /// <param name="loadname_"></param>
    public void loadMap(string loadname_)
    {
        var statustext = Resources.Load<TextAsset>("StageData/" + loadname_ + "_StatusData");

        using (var sr = new StringReader(statustext.text))
        {
            string line = sr.ReadLine();
            chip_num_x = stringToInt(line, 0);
            chip_num_y = stringToInt(line, 1);
        }

        for (int i = 0; i < (int)LayerController.Layer.LAYER_MAX; i++)
        {
            string layername = LayerController.layernumToString(i);
            Sprite[] loadsprite = Resources.LoadAll<Sprite>("Textures/MapChip/" + layername);

            var stagelayertext = Resources.Load<TextAsset>("StageData/" + loadname_ + "_" + layername + "Data");
            using (var sr = new StringReader(stagelayertext.text))
            {
                int index = 0;
                List<List<GameObject>> tempblock_xy = new List<List<GameObject>>();
                for (int y = 0; y < chip_num_y; y++)
                {
                    string line = sr.ReadLine();

                    List<GameObject> tempblock_x = new List<GameObject>();
                    for (int x = 0; x < chip_num_x; x++)
                    {
                        GameObject block;
                        block = Resources.Load<GameObject>("Prefabs/BlockBase");

                        int number = stringToInt(line, x);

                        var renderer = block.GetComponent<SpriteRenderer>();
                        if (number != -1 && i != (int)LayerController.Layer.EVENT)
                        {
                            renderer.sprite =
                                System.Array.Find<Sprite>(
                                    loadsprite, (sprite) => sprite.name.Equals(
                                        layername + "_" + number.ToString()));

                            var renderer_rect = renderer.sprite.rect;
                            var size = (int)(renderer_rect.width);

                            chip_size = chip_scale * size * 0.01f;
                        }
                        else
                        {
                            // 空白読み
                            renderer.sprite = null;
                        }

                        block.GetComponent<Block>().number = number;
                        // Astarで使う一次元配列番号
                        block.GetComponent<Block>().index = index;
                        index++;

                        block.transform.position = new Vector3(
                            chip_size * x,
                            chip_size * y * -1,
                            -i * 0.005f);

                        block.transform.localScale = new Vector2(chip_scale, chip_scale);

                        block.GetComponent<BoxCollider2D>().isTrigger = true;
                        if (i == (int)LayerController.Layer.WALL)
                        {
                            if (number != -1)
                            {
                                block.GetComponent<BoxCollider2D>().isTrigger = false;
                            }
                        }

                        tempblock_x.Add(Instantiate(block));
                        tempblock_x[x].transform.parent = gameObject.transform;
                    }
                    tempblock_xy.Add(tempblock_x);
                }
                blocks.Add(tempblock_xy);

            }
        }

        blockcomponents = new List<List<List<Block>>>();
        for (int i = 0; i < (int)LayerController.Layer.LAYER_MAX; i++)
        {
            List<List<Block>> component_block_temp_xy = new List<List<Block>>();
            for (int y = 0; y < chip_num_y; y++)
            {
                List<Block> commponent_block_temp_x = new List<Block>();
                for (int x = 0; x < chip_num_x; x++)
                {
                    commponent_block_temp_x.Add(blocks[i][y][x].GetComponent<Block>());
                }
                component_block_temp_xy.Add(commponent_block_temp_x);
            }
            blockcomponents.Add(component_block_temp_xy);
        }

    }

    /// <summary>
    /// マップをクリアする関数
    /// </summary>
    public void mapClear()
    {
        for (int i = 0; i < (int)LayerController.Layer.LAYER_MAX; i++)
        {
            for (int y = 0; y < chip_num_y; y++)
            {
                for (int x = 0; x < chip_num_x; x++)
                {
                    Destroy(blocks[i][y][x]);
                    Destroy(blockcomponents[i][y][x]);
                }
            }
        }
        blocks.Clear();
        blockcomponents.Clear();
    }


    /// <summary>
    /// 文字列を空白区切りでintに変換する関数
    /// </summary>
    /// <param name="value_"></param>
    /// <param name="num_"></param>
    /// <returns></returns>
    public int stringToInt(string value_, int num_)
    {
        int retvalue = 0;
        int value_count = 0;
        string temp_value = "";
        char[] c = value_.ToCharArray();
        bool is_blank = false;

        for (int i = 0; i < value_.Length; i++)
        {
            if (is_blank)
            {
                if (c[i] != ' ')
                {
                    is_blank = false;
                }
            }
            if (is_blank == false)
            {
                if (c[i] == ' ')
                {
                    is_blank = true;

                    if (value_count == num_)
                    {
                        retvalue = int.Parse(temp_value);
                        break;
                    }
                    temp_value = "";
                    value_count++;
                    continue;
                }
                temp_value += c[i];
            }

        }
        return retvalue;
    }
}


