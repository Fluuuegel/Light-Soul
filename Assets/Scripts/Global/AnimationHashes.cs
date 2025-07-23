// AnimationHashes.cs
using UnityEngine;

namespace Global.Hashes
{
    public static class AnimationHashes
    {
        public static readonly int Motion = Animator.StringToHash("Motion");
        public static readonly int Sprint = Animator.StringToHash("Sprint");
        public static readonly int Grounded = Animator.StringToHash("Grounded");
        public static readonly int AnimationMove = Animator.StringToHash("AnimationMove");
        public static readonly int Dodge = Animator.StringToHash("Dodge");

        public static readonly int Crouch = Animator.StringToHash("Crouch");
        public static readonly int Jump = Animator.StringToHash("Jump");
        public static readonly int FreeFall = Animator.StringToHash("FreeFall");

        public static readonly int LAttack = Animator.StringToHash("LAttack");
        public static readonly int HAttack = Animator.StringToHash("HAttack");
        public static readonly int Denfense = Animator.StringToHash("Denfense");
        public static readonly int Horizontal = Animator.StringToHash("Horizontal");
        public static readonly int Vertical = Animator.StringToHash("Vertical");

        public static readonly int Equip = Animator.StringToHash("Equip"); // Equip trigger
        public static readonly int IsEquipped = Animator.StringToHash("IsEquipped"); // Save the state
        public static readonly int Equipped = Animator.StringToHash("Equipped"); // Control the blend tree for locomotion
        public static readonly int Lock = Animator.StringToHash("Lock");
    }
}