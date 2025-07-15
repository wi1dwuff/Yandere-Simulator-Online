using UnityEngine;

public class Zoom : MonoBehaviour
{
    public RPG_Camera camera;
    public float targetZoom;
    public float zoom;

    // Update is called once per frame
    private void Update()
    {
        float input = Input.GetAxis("Mouse ScrollWheel");
        targetZoom += input;
        targetZoom = Mathf.Clamp(targetZoom, 0f, 0.4f);

        zoom = Mathf.MoveTowards(zoom, targetZoom, Time.deltaTime);

        float calculatedDistance = 2f - (zoom * 3.33333f);
        camera.distance = calculatedDistance;
        camera.distanceMax = calculatedDistance;
        camera.distanceMin = calculatedDistance;

        Vector3 targetPosition = transform.localPosition;
        targetPosition.y = Mathf.Lerp(targetPosition.y, 1f + zoom, Time.deltaTime * 8f);
        transform.localPosition = targetPosition;
    }
}
