﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private Shader _shader;

    [SerializeField]
    private float
        _diameter,
        _effectMulti,
        _timeMulti = 1,
        _darken = 1,
        _maxDistance,
        _scalar = 1,
        _alpha = 0.2f;

    [SerializeField]
    private int
        _maxBounces = 6;

    [SerializeField]
    private Color _color;

    private List<GameObject >_parts;

    private void Awake()
    {
        StateMachine.change += CheckEnabled;
    }

    private void Start()
    {
        _parts = new List<GameObject>();
        enabled = false;

        Vector3 scale = GetComponentInParent<PlayerController>().transform.localScale;

		if (ServiceLocator.Locate<Menu> ().CurrentState == State.Game)
			CheckEnabled (State.Game);
    }

    private void Update()
    {
        UpdateLaser();
    }

    private void CheckEnabled(State state)
    {
		if (state != State.Game)
		{
			enabled = false;
			return;
		}

        enabled = ServiceLocator.Locate<Settings>().GetBool(Setting.Laser);
    }

    private void UpdateLaser()
    {
        Vector3 direction = transform.forward;
        Vector3 position = transform.position;

        float distance = 0;
        int i = 0;

        while (distance < _maxDistance && i < _maxBounces)
        {
            GameObject laserPart;
            Material mat;

            if (_parts.Count - 1 < i)
            {
                laserPart = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                _parts.Add(laserPart);
                laserPart.transform.parent = transform;

                mat = new Material(_shader);
                laserPart.GetComponent<MeshRenderer>().material = mat;
                SetUniforms(mat);

                Collider laserCol = laserPart.GetComponent<Collider>();
                laserCol.enabled = false;
                Destroy(laserCol);
            }
            else
            {
                laserPart = _parts[i];
                mat = laserPart.GetComponent<MeshRenderer>().material;
            }

            Ray ray = new Ray(position, direction);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit))
                break;

            Transform trans = _parts[i].transform;

            trans.position = (position + hit.point) / 2;
            trans.localScale = new Vector3(_diameter, Vector3.Distance(position, hit.point) * _scalar, _diameter);

            trans.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up) * Quaternion.Euler(90, 0, 0);

            mat.SetVector("_lastPos", position);
            mat.SetVector("_direction", new Vector2(direction.x, direction.z));
            mat.SetFloat("_lastDist", distance);
            mat.SetFloat("_time", -Time.realtimeSinceStartup * _timeMulti);

            Debug.DrawLine(position, hit.point);
            position = hit.point;
            direction = Vector3.Reflect(direction, hit.normal);
            distance += hit.distance;
            i++;
        }

        for (int j = i; j < _parts.Count; j++)
        {
            Destroy(_parts[j]);
            _parts.RemoveAt(j);
        }
    }

    private void SetUniforms(Material mat)
    {
        _color.a = _alpha;

        mat.SetColor("_color", _color);
        mat.SetFloat("_effectMulti", _effectMulti);
        mat.SetFloat("_darken", _darken);
        mat.SetFloat("_maxDist", _maxDistance);
    }

    private void OnValidate()
    {
        if (!Application.isPlaying || _parts == null)
            return;

        foreach (GameObject item in _parts)
            SetUniforms(item.GetComponent<MeshRenderer>().material);
    }

    private void OnDestroy()
    {
        StateMachine.change -= CheckEnabled;
    }

    private void OnDisable()
    {
		if (isActiveAndEnabled)
        	StartCoroutine(RemoveLaser());
    }

    IEnumerator RemoveLaser()
    {
        yield return new WaitForEndOfFrame();

        for (int i = _parts.Count - 1; i >= 0; i--)
        {
            Destroy(_parts[i]);
            _parts.RemoveAt(i);
        }
    }

    public Color Color
    {
        set { _color = value; }
    }
}
