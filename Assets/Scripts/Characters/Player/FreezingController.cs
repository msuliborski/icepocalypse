using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Scripts.Variables;

public class FreezingController : MonoBehaviour {
    public float FreezeTime = 30.0f;
    public float WarmMultiplier = 2.0f;
    public GameEvent FreezeEvent;

    private FrostEffect frostEffect;
    float _freezeLevel;
    bool _freezed;
    bool _inWarmArea;

    bool _gameStarted = false;

	void Start()
	{
        _freezeLevel = 0;
        _freezed = false;
        _inWarmArea = false;
        frostEffect = Camera.main.GetComponent<FrostEffect>();

        StartCoroutine(StartFrostEffect());

	}

    public void OnGameStarted()
    {
        _gameStarted = true;
        StopAllCoroutines();
        frostEffect.FrostAmount = 0;

        StartCoroutine(TakeHPWhenFreezed());
    }


    public void OnPlayerDeath()
    {
        _gameStarted = false;
        StopAllCoroutines();
        StartCoroutine(StartFrostEffect());
    }

    IEnumerator TakeHPWhenFreezed()
    {
        while(true)
        {
            if (_freezeLevel / FreezeTime > 0.5f && _gameStarted)
            {
                GetComponent<PlayerControllerExperimental>().TakeHP(10);
            }
            yield return new WaitForSeconds(4.0f);
        }
    }

    IEnumerator StartFrostEffect()
    {
        float t = 0f;
        while (!_gameStarted)
        {
            for (t = 4f; t > 0f; t -= Time.unscaledDeltaTime)
            {
                frostEffect.FrostAmount = (1.0f - (t / 4.0f)) * 0.5f;
                yield return null;
            }

            yield return new WaitForSeconds(2.0f);
            for (t = 4f; t > 0f; t -= Time.unscaledDeltaTime)
            {
                
                frostEffect.FrostAmount = (t / 4f) * 0.5f;
                yield return null;
            }

            yield return new WaitForSeconds(2.0f);
        }


        yield return null;
    }

	void Update()
	{

        if (!_gameStarted)
            return;
        if (_freezed)
            return;
        

        if (_freezeLevel > FreezeTime)
        {
            _freezed = true;
        } 
        else if (_inWarmArea)
        {
            if (_freezeLevel > 0)
            {
                _freezeLevel -= Time.deltaTime * WarmMultiplier;
            }
            else
            {
                _freezeLevel = 0;
            }

            if (_freezed)
            {
                _freezed = false;
            }
                
        } 
        else 
        {
            _freezeLevel += Time.deltaTime;
        }

        frostEffect.FrostAmount = (_freezeLevel / FreezeTime);

    }

    /* void Freeze()
    {
        FreezeEvent.Raise();
    }*/

    public void OnWarmAreaEnter()
    {
        _inWarmArea = true;
    }

	public void OnWarmAreaExit()
    {
        _inWarmArea = false;
    }







}
