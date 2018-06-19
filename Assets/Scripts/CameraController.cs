using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    private GameObject _player;

    ParticleSystem[] _childrenParticles;
    void Start() 
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _childrenParticles = gameObject.GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_player != null)
        {
            gameObject.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y, gameObject.transform.position.z);
        }
    }

    public void OnWarmAreaEnter()
    {
        foreach (ParticleSystem particleSystem in _childrenParticles) 
        {
            particleSystem.enableEmission = false;
        }
    }

    public void OnWarmAreaExit()
    {
        foreach (ParticleSystem particleSystem in _childrenParticles) 
        {
            particleSystem.enableEmission = true;
        }
    }
};