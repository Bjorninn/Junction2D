using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{

    public Transform player;
    public float distanceZ;
    public float moveX;
    public float moveY;

    private Transform trans;

    void Awake()
    {
        trans = GetComponent<Transform>();
    }

     void Update()
     {

         trans.localPosition = new Vector3(player.position.x + moveX, player.position.y + moveY, player.position.z - distanceZ);

     }

}
