using UnityEngine;

namespace Necromatic.Character
{
    public class CharacterMovement : MonoBehaviour
    {

        [SerializeField] private float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
        [SerializeField] private float m_MoveSpeedMultiplier = 1f;
        [SerializeField] private float m_AnimSpeedMultiplier = 1f;
        [SerializeField] private Animator m_Animator;
        [SerializeField] private Rigidbody m_Rigidbody;
        [SerializeField] private CapsuleCollider m_Capsule;
        [SerializeField] private float m_TurnSpeed = 10f;
        private const float k_Half = 0.5f;
        private float m_ForwardAmount;
        private Vector3 m_GroundNormal;
        private float m_CapsuleHeight;
        private Vector3 m_CapsuleCenter;

        private void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_CapsuleHeight = m_Capsule.height;
            m_CapsuleCenter = m_Capsule.center;
        }

        public void Move(Vector3 move)
        {
            var rawMove = move;
            move.Normalize();
            move = transform.InverseTransformDirection(move);
            move = Vector3.ProjectOnPlane(move, m_GroundNormal);
            m_ForwardAmount = move.z;
            if (rawMove != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, Mathf.Atan2(rawMove.x, rawMove.z) * Mathf.Rad2Deg, 0), m_TurnSpeed * Time.deltaTime);
            }
            UpdateAnimator(move);
        }

        private void UpdateAnimator(Vector3 move)
        {
            m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);

            float runCycle = Mathf.Repeat(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);

            if (move.magnitude > 0)
            {
                m_Animator.speed = m_AnimSpeedMultiplier;
            }
            else
            {
                m_Animator.speed = 1;
            }
            var velocity = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * move;
            m_Rigidbody.velocity = m_MoveSpeedMultiplier * velocity / Time.deltaTime;
        }

    }
}
