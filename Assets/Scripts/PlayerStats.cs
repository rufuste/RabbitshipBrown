using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public delegate void OnHealthChangedDelegate();
    public OnHealthChangedDelegate onHealthChangedCallback;


    #region Sigleton
    private static PlayerStats instance;
    public static PlayerStats Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerStats>();
            }

            return instance;
        }
    }
    #endregion

    public bool isInvulnerable;

    [SerializeField]
    private float health;
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private float maxTotalHealth = 0;
    [SerializeField]
    private float speed = 0;
    [SerializeField]
    private float damage = 0;

    public float Health { get { return health; } }
    public float MaxHealth { get { return maxHealth; } }
    public float Damage { get { return damage; } }

    [HideInInspector]
    public float Speed { get { return speed; } }

    public Image deathScreen;
    public Text gameOverText;

    public bool m_Fading;

    private bool damaged = false;
    public Image damageImage;
    public float flashSpeed = 5f;
    public Color flashColour;
    public new AudioSource audio;
    public AudioClip injureClip;
    Shooting shooting;
    bool dead = false;
    
    public Score playerScore;
    private bool activatedAbility = false;
    ParticleSys ps;
    private Vector3 standardSize;
    private int scoreActivated = 0;
    public float abilityTime;
    public int abilityWait;

    private void Start()
    {
        deathScreen.canvasRenderer.SetAlpha(0.01f);
        gameOverText.canvasRenderer.SetAlpha(0.01f);

        playerScore = GameObject.FindGameObjectWithTag("Score").GetComponent<Score>();
        shooting = GetComponent<Shooting>();
        ps = GetComponentInChildren<ParticleSys>();
    }

    public void Update()
    {
        if (playerScore.GetScore() > scoreActivated && playerScore.GetScore() % abilityWait == 0)
        {
            scoreActivated = playerScore.GetScore();
            scoreActivated += 4;
            TripleSpeed();
        }

        if (dead)
        {
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<Shooting>().enabled = false;
            deathScreen.CrossFadeAlpha(1.0f, 3.0f, false);
            gameOverText.CrossFadeAlpha(1.0f, 2.0f, false);
        }
        // If the player damaged
        if (damaged)
        {
            // Set to flash colour
            damageImage.color = flashColour;
            if (Health <= 0)
                dead = true;

        }
        else
        {
            // Transition the colour back to clear.
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        // Reset the damaged flag.
        damaged = false;
    }

    public void Heal(float health)
    {
        this.health += health;
        ClampHealth();
    }

    public void TakeDamage(float dmg)
    {

        if (!isInvulnerable)
        {
            audio.clip = injureClip;
            audio.pitch = (Random.Range(0.8f, 1.1f));
            audio.Play();
            health -= dmg;
            ClampHealth();
            damaged = true;
        }
    }

    public void AddHealth()
    {
        if (maxHealth < maxTotalHealth)
        {
            maxHealth += 1;
            health = maxHealth;

            if (onHealthChangedCallback != null)
                onHealthChangedCallback.Invoke();
        }
    }

    void ClampHealth()
    {
        health = Mathf.Clamp(health, 0, maxHealth);

        if (onHealthChangedCallback != null)
            onHealthChangedCallback.Invoke();
    }

    void TripleSpeed()
    {
        float standard;
        standard = shooting.fireRate;
        standardSize = gameObject.transform.localScale;
        shooting.fireRate = shooting.fireRate / 3;
        gameObject.transform.localScale *= 2;
        
        ps.Activate();
        StartCoroutine(Ability(abilityTime, standard, standardSize));
    }

    

    IEnumerator Ability(float abilityTime, float standard, Vector3 standardSize)
    {
        yield return new WaitForSeconds(abilityTime);
        gameObject.transform.localScale = standardSize;
        shooting.fireRate = standard;
        ps.Deactivate();
    }


}
