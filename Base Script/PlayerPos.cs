using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPos : MonoBehaviour
{
    

    public static bool FindPlayerPos(float inRange, GameObject playerPos, float inView, GameObject self)
    {
        float distDif = Vector3.Distance(playerPos.transform.position, self.transform.position);
        if (distDif <= inRange && PlayerIsInSight(inView, playerPos, self))
        {
            return true;
        }

        return false;
    }

    public static bool PlayerIsInSight(float inView, GameObject playerPos, GameObject self)
    {
        Vector3 targetDir = playerPos.transform.position - self.transform.position;
        Vector3 forward = self.transform.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);
        float selfHeight = self.GetComponent<Collider>().bounds.size.y;

        if (angle <= inView && angle >= -inView && playerPos.transform.position.y <= self.transform.position.y + selfHeight/2 + selfHeight && playerPos.transform.position.y >= (self.transform.position.y - selfHeight) - selfHeight / 2)
        {
            return true;
        }
        return false;
    }
}
