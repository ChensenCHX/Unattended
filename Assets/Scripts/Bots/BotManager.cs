using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Bots
{
    public class BotManager : SingletonMono<BotManager>
    {
        private readonly Dictionary<int, BotBehaviour> usingBots = new();
        public GameObject BotPrefab;
        
        public bool AllocBot(int id)
        {
            usingBots[id] = Instantiate(BotPrefab).GetComponent<BotBehaviour>();
            // 这里只需要is not即可 刚创建的瞬间它还不存在
            return usingBots[id] is not null;
        }
        public void ReleaseBot(int id)
        {
            Destroy(usingBots[id].gameObject);
            usingBots.Remove(id);
        }
        public void ReleaseAllBots()
        {
            usingBots.ToList().ForEach(kv => Destroy(kv.Value.gameObject));
            usingBots.Clear();
        }
        public int GetRemainBots() => usingBots.Count;
        public bool GetBotByID(int id, out BotBehaviour bot) => usingBots.TryGetValue(id, out bot);
    }
}