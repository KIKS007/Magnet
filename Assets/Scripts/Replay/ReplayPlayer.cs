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
        public List<BoolData> dashData = new List<BoolData>();
        public List<BoolData> stunData = new List<BoolData>();
        public List<BoolData> safeData = new List<BoolData>();

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
            playerScript.OnSafe += SafeStart;
            playerScript.OnSafeEnd += SafeEnd;
            playerScript.OnDeath += DashEnd;

            playerFXScript.OnStunFXON += StunON;
            playerFXScript.OnStunFXOFF += StunOFF;
        }

        public override void OnClear()
        {
            base.OnClear();

            dashData = new List<BoolData>();
            stunData = new List<BoolData>();
            safeData = new List<BoolData>();
        }

        void Dash()
        {
            if (ReplayManager.Instance.isRecording)
                dashData.Add(new BoolData(true));
        }

        void DashEnd()
        {
            if (ReplayManager.Instance.isRecording)
                dashData.Add(new BoolData(false));
        }

        void DashDispo()
        {
            if (ReplayManager.Instance.isRecording)
                ReplayManager.Instance.particlesReplay.Add(new ReplayManager.Particles(playerFXScript.dashAvailableFX));
        }

        void StunON()
        {
            if (ReplayManager.Instance.isRecording)
                stunData.Add(new BoolData(true));
        }

        void StunOFF()
        {
            if (ReplayManager.Instance.isRecording)
                stunData.Add(new BoolData(false));
        }

        void SafeStart()
        {
            if (ReplayManager.Instance.isRecording)
                safeData.Add(new BoolData(true));
        }

        void SafeEnd()
        {
            if (ReplayManager.Instance.isRecording)
                safeData.Add(new BoolData(false));
        }

        public override void OnRecordingStart()
        {
            base.OnRecordingStart();

            dashData.Add(new BoolData(false));
            stunData.Add(new BoolData(true));
            safeData.Add(new BoolData(false));
        }

        public override void Replay(float t)
        {
            base.Replay(t);

            bool enable = false;

            if (dashData.Count > 0)
                enable = dashData[0].enabled;

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


            if (stunData.Count > 0)
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
            {
                if (!playerFXScript.stunned)
                    playerFXScript.StunON();
            }
            else
            {
                if (playerFXScript.stunned)
                    playerFXScript.StunOFF();
            }

            if (safeData.Count > 0)
                enable = safeData[0].enabled;

            foreach (var d in safeData)
            {
                if (d.time <= t)
                    enable = d.enabled;
                else
                {
                    break;
                }
            }

            if (enable)
            {
                if (playerFXScript.playerMesh.gameObject.activeSelf)
                    playerFXScript.playerMesh.gameObject.SetActive(false);

                if (!playerFXScript.stunMesh.gameObject.activeSelf)
                    playerFXScript.stunMesh.gameObject.SetActive(true);
            }
            else
            {
                if (playerFXScript.stunMesh.gameObject.activeSelf)
                    playerFXScript.stunMesh.gameObject.SetActive(false);

                if (!playerFXScript.playerMesh.gameObject.activeSelf)
                    playerFXScript.playerMesh.gameObject.SetActive(true);
            }
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
            playerScript.OnSafe -= SafeStart;
            playerScript.OnSafeEnd -= SafeEnd;
            playerScript.OnDeath -= DashEnd;

            playerFXScript.OnStunFXON -= StunON;
            playerFXScript.OnStunFXOFF -= StunOFF;
        }

        [Serializable]
        public class BoolData
        {
            public float time;
            public bool enabled;

            public BoolData(bool enable)
            {
                time = ReplayManager.Instance.GetCurrentTime();
                enabled = enable;
            }
        }
    }
}
