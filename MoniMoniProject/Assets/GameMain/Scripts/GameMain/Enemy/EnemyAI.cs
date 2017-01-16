using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.IO;

public class EnemyAI : MonoBehaviour
{

    // エネミーの番号
    public int enemynumber = 0;

    Animator anim;
    // エネミーの向き
    public enum EnemyDirection
    {
        UP, DOWN, RIGHT, LEFT
    }

    public EnemyDirection direction = EnemyDirection.DOWN;
    public Vector2 vec;
    public float speed = 0;
    public float up_speed = 0;

    public int locateplayermode_count;
    public int locateplayermode_maxcount;

    public enum State
    {
        IDLE,                   // 移動しない状態
        ROOT_NORMALMOVE,        // 移動している状態
        ROOT_LOCATEPLAYERMOVE,  // プレイヤーを見つけて速度が上がった状態
        ROOT_CHANGE,            // 通っていたルートを変更してる状態
        SPENT_TRAP,             // トラップにかかった状態
    }

    public State state;
    State currentstate;

    public MapChipController mapchip;
    public PlayerController player;

    public enum RootType
    {
        NONE,   // 道なし
        START,  // 道の始まり
        ROOT,   // 道
    }

    // ルートの配列(ルートの種類,y,x)
    public List<List<List<RootType>>> roots;
    public RootType current_root;

    // ルートの数
    public int rootmax;
    // 通っているルート番号
    public int rootnum;
    public int current_rootnum;

    // 周回数
    public int root_roundcount;
    // 周回数によってルートを変更する回数
    public int root_roundmax;


    void Start()
    {
        aiSetup();
    }

    public void aiSetup()
    {
        // プレハブからヒエラルキーのオブジェクトがもらえなかったので仕方なく名前検索
        mapchip = GameObject.Find("MapManager").GetComponent<MapChipController>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();

        vec = new Vector2();
        currentstate = State.IDLE;
        state = State.ROOT_CHANGE;
        anim = GetComponent<Animator>();
        roots = new List<List<List<RootType>>>();
        rootsLoad();
        rootRandomDecide();
    }


    void Update()
    {
        stateChangeUpdate();
        animationUpdate();
        rootmove();
        rootChangeMove();
        lookForPlayer();
        locatePlayerModeUpdate();
    }

    void animationUpdate()
    {
        anim.SetInteger("enemy_direction", (int)direction);
        anim.SetFloat("up_down_vec", vec.y * 100);
        anim.SetFloat("right_left_vec", vec.x * 100);
    }

    // 移動先のマップ番号
    Vector2 nextmovecell;

    // 移動した量
    float move_value;

    /// <summary>
    /// ルートを移動する関数
    /// </summary>
    private void rootmove()
    {
        if (state == State.ROOT_NORMALMOVE ||
            state == State.ROOT_LOCATEPLAYERMOVE)
        {
            if (move_value >= mapchip.chip_size)
            {
                move_value = 0;
                int x = (int)retCell().x;
                int y = (int)retCell().y;
                adjustPosition(x, y);

                nextmovecell = nextMoveCell();
                if (roots[rootnum][y][x] == RootType.START)
                {
                    root_roundcount++;
                    if (root_roundcount >= root_roundmax)
                    {
                        state = State.ROOT_CHANGE;
                        root_roundcount = 0;
                        return;
                    }
                }
                directionChange(x, y, (int)nextmovecell.x, (int)nextmovecell.y);
            }
            else
            {
                float s;
                if (state == State.ROOT_LOCATEPLAYERMOVE)
                    s = up_speed;
                else
                    s = speed;
                directionMove(s);
                move_value += s;
            }
        }

    }

    /// <summary>
    /// 向きによって移動する関数
    /// </summary>
    void directionMove(float speed_)
    {
        vec = Vector2.zero;

        switch (direction)
        {
            case EnemyDirection.UP:
                vec.y += speed_;
                break;
            case EnemyDirection.DOWN:
                vec.y -= speed_;
                break;
            case EnemyDirection.RIGHT:
                vec.x += speed_;
                break;
            case EnemyDirection.LEFT:
                vec.x -= speed_;
                break;
        }
        transform.Translate(vec);
    }

