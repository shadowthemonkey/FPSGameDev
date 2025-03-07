using UnityEngine;

public class PlayerLadder : MonoBehaviour
{
    // this script sets a ladder state for climbing

    private PlayerState playerState;

    private void Awake()
    {
        playerState = GetComponent<PlayerState>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            playerState.SetOnLadder(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            playerState.SetOnLadder(false);
        }
    }
}