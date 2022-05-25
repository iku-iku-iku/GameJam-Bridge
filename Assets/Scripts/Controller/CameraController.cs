using Common;
using UnityEngine;

namespace Controller
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private GameObject follow;

        [SerializeField] private float reachTime;

        public Player player;

        private void Start()
        {
            follow = player == Player.P1 ? Global.P1.gameObject : Global.P2.gameObject;
        }

        private void Update()
        {
            var transformPosition = follow.transform.position;
            transform.position = new Vector3(transformPosition.x, transformPosition.y, transform.position.z);
            // transform.DOMove(new Vector3(transformPosition.x, transformPosition.y, transform.position.z), reachTime);
        }
    }
}
