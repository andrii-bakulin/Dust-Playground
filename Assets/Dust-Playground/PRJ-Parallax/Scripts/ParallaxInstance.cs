using UnityEngine;

namespace DustEngine
{
    [AddComponentMenu("Dust-Playground/Support/Parallax Instance")]
    public class ParallaxInstance : DuMonoBehaviour
    {
        [SerializeField]
        internal Parallax m_ParentParallax = null;
        public Parallax parentParallax => m_ParentParallax;

        //--------------------------------------------------------------------------------------------------------------

        public void Initialize(Parallax parallax)
        {
            m_ParentParallax = parallax;
        }
    }
}
