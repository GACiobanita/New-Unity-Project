using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class TurretSystem : MonoBehaviour {

    [System.Serializable]
    public class TurretObjects
    {
        public GameObject turretObj;
        [SerializeField]
        public bool tracker;
        public enum shootingType
        {
            None, Repeater, Burst
        }
        public shootingType turretType;
    }


    [SerializeField]
    public List<TurretObjects> turretList;
    List<TurretObjects> trackerList=new List<TurretObjects>();
    public Transform playerShip;

    void Start()
    {
        SetupTrackers();
    }

    private void Update()
    {
       if(trackerList.Count>0)
        {
            UpdateTurretTarget();
        }
        ShootingRoutine();
    }

    void UpdateTurretTarget()
    {
        Vector2 targetPoint;
        float angle;
        Quaternion rotation;
        foreach (TurretObjects turret in trackerList)
        {
            targetPoint = playerShip.position - turret.turretObj.transform.position;
            angle = Mathf.Atan2(targetPoint.y, targetPoint.x) * Mathf.Rad2Deg;
            rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            turret.turretObj.transform.rotation = rotation;
        }
    }

    void SetupTrackers()
    {
        foreach (TurretObjects turret in turretList)
        {
            if (turret.tracker)
            {
                trackerList.Add(turret);
            }
        }
    }

    void ShootingRoutine()
    {
        foreach(TurretObjects turret in turretList)
        {
            turret.turretObj.GetComponent<Turret>().ShootCaller(turret.turretType.ToString());
        }
    }
}
