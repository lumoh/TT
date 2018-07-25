using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    public BlockColor Color = BlockColor.None;
    public BlockType Type = BlockType.None;
    public int Amount = 0;
    public bool Complete = false;

    private Text _amountText;

    void Awake()
    {
        _amountText = GetComponentInChildren<Text>();
        setText();
    }

    public void Collect()
    {
        Amount--;
        setText();
        if(Amount == 0)
        {
            Complete = true;
        }
    }

    public void Reset()
    {
        Complete = false;
        Amount = 10;
        setText();
    }

    private void setText()
    {
        if(_amountText != null)
        {
            _amountText.text = Amount.ToString();
        }
    }
}

