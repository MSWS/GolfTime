using UnityEngine;

public class IndicatorPositioner : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    private bool isVisible = false;

    void Update()
    {
        if (Input.GetMouseButton(0))
            return;

        // Cast ray from camera to where mouse is pointing
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit))
        {
            notOnPlayer();
            return;
        }

        Vector3 hitPoint = hit.point;
        if (!hit.collider.gameObject.CompareTag("Player"))
        {
            notOnPlayer();
            return;
        }

        onPlayer();
        gameObject.transform.position = hitPoint;
        // We moved the indicator, update rotation to face player
        gameObject.transform.LookAt(player);
    }

    private void notOnPlayer()
    {
        if (!isVisible)
            return;
        // Hide all child renderers
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;
        isVisible = false;
    }

    private void onPlayer()
    {
        if (isVisible)
            return;
        // Show all child renderers
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = true;
        isVisible = true;
    }
}
