
using UnityEngine;

public class NPCController : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 30 * Time.deltaTime, 0);
    }
}
 