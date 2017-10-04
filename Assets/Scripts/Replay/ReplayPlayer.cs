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
        public List<StunData> stunData = new List<StunData>();

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

            playerFXScript.OnStunFXON += StunON;
            playerFXScript.OnStunFXOFF += StunOFF;
        }

        public override void OnClear()
        {
            base.OnClear();

            dashData = new List<DashData>();
            stunData = new List<StunData>();
        }

        void Dash()
        {
            if (ReplayManager.Instance.isRecording)
                dashData.Add(new DashData(true));
        }

        void DashEnd()
        {
            if (ReplayManager.Instance.isRecording)
                dashData.Add(new DashData(false));
        }

        void DashDispo()
        {
            if (ReplayManager.Instance.isRecording)
                ReplayManager.Instance.particlesReplay.Add(new ReplayManager.Particles(playerFXScript.dashAvailableFX));
        }

        void StunON()
        {
            if (ReplayManager.Instance.isRecording)
                stunData.Add(new StunData(true));
        }

        void StunOFF()
        {
            if (ReplayManager.Instance.isRecording)
                stunData.Add(new StunData(false));
        }

        public override void OnRecordingStart()
        {
            base.OnRecordingStart();

            dashData.Add(new DashData(false));
            stunData.Add(new StunData(true));
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

            if (enable)
                playerFXScript.dashFX.Play();
            else
                playerFXScript.dashFX.Stop();


            enable = stunData[0].enabled;

            foreach (var d in stunData)
            {
                if (d.time <= t)
                    enable = d.enabled;
                else
                {
                    break;
                }
            }

            if (enable)
                playerFXScript.StunON();
            else
                playerFXScript.StunOFF();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

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

            playerFXScript.OnStunFXON -= StunON;
            playerFXScript.OnStunFXOFF -= StunOFF;
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

        [Serializable]
        public class StunData
        {
            public float time;
            public bool enabled;

            public StunData(bool enable)
            {
                time = ReplayManager.Instance.GetCurrentTime();
                enabled = enable;
            }
        }
    }
}
