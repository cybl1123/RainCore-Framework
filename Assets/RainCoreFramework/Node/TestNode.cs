using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNode : MonoBehaviour,ICanUseMonoUpdater
{
    [SerializeField] string Name;
    [SerializeField] int _updateOrder = 0;
    public int UpdateOrder { get =>  _updateOrder; set => _updateOrder =value; }
    bool _inUpdate = false;
    public bool InUpdater { get => _inUpdate; set => _inUpdate=value; }

    void ICanUseMonoUpdater.FixedUpdate()
    {
        Debug.Log(Name+":"+ "FixedUpdate更新"+":顺序"+_updateOrder);
    }

    void ICanUseMonoUpdater.LateUpdate()
    {
        Debug.Log(Name + ":" + "LateUpdate更新" + ":顺序" + _updateOrder);
    }
    void ICanUseMonoUpdater.Update()
    {
        Debug.Log(Name + ":" + "Update更新" + ":顺序" + _updateOrder);
    }
    // Start is called before the first frame update
    void Start()
    {
        this.EnableMonoUpdate();   
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.EnableMonoUpdate();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            this.DisableMonoUpdate();
        }
    }
}
