﻿using Server.ExineDatabase;

namespace Server.ExineObjects.Monsters
{
    public class TrollBomber : AxeSkeleton
    {
        protected internal TrollBomber(MonsterInfo info)
            : base(info)
        {
        }

        protected override void CompleteRangeAttack(IList<object> data)
        {
            MapObjectSrv target = (MapObjectSrv)data[0];
            int damage = (int)data[1];
            DefenceType defence = (DefenceType)data[2];

            if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

            List<MapObjectSrv> targets = FindAllTargets(2, target.CurrentLocation);

            if (targets.Count == 0) return;

            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].Attacked(this, targets[i] == target ? damage : damage / 2, defence);
            }
        }
    }
}
