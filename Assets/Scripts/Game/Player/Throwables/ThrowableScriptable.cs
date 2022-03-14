using UnityEngine;

namespace Game.Player.Throwables
{
    [CreateAssetMenu(fileName = "New Throwable", menuName = "Throwable")]
    public class ThrowableScriptable : ScriptableObject
    {
        public GameObject ThrowObject;

        public float Velocity;
        public float Timer;

        public virtual void OnCollide(Collision coll, GameObject obj)
        {

        }

        public virtual void Trigger(Transform location)
        {

        }
    }
}
