using System.IO;
using System.Collections.Generic;

namespace NewYPF
{
    public enum MirAction : byte
    {
        Standing,
        Walking,
        Running,
        Pushed,
        DashL,
        DashR,
        DashFail,
        Stance,
        Stance2,
        Attack1,
        Attack2,
        Attack3,
        Attack4,
        Attack5,
        AttackRange1,
        AttackRange2,
        AttackRange3,
        Special,
        Struck,
        Harvest,
        Spell,
        Die,
        Dead,
        Skeleton,
        Show,
        Hide,
        Stoned,
        Appear,
        Revive,
        SitDown,
        Mine,
        Sneek,
        DashAttack,
        Lunge,

        WalkingBow,
        RunningBow,
        Jump,

        MountStanding,
        MountWalking,
        MountRunning,
        MountStruck,
        MountAttack,

        FishingCast,
        FishingWait,
        FishingReel
    }

    public class FrameSet : Dictionary<MirAction, M2Frame>
    {
        public FrameSet() { }
        public FrameSet(IDictionary<MirAction, M2Frame> dictionary) : base(dictionary) { }

        public static FrameSet DefaultMonsterFrameSet = new FrameSet
        {
            { MirAction.Standing, new M2Frame(0, 4, 0, 500) },
            { MirAction.Walking, new M2Frame(32, 6, 0, 100) },
            { MirAction.Attack1, new M2Frame(80, 6, 0, 100) },
            { MirAction.Struck, new M2Frame(128, 2, 0, 200) },
            { MirAction.Die, new M2Frame(144, 10, 0, 100) },
            { MirAction.Dead, new M2Frame(153, 1, 9, 1000) },
            { MirAction.Revive, new M2Frame(144, 10, 0, 100) { Reverse = true } }
        };

        public static FrameSet DefaultNPCFrameSet = new FrameSet
        {
            { MirAction.Standing, new M2Frame(0, 4, 0, 450) },
            { MirAction.Harvest, new M2Frame(12, 10, 0, 200) }
        };
    }

    public class M2Frame
    {
        public int Start { get; set; }
        public int Count { get; set; }
        public int Skip { get; set; }
        public int Interval { get; set; }

        public int EffectStart { get; set; }
        public int EffectCount { get; set; }
        public int EffectSkip { get; set; }
        public int EffectInterval { get; set; }

        public bool Reverse { get; set; }
        public bool Blend { get; set; }

        public int OffSet
        {
            get { return Count + Skip; }
        }

        public M2Frame(int start, int count, int skip, int interval, int effectstart = 0, int effectcount = 0, int effectskip = 0, int effectinterval = 0)
        {
            Start = start;
            Count = count;
            Skip = skip;
            Interval = interval;
            EffectStart = effectstart;
            EffectCount = effectcount;
            EffectSkip = effectskip;
            EffectInterval = effectinterval;
        }

        public M2Frame(BinaryReader reader)
        {
            Start = reader.ReadInt32();
            Count = reader.ReadInt32();
            Skip = reader.ReadInt32();
            Interval = reader.ReadInt32();
            EffectStart = reader.ReadInt32();
            EffectCount = reader.ReadInt32();
            EffectSkip = reader.ReadInt32();
            EffectInterval = reader.ReadInt32();
            Reverse = reader.ReadBoolean();
            Blend = reader.ReadBoolean();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Start);
            writer.Write(Count);
            writer.Write(Skip);
            writer.Write(Interval);
            writer.Write(EffectStart);
            writer.Write(EffectCount);
            writer.Write(EffectSkip);
            writer.Write(EffectInterval);
            writer.Write(Reverse);
            writer.Write(Blend);
        }
    }
}
