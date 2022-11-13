using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Prototype_PhysicsSimulation : MonoBehaviour
{
    [SerializeField] private int simulationCount;
    [SerializeField] private int rigidbodyTestCount;

    [SerializeField] private GameObject rigidbodyObject_Prefab;
    private GameObject rigidbodyObject;

    private Scene _mainScene;
    private PhysicsScene _mainPhysicsScene;

    private Scene _simulationScene;
    private PhysicsScene _simulationPhysicsScene;

    private void Awake()
    {
        _mainScene = SceneManager.GetActiveScene();
        _mainPhysicsScene = _mainScene.GetPhysicsScene();

        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _simulationPhysicsScene = _simulationScene.GetPhysicsScene();

        Physics.autoSimulation = false;

        for (int i = 0; i < rigidbodyTestCount; i++)
        {
            rigidbodyObject = GameObject.Instantiate(rigidbodyObject_Prefab, Vector3.zero, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(rigidbodyObject, _simulationScene);
        }
    }

    private void FixedUpdate()
    {
        _mainPhysicsScene.Simulate(Time.fixedDeltaTime);
    }

    private void Update()
    {
        for (int i = 0; i < simulationCount; i++)
        {
            _simulationPhysicsScene.Simulate(Time.fixedDeltaTime);
        }
    }
}
