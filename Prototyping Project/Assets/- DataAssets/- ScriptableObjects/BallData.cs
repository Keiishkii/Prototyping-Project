using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Asset/Ball Data")]
public class BallData : ScriptableObject
{
    [SerializeField] private BallData _privateInstance;

    public static BallData instance
    {
        get
        {
            if (instance == null)
            {
                return new BallData()._privateInstance;
            }
            else
            {
                return instance;
            }
        }
    }

    public float BallRadius;
}
