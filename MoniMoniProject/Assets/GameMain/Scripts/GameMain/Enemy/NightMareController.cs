using UnityEngine;
using System.Collections;

public class NightMareController : MonoBehaviour
{
    [SerializeField]
    MapChipController mapchip;

    public bool is_move;
    public float speed;

    public int movecount;

    public Vector2 prev_cell;
    // Use this for initialization
    void Start()
    {
        is_move = false;
        movecount = 0;
        prev_cell = retCell();
    }

    // Update is called once per frame
    void Update()
    {

     

    }

    public Vector2 retCell()
    {
        var pos = transform.position;
        Vector2 cell_f = pos / mapchip.chip_size;

        Vector2 cell_i = new Vector2(Mathf.RoundToInt(cell_f.x), Mathf.RoundToInt(cell_f.y));
        cell_i.x = Mathf.Abs(cell_i.x);
        cell_i.y = Mathf.Abs(cell_i.y);

        return cell_i;
    }
}
