using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// object that resides on a tile on the board
/// </summary>
public class BoardObject : MonoBehaviour 
{
    [HideInInspector] public Tile MyTile;
    public Board MyBoard { get { return MyTile.Board; } }

    public int X
    {
        get 
        {
            int x = -1;
            if (MyTile != null)
            {
                x = MyTile.X;
            }
            return x;
        }
    }

    public int Y
    {
        get 
        {
            int y = -1;
            if (MyTile != null)
            {
                y = MyTile.Y;
            }
            return y;
        }
    }

    [Header("Board Settings")]
    public int TileLayer;

    [HideInInspector] public BoardObjectState State = BoardObjectState.SETTLED;

    [SerializeField] private BoardObjectColor _color = BoardObjectColor.NONE;
    public BoardObjectColor Color 
    { 
        get { return _color; } 
        set 
        {
            _color = value; 
            setSpriteForColor();
        }
    }

    [SerializeField] private BoardObjectType _type = BoardObjectType.NONE;
    public BoardObjectType Type 
    {       
        get { return _type; } 
        set { /* TODO */ }
    }

    [SerializeField] public bool Swappable = true;
    [SerializeField] public bool Matchable = true;
    private bool _active = true;

    [HideInInspector] public int LastMove = 0;

    public float SwapSpeed = 0.2f;

    [Header("Sprite Settings")]
    public SpriteRenderer MySpriteRenderer;   
    public Sprite NONE;
    public Sprite RED;
    public Sprite ORANGE;
    public Sprite YELLOW;
    public Sprite GREEN;
    public Sprite BLUE;
    public Sprite PURPLE;

	// Use this for initialization
	public virtual void Start() 
    {
        State = BoardObjectState.SETTLED;
	}

    public virtual void Init(BoardObjectColor c, int tileLayer)
    {
        TileLayer = tileLayer;
        Color = c;
    }

    public bool CanSwap()
    {
        bool settled = State == BoardObjectState.SETTLED;
        return Swappable &&  settled && _active;
    }

    public bool CanMatch()
    {
        bool settled = State == BoardObjectState.SETTLED;
        return Matchable && settled && _active;
    }

    public void Swap(Tile t)
    {
        if(CanSwap())
        {
            MyTile.RemoveBoardObject(TileLayer);
            State = BoardObjectState.SWAPPING;
            LeanTween.move(gameObject, t.transform.position, 0.1f).setOnComplete(() =>
            {
                t.AddBoardObject(this);
                State = BoardObjectState.SETTLED;
            });
        }
    }

    /// <summary>
    /// TEMP CONTROLLER
    /// </summary>
    public void OnMouseDown()
    {
        Tile adjacentTile = MyBoard.GetTile(X + 1, Y, true);
        if (adjacentTile != null)
        {
            MyBoard.Swap(MyTile, adjacentTile);
        }
    }

    public virtual void Break()
    {        
        SetActive(false);
        State = BoardObjectState.BREAKING;

        LeanTween.delayedCall(gameObject, 2f, () =>
        {
            MyTile.RemoveBoardObject(TileLayer);
            Destroy(gameObject);
        });
    }

    public virtual void OnDestroy()
    {
        LeanTween.cancel(gameObject);
    }

    public void SetActive(bool active)
    {
        Color c = MySpriteRenderer.color;
        c.a = 0.25f;
        MySpriteRenderer.color = c;
        _active = active;
    }

    protected virtual void setSpriteForColor()
    {
        if (MySpriteRenderer != null)
        {
            if (_color == BoardObjectColor.RED)
            {
                MySpriteRenderer.sprite = RED;
            }
            else if (_color == BoardObjectColor.ORANGE)
            {
                MySpriteRenderer.sprite = ORANGE;
            }
            else if (_color == BoardObjectColor.YELLOW)
            {
                MySpriteRenderer.sprite = YELLOW;
            }
            else if (_color == BoardObjectColor.GREEN)
            {
                MySpriteRenderer.sprite = GREEN;
            }
            else if (_color == BoardObjectColor.BLUE)
            {
                MySpriteRenderer.sprite = BLUE;
            }
            else if (_color == BoardObjectColor.PURPLE)
            {
                MySpriteRenderer.sprite = PURPLE;
            }
        }
    }
}
