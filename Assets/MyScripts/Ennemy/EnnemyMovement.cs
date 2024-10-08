using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(AgentLinkMover))]
public class EnnemyMovement : MonoBehaviour
{
   
    [SerializeField]
    public Transform targetPosition;
    
    private float updateSPeed = 0.1f;
    [SerializeField]
    public float UpdateRate = 0.1f;

    [SerializeField]
    private Animator animator;

    private const string IsWalking = "IsWalking";
    private const string Jump = "Jump";
    private const string Landed = "Landed";

    private Coroutine FollowCoroutine;
    private NavMeshAgent agent;
    private AgentLinkMover agentLinkMover;


    private void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agentLinkMover = this.GetComponent<AgentLinkMover>();

        agentLinkMover.OnLinkStart += HandleLinkStart;
        agentLinkMover.OnLinkEnd += HandleLinkEnd;

    }


    private void Start()
    {
        if(targetPosition == null)
        {
            targetPosition.position = GameObject.FindGameObjectWithTag("Player").transform.position;
        }
    }
    private void Update()
    {
        animator.SetBool(IsWalking, agent.velocity.magnitude > 0.01f);
    }

    private void HandleLinkStart()
    {
        animator.SetTrigger(Jump);
    }
    private void HandleLinkEnd()
    {
        animator.SetTrigger(Landed);
    }

    public void StartChasing()
    {
        if (FollowCoroutine == null)
        {
            FollowCoroutine = StartCoroutine(FollowTarget());
        }
        else
        {
            Debug.LogWarning("Called StartChasing on Enemy that is already chasing! This is likely a bug in some calling class!");
        }
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);

        while (gameObject.activeSelf && agent.isActiveAndEnabled)
        {
            agent.SetDestination(targetPosition.position);
            yield return Wait;
        }
    }

    public void StopFlanking(Transform player)
    {
       targetPosition.position = player.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Flank"))
        {
            Debug.Log("change position");
            StopFlanking(other.gameObject.GetComponent<Flank>().playerPosition);
        }
        else
        {
            Debug.Log(other.ToString());
        }
    }

   
}
