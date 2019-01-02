using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAgentComponent {

  public bool GoalReached;


  public float speed;
  public float avgSpeed;
  public float rotationSpeed;
  public float avgRotationSpeed;

  private FishAgentComponent[] gos; //list of all the fish

  private Vector3 goalPosition;

  private float tankSize;
  private float tankOffSet;
  private Vector3 tankOrigin;

  float distanceToGoal;
  public bool requestGoalUpdate;

  [HideInInspector]
  public Vector3 Velocity;
  public Vector3 Cohesion;
  public Vector3 Separation;
  public Vector3 Alignment;
  private Vector3 Zero;
  [HideInInspector]
  public int UniqueID;

  [HideInInspector]
  public TransformComponent abstrat_transform;
  public Vector3 lastPosition;
  float vectorLenght;

  public bool useCohesion;

  int Choesion_frame_break;
  int elapesd_frame;
  public FishAgentComponent(int id, float tanksize, float tankoffset, Vector3 tankorigin) {
    abstrat_transform = new TransformComponent();
    UniqueID = id;
    speed = Random.Range(3, 9f) * 2;
    avgSpeed = speed;
    rotationSpeed = Random.Range(2, 6f);
    avgRotationSpeed = rotationSpeed;
    tankSize = tanksize;
    tankOffSet = tankoffset;
    tankOrigin = tankorigin;
    Velocity = Random.insideUnitSphere;
    Zero = Vector3.zero;
    useCohesion = true;
    Choesion_frame_break = Random.Range(200, 800);
    elapesd_frame = Choesion_frame_break;
  }

  public void SetFishList(FishAgentComponent[] Facps) {
    gos = Facps;
    lastPosition = abstrat_transform.Position;
  }

  public void GoalPosUpdated(Vector3 newgoal) {
    goalPosition = newgoal;
    requestGoalUpdate = true;
  }

  public void SyncVars() {
    if(elapesd_frame > 0 && !useCohesion)
      elapesd_frame--;
    else if(!useCohesion)
      useCohesion = true;


    lastPosition = abstrat_transform.Position;
    vectorLenght = lastPosition.x * lastPosition.x + lastPosition.y * lastPosition.y + lastPosition.z * lastPosition.z;
  }
  public void BrakeCohesion() {
    if(useCohesion) {
      Choesion_frame_break = Random.Range(400, 1000);
      elapesd_frame = Choesion_frame_break;
      useCohesion = false;
    }
  }
  Vector3 posB;
  Vector3 posA;
  float LenghtB;
  float LenghtA;
  int Neighbors = 0;
  int CloseRangeNeighbors = 0;
  float Lineardistance = 0;
  float AvgRotationspeed = 0;

  public void ApplayRule() {
    posB = lastPosition;
    distanceToGoal = Distance(posB, goalPosition);

    if(!useCohesion) {
      if(distanceToGoal < 6)
        GoalReached = true;
      Velocity = (goalPosition - posB).normalized;
      return;
    }


    if(distanceToGoal < 6 && requestGoalUpdate) {
      requestGoalUpdate = false;
      GoalReached = true;
      return;
    }

    if(Distance(posB, tankOrigin) > tankSize - tankOffSet) {
      Velocity = (tankOrigin - posB).normalized;
    }

    Neighbors = 0;
    CloseRangeNeighbors = 0;
    AvgRotationspeed = rotationSpeed;

    Cohesion = Zero;
    Separation = Zero;
    Alignment = Zero;

    LenghtB = vectorLenght;

    avgSpeed = speed;
    AvgRotationspeed = rotationSpeed;

    for(int i = 0; i < gos.Length; i++) {
      if(gos[i].UniqueID != UniqueID) {
        posA = gos[i].lastPosition;

        LenghtA = gos[i].vectorLenght;//vector lenght

        Lineardistance = (LenghtA > LenghtB ? LenghtA - LenghtB : LenghtB - LenghtA);
        if(Lineardistance < 5) {

          Cohesion += posA;
          Alignment += gos[i].Velocity;

          Neighbors++;
          if(Lineardistance < 2f) {
            Separation -= (posA - posB);
            AvgRotationspeed += gos[i].rotationSpeed;
            avgSpeed += gos[i].speed;
            CloseRangeNeighbors++;
          }
        }
      }
    }

    if(Neighbors > 0) {
      avgSpeed /= CloseRangeNeighbors + 1;
      AvgRotationspeed /= CloseRangeNeighbors + 1;

      Cohesion = (Cohesion / Neighbors) - posB + goalPosition;

      Alignment /= Neighbors;


      Velocity = (Cohesion + Separation * 4 + Alignment).normalized;//more weight on the alignement
    }
    else if(useCohesion) { // no neighbors just snap to goal pos than
      Velocity = (goalPosition - posB).normalized;
    }
  }

  public float Distance(Vector3 a, Vector3 b) {
    float x = a.x - b.x;
    float y = a.y - b.y;
    float z = a.z - b.z;
    return Mathf.Sqrt(x * x + y * y + z * z);
  }

}
