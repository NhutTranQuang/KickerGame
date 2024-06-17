using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour, InterfaceCollision
{
    [SerializeField] private float levelLoadDelay = 1f;

    [SerializeField] private AudioClip crash;
    [SerializeField] private AudioClip success;
    private AudioSource audioSource;

    [SerializeField] private ParticleSystem successParticles;
    [SerializeField] private ParticleSystem crashParticles;

    [SerializeField] private GameObject acidGameObject;

    private Rigidbody rb;

    private bool isTransitioning = false;
    private bool collisionDisable = false;

    public int lastScene;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        RespondToDebugKeys();
        LockXYRotation();
    }

    private void LockXYRotation()
    {
        //Lock X, Y rotation
        transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z);
    }

    private void RespondToDebugKeys()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisable = !collisionDisable; //toggle collision
            Debug.Log("Collision Disable: " + collisionDisable);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || collisionDisable) return;

        switch (collision.gameObject.tag)
        {
            case "Finish":
                StartSuccessSequence();
                break;
            case "Acid":
                StartAcidSequence();
                break;
            case "Friendly":
                break;
          
            default:
                StartCrashSequence();
                break;
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ChangePosition"))
        {
            Vector3 currentPos = transform.localScale;
            currentPos.x *= -1;
            transform.localScale = currentPos;
            Debug.Log("isCollision");
            Destroy(other);
        }
    }

    public void StartSuccessSequence()
    {
        // Play & stop the previous audio
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(success);

        // Trigger particle
        successParticles.Play();

        // Disable movement and load next level
        GetComponent<Movement>().enabled = false;
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    public void StartAcidSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        acidGameObject.GetComponent<BoxCollider>().enabled = false;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezePositionX;
        GetComponent<Movement>().enabled = false;
        Invoke("ReloadLevel", levelLoadDelay);
    }

    // Delay Load Scene
    public void StartCrashSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(crash);
        crashParticles.Play();  
        GetComponent<Movement>().enabled = false;
        Invoke("ReloadLevel", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        int currentScneneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentScneneIndex + 1;

        if(nextScene == SceneManager.sceneCountInBuildSettings)
        {
            nextScene = 0;
        }

        SceneManager.LoadScene(nextScene);
    }

    private void ReloadLevel()
    {
        int currentScneneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScneneIndex);
    }
}
