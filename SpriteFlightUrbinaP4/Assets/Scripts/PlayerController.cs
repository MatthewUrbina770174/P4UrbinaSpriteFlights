using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private float elapsedTime = 0f;
    private float score = 0f;
    public float scoreMultiplier = 25f;
    public float thrustForce = 1f;
    public float maxSpeed = 5f;
    public GameObject boosterFlame;
    public Rigidbody2D rb;
    public UIDocument uiDocument;
    private Label scoreText;
    private Button restartButton;
    public GameObject explosionEffect;
    public GameObject borderParent;
    private int highScore = 0;
    private Label highScoreText;
   
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;

        highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScoreText");
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score:" + highScore;
        highScoreText.style.display = DisplayStyle.None;

    }

    void Update()
    {

        elapsedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
        Debug.Log("Score: " + score);
        scoreText.text = "Score: " + score;

        if (score > highScore)
        {
            highScore = (int)score;
            highScoreText.text = "High Score: " + highScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }


        if (Mouse.current.leftButton.isPressed)
        {
            // Calculate mouse direction
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            Vector2 direction = (mousePos - transform.position).normalized;

            // Move player in direction of mouse
            transform.up = direction;
            rb.AddForce(direction * thrustForce);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            boosterFlame.SetActive(true);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            boosterFlame.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerPrefs.Save();
        highScoreText.style.display = DisplayStyle.Flex;

        Destroy(gameObject);
        Instantiate(explosionEffect, transform.position, transform.rotation);
        restartButton.style.display = DisplayStyle.Flex;
        borderParent.SetActive(false);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
