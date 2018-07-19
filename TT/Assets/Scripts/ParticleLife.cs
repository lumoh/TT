using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLife : MonoBehaviour 
{
    public float Duration;

	// Use this for initialization
	void Start () 
    {
        LeanTween.delayedCall(gameObject, Duration, () =>
        {
            Destroy(gameObject);
        });
	}
}
