/* IN-GAME REPLAY - @madebyfeno - <feno@ironequal.com>
 * You can use it in commercial projects (and non-commercial project of course), modify it and share it.
 * Do not resell the resources of this project as-is or even modified. 
 * TL;DR: Do what the fuck you want but don't re-sell it
 * 
 * ironequal.com
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace Replay
{
    public class ReplayPlayer : ReplayComponent
    {
        public List<DashData> dashData = new List<DashData>();

        private PlayersGameplay playerScript;
        private PlayersFXAnimations playerFXScript;

        protected override void Start()
        {
            base.Start();

            playerScript = GetComponent<PlayersGameplay>();
            playerFXScript = GetComponent<PlayersFXAnimations>();

            playerScript.OnDashAvailable += DashDispo;
            playerScript.OnDash += Dash;
            playerScript.OnDashEnd += DashEnd;
        }

        public override void OnClear()
        {
            base.OnClear();

            dashData = new List<DashData>();
        }

        void Dash()
        {
            dashData.Add(new DashData(true));
        }

        void DashEnd()
        {
            dashData.Add(new DashData(false));
        }

        void DashDispo()
        {
            ReplayManager.Instance.particlesReplay.Add(new ReplayManager.Particles(playerFXScript.dashAvailableFX));
        }

        public override void OnRecordingStart()
        {
            base.OnRecordingStart();

            dashData.Add(new DashData(false));
        }

        public override void Replay(float t)
        {
            base.Replay(t);

            bool enable = dashData[0].enabled;

            foreach (var d in dashData)
            {
                if (d.time <= t)
                    enable = d.enabled;
                else
                {
                    break;
                }
            }

            if (playerFXScript == null)
                Debug.LogError("FXScript Null", this);

            if (playerFXScript.dashFX == null)
                Debug.LogError("DashFX Null", this);

            if (enable)
                playerFXScript.dashFX.Play();
            else
                playerFXScript.dashFX.Stop();
        }

        void OnDestroy()
        {
            if (GlobalVariables.applicationIsQuitting)
                return;

            if (playerScript == null)
            {
                playerScript = GetComponent<PlayersGameplay>();
                playerFXScript = GetComponent<PlayersFXAnimations>();
            }

            playerScript.OnDashAvailable -= DashDispo;
            playerScript.OnDash -= Dash;
            playerScript.OnDashEnd -= DashEnd;
        }

        [Serializable]
        public class DashData
        {
            public float time;
            public bool enabled;

            public DashData(bool enable)
            {
                time = ReplayManager.Instance.GetCurrentTime();
                enabled = enable;
            }
        }
    }
}
