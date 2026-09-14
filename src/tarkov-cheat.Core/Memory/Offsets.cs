namespace TarkovCheat.Core.Memory;

/// <summary>Structure offsets for Escape from Tarkov — update after each game patch.</summary>
public static class Offsets
{
    /// <summary>Module-level addresses (relative to module base).</summary>
    public static class Client
    {
        public const nint LocalPlayer        = 0x1666FD4;
        public const nint EntityList         = 0x183CDA2;
        public const nint ViewMatrix         = 0x19E6D5D;
        public const nint GameRules          = 0x194B2FD;
        public const nint GlobalVars         = 0x1888F75;
        public const nint InputSystem        = 0x1A0C36E;
    }

    /// <summary>Entity / pawn struct field offsets.</summary>
    public static class Entity
    {
        public const nint Health             = 0x3D4;
        public const nint TeamNum            = 0x3E6;
        public const nint Origin             = 0x15A2;
        public const nint EyeAngles          = 0x1463;
        public const nint SceneNode          = 0x35D;
        public const nint ModelState         = 0x16E;
        public const nint ShotsFired         = 0x22FD;
        public const nint AimPunch           = 0x186C;
        public const nint IsScoped           = 0x2475;
        public const nint CrosshairId        = 0x1578;
        public const nint Flags              = 0x16E;
        public const nint Velocity           = 0x15E0;
        public const nint FlashDuration      = 0x155D;
        public const nint SpottedMask        = 0x17A3;
        public const nint BoneMatrix         = 0xF56;
    }

    /// <summary>Bone indices for skeleton rendering and aim targeting.</summary>
    public static class Bones
    {
        public const int Head               = 6;
        public const int Neck               = 5;
        public const int SpineUpper         = 4;
        public const int SpineMid           = 3;
        public const int Pelvis             = 0;
        public const int LeftShoulder       = 8;
        public const int LeftElbow          = 9;
        public const int LeftHand           = 13;
        public const int RightShoulder      = 30;
        public const int RightElbow         = 31;
        public const int RightHand          = 35;
        public const int LeftKnee           = 22;
        public const int LeftFoot           = 24;
        public const int RightKnee          = 44;
        public const int RightFoot          = 46;
    }
}
