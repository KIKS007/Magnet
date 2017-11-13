using UnityEngine;
using System.Collections;

public class PlayersDeadZone : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 pos = other.transform.position;
            //Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, new Vector3(0, 0, 0));

            GameObject instantiatedParticles = Instantiate(GlobalVariables.Instance.DeadParticles, pos, rot) as GameObject;
            instantiatedParticles.transform.SetParent(GlobalVariables.Instance.particlesParent);
            instantiatedParticles.transform.position = new Vector3(instantiatedParticles.transform.position.x, 2f, instantiatedParticles.transform.position.z);
            instantiatedParticles.transform.LookAt(new Vector3(0, 0, 0));
            instantiatedParticles.GetComponent<ParticleSystemRenderer>().material.color = GlobalVariables.Instance.playersColors[(int)other.gameObject.GetComponent<PlayersGameplay>().playerName];

            other.GetComponent<PlayersGameplay>().Death(DeathFX.All, other.transform.position);
        }
    }
}
