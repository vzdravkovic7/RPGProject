using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroController : MonoBehaviour
{
    private SpriteRenderer sr;
    private Rigidbody2D body;
    private Animator animator;

    float moveSpeed = 0.8f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        body.velocity = new Vector3(horizontalInput * moveSpeed, verticalInput * moveSpeed);

        // Sprite Renderer logic
        if (horizontalInput > 0) { 
            sr.flipX = false;
        } else if(horizontalInput < 0)
        {
            sr.flipX = true;
        }

        // Animation logic
        if ((Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput)) != 0)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Enemy>() != null)
        {
            // Start battle
            // Disable the current game mechanics
            Time.timeScale = 0;
            FindObjectOfType<Camera>().GetComponent<AudioListener>().enabled = false;

            // Battleview
            SceneManager.LoadScene("BattleView", LoadSceneMode.Additive);
            SceneManager.sceneLoaded += OnBattleViewStart;
        }
    }

    private void OnBattleViewStart(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnBattleViewStart;
        StartCoroutine(BattleViewStart(scene));
    }

    private IEnumerator BattleViewStart(Scene scene)
    {
        while(scene.isLoaded == false)
        {
            yield return new WaitForEndOfFrame();
        }

        SceneManager.SetActiveScene(scene);

        yield return new WaitForEndOfFrame();

        GameObject blurPanel = GameObject.FindGameObjectWithTag("blurPanel");
        blurPanel.GetComponent<Krivodeling.UI.Effects.UIBlur>().BeginBlur(5f);

        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera.SetActive(false);

        GameObject battleViewCamera = GameObject.FindGameObjectWithTag("battleViewCamera");
        battleViewCamera.SetActive(true);

        battleViewCamera.transform.SetPositionAndRotation(mainCamera.transform.position, mainCamera.transform.rotation);
    }
}
