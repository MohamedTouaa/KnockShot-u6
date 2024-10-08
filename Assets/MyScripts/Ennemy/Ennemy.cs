using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ennemy : PoolableObject
{
    public EnnemyMovement ennemyMovement;
    public NavMeshAgent meshAgent;
    public EnemyScriptableObject EnemyScriptableObject;
    private int health = 100;

    public virtual void OnEnable()
    {
        SetupAgentFromConfiguration();
    }

    public override void OnDisable()
    {
        base.OnDisable();

        meshAgent.enabled = false;
    }

    public virtual void SetupAgentFromConfiguration()
    {
        meshAgent.acceleration = EnemyScriptableObject.Acceleration;
       meshAgent.angularSpeed = EnemyScriptableObject.AngularSpeed;
        meshAgent.areaMask = EnemyScriptableObject.AreaMask;
        meshAgent.avoidancePriority = EnemyScriptableObject.AvoidancePriority;
        meshAgent.baseOffset = EnemyScriptableObject.BaseOffset;
        meshAgent.height = EnemyScriptableObject.Height;
        meshAgent.obstacleAvoidanceType = EnemyScriptableObject.ObstacleAvoidanceType;
        meshAgent.radius = EnemyScriptableObject.Radius;
        meshAgent.speed = Random.Range(EnemyScriptableObject.minSpeed, EnemyScriptableObject.maxSpeed);
        meshAgent.stoppingDistance = EnemyScriptableObject.StoppingDistance;

        ennemyMovement.UpdateRate = EnemyScriptableObject.AIUpdateInterval;

        health = EnemyScriptableObject.Health;
    }
}
