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
    private SpringJoint joint;

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

        if(Input.GetKeyUp(grappleKey)) StopGrapple();

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

        //pm.freeze = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, grappleableSurface))
        {
            grappleContactPoint = hit.point;
            
            //Swinging implementation
            joint = pm.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grappleContactPoint;
            
            //////////Copypasta

            float distanceFromPoint = Vector3.Distance(pm.transform.position, grappleContactPoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = 0;//distanceFromPoint * 0.25f;

            //Adjust these values to fit your game.
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
            //////////////Copypaste END

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

        //Invoke(nameof(StopGrapple), 1f);

    }

    public void StopGrapple()
    {
        pm.freeze = false;
        
        grappling = false;

        grappleCooldownTimer = grappleCooldown;

        lr.enabled = false;
        Destroy(joint);
    }


}