    /// <summary>
    /// 今乗っているブロックと今向いている方向から次移動する番号を返す関数
    /// </summary>
    Vector2 nextMoveCell()
    {
        int cell_x = (int)retCell().x;
        int cell_y = (int)retCell().y;
        cell_x = Mathf.Clamp(cell_x, 0, mapchip.chip_num_x - 1);
        cell_y = Mathf.Clamp(cell_y, 0, mapchip.chip_num_y - 1);

        int next_cell_x = cell_x;
        int next_cell_y = cell_y;

        switch (direction)
        {
            case EnemyDirection.UP:
                // 前
                if (cell_y - 1 > 0)
                    if (roots[rootnum][cell_y - 1][cell_x] != RootType.NONE)
                    {
                        next_cell_y -= 1;
                        break;
                    }
                // 左
                if (cell_x - 1 > 0)
                    if (roots[rootnum][cell_y][cell_x - 1] != RootType.NONE)
                    {
                        next_cell_x -= 1;
                        break;
                    }
                // 右
                if (cell_x + 1 < mapchip.chip_num_x)
                    if (roots[rootnum][cell_y][cell_x + 1] != RootType.NONE)
                    {
                        next_cell_x += 1;
                        break;
                    }
                // 後ろ
                next_cell_y += 1;
                break;
            case EnemyDirection.DOWN:
                // 前
                if (cell_y + 1 < mapchip.chip_num_y)
                    if (roots[rootnum][cell_y + 1][cell_x] != RootType.NONE)
                    {
                        next_cell_y += 1;
                        break;
                    }
                // 左
                if (cell_x + 1 < mapchip.chip_num_x)
                    if (roots[rootnum][cell_y][cell_x + 1] != RootType.NONE)
                    {
                        next_cell_x += 1;
                        break;
                    }
                // 右
                if (cell_x - 1 > 0)
                    if (roots[rootnum][cell_y][cell_x - 1] != RootType.NONE)
                    {
                        next_cell_x -= 1;
                        break;
                    }
                // 後ろ
                next_cell_y -= 1;
                break;
            case EnemyDirection.RIGHT:
                // 前
                if (cell_x + 1 < mapchip.chip_num_x)
                    if (roots[rootnum][cell_y][cell_x + 1] != RootType.NONE)
                    {
                        next_cell_x += 1;
                        break;
                    }
                // 左
                if (cell_y - 1 > 0)
                    if (roots[rootnum][cell_y - 1][cell_x] != RootType.NONE)
                    {
                        next_cell_y -= 1;
                        break;
                    }
                // 右
                if (cell_y + 1 < mapchip.chip_num_y)
                    if (roots[rootnum][cell_y + 1][cell_x] != RootType.NONE)
                    {
                        next_cell_y += 1;
                        break;
                    }
                // 後ろ
                next_cell_x -= 1;
                break;
            case EnemyDirection.LEFT:
                // 前
                if (cell_x - 1 > 0)
                    if (roots[rootnum][cell_y][cell_x - 1] != RootType.NONE)
                    {
                        next_cell_x -= 1;
                        break;
                    }
                // 左
                if (cell_y + 1 < mapchip.chip_num_y)
                    if (roots[rootnum][cell_y + 1][cell_x] != RootType.NONE)
                    {
                        next_cell_y += 1;
                        break;
                    }
                // 右
                if (cell_y - 1 > 0)
                    if (roots[rootnum][cell_y - 1][cell_x] != RootType.NONE)
                    {
                        next_cell_y -= 1;
                        break;
                    }

                // 後ろ
                next_cell_x += 1;
                break;
        }

        return new Vector2(next_cell_x, next_cell_y);
    }

    /// <summary>
    /// 今の番号と移動先の番号で方向を変える関数
    /// </summary>
    /// <param name="current_">今の番号x,y</param>
    /// <param name="next_">移動先の番号x,y</param>
    void directionChange(int current_x_, int current_y_, int next_x_, int next_y_)
    {
        if (current_y_ > next_y_)
        {
            direction = EnemyDirection.UP;
            return;
        }
        else if (current_y_ < next_y_)
        {
            direction = EnemyDirection.DOWN;
            return;
        }
        if (current_x_ < next_x_)
        {
            direction = EnemyDirection.RIGHT;
            return;
        }
        else if (current_x_ > next_x_)
        {
            direction = EnemyDirection.LEFT;
        }
    }

    /// <summary>
    /// もらった番号のブロックの中心位置に自分を移動させる
    /// </summary>
    private void adjustPosition(int x_, int y_)
    {
        transform.position = new Vector3(mapchip.blocks[(int)LayerController.Layer.FLOOR][y_][x_].transform.position.x,
            mapchip.blocks[(int)LayerController.Layer.FLOOR][y_][x_].transform.position.y,
            -0.4f);
    }

    /// <summary>
    /// enemyの位置からcell番号(x, y)を返す
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

