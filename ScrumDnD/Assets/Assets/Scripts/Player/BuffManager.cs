using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class BuffManager : MonoBehaviour
    {
        private Coroutine _coroutine;

        private enum BuffStatus {
            SpeedBuff, RangeBuff, Neutral
        }

        private BuffStatus _buffStatus = BuffStatus.Neutral;

        public float _rangeDuration = 5f;
        public float _speedDuration = 5f;

        public bool ManagerRangeActive()
        {
            
            return _buffStatus == BuffStatus.RangeBuff;
        }

        public bool ManagerSpeedActive()
        {
            return _buffStatus == BuffStatus.SpeedBuff;
        }

        private IEnumerator RemainedBuffed(float buffDuration)
        {
            while (true)
            {
                yield return new WaitForSeconds(buffDuration);
                _buffStatus = BuffStatus.Neutral;
                StopCoroutine(_coroutine);
            }
        }

        public bool ManagerCanBuffPlayer()
        {
            return _buffStatus == BuffStatus.Neutral;
        }

        public void ManagerBuffPlayer(string pExpectedBuff)
        {
            switch (pExpectedBuff)
            {
                case "Speed":
                    _buffStatus = BuffStatus.SpeedBuff;
                    _coroutine = StartCoroutine(RemainedBuffed(_speedDuration));
                    
                    break;
                case "Range":
                    _buffStatus = BuffStatus.RangeBuff;
                    _coroutine = StartCoroutine(RemainedBuffed(_rangeDuration));
                    break;
                default:
                    Debug.Log("Ability Not Implemented");
                    break;
            }
        }
    }
}