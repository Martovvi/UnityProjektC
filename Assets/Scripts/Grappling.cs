using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask grappleableSurface;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grappleContactPoint;

    [Header("Cooldown")]
    public float grappleCooldown;
    private float grappleCooldownTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey)) StartGrapple();

        if (grappleCooldownTimer > 0)
            grappleCooldownTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (grappling)
            lr.SetPosition(0, gunTip.position);
    }

    private void StartGrapple()
    {
        if (grappleCooldownTimer > 0) return;

        grappling = true;

        pm.freeze = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, grappleableSurface))
        {
            grappleContactPoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grappleContactPoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
        lr.enabled = true;
        lr.SetPosition(1, grappleContactPoint);
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grappleContactPointRelativeYPos = grappleContactPoint.y - lowestPoint.y;
        float highestPointOnArc = grappleContactPointRelativeYPos + overshootYAxis;

        if (grappleContactPointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grappleContactPoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);

    }

    public void StopGrapple()
    {
        pm.freeze = false;
        
        grappling = false;

        grappleCooldownTimer = grappleCooldown;

        lr.enabled = false;
    }


}
