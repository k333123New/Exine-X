using System.IO;
using System.Collections.Generic;

namespace LibraryEditor
{
    public class FrameSet : Dictionary<ExAction, Frame>
    {
        public FrameSet() { }
        public FrameSet(IDictionary<ExAction, Frame> dictionary) : base (dictionary) { }

        public static FrameSet DefaultMonsterFrameSet = new FrameSet
        {
            { ExAction.Standing, new Frame(0, 4, 0, 500) },
            { ExAction.Walking, new Frame(32, 6, 0, 100) },
            { ExAction.Attack1, new Frame(80, 6, 0, 100) },
            { ExAction.Struck, new Frame(128, 2, 0, 200) },
            { ExAction.Die, new Frame(144, 10, 0, 100) },
            { ExAction.Dead, new Frame(153, 1, 9, 1000) },
            { ExAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true } }
        };

        public static FrameSet DefaultNPCFrameSet = new FrameSet
        {
            { ExAction.Standing, new Frame(0, 4, 0, 450) },
            { ExAction.Harvest, new Frame(12, 10, 0, 200) }
        };
    }

    public class Frame
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

        public Frame(int start, int count, int skip, int interval, int effectstart = 0, int effectcount = 0, int effectskip = 0, int effectinterval = 0)
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

        public Frame(BinaryReader reader)
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
