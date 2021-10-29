using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Next : MonoBehaviour
{
    private Transform _originalRotations;
    private Transform _fixedRotations;

    private void Awake()
    {
        _originalRotations = GameObject.Find("Original Rotations").transform;
        _fixedRotations = GameObject.Find("Fixed Rotations").transform;
    }

    public void NextButtonClicked()
    {
        StartCoroutine("ShowFixedRotations");
        gameObject.GetComponent<Button>().interactable = false;
    }

    private IEnumerator ShowFixedRotations()
    {
        Vector3 startPositionOriginal = _originalRotations.position;
        Vector3 startPositionFixed = _fixedRotations.position;
        Vector3 endPositionOriginal = new Vector3(-18, 0, 0);
        Vector3 endPositionFixed = new Vector3(12, 0, 0);

        float duration = 1f;
        float startTime = Time.time;
        float endTime = Time.time + duration;

        while(Time.time < endTime + 0.1f)
        {
            float elapsed = Time.time - startTime;
            float pctComplete = elapsed / duration;

            _originalRotations.position = Vector3.Slerp(startPositionOriginal, endPositionOriginal, pctComplete);
            _fixedRotations.position = Vector3.Slerp(startPositionFixed, endPositionFixed, pctComplete);

            yield return null;
        }
    }
}
