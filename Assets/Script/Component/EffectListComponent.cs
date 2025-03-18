using UnityEngine;

namespace WannaBe
{
    public class EffectListComponent : MonoBehaviour
    {
        public EffectList Effects { get; private set; } = new EffectList();

        private void Update()
        {
            Effects.UpdateEffects(Time.deltaTime);
        }

        /// <summary>
        /// ����������� ��� ������� ������ (���������, ��������� � ���).
        /// </summary>
        public void ResetEffects()
        {
            Effects.ResetEffects();
        }
    }
}
