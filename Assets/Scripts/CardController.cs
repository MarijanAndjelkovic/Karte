using UnityEngine;
using Photon.Pun;

public class CardController : MonoBehaviourPun
{
    private Camera mainCamera;
    public float surfaceOffset = 0.05f;
    public float dragPopOutDistance = 0.1f;

    private Collider orangeZoneCollider;
    public MeshRenderer faceRenderer;
    private HandManager handManager;
    public bool isDropped = false;

    public static float pileHeightOffset = 0f;

    public bool isHovering = false;
    public bool isDragging = false;

    private Vector3 startPosition;

    public string bojaKarte;
    public string znakKarte;

    private float hoverCooldown = 0f;

    void Start()
    {
        mainCamera = Camera.main;
        handManager = FindObjectOfType<HandManager>();

        GameObject orangeZone = GameObject.Find("OrangeZone");
        if (orangeZone != null) orangeZoneCollider = orangeZone.GetComponent<Collider>();
    }

    void Update()
    {
        if (isDropped || isDragging) return;

        if (Time.time < hoverCooldown)
        {
            if (isHovering) { isHovering = false; handManager?.ReorganizeHand(); }
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        CardController topCC = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            CardController cc = hit.collider.GetComponent<CardController>();
            if (cc != null && !cc.isDropped)
            {
                float dist = hit.distance;

                if (cc.isHovering)
                {
                    dist -= 0.005f;
                }

                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    topCC = cc;
                }
            }
        }

        bool mouseIsOverMe = (topCC == this);

        if (mouseIsOverMe && !isHovering)
        {
            isHovering = true;
            handManager?.ReorganizeHand();
        }
        else if (!mouseIsOverMe && isHovering)
        {
            isHovering = false;
            handManager?.ReorganizeHand();
        }
    }

    void OnMouseDown()
    {
        if (isDropped) return;

        isDragging = true;
        isHovering = false;
        startPosition = transform.position;
        if (handManager != null) handManager.ReorganizeHand();
    }

    void OnMouseDrag()
    {
        if (isDropped || !isDragging) return;

        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x < 0 || mousePos.y < 0 || mousePos.x > Screen.width || mousePos.y > Screen.height) return;

        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        bool hitRedZone = false;
        bool hitTableZone = false;
        bool hitGreenZone = false;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.name == "RedZone") hitRedZone = true;
            else if (hit.collider.name == "OrangeZone") hitTableZone = true;
            else if (hit.collider.name == "GreenZone") hitGreenZone = true;
        }

        if (hitGreenZone && !hitTableZone && !hitRedZone)
        {
            Plane handPlane = new Plane(-mainCamera.transform.forward, startPosition - mainCamera.transform.forward * dragPopOutDistance);
            if (handPlane.Raycast(ray, out float handDistance))
            {
                transform.position = ray.GetPoint(handDistance);
            }
            transform.rotation = mainCamera.transform.rotation;
            return;
        }

        float currentPlaneHeight = surfaceOffset;
        if (hitRedZone) currentPlaneHeight = surfaceOffset + pileHeightOffset + 0.1f;

        if (hitRedZone || hitTableZone)
        {
            Plane tablePlane = new Plane(Vector3.up, new Vector3(0, currentPlaneHeight, 0));
            if (tablePlane.Raycast(ray, out float distance))
            {
                Vector3 idealPosition = ray.GetPoint(distance);
                if (orangeZoneCollider != null)
                {
                    Bounds bounds = orangeZoneCollider.bounds;
                    Vector3 center = bounds.center;
                    float radius = Mathf.Min(bounds.extents.x, bounds.extents.z);

                    Vector2 offset = new Vector2(idealPosition.x - center.x, idealPosition.z - center.z);
                    if (offset.magnitude > radius)
                    {
                        offset = offset.normalized * radius;
                        idealPosition.x = center.x + offset.x;
                        idealPosition.z = center.z + offset.y;
                    }
                }
                transform.position = idealPosition;
            }
        }
        else if (orangeZoneCollider != null)
        {
            Bounds bounds = orangeZoneCollider.bounds;
            Vector3 center = bounds.center;
            float radius = Mathf.Min(bounds.extents.x, bounds.extents.z);

            transform.position = NajblizaTackaNaIviciStola(mousePos, center, radius, currentPlaneHeight);
        }

        transform.rotation = Quaternion.Euler(90, mainCamera.transform.eulerAngles.y, 0);
    }

    private Vector3 NajblizaTackaNaIviciStola(Vector2 mouseScreenPos, Vector3 tableCenter, float radius, float height)
    {
        float bestAngle = 0f;
        float minSqrDist = Mathf.Infinity;

        for (int i = 0; i < 72; i++)
        {
            float angle = i * 5f * Mathf.Deg2Rad;
            Vector3 worldPoint = new Vector3(
                tableCenter.x + Mathf.Cos(angle) * radius,
                height,
                tableCenter.z + Mathf.Sin(angle) * radius
            );

            Vector3 screenPoint = mainCamera.WorldToScreenPoint(worldPoint);
            if (screenPoint.z < 0) continue;

            float sqrDist = ((Vector2)screenPoint - mouseScreenPos).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                bestAngle = angle;
            }
        }

        float fineStep = 0.5f * Mathf.Deg2Rad;
        for (int i = -10; i <= 10; i++)
        {
            float angle = bestAngle + i * fineStep;
            Vector3 worldPoint = new Vector3(
                tableCenter.x + Mathf.Cos(angle) * radius,
                height,
                tableCenter.z + Mathf.Sin(angle) * radius
            );

            Vector3 screenPoint = mainCamera.WorldToScreenPoint(worldPoint);
            if (screenPoint.z < 0) continue;

            float sqrDist = ((Vector2)screenPoint - mouseScreenPos).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                bestAngle = angle;
            }
        }

        return new Vector3(
            tableCenter.x + Mathf.Cos(bestAngle) * radius,
            height,
            tableCenter.z + Mathf.Sin(bestAngle) * radius
        );
    }

    void OnMouseUp()
    {
        if (isDropped || !isDragging) return;

        isDragging = false;
        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x < 0 || mousePos.y < 0 || mousePos.x > Screen.width || mousePos.y > Screen.height)
        {
            hoverCooldown = Time.time + 0.25f;
            if (handManager != null) handManager.ReorganizeHand();
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        bool droppedOnRed = false;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.name == "RedZone") { droppedOnRed = true; break; }
        }

        if (droppedOnRed)
        {
            Debug.Log("Karta uspešno bačena: " + bojaKarte + " " + znakKarte);

            Plane tablePlane = new Plane(Vector3.up, new Vector3(0, surfaceOffset, 0));
            if (tablePlane.Raycast(ray, out float distance))
            {
                Vector3 dropPosition = ray.GetPoint(distance);
                if (orangeZoneCollider != null)
                {
                    Bounds bounds = orangeZoneCollider.bounds;
                    dropPosition.x = Mathf.Clamp(dropPosition.x, bounds.min.x, bounds.max.x);
                    dropPosition.z = Mathf.Clamp(dropPosition.z, bounds.min.x, bounds.max.x);
                }

                pileHeightOffset += 0.001f;
                dropPosition.y += pileHeightOffset;

                transform.position = dropPosition;
            }

            transform.rotation = Quaternion.Euler(90, mainCamera.transform.eulerAngles.y, 0);

            isDropped = true;
            transform.SetParent(null);
            if (handManager != null) handManager.RemoveCard(this.gameObject);
        }
        else
        {
            hoverCooldown = Time.time + 0.25f;
            if (handManager != null) handManager.ReorganizeHand();
        }
    }
}