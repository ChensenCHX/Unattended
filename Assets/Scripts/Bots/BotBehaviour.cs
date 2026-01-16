using System;
using UnityEngine;

namespace Bots
{
    public class BotBehaviour : MonoBehaviour
    {
        public void GetPosition(out int x, out int y)
        {
            x = Mathf.RoundToInt(this.transform.position.x);
            y = Mathf.RoundToInt(this.transform.position.y);
        }
    }
}