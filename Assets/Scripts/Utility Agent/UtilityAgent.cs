using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UtilityAgent : Agent
{

    [SerializeField] Perception perception;
    [SerializeField] MeterUI meter;

    Need[] needs;
    UtilityObject activeUitlityObject = null;

    static readonly float MIN_SCORE = 0.1f;
    public bool isUsingUtilityObject { get { return activeUitlityObject != null; } }

    public float happiness
    {
        get
        {
            float totalMotive = 0;
            foreach(var need in needs)
            {
                totalMotive += need.motive;
            }
            return 1 - (totalMotive / needs.Length);
        }
    }

    void Start()
    {
        needs = GetComponentsInChildren<Need>();
        meter.text.text = "";
    }

    void Update()
    {
        animator.SetFloat("speed", movement.velocity.magnitude);

        if(activeUitlityObject == null)
        {
            var gameObjects = perception.GetGameObjects();
            List<UtilityObject> utilityObjects = new List<UtilityObject>();
            foreach(var go in gameObjects)
            {
                if(go.TryGetComponent<UtilityObject>(out UtilityObject uo))
                {
                    uo.visible = true;
                    uo.score = GetUtilityObjectScore(uo);
                    if(uo.score > MIN_SCORE) utilityObjects.Add(uo);
                }
            }
            activeUitlityObject = (utilityObjects.Count() > 0) ? utilityObjects[0] : null;
            if(activeUitlityObject != null)
            {
                StartCoroutine(ExecuteUtilityObject(activeUitlityObject));
            }
        }

    }

    void LateUpdate()
    {
        meter.slider.value = happiness;
        meter.worldPosition = transform.position + Vector3.up * 4;
    }

    IEnumerator ExecuteUtilityObject(UtilityObject uO)
    {
        movement.MoveTowards(uO.location.position);
        while(Vector3.Distance(transform.position, uO.location.position) > 0.5f)
        {
            Debug.DrawLine(transform.position, uO.location.position);
            yield return null;
        }
        print("start effect");

        if (uO.effect != null) uO.effect.SetActive(true);
        yield return new WaitForSeconds(uO.duration);

        print("stop effect");
        if (uO.effect != null) uO.effect.SetActive(false);

        ApplyUtilityObject(uO);

        activeUitlityObject = null;

        yield return null;
    }

    void ApplyUtilityObject(UtilityObject utilityObject)
    {
        foreach (var effector in utilityObject.effectors)
        {
            Need need = GetNeedByType(effector.type);
            if(need != null)
            {
                need.input += effector.change;
                need.input = Mathf.Clamp(need.input, -1, 1);
            }
        }
    }

    float GetUtilityObjectScore(UtilityObject uO)
    {
        float score = 0;

        foreach(var effector in uO.effectors)
        {
            Need need = GetNeedByType(effector.type);
            if(need != null)
            {
                float futureNeed = need.getMotive(need.input + effector.change);
                score += need.motive - futureNeed;
            }
        }

        return score;
    }

    Need GetNeedByType(Need.Type type)
    {
        return needs.First(need => need.type == type);
    }

    /*private void OnGUI()
    {
        Vector2 screen = Camera.main.WorldToScreenPoint(transform.position);

        GUI.color = Color.black;
        int offset = 0;
        foreach (var need in needs)
        {
            GUI.Label(new Rect(screen.x + 20, Screen.height - screen.y - offset, 300, 20), need.type.ToString() + ": " + need.motive);
            offset += 20;
        }
        //GUI.Label(new Rect(screen.x + 20, Screen.height - screen.y - offset, 300, 20), mood.ToString());
    }*/
}