    enum AstarState
    {
        SEARCH,
        WORK_START,
        WORK,
        END,
    }

    AstarState astarstate;
    AStar.RootPosition goalpos;
    AStar.NodeManager nodemanager;
    AStar.Node node;
    List<AStar.RootPosition> rootlist;

    int astar_trycount = 0;
    int astarmovecount = 0;

    /// <summary>
    /// A*準備
    /// </summary>
    void astarSetup()
    {
        rootRandomDecide();
        astarstate = AstarState.SEARCH;

        int x = (int)retCell().x;
        int y = (int)retCell().y;
        var startpos = new AStar.RootPosition(x, y);

        Vector2 tempgoalpos;
        tempgoalpos = getGoalCell();
        goalpos = new AStar.RootPosition((int)tempgoalpos.x, (int)tempgoalpos.y);

        nodemanager = new AStar.NodeManager(goalpos.x, goalpos.y, mapchip);
        node = nodemanager.openNode(startpos.x, startpos.y, 0, null);
        nodemanager.addOpenNode(node);

        rootlist = new List<AStar.RootPosition>();

        astar_trycount = 0;
        astarmovecount = 0;
    }

    /// <summary>
    /// ルートを変更して移動する関数
    /// </summary>
    void rootChangeMove()
    {
        if (state != State.ROOT_CHANGE) return;
        int x = (int)retCell().x;
        int y = (int)retCell().y;
        if (astarstate == AstarState.SEARCH)
        {
            while (astar_trycount < 5000)
            {
                nodemanager.removeOpenNode(node);
                nodemanager.openAround(node);

                node = nodemanager.searchMinScoreNodeFromOpenNodeList();

                if (node == null)
                    break;
                if (node.x == goalpos.x && node.y == goalpos.y)
                {
                    nodemanager.removeOpenNode(node);
                    node.getRootList(rootlist);
                    rootlist.Reverse();
                    astarstate = AstarState.WORK_START;
                    astar_trycount = 0;
                    break;
                }
                astar_trycount++;
            }
        }
        else if (astarstate == AstarState.WORK_START)
        {
            if (astarmovecount >= rootlist.Count - 1)
            {
                astarmovecount = 0;
                state = State.ROOT_NORMALMOVE;
                return;
            }
            astarmovecount++;
            nextmovecell = new Vector2(rootlist[astarmovecount].x, rootlist[astarmovecount].y);
            directionChange(x, y, (int)nextmovecell.x, (int)nextmovecell.y);
            astarstate = AstarState.WORK;
        }
        else if (astarstate == AstarState.WORK)
        {
            float s = up_speed;
            directionMove(s);
            move_value += s;
            if (move_value >= mapchip.chip_size)
            {
                adjustPosition(x, y);
                astarstate = AstarState.WORK_START;
                move_value = 0;
            }
        }
    }

    /// <summary>
    /// ルートをランダムに変更する関数（同じルートは通らない）
    /// </summary>
    void rootRandomDecide()
    {
        if (rootmax <= 0) return;
        while (true)
        {
            int rand_num = Random.Range(0, rootmax);
            if (rand_num != rootnum)
            {
                rootnum = rand_num;
                break;
            }
        }
    }

    Vector2 getGoalCell()
    {
        Vector2 goalpos = new Vector2();

        for (int y = 0; y < mapchip.chip_num_y; y++)
        {
            for (int x = 0; x < mapchip.chip_num_x; x++)
            {
                if (roots[rootnum][y][x] == RootType.START)
                    goalpos = new Vector2(x, y);
            }
        }

        return goalpos;
    }

    /// <summary>
    /// エネミーの状態が変わった時に何かを起こす関数
    /// </summary>
    void stateChangeUpdate()
    {
        if (currentstate == state) return;
        currentstate = state;

        switch (state)
        {
            case State.IDLE:
                break;
            case State.ROOT_NORMALMOVE:
                nextmovecell = nextMoveCell();
                break;
            case State.ROOT_LOCATEPLAYERMOVE:
                break;
            case State.ROOT_CHANGE:
                astarSetup();
                break;
            case State.SPENT_TRAP:
                break;
        }

    }

