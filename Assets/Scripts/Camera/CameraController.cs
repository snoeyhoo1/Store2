using UnityEngine;

namespace Store1
{
    public class CameraController : MonoBehaviour
    {
        Camera cam;
        Vector3 target = new Vector3(0, 1.2f, 0.8f);
        float zoom = 8.6f;
        Vector2 lastTouch;
        bool dragging;

        void Start()
        {
            cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera");
                go.tag = "MainCamera";
                cam = go.AddComponent<Camera>();
            }
            cam.orthographic = true;
            cam.orthographicSize = zoom;
            cam.transform.position = new Vector3(10.5f, 12.5f, -13.5f);
            cam.transform.LookAt(target);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(.60f, .76f, .86f);

            var sunGo = new GameObject("Sun");
            var sun = sunGo.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.05f;
            sun.shadows = LightShadows.Soft;
            sun.shadowStrength = .65f;
            sun.shadowBias = .05f;
            sun.transform.rotation = Quaternion.Euler(48, -35, 0);

            RenderSettings.ambientLight = new Color(.72f, .74f, .78f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(.60f, .76f, .86f);
            RenderSettings.fogDensity = .008f;
        }

        void Update()
        {
            if (cam == null) return;

            // Desktop/editor zoom.
            float wheel = Input.mouseScrollDelta.y;
            if (Mathf.Abs(wheel) > .01f) SetZoom(zoom - wheel * .4f);

            // Mobile: one-finger drag pans the view, two fingers pinch-zoom.
            if (Input.touchCount == 1)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began) { lastTouch = t.position; dragging = true; }
                else if (t.phase == TouchPhase.Moved && dragging)
                {
                    Vector2 delta = t.position - lastTouch;
                    lastTouch = t.position;
                    target += new Vector3(-delta.x, 0, -delta.y) * (0.0065f * zoom);
                    target.x = Mathf.Clamp(target.x, -4.5f, 4.5f);
                    target.z = Mathf.Clamp(target.z, -4.5f, 4.5f);
                    cam.transform.position += new Vector3(-delta.x, 0, -delta.y) * (0.0065f * zoom);
                }
                else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) dragging = false;
            }
            else if (Input.touchCount >= 2)
            {
                Touch a = Input.GetTouch(0), b = Input.GetTouch(1);
                Vector2 aPrev = a.position - a.deltaPosition;
                Vector2 bPrev = b.position - b.deltaPosition;
                float oldDistance = Vector2.Distance(aPrev, bPrev);
                float newDistance = Vector2.Distance(a.position, b.position);
                if (oldDistance > .01f) SetZoom(zoom - (newDistance - oldDistance) * .008f);
            }

            cam.transform.LookAt(target);
        }

        void SetZoom(float value)
        {
            zoom = Mathf.Clamp(value, 6.4f, 10.2f);
            cam.orthographicSize = zoom;
        }
    }
}
