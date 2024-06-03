using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoOChiGame.Utilts
{
    public static class Utilts{
        public static Vector3 GetRandomDir()
        {
            return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

    }
}
