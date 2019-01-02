using UnityEngine;
public struct TransformComponent {
  private Transform T;
  private Vector3 lastPos;
  private Quaternion lastRot;
  public void SetPosition(Vector3 pos) {
    T.position = pos;
    this.lastPos = pos;
    this.lastRot = T.rotation;
  }
  public Vector3 Position {
    get { return lastPos; }
    set {
      lastPos = value;
      T.position = lastPos;
    }
  }
  public Quaternion Rotation {
    get { return lastRot; }
    set {
      lastRot = value;
      T.rotation = lastRot;
    }
  }
  public Transform Parent {
    get { return T.parent; }
    set { T.parent = value; }
  }
  public void Set(Transform t) {
    T = t;
  }
}