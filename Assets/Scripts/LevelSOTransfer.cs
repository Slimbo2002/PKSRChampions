using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSOTransfer : MonoBehaviour
{
    public static LevelSOTransfer Instance { get; private set; }
    public LevelSO currentLevelSO;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
