using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHandler : MonoBehaviour
{
    
    public delegate bool UpdateCalls();
    float unitDelay;
    float unitMaxDelay;

    public void StartTimedUpdate(float _unitDelay, float _unitMaxDelay, UpdateCalls func)
    {
        Debug.Log("Start Coroutine called!");
        unitDelay = _unitDelay;
        unitMaxDelay = _unitMaxDelay;
        StartCoroutine(TimedUpdateRoutine(func));
    }

    // OK
    IEnumerator TimedUpdateRoutine (UpdateCalls cb)
	{
        Debug.Log("Starting Coroutine!");
		while (cb()) {
			yield return null;
		}
	}
}
