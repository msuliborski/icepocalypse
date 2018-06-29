using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject _player;
    PlayerControllerExperimental _playerControllerScript;
    ParticleSystem[] _childrenParticles;
    void Start() 
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _childrenParticles = gameObject.GetComponentsInChildren<ParticleSystem>();
        _playerControllerScript = _player.GetComponent<PlayerControllerExperimental>();
        transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_player != null)
        {
            if (_playerControllerScript.InEndOfTube)
            {
                Vector3 dest = new Vector3(_player.transform.position.x, _player.transform.position.y - 2, transform.position.z);
                if (transform.position != dest)
                {
                    float step = 2f * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, dest, step);
                }
            }
            else transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y, transform.position.z);
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

    private void moveDownInTube()
    {

    }
};
