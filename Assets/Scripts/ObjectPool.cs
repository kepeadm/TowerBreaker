using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private Dictionary<string, Queue<GameObject>> _pools = new();
    private Dictionary<string, GameObject>        _prefabMap = new();

    void Awake() => Instance = this;

    public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        string key = prefab.name;

        if (!_pools.ContainsKey(key))    _pools[key]    = new Queue<GameObject>();
        if (!_prefabMap.ContainsKey(key)) _prefabMap[key] = prefab;

        GameObject obj;
        if (_pools[key].Count > 0)
        {
            obj = _pools[key].Dequeue();
            obj.transform.SetPositionAndRotation(pos, rot);
            obj.SetActive(true);
        }
        else
        {
            obj      = Instantiate(prefab, pos, rot);
            obj.name = key;
        }
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        if (_pools.ContainsKey(obj.name))
            _pools[obj.name].Enqueue(obj);
    }
}