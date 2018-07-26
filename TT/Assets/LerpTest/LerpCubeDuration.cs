using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpCubeDuration : MonoBehaviour 
{   
    public Vector3 EndPos;
    public float Duration;

    private float _startTime;
    private Vector3 _startPos;

	// Use this for initialization
	void Start() 
    {
        _startTime = Time.time;
        _startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update() 
    {
        float elapsedTime = Time.time - _startTime;
        if(elapsedTime >= Duration)
        {
            transform.position = EndPos;
        }
        else
        {
            float percent = elapsedTime / Duration;
            transform.position = lerp(_startPos, EndPos, percent);
        }
	}

    Vector3 lerp(Vector3 start, Vector3 end, float percent)
    {
        return start + ((end - start) * percent);
    }
}
