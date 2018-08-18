using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Ocillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;

    float _movementFactor;

    Vector3 _startingPos;

	// Use this for initialization
	void Start () {
        _startingPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (period <= Mathf.Epsilon)
            return;

        float cycles = Time.time / period;

        //Vector3 offset = movementFactor * movementVector;
        //transform.position = _startingPos + offset;

        const float tau = Mathf.PI * 2; // about 6.28

        float rawSinWave = Mathf.Sin(cycles * tau);

        _movementFactor = rawSinWave / 2f + 0.5f;

        Vector3 offset = _movementFactor * movementVector;
        transform.position = _startingPos + offset;
    }
}
