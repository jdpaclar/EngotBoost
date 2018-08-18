using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class Rocket : MonoBehaviour
{
    Rigidbody _rigidBody;
    AudioSource _audioSource;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 50;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem particleThrust;
    [SerializeField] ParticleSystem particleSuccess;
    [SerializeField] ParticleSystem particleDeath;

    enum State
    {
        Alive,
        Dead,
        Trancending
    };

    State _state = State.Alive;
    bool _collisionsDisabled = false;

    // Use this for initialization
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild)
            RespondToDebugKeys();
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
            LoadNextLevel();
        else if (Input.GetKeyDown(KeyCode.C))
            _collisionsDisabled = !_collisionsDisabled;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_state != State.Alive || _collisionsDisabled)
            return;

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        _state = State.Dead;
        _audioSource.Stop();
        _audioSource.PlayOneShot(death);
        particleDeath.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void StartSuccessSequence()
    {
        _state = State.Trancending;
        _audioSource.Stop();
        _audioSource.PlayOneShot(success);
        particleSuccess.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }

        SceneManager.LoadScene(nextSceneIndex);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            StopApplyThrust();
        }
    }

    private void StopApplyThrust()
    {
        _audioSource.Stop();
        particleThrust.Stop();
    }

    private void ApplyThrust()
    {
        _rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);

        if (!_audioSource.isPlaying)
            _audioSource.PlayOneShot(mainEngine);

        particleThrust.Play();
    }

    private void RespondToRotateInput()
    {
        _rigidBody.angularVelocity = Vector3.zero; // remove rotation due to physics

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
    }
}
