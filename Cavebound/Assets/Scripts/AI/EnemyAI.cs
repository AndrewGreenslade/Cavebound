using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
 
public class EnemyAI : NetworkBehaviour
{
    public Transform target;

    public List<Node> path = new List<Node>();

    [SyncVar]
    public float health = 100;

    public float StopDistance = 2;
    public float sightRange = 16;
    public float speed = 1;
    public float jumpForce = 5;
    public float bulletSpeed = 5;

    public bool lineOfSight;

    public LayerMask mask;
    public Animator animator;

    public GameObject laser;
    public Transform ShootPos;

    private float shootTimer;
    public float timeToShoot;

    public GameObject damageNumber;
    public GameObject damageNumberSpawnPoint;

    void Start()
    {
        InvokeRepeating("findNearestPlayer", 1, 20);
    }

    private void FindMySpawn()
    {

        transform.position = MapGenerator.instance.FindSafeSpawn();
    }

    private void findNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 0)
        {
            target = players[0].transform;

            foreach (GameObject player in players)
            {
                if (Vector3.Distance(player.transform.position, transform.position) < Vector3.Distance(target.position, transform.position))
                {
                    target = player.transform;
                }
            }
        }
    }

    private void Update()
    {
        if(target == null) 
        {
            if (NetworkManager.ClientManager.Clients.Count > 0)
            {
                findNearestPlayer();
                Invoke("FindMySpawn", 2.0f);
            }

            return;
        }

        if (!IsServer)
        {
            return;
        }

        if (!lineOfSight)
        {
            path.Clear();
            LineOfSightCheck();
        }
        else
        {
            updateLaserFirPoint();

            shootTimer += Time.fixedDeltaTime;

            if(shootTimer > timeToShoot)
            {
                Shoot();
            }

            FindPath();
            Move();
        }

        if (health <= 0)
        {
            ServerManager.Despawn(gameObject);
        }

        animator.SetBool("IsFollowing", lineOfSight);
    }

    void LineOfSightCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (target.position - transform.position).normalized, lineOfSight? sightRange * 2 : sightRange, mask);

        if (hit.collider != null && hit.collider.tag == "Player")
        {
            lineOfSight = true;
        } else 
        lineOfSight = false;

        if (lineOfSight)
            Invoke("LineOfSightCheck", 5.0f);
    }

    [Server]
    void Shoot()
    {
        GameObject firedLaser = Instantiate( laser, ShootPos.position, ShootPos.rotation);
        firedLaser.GetComponent<Rigidbody2D>().velocity = firedLaser.transform.right *  bulletSpeed;
        Destroy(firedLaser, 10.0f);
        ServerManager.Spawn(firedLaser);
        shootTimer = 0;
    }

    [Server]
    public void damageAI(float t_damage)
    {
        health -= t_damage;

        GameObject damageText = Instantiate(damageNumber, damageNumberSpawnPoint.transform.position, Quaternion.identity);
        damageText.GetComponent<TextMeshPro>().text = "-" + t_damage.ToString();
        ServerManager.Spawn(damageText);
    }


    [Server]
    private void updateLaserFirPoint()
    {
        Vector3 FinalPos = target.transform.position - ShootPos.position;
        float angle = Mathf.Atan2(FinalPos.y, FinalPos.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);

        ShootPos.rotation = Quaternion.Slerp(ShootPos.rotation, rot, 2.0f * Time.deltaTime);
    }

    void Move()
    {
        Vector3 newPos = new Vector3(path[0].localX + 0.5f, path[0].localY + 0.5f, 0) - transform.position;
        //if (newPos.y > 0.5f)
        //{
        //    GetComponent<Rigidbody2D>().AddForce(transform.up * newPos.y * jumpForce, ForceMode2D.Impulse);
        //}
        //newPos.y = 0;

        if (Vector3.Distance(target.transform.position, transform.position) >= StopDistance)
        {
            GetComponent<Rigidbody2D>().MovePosition(transform.position + newPos.normalized * Time.fixedDeltaTime * speed);
        } 
    }

    void FindPath(int distance = 0)
    {
        Node startNode = AiNav.instance.nodes[(int)transform.position.x, (int)transform.position.y];
        Node endNode = AiNav.instance.nodes[(int)target.position.x, (int)target.position.y];

        if (distance != 0 && GetDistance(startNode, endNode) >= distance)
        {
            return;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                RetracePath(startNode, endNode);
                return;
            }

            foreach (Node neighbour in currentNode.GetNeighbours())
            {
                if (!neighbour.greenNode || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, endNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }

        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> _path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            _path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        _path.Reverse();
        path = _path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.localX - nodeB.localX);
        int dstY = Mathf.Abs(nodeA.localY - nodeB.localY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Debug.DrawRay(transform.position, (target.position - transform.position).normalized * (lineOfSight ? sightRange * 2 : sightRange), Color.red);
            for (int i = 0; i < path.Count; i++)
            {
                if (i <= path.Count - 2)
                    Debug.DrawLine(path[i].transform.position, path[i + 1].transform.position);
            }
        }
    }
}
