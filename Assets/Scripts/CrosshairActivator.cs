using UnityEngine;

namespace Menu
{
    public class CrosshairActivator : MonoBehaviour
    {
        public Crosshair game, menu;

        private void OnEnable()
        {
            menu.ActivateCrosshair();
        }

        private void OnDisable()
        {
            game.ActivateCrosshair();
        }
    }
}
