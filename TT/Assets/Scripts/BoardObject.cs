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

    [SerializeField] private bool _canSwap = false;
    public bool CanSwap { get { return _canSwap; } }

    [SerializeField] private bool _canMatch = false;
    public bool CanMatch { get { return _canMatch; } }

    [HideInInspector] public int LastMove = 0;

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

    public void OnMouseDown()
    {
        if (State == BoardObjectState.SETTLED && CanSwap)
        {
            Tile adjacentTile = MyBoard.GetTile(X + 1, Y);
            if (adjacentTile != null)
            {
                BoardObject boardObject = adjacentTile.GetBoardObect(TileLayer);
                if (boardObject != null && boardObject.State == BoardObjectState.SETTLED && boardObject.CanSwap)
                {
                    State = BoardObjectState.SWAPPING;
                    boardObject.State = BoardObjectState.SWAPPING;

                    LeanTween.move(boardObject.gameObject, MyTile.transform.position, 0.25f).setEase(LeanTweenType.easeOutSine);
                    LeanTween.move(gameObject, boardObject.MyTile.transform.position, 0.25f).setEase(LeanTweenType.easeOutSine).setOnComplete(() =>
                    {
                        Tile oldTile = boardObject.MyTile;
                        MyTile.RemoveBoardObject(TileLayer);
                        MyTile.AddBoardObject(boardObject);
                        oldTile.AddBoardObject(this);

                        boardObject.State = BoardObjectState.SETTLED;
                        State = BoardObjectState.SETTLED;
                    });
                }
            }
        }              
    }

    public virtual void Break()
    {
        State = BoardObjectState.BREAKING;
        MyTile.RemoveBoardObject(TileLayer);
        Destroy(gameObject);
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
        _canSwap = false;
        _canMatch = false;
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
