using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


public class TimeToFight : MonoBehaviour
{
    public enum State
    {
        Chase,
        Attack,
        Block,
        Idle,
        Wander
    }

    public State state = State.Idle;
    public Vector3 chosenDest;
    public GameObject invis;
    private GameObject checkedheight;
    RaycastHit hit;

    Ray downRay;
    Ray upRay;
    public TimeToFight instance;

    private void Awake()
    {
        instance = this;
    }
    public void ChoseChasingWhatStateToAttackPlayer(NavMeshAgent agent, GameObject playerPos, GameObject Gself, Animator anim, bool MultiAttack) 
    {
        switch (state)
        {
            case State.Chase:
                ChasePlayer(playerPos, Gself, anim, agent);
                break;
            case State.Attack:
                AttackPlayer(playerPos, anim, agent, Gself, MultiAttack);
                break;
            case State.Block:
                BlockPlayer(playerPos, anim, agent);
                break;
            case State.Idle:
                IdlePlayer(playerPos, anim, agent, Gself); 
                break;
            case State.Wander:
                WanderPlayer(playerPos, anim, agent, Gself);
                break;
        }
    }
    private float distDif;
    private int random; // random Attacks
    public void AttackPlayer(GameObject playerPos, Animator anim, NavMeshAgent agent, GameObject self, bool MultiAttack)
    {
        distDif = Vector3.Distance(playerPos.transform.position, self.transform.position);
        
        if (MultiAttack)
        {
            random = Random.Range(0, 3);
            switch(random)
            {
                case 1:
                    
                    anim.SetBool("Attack2", false);
                    anim.SetBool("Attack3", false);

                    anim.SetBool("Attack", true);
                    break;
                case 2:
                    if (HasParameter("Attack2", anim))
                    {
                        anim.SetBool("Attack", false);
                        anim.SetBool("Attack3", false);

                        anim.SetBool("Attack2", true);
                    }
                    else
                    {
                        anim.SetBool("Attack", true);
                    }
                    break;
                case 3:
                    if (HasParameter("Attack3", anim))
                    {
                        anim.SetBool("Attack", false);
                        anim.SetBool("Attack2", false);

                        anim.SetBool("Attack3", true);
                    }
                    else
                    {
                        anim.SetBool("Attack", true);
                    }
                    break;
                default:
                    anim.SetBool("Attack", false);
                    anim.SetBool("Attack2", false);
                    anim.SetBool("Attack3", false);

                    anim.SetBool("Attack", true);
                    break;
            }
                 
        }
        else
        {
            anim.SetBool("Attack", true);            
            anim.SetBool("Attack2", false);
            anim.SetBool("Attack3", false);
        }

        if (distDif >= 3.5f)
        {
            anim.SetBool("Attack", false);            
            anim.SetBool("Attack2", false);
            anim.SetBool("Attack3", false);
            state = State.Chase;
        }        
    }

    public static bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }


    public void ChasePlayer(GameObject playerPos, GameObject self, Animator anim, NavMeshAgent agent)
    {
        agent.SetDestination(playerPos.transform.position);

        distDif = Vector3.Distance(playerPos.transform.position, self.transform.position);
        if (distDif <= 3.5f)
        {
            state = State.Attack;
        }        
    }

    public void BlockPlayer(GameObject playerPos, Animator anim, NavMeshAgent agent)
    {

    }

    
    private Coroutine coroutine;
    private Coroutine wand;
    public void IdlePlayer(GameObject playerPos, Animator anim, NavMeshAgent agent, GameObject self) // add a way to go between idle/chase
    {
        
        if (coroutine == null)
        {            
            //instance.StartCoroutine(LookAroundArea(self));
            coroutine = StartCoroutine(LookAroundArea(self));
        }
        
        if (wand == null)
        {
            //instance.StartCoroutine(RandomlyWander());         
            wand = StartCoroutine(RandomlyWander());
        }
    }

    IEnumerator RandomlyWander()
    {
        yield return new WaitForSeconds(1.0f);
        if (10 >= Random.Range(1, 100))
        {
            state = State.Wander;
        }
        wand = null;
    }

    IEnumerator LookAroundArea(GameObject self)
    {
        if (1 >= Random.Range(1, 1000))
        {
            // Cache the start, left, and right extremes of our rotation.
            Quaternion start = self.transform.rotation;
            Quaternion left = start * Quaternion.Euler(0, -45, 0);
            Quaternion right = start * Quaternion.Euler(0, 45, 0);

            // Yield control to the Rotate coroutine to execute
            // each turn in sequence, and resume here after each
            // invocation of Rotate finishes its work.

            yield return Rotate(self.transform, start, left, 1.0f);
            yield return Rotate(self.transform, left, right, 2.0f);
            yield return Rotate(self.transform, right, start, 1.0f);
        }
        coroutine = null;
    }

    NavMeshHit hite;
    private bool coru = false;
    Coroutine cro = null;
    public void WanderPlayer(GameObject playerPos, Animator anim, NavMeshAgent agent, GameObject self)
    {
        agent.isStopped = false;
        if (coru == false)
        {
            coru = true;
            agent.SetDestination(GetDest(self));
        }        
        anim.SetBool("walk", true);
               
        if (Vector3.Distance(agent.transform.position, agent.destination) <= 0.5f || Vector3.Distance(agent.transform.position, agent.destination) <= -0.5f)
        {
            state = State.Idle;
            if (cro == null)
                cro = StartCoroutine(AnimDoneYet(anim));

            anim.SetBool("walk", false);
            agent.ResetPath();
        }      
    }
    
    IEnumerator AnimDoneYet(Animator anim)
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
       
    }        
     
    private Vector3 GetDest(GameObject self)
    {
        chosenDest = new Vector3(self.transform.position.x + Random.Range(-30, 30), self.transform.position.y, self.transform.position.z + Random.Range(-30, 30));
        checkedheight = Instantiate(invis, chosenDest, Quaternion.identity);        
               
        downRay = new Ray(checkedheight.transform.position, -transform.up);
        upRay = new Ray(checkedheight.transform.position, transform.up);
        
        if (Physics.Raycast(upRay, out hit) || Physics.Raycast(downRay, out hit))
        {
            if (hit.collider != null && hit.collider == CompareTag("Terrain")) 
            {
                chosenDest.y = hit.collider.transform.position.y;
            }
        }
        
        return chosenDest;
    }       

    IEnumerator Rotate(Transform self, Quaternion from, Quaternion to, float duration)
    {

        for (float t = 0; t < 1f; t += Time.deltaTime / duration)
        {
            // Rotate to match our current progress between from and to.
            self.rotation = Quaternion.Slerp(from, to, t);
            // Wait one frame before looping again.
            yield return null;
        }

        // Ensure we finish exactly at the destination orientation.
        self.rotation = to;
    }

}
