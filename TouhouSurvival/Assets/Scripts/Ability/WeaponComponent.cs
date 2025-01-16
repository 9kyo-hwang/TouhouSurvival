namespace Unchord
{
    public abstract class WeaponComponent : AbilityComponent
    {
        public WeaponActivationMode weaponActivationMode = WeaponActivationMode.FixedCooltime;
        public float fixedCooltime = 1.0f;
        public float variableCooltimeMin = 1.0f;
        public float variableCooltimeMax = 2.0f;
    }
}