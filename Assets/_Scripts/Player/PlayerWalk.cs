﻿using UnityEngine;
using System.Collections;

public class PlayerWalk : MonoBehaviour {
    private NavMeshAgent agent;
    public Vector3 targetPos;
    public GameObject[] feedBackElements;
    public GameObject TPArcher;

    private PlayerController PC;
    private Animation archerAni;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        PC = GetComponent<PlayerController>();
        archerAni = TPArcher.GetComponent<Animation>();
    }

    private float coolDownAnimation = 1f;
    private float coolWalkAnimation = 1f;
	void Update() {
        coolWalkAnimation += Time.deltaTime;
        if(PC.isDead())
            return;
        if(isWalking()) {
            Debug.Log("Walking");
            archerAni.clip = archerAni.GetClip("Walk");
            if(!archerAni.isPlaying)
                archerAni.Play();
        } else {
            archerAni.clip = archerAni.GetClip("Death");
            archerAni.Stop();
        }
        if(Input.GetMouseButtonUp(0)) {
            coolWalkAnimation = coolDownAnimation;
        }
        if(Input.GetMouseButton(0) && PC.currentCameraMode == PlayerController.CameraMode.Third) {
            if(CheckClickedLayer() == 8) {
                SetTargetPosition(CheckClickedLayer());
                agent.SetDestination(targetPos);
                WalkPointAnim(targetPos, (coolWalkAnimation >= coolDownAnimation));
            }
        }
	}

    private void SetTargetPosition(int layer) {
        if(layer == 8) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit)) //if the raycast hit somthing
            {
                if(hit.transform.gameObject.layer == 8) //if it hit an object in the ground layer
                    targetPos = hit.point; //get the point where ray hit the object
            }
        }
    }

    private Vector3 lastPos;
    public bool isWalking() {
        float dist = Vector3.Distance(transform.position, lastPos);
        if(dist == 0f)
            return false;
        lastPos = transform.position;
        return true;
    }

    public void WalkPointAnim(Vector3 Point, bool spawnRing) {
        Point = new Vector3(Point.x, Point.y + 0.03f, Point.z);
        if(spawnRing) {
            GameObject tempElement = Instantiate(feedBackElements[0], Point, Quaternion.identity) as GameObject;
            tempElement.transform.eulerAngles = (new Vector3(90, 0, 0));
            tempElement.name = "Mouse Click Location: " + Point;
            Destroy(tempElement, 0.5f);
            coolWalkAnimation = 0f;
        }
    }

    private int CheckClickedLayer() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit) && hit.transform != null)
            return hit.transform.gameObject.layer;

        return 9;
    }
}