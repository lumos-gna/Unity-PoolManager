using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    private Dictionary<string, IObjectPool<GameObject>> _pools = new ();
    private HashSet<GameObject> _activeObjects = new();

    
    public void CreatePool(string key, GameObject prefab, int defaultCapacity = 10, int maxSize = 100)
    {
        if (_pools.ContainsKey(key))
            return;

        var pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab),
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        _pools.Add(key, pool);
    }


    public GameObject Get(string key)
    {
        if (_pools.ContainsKey(key))
        {
            var obj = _pools[key].Get();
            _activeObjects.Add(obj);
            return obj;
        }
        
        Debug.LogError($"{key} 를 찾을 수 없음!");
        return null;
    }

    public void Release(string key, GameObject obj)
    {
        if (_activeObjects.Remove(obj)) 
        {
            _pools[key].Release(obj);
            return;
        }
        
        Debug.LogError($"{key} 를 찾을 수 없음!");
        Destroy(obj);
    }
    
    public void ReleaseAll(string key) 
    {
        foreach (var obj in new List<GameObject>(_activeObjects)) 
        {
            Release(key, obj); 
        }
    }
}
