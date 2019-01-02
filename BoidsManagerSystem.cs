using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsManagerSystem : MonoBehaviour {
  public bool showfishdebug;
  private BoidsManagerSystem bms;

  public GameObject Fish;

  public float frameToSkip = 1;//7
  private int frameSkipped;

  public float tankSize;//200
  public int fishCount;//100
  [HideInInspector]
  public FishAgentComponent[] fishAgentsCmp;

  public Vector3 goalPosition;
  public int fishOnGoal;
  [HideInInspector]
  public float tankOffset;

  private void Start() {
    bms = this;
    fishAgentsCmp = new FishAgentComponent[fishCount];
    GenerateComponent();
    RandomizeGoalPosition();
  }

  void GenerateComponent() {
    for(int i = 0; i < fishCount; i++) {
      Vector3 RandomPosition = transform.position + new Vector3(Random.Range(-tankSize, tankSize), Random.Range(-tankSize, tankSize), Random.Range(-tankSize, tankSize));

      fishAgentsCmp[i] = new FishAgentComponent(i, tankSize, tankOffset, transform.position);
      fishAgentsCmp[i].abstrat_transform.Set(Instantiate(Fish).transform);
      fishAgentsCmp[i].abstrat_transform.Position = RandomPosition;
      fishAgentsCmp[i].abstrat_transform.Parent = transform;
    }

    for(int i = 0; i < fishCount; i++)
      fishAgentsCmp[i].SetFishList(fishAgentsCmp);
  }

  void Update() {
    CheckFishesOnGoal();

    if(fishOnGoal >= fishCount / 7) {
      RandomizeGoalPosition();
      fishOnGoal = 0;
    }

    UpdateFishes();

  }
  void LateUpdate() {
    int rndevnt = Random.Range(1, 350);
    for(int i = 0; i < fishCount; i++) {
      if(rndevnt == 2) {
        fishAgentsCmp[i].BrakeCohesion();
        fishAgentsCmp[i].GoalPosUpdated(GetRandomPosition());
      }

      fishAgentsCmp[i].SyncVars();
    }
  }
  void UpdateFishes() {
    if(frameSkipped >= frameToSkip) {
      for(int i = 0; i < fishCount; i++) {
        fishAgentsCmp[i].ApplayRule();
      }
      frameSkipped = 0;
    }
    else
      frameSkipped++;

    MoveTransforms();
  }

  void MoveTransforms() {
    float deltatime = Time.deltaTime;
    for(int i = 0; i < fishCount; i++) {
      fishAgentsCmp[i].abstrat_transform.Rotation = Quaternion.Slerp(fishAgentsCmp[i].abstrat_transform.Rotation, Quaternion.LookRotation(fishAgentsCmp[i].Velocity), deltatime * fishAgentsCmp[i].avgRotationSpeed);
      fishAgentsCmp[i].abstrat_transform.Position = fishAgentsCmp[i].lastPosition + (fishAgentsCmp[i].Velocity * deltatime * fishAgentsCmp[i].avgSpeed);
    }
  }

  void RandomizeGoalPosition() {
    goalPosition = GetRandomPosition();
    for(int i = 0; i < fishCount; i++) {
      fishAgentsCmp[i].GoalPosUpdated(goalPosition);
    }
  }

  Vector3 GetRandomPosition() {
    return new Vector3(Random.Range(-tankSize + tankOffset, tankSize - tankOffset), Random.Range(-tankSize + tankOffset, tankSize - tankOffset), Random.Range(-tankSize + tankOffset, tankSize - tankOffset)) + transform.position;
  }


  public void CheckFishesOnGoal() {
    for(int i = 0; i < fishCount; i++) {
      if(fishAgentsCmp[i].GoalReached) {
        if(fishAgentsCmp[i].useCohesion) {
          GoalReached();
          fishAgentsCmp[i].GoalReached = false;
        }
        else {
          fishAgentsCmp[i].GoalPosUpdated(GetRandomPosition());
          fishAgentsCmp[i].GoalReached = false;
        }
      }
    }
  }
  public void GoalReached() {
    fishOnGoal++;
  }

  void OnDrawGizmos() {
    if(!showfishdebug) return;
    Gizmos.color = new Color(1, 1, 1, 0.2f);
    Gizmos.DrawCube(transform.position, Vector3.one * (tankSize * 2));
    Gizmos.color = new Color(0, 0, 0, 1);
    Gizmos.DrawSphere(goalPosition, 0.3f);

    if(Application.isPlaying) {
      for(int i = 0; i < fishCount; i++) {
        showCohesionDebug(fishAgentsCmp[i]);
        showAlignmentDebug(fishAgentsCmp[i]);
        showSeparationDebug(fishAgentsCmp[i]);
      }
    }
  }

  public void showAlignmentDebug(FishAgentComponent f) {
    Debug.DrawRay(f.lastPosition, f.Alignment.normalized * 10, Color.blue);
  }

  public void showCohesionDebug(FishAgentComponent f) {
    Debug.DrawRay(f.lastPosition, f.Cohesion.normalized * 10, Color.green);
  }
  public void showSeparationDebug(FishAgentComponent f) {
    Debug.DrawRay(f.lastPosition, f.Separation.normalized * 10, Color.red);
  }
}
