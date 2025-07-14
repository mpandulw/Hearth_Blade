using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player;

    void Start()
    {
        string spawnPointName = PlayerPrefs.GetString("PlayerSpawnPoint", "SpawnPoint_Entry");
        Debug.Log(spawnPointName);
        Transform spawnPoint = GameObject.Find(spawnPointName)?.transform;

        if (spawnPoint != null && player != null)
        {
            player.transform.position = spawnPoint.position;
        }
    }
}
