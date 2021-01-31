using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NpcBaseDebug : MonoBehaviour
{
    public float MeleeRange = 3f;
    public float Health = 150;
    [SerializeField]
    private GameObject characterAvatar;
    [SerializeField]
    private GameObject deathGibPrefab;
    [SerializeField]
    private AudioSource attackSound;
    [SerializeField]
    private AudioSource deathSound;
    private Animator anim;
    private bool attacking = false;
    private List<PyrrhicPlayer> players;
    private float playerRefreshSeconds = 10f;
    private float elapsedSeconds = 0f;
    private float attackCooldownElapsed = 0f;
    private float attackCooldownSeconds = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        FindPlayers();
    }

    public void HandleHit(float damage)
    {
        Health -= damage;
        deathSound.Play();
        if (Health <= 0)
        {
           StartCoroutine("Die");
        }
    }

    public IEnumerator Die() {
        anim.Play("Die");
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            yield return null;
        }
        Instantiate(deathGibPrefab, gameObject.transform.position, Quaternion.identity, null);
        Destroy(gameObject);

    }

    private void FindPlayers()
    {
        players = FindObjectsOfType<PyrrhicPlayer>().ToList();
    }

    private List<PyrrhicPlayer> GetPlayersInRange()
    {
        List<PyrrhicPlayer> playersInRange = new List<PyrrhicPlayer>();
        foreach (var player in players)
        {
            if (Vector3.Distance(player.gameObject.transform.position, gameObject.transform.position) <= MeleeRange)
            {
                playersInRange.Add(player);
            }
        }
        return playersInRange;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedSeconds += Time.deltaTime;
        if (elapsedSeconds >= playerRefreshSeconds)
        {
            FindPlayers();
            elapsedSeconds = 0;
        }
        var playersInRange = GetPlayersInRange();
        if (playersInRange.Count > 0 && attacking == false)
        {
            Attack();
        }
        if (attacking)
        {
            attackCooldownElapsed += Time.deltaTime;
            if (attackCooldownElapsed >= attackCooldownSeconds) 
            {
                attacking = false;
            }
        }
    }

    void Attack()
    {
        attacking = true;
        anim.Play("Kick");
        if (attackSound != null && attackSound.isPlaying == false)
        {
            attackSound.Play();
        }
    }



}
