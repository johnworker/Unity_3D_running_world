using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TriggerVolume : MonoBehaviour
{
    public UnityEvent<GameObject> OnEnter;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out CharactorMovement player))
        {
            OnEnter.Invoke(player.gameObject);
        }
    }

    // ���`���s���J
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
