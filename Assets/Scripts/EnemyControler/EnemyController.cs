using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float DmgPerFrame = 0.1f;
    public bool Staggered = false;

    [SerializeField]
    private float Health = 12.5f;

    [SerializeField]
    private float StaggerCooldown = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WasHit(float momentum)
    {
        Health -= momentum;

        if (Health <= 0.0f)
        {
            Despawn();
            return;
        }

        StartCoroutine(Stagger());
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }

    private IEnumerator Stagger()
    {
        Staggered = true;

        yield return new WaitForSeconds(StaggerCooldown);

        Staggered = false;
    }
}