    /// <summary>
    /// ルート読み込み
    /// </summary>
    void rootsLoad()
    {
        int chip_x = mapchip.chip_num_x;
        int chip_y = mapchip.chip_num_y;

        rootmax = 0;
        while (true)
        {
            var readtext = Resources.Load<TextAsset>("StageData/"
               + mapchip.select_map_name
               + "_Enemy" + enemynumber
               + "Root" + rootmax + "Data");


            if (readtext == null)
                break;

            int[][] loadrootnums = new int[chip_y][];
            for (int y = 0; y < chip_y; y++)
            {
                loadrootnums[y] = new int[chip_x];
            }

            using (var sr =
                new StringReader(readtext.text))
            {
                // スタートの番号
                int usestartnum = int.MaxValue;

                for (int y = 0; y < chip_y; y++)
                {
                    string line = sr.ReadLine();
                    for (int x = 0; x < chip_x; x++)
                    {
                        loadrootnums[y][x] = mapchip.stringToInt(line, x);
                        if (loadrootnums[y][x] != -1)
                        {
                            if (usestartnum > loadrootnums[y][x])
                                usestartnum = loadrootnums[y][x];

                        }
                    }
                }

                List<List<RootType>> temproot_xy = new List<List<RootType>>();
                for (int y = 0; y < chip_y; y++)
                {
                    List<RootType> temploot_x = new List<RootType>();
                    for (int x = 0; x < chip_x; x++)
                    {
                        RootType tempnum = RootType.NONE;
                        if (loadrootnums[y][x] != -1)
                        {
                            if (usestartnum == loadrootnums[y][x])
                            {
                                tempnum = RootType.START;
                            }
                            else
                            {
                                tempnum = RootType.ROOT;
                            }
                        }

                        temploot_x.Add(tempnum);
                    }
                    temproot_xy.Add(temploot_x);
                }
                roots.Add(temproot_xy);

                sr.Close();
            }
            rootmax++;
        }
    }

    void lookForPlayer()
    {
        if (state == State.ROOT_NORMALMOVE)
            if (searchPlayer())
            {
                state = State.ROOT_LOCATEPLAYERMOVE;
            }
    }

    /// <summary>
    /// プレイヤーが見つかったかどうか(とりあえず周囲八マスだけ)
    /// </summary>
    bool searchPlayer()
    {
        int px = (int)player.retCell().x;
        int py = (int)player.retCell().y;
        int ex = (int)retCell().x;
        int ey = (int)retCell().y;
        if (direction == EnemyDirection.UP)
        {
            if ((ex + 1 == px && ey == py) ||
                (ex - 1 == px && ey == py) ||
                (ex == px && ey + 1 == py) ||
                (ex == px && ey - 1 == py) ||
                (ex + 1 == px && ey + 1 == py) ||
                (ex + 1 == px && ey - 1 == py) ||
                (ex - 1 == px && ey + 1 == py) ||
                (ex - 1 == px && ey - 1 == py))
            {
                return true;
            }
        }
        else if (direction == EnemyDirection.DOWN)
        {
            if ((ex + 1 == px && ey == py) ||
                (ex - 1 == px && ey == py) ||
                (ex == px && ey + 1 == py) ||
                (ex == px && ey - 1 == py) ||
                (ex + 1 == px && ey + 1 == py) ||
                (ex + 1 == px && ey - 1 == py) ||
                (ex - 1 == px && ey + 1 == py) ||
                (ex - 1 == px && ey - 1 == py))
            {
                return true;
            }
        }
        else if (direction == EnemyDirection.RIGHT)
        {
            if ((ex + 1 == px && ey == py) ||
                (ex - 1 == px && ey == py) ||
                (ex == px && ey + 1 == py) ||
                (ex == px && ey - 1 == py) ||
                (ex + 1 == px && ey + 1 == py) ||
                (ex + 1 == px && ey - 1 == py) ||
                (ex - 1 == px && ey + 1 == py) ||
                (ex - 1 == px && ey - 1 == py))
            {
                return true;
            }
        }
        else if (direction == EnemyDirection.LEFT)
        {
            if ((ex + 1 == px && ey == py) ||
                (ex - 1 == px && ey == py) ||
                (ex == px && ey + 1 == py) ||
                (ex == px && ey - 1 == py) ||
                (ex + 1 == px && ey + 1 == py) ||
                (ex + 1 == px && ey - 1 == py) ||
                (ex - 1 == px && ey + 1 == py) ||
                (ex - 1 == px && ey - 1 == py))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 警戒モードのアップデート
    /// </summary>
    void locatePlayerModeUpdate()
    {
        if (state != State.ROOT_LOCATEPLAYERMOVE) return;

        locateplayermode_count++;
        if (locateplayermode_count >= locateplayermode_maxcount)
        {
            state = State.ROOT_NORMALMOVE;
            locateplayermode_count = 0;
        }
    }

}
