using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    public BoardObjectColor Color = BoardObjectColor.NONE;
    public BoardObjectType Type = BoardObjectType.NONE;
    public int Amount = 0;
    public bool Complete = false;

    private Text _amountText;

    void Awake()
    {
        _amountText = GetComponentInChildren<Text>();
    }

    public void Collect()
    {
        Amount--;
        if(_amountText != null)
        {
            _amountText.text = Amount.ToString();
        }
    }

    public void Reset()
    {
        Complete = false;
        Amount = 10;
        if(_amountText != null)
        {
            _amountText.text = Amount.ToString();
        }
    }
}

