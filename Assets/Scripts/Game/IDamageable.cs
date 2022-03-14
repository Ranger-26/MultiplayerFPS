using Mirror;

namespace Game
{
    public interface IDamageable
    {
        void ServerDamage(float amount, float multiplier);
    }
}