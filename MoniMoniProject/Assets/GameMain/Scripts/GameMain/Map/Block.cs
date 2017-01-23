using UnityEngine;
using System.Collections;


/// <summary>
/// ブロック単体のクラス
/// </summary>
public class Block : MonoBehaviour
{
    private bool is_select;
    public bool IsSelect
    {
        get { return is_select; }
        set { is_select = value; }
    }

    [SerializeField]
    public EventManager event_manager = null;

    [SerializeField]
    SpriteRenderer spriterenderer;

    public int number;
    public int index;
}
