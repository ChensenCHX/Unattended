using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using UnityEngine;
using Utils;

namespace Bots
{
    public class BotManager : SingletonMono<BotManager>
    {
        private readonly Dictionary<int, BotBehaviour> usingBots = new();
        public GameObject BotPrefab;
        
        public bool AllocBot(int id, int x, int y)
        {
            usingBots[id] = Instantiate(BotPrefab).GetComponent<BotBehaviour>();
            usingBots[id].transform.position = new Vector3(x, GlobalConsts.BotStanderYAxisValue, y);
            usingBots[id].FadeIn();
            // 这里只需要is not即可 刚创建的瞬间它还不存在
            return usingBots[id] is not null;
        }
        public void ReleaseBot(int id)
        {
            usingBots[id].FadeOut();
            usingBots.Remove(id);
        }
        public void ReleaseAllBots()
        {
            usingBots.ToList().ForEach(kv => ReleaseBot(kv.Key));
            usingBots.Clear();
        }
        public int GetRemainBots() => usingBots.Count;
        public bool GetBotByID(int id, out BotBehaviour bot) => usingBots.TryGetValue(id, out bot);
    }
}