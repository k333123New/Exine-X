﻿using log4net;
using System.Runtime.CompilerServices;
using Server.ExineDatabase;

namespace Server.ExineObjects
{
    public class Reporting
    {
        private static readonly ILog log = Logger.GetLogger(LogType.Player);

        protected static MessageQueue MessageQueue
        {
            get { return MessageQueue.Instance; }
        }

        private readonly PlayerObjectSrv _player;

        public Reporting(PlayerObjectSrv player)
        {
            this._player = player;
        }

        #region Move Actions

        public void MapChange(MapInfo oldMap, MapInfo newMap, [CallerMemberName] string source = "")
        {
            string message = $"Moved Map {oldMap.FileName} => {newMap.FileName}";

            LogMessage(message, source);
        }

        #endregion
        
        #region Item Actions

        public void ItemSplit(UserItem item, UserItem newItem, MirGridType grid, [CallerMemberName] string source = "")
        {
            string message = $"Item Split - {item.Info.Name} from x{item.Count + newItem.Count} to x{item.Count} in {grid} - Created new item ({newItem.UniqueID}) x {newItem.Count}";

            LogMessage(message, source);
        }

        public void ItemMerged(UserItem fromItem, UserItem toItem, int slotFrom, int slotTo, MirGridType gridFrom, MirGridType gridTo, [CallerMemberName] string source = "")
        {
            string message = $"Item Merged - {fromItem.Info.Name} with {toItem.Info.Name} from {slotFrom} ({gridFrom}) to {slotTo} ({gridTo}) ({toItem.UniqueID})";

            LogMessage(message, source);
        }
        public void ItemCombined(UserItem fromItem, UserItem toItem, int slotFrom, int slotTo, MirGridType grid, [CallerMemberName] string source = "")
        {
            string message = $"Item Combined - {fromItem.Info.Name} with {toItem.Info.Name} from {slotFrom} to {slotTo} in {grid} ({toItem.UniqueID})";

            LogMessage(message, source);
        }

        public void ItemMoved(UserItem item, MirGridType from, MirGridType to, int slotFrom, int slotTo, string info = "", [CallerMemberName] string source = "")
        {
            string message = $"Item Moved - {(item != null ? item.Info.Name : "Empty")} from {from}:{slotFrom} to {to}:{slotTo} ({item?.UniqueID}) {info}";

            LogMessage(message, source);
        }

        public void ItemChanged(UserItem item, uint amount, int state, [CallerMemberName] string source = "")
        {
            string type = string.Empty;

            switch (state)
            {
                case 1:
                    type = "Lost";
                    break;
                case 2:
                    type = "Gained";
                    break;
            }

            string message = $"Item {type} - {item.Info.Name} x{amount} ({item.UniqueID})";

            LogMessage(message, source);
        }

        public void ItemChangedHero(UserItem item, uint amount, int state, [CallerMemberName] string source = "")
        {
            string type = string.Empty;

            switch (state)
            {
                case 1:
                    type = "Lost";
                    break;
                case 2:
                    type = "Gained";
                    break;
            }

            string message = $"Item {type} - {item.Info.Name} x{amount} ({item.UniqueID})";

            LogHeroMessage(message, source);
        }

        public void ItemGSBought(GameShopItem item, uint amount, uint CreditCost, uint GoldCost, [CallerMemberName] string source = "")
        {
            string message = $"Purchased {item.Info.FriendlyName} x{amount} for {CreditCost} Credits and {GoldCost} Gold.";

            LogMessage(message, source);
        }
 

        public void GoldChanged(uint amount, bool lost = true, string info = "", [CallerMemberName] string source = "")
        {
            string message = $"Gold{(lost ? "Lost" : "Gained")} - x{amount} {info}";

            LogMessage(message, source);
        }

        public void CreditChanged(uint amount, bool lost = true, string info = "", [CallerMemberName] string source = "")
        {
            string message = $"Credit{(lost ? "Lost" : "Gained")} - x{amount} {info}";

            LogMessage(message, source);
        }

        public void ItemError(MirGridType from, MirGridType to, int slotFrom, int slotTo, [CallerMemberName] string source = "")
        {
            string message = $"Item Moved Error - from {from}:{slotFrom} to {to}:{slotTo}";

            LogMessage(message, source);
        }

        #endregion

        #region Kill Actions

        public void KilledPlayer(PlayerObjectSrv obj, string info = "", [CallerMemberName] string source = "")
        {
            string message = $"Killed Player {obj.Name} {info}";

            LogMessage(message, source);
        }

        public void KilledMonster(MonsterObjectSrv obj, string info = "", [CallerMemberName] string source = "")
        {
            string message = $"Killed Monster {obj.Name} {info}";

            LogMessage(message, source);
        }

        #endregion

        #region Other Actions

        public void Levelled(int level)
        {
            string message = $"Levelled to {level}";

            LogMessage(message, "");
        }

        public void Died(string map)
        {
            string message = $"Died - Map {map}";

            LogMessage(message, "");
        }

        public void Connected(string ipAddress)
        {
            string message = $"Connected - {ipAddress}";

            LogMessage(message, "");
        }

        public void Disconnected(string reason)
        {
            string message = $"Disconnected - {reason}";

            LogMessage(message, "");
        }

        #endregion

        #region Private Methods

        private void LogMessage(string message, string source)
        {
            try
            {
                var logMessage = $"{_player.Name} - {source} : {message}";

                log.Info(logMessage);
            }
            catch (Exception ex)
            {
                MessageQueue.SendMsg(ex);
            }
        }

        private void LogHeroMessage(string message, string source)
        {
            try
            {
                var logMessage = $"{_player.Name}[Hero] - {source} : {message}";

                log.Info(logMessage);
            }
            catch (Exception ex)
            {
                MessageQueue.SendMsg(ex);
            }
        }

        #endregion
    }
}
