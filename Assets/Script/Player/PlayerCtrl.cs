using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour, IContainable<Container>
{
    private Rigidbody rig;
    private PlayerInput input;
    private Animator animator;
    private GameObject model;
    //public GameObject heldWare;
    public Transform holdFoodMarkTrans;
    private MatchTargetWeightMask matchTargetWeightMask;
    public bool isCanCtrl = true;
    public bool isHoldFoodSet = false;

    public float rotate_speed;
    public float speed;

    public Vector3 DropFoodPos { get; set; }

    public GameObject BIndicator;
    public GameObject CIndicator;
    public GameObject PIndicator;
    public GameObject SFIndicator;
    public GameObject SMIndicator;
    public TableName AtTable;

    private List<Container> container = new List<Container>(1);
    public List<Container> Contents
    {
        get { return container; }
    }

    private void Awake()
    {
        rig = this.GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        model = transform.Find("Model").gameObject;
        holdFoodMarkTrans = transform.Find("Model/metarig.001/HoldFoodMark").transform;
        matchTargetWeightMask = new MatchTargetWeightMask(new Vector3(1, 1, 1), 0);
    }

    private void Update()
    {
        GameObject curIndicator = null;
        switch (GameManager.Instance.sequenceManager.CurStep.Utensil)
        {
            case "Selecttable":
                curIndicator = SMIndicator;
                break;
            case "EggBowl":
                curIndicator = PIndicator;
                break;
            case "ChoppingBLock":
                curIndicator = CIndicator;
                break;
            case "Pan":
                curIndicator = SFIndicator;
                break;
            case "Pot":
                curIndicator = BIndicator;
                break;
            default:
                break;
        }

        if (isCanCtrl)
        {
            curIndicator.SetActive(true);
        }
        else
        {
            curIndicator.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        Move();
        SwitchHoldBolw();
    }

    void SwitchHoldBolw()
    {
        if (isHoldFoodSet)
        {
            animator.SetFloat("isHoldBowl", 1);
        }
        else
        {
            animator.SetFloat("isHoldBowl", 0);
        }
    }

    void Move()
    {
        if (!isCanCtrl)
            return;

        if (Mathf.Abs(input.hor) > 1e-6 || Mathf.Abs(input.ver) > 1e-6)
        {
            transform.position += new Vector3(input.hor * speed * Time.deltaTime, 0, input.ver * speed * Time.deltaTime);
            transform.LookAt(transform.position + Vector3.right * input.hor + Vector3.forward * input.ver);

            animator.SetFloat("walk", Mathf.Sqrt(input.hor * input.hor + input.ver * input.ver));
        }
    }

    public void Hide()
    {
        animator.SetFloat("walk", 0);
        model.SetActive(false);
    }

    public void Show()
    {
        model.SetActive(true);
    }

    //IContainer Implement
    public void AddToContents(Container content)
    {
        if (Contents.Count >= 1)
        {
            Debug.LogError("Try to add a content when the player have already held one");
            return;
        }

        Contents.Add(content);
        isHoldFoodSet = true;
        content.transform.position = holdFoodMarkTrans.position;
        content.transform.SetParent(transform.Find("Model/metarig.001").transform);
    }

    public Container TakeTheOneTo(IContainable<Container> container)
    {
        if (Contents.Count <= 0)
        {
            Debug.Log("Nothing in player's hand");
            return null;
        }

        isHoldFoodSet = false;
        Container movedContainer = Contents[0];
        container.AddToContents(movedContainer);
        Contents.RemoveAt(0);
        return movedContainer;
    }

    public Container TakeOneTo(Container content, IContainable<Container> container)
    {
        throw new System.NotImplementedException();
    }

    public void AddRangeToContents(List<Container> contents, Vector3 posOffset)
    {
        throw new System.NotImplementedException();
    }
    public List<Container> TakeOutAllTo(IContainable<Container> container)
    {
        throw new System.NotImplementedException();
    }
}
