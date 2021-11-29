using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class Enemy
{
    public float health;
    public float speed;
    public NavMeshAgent agent;
    public float inRange;
    public float inView;
    

    public Enemy(float health, float speed, NavMeshAgent agent, float inRange, float inView)
    {
        this.health = health;
        this.speed = speed;
        this.agent = agent;
        this.inRange = inRange;
        this.inView = inView;
        
    }
}

public class EnemyBaseScript : MonoBehaviour
{
    public float health;
    public float speed;
    public NavMeshAgent agent;
    public float inRange;
    public float inView;    
    public Enemy enemy;
    public GameObject self;
    public bool MultiAttack = false;

    public GameObject playerPos;

    private float SelectedDifficulty;
        
    private Animator anim;

    private bool lookOnce = true;

    private PlayerPos pl;
    private Weapon weaponGettingHitBy;
    private TakeDamage tk;
    private TimeToFight TtF;
    private Difficulty dif;

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        TtF = gameObject.GetComponent<TimeToFight>();

        if (SceneManager.sceneCount == 2)
        {
            SelectedDifficulty = GameObject.FindGameObjectWithTag("ButtonController").GetComponent<Difficulty>().difficulty;
            SceneManager.UnloadSceneAsync("DifficultyChoosing");
        }
        else
        {
            SelectedDifficulty = 1;
        }
        

        health = health * SelectedDifficulty;
        speed = speed * SelectedDifficulty;
        inRange = inRange * SelectedDifficulty;
        inView = inView * SelectedDifficulty;
        

        enemy = new Enemy(health, speed, agent, inRange, inView);
        agent = gameObject.GetComponent<NavMeshAgent>();
        

        
    }

    public void Update()
    {
        Invoke("LookForPlayer", 5.0f);
        if (lookOnce)
        {
            LookForPlayer();
            lookOnce = false;
        }

        if (health < 0)
        {
            health = 0;
            StartCoroutine(JustDies());
        }
                
        TtF.ChoseChasingWhatStateToAttackPlayer(agent, playerPos, self, anim, MultiAttack);         
       
    }    
        
    IEnumerator JustDies()
    {
        anim.SetBool("isDead", true);
        //GetComponent<Animator>().SetBool("isDead", true);

        yield return new WaitForSeconds(4.5f);
        Destroy(this.gameObject);
    }

    public GameObject LookForPlayer()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player");
        return playerPos;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Weapon")
        {
            weaponGettingHitBy = new Weapon(col.gameObject, col.gameObject.GetComponent<WeaponBaseScript>().damage);
            health = TakeDamage.TakeDmg(health, weaponGettingHitBy);
        }
    }
}

