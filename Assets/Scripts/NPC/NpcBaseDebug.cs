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
    private AudioSource deathSound;
    private Animator anim;
    private List<PyrrhicPlayer> players;
    private float playerRefreshSeconds = 10f;
    private float elapsedSeconds = 0f;

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

    public List<PyrrhicPlayer> GetPlayersInRange()
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
    }
}
