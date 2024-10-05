using UnityEngine;

public class IndicatorPositioner : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    private float playerScale;

    void Start()
    {
        playerScale = player.localScale.x;
    }

    void Update()
    {
        // Cast ray from camera to where mouse is pointing
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit))
            return;
        Vector3 hitPoint = hit.point;
        if (!hit.collider.gameObject.CompareTag("Player"))
            return;
        gameObject.transform.position = hitPoint;
        // We moved the indicator, update rotation to face player
        gameObject.transform.LookAt(player);
    }
}
