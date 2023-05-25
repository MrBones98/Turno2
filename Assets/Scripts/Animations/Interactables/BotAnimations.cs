using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BotAnimations : MonoBehaviour
{
    [SerializeField] private GameObject _eyeLeft, _eyeRight;
    private bool _blinkInProgress = false;

    private float _eyeClosedScaleY = .05f;
    //float randomTimer = Random.Range(5f, 20f);

    void Update()
    {
        if (_blinkInProgress == false)
        {
            StartCoroutine(Blink(Random.Range(5f, 20f)));
        }
    }

    private IEnumerator Blink(float delay)
    {
        _blinkInProgress = true;
        yield return new WaitForSeconds(delay);

        _eyeLeft.transform.DOScaleY(_eyeClosedScaleY, .2f).SetLoops(2, LoopType.Yoyo);
        _eyeRight.transform.DOScaleY(_eyeClosedScaleY, .2f).SetLoops(2, LoopType.Yoyo);

        _blinkInProgress = false;
        yield return null;
    }
}
