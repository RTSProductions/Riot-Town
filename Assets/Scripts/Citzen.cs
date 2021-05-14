using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Citzen : MonoBehaviour
{
    public bool isRiotor = false;

    public bool next = true;

    Animator animator;

    NavMeshAgent agent;

    SphereCollider trigger;

    public Transform target;

    AudioSource mob;

    Transform leader;

    public Transform camSpot;

    float walkPointRange = 100;

    public Material[] skinTones;

    public Material[] shirts;

    public Material[] pants;

    public MeshRenderer[] skinMeshs;


    bool triggerEntered = false;

    bool willThrow = false;

    bool canMakeNoise = false;

    Vector3 walkPoint;

    bool walkPointSet = false;

    public MeshRenderer body;

    public int randomOfset = 0;

    public LayerMask ground;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WillMakeNoise());

        mob = GetComponent<AudioSource>();

        trigger = this.gameObject.AddComponent<SphereCollider>();

        trigger.isTrigger = true;

        trigger.radius = 2;

        next = (Random.value < 0.5);

        randomOfset = Random.Range(0, 5);

        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();

        var skinRandom = new System.Random();
        var skinIndex = skinRandom.Next(skinTones.Length);

        Material skinColor = skinTones[skinIndex + randomOfset];



        foreach (var skin in skinMeshs)
        {
            skin.sharedMaterial = skinColor;
        }



        var pantsRandom = new System.Random();
        var pantsIndex = pantsRandom.Next(pants.Length);

        Material pantsColor = pants[pantsIndex + randomOfset];



        var shirtRandom = new System.Random();
        var shirtIndex = shirtRandom.Next(shirts.Length);

        Material shirtColor = shirts[shirtIndex + randomOfset];

        Material[] bodyMats = new Material[2];

        bodyMats[0] = shirtColor;
        bodyMats[1] = pantsColor;



        body.sharedMaterials = bodyMats;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Rioting", isRiotor);

        if (isRiotor == true)
        {
            if (target != null)
            {
                if (triggerEntered == false)
                {
                    if (willThrow == false)
                    {
                        target = null;
                    }
                }
            }
        }

        

        if (isRiotor == false)
        {
            

            float distace = Vector3.Distance(target.position, transform.position);

            agent.SetDestination(target.position);

            if (distace <= 1)
            {
                Waypoint waypoint = target.GetComponent<Waypoint>();

                if (waypoint != null)
                {
                    if (next == true)
                    {
                        target = waypoint.nextWaypoint.transform;
                    }
                    else
                    {
                        target = waypoint.previousWaypoint.transform;
                    }
                }
            }
        }
        else
        {

            trigger.radius = 6;
            if (willThrow == true)
            {
                Throwable throwable = target.GetComponent<Throwable>();


                    float distace = Vector3.Distance(target.position, transform.position);
                    if (distace <= 2)
                    {
                        throwable.Throw(this.transform);

                        willThrow = false;

                        StartCoroutine(Throw());

                        if (triggerEntered == false)
                        {
                            target = null;
                        }
                        else
                        {
                            target = leader;
                        }
                        
                    }
                

            }

            if (target != null)
            {
                agent.SetDestination(target.position);
            }
            if (target == null)
            {
                WalkRandom();
            }
            
        }

        
    }
    IEnumerator Throw()
    {
        animator.SetBool("Throwing", true);

        yield return new WaitForSeconds(0.457f);

        animator.SetBool("Throwing", false);
    }

    void WalkRandom()
    {


        if (!walkPointSet)
        {
            SeachWalkPoint();
        }
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }
        float distace = Vector3.Distance(walkPoint, transform.position);
        if (distace <= 1)
        {
            walkPointSet = false;
        }
    }

    void SeachWalkPoint()
    {
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, ground))
        {
            walkPointSet = true;
            StartCoroutine(WalkNewPlace());
        }

        
    }
    IEnumerator WalkNewPlace()
    {
        //walkPointSet = true;

        yield return new WaitForSeconds(5f);

        walkPointSet = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isRiotor == false)
        {
            Citzen citzen = other.GetComponent<Citzen>();

            if (citzen != null)
            {
                if (citzen.isRiotor == true && isRiotor == false)
                {

                    isRiotor = (Random.value < 0.5);

                    if (isRiotor == true)
                    {
                        triggerEntered = true;
                        target = citzen.transform;
                        leader = citzen.transform;
                        if (canMakeNoise == true)
                        {
                            mob.Play();
                        }
                    }

                }
            }
            
        }
        else
        {
            Throwable throwable = other.GetComponent<Throwable>();
            if (throwable != null)
            {
                target = throwable.transform;
                willThrow = true;
            }
        }


    }
    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Focus();
        }
    }
    void Focus()
    {
        Camera cam = FindObjectOfType<Camera>();

        FocusOnObject focusOnObject = cam.GetComponent<FocusOnObject>();

        focusOnObject.Focus(camSpot, this.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        if (isRiotor == true)
        {
            Throwable throwable = other.GetComponent<Throwable>();
            if (throwable != null)
            {
                target = null;
                willThrow = false;

                target = leader;
            }
        }
    }
    IEnumerator WillMakeNoise()
    {
        canMakeNoise = false;

        yield return new WaitForSeconds(20);

        canMakeNoise = true;
    }
    
}
