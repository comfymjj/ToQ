using UnityEngine;
using System.Collections;

public class DrawLocation : MonoBehaviour {

	void onDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}
