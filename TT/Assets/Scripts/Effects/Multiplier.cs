using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Multiplier : MonoBehaviour 
{
    public float Duration;
    public GameObject Outline;
    public Text Text;
    public Canvas Canvas;

    public void Init(int multiplier, Camera cam)
    {
        if(Outline != null && Text != null && Canvas != null)
        {
            Canvas.worldCamera = cam;
            Text.text = multiplier.ToString() + "x";
            LeanTween.rotateAround(Outline, Vector3.forward, 360f, Duration).setOnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}
