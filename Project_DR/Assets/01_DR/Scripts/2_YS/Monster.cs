using BNG;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;


public class Monster : MonoBehaviour
{
    //스턴 추가 - hit상태
    public enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        STUN,
        DIE
    }

    public State state = State.IDLE;


    public enum Type
    {
        HUMAN_ROBOT,
        HUMAN_GOLEM,
        BEAST_SPIDER,
        BEAST_STING,
        SIMPLE_FUNGI,
        SIMPLE_SPOOK
    }
    public Type monsterType = Type.HUMAN_ROBOT;

    [Header("몬스터 원거리 관련")]
    public Transform bulletport;
    public GameObject monsterBullet;

    [Header("몬스터 테이블")]
    public float hp = default;       //체력이랑 damageble 보내준다
    public float attack = default;
    public float attDelay = default;   //공격간격
    public float speed = default;
    public float recRange = 30.0f;   //pc 인식범위
    public float attRange = 2.0f;   //pc 공격범위
    public float stunDelay = 1f;

    [Header("트랜스폼")]
    public Transform monsterTr;
    public Transform playerTr;

    [Header("몬스터 컴포넌트")]
    public Animator anim;
    public Rigidbody rigid;
    public NavMeshAgent nav;

    Damageable damageable;

    public readonly int hashRun = Animator.StringToHash("isRun");

    public readonly int hashWalkingAttack = Animator.StringToHash("isWalkingAttack");
    
    public readonly int hashAttack = Animator.StringToHash("isAttack");
    public readonly int hashAttack2 = Animator.StringToHash("isAttack2");
    public readonly int hashAttack3 = Animator.StringToHash("isAttack3");
    public readonly int hashAttack4 = Animator.StringToHash("isAttack4");

    public readonly int hashAttackRuning = Animator.StringToHash("isAttackRuning");
    public readonly int hashAttackRuning2 = Animator.StringToHash("isAttackRuning2");
    public readonly int hashAttackRuning3 = Animator.StringToHash("isAttackRuning3");

    public readonly int hashHit = Animator.StringToHash("isDamage");

    public readonly int hashDie = Animator.StringToHash("isDie");

    public readonly int hashidle = Animator.StringToHash("isIdle");

    public bool isDie = false;
    public bool isStun = false;

    IEnumerator stunRoutine; // 스턴 루틴

    // Start is called before the first frame update
    void Awake()
    {
        //GetData();


    }
    private void Start()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<PlayerPosition>().playerPos;

        damageable = GetComponent<Damageable>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        InitMonster();
    }

    public void InitMonster()
    {
        isDie = false;
        nav.isStopped = false;
        StartCoroutine(MonsterState());
        StartCoroutine(MonsterAction());
    }


    void FixedUpdate()
    {
        if (state != State.DIE)
        {
            // Look At Y 각도로만 기울어지게 하기
            Vector3 targetPostition = 
                new Vector3(playerTr.position.x,this.transform.position.y, playerTr.position.z);
            this.transform.LookAt(targetPostition);
            //transform.LookAt(playerTr.position);
        }

       
    }

    //public void GetData()
    //{
    //    damageable.Health = (float)DataManager.GetData(7001, "HP"); //이름으로 가져오는거라서 순서상관 X 0번째 행  //변수 선언은 해야함
    //    speed = (float)DataManager.GetData(3001, "MoveSpeed");
    //    coolTime = (float)DataManager.GetData(3001, "Cooltime");
    //    //traceDist = (float)DataManager.GetData(3001, "CheckRange");

    //    speedReduce = (float)DataManager.GetData(7041, "Speed_Reduce");
    //}

  
    // 스테이트를 관리하는 코루틴
    // 역할 : 스테이트를 변환만 해준다. 다른건 없음.
    IEnumerator MonsterState()
    {
        while (true)
        {
            // 체력이 0 이하면 죽은 상태로 전이
            if (damageable.Health <= 0)
                state = State.DIE;

            // 스턴을 맞을 경우 스턴 상태로 전이
            else if(isStun)
                state = State.STUN;

            else
            {
                float distance = Vector3.Distance(playerTr.position, monsterTr.position);

                if (distance <= attRange)
                {
                    state = State.ATTACK;
                }
                else if (distance <= recRange)
                {
                    state = State.TRACE;
                }
                else
                {
                    state = State.IDLE;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    // 몬스터의 상태에 따라 전이되는 액션
    IEnumerator MonsterAction()
    {
        while(true)
        { 
        switch (state)
        {
            // IDLE 상태 =======================================================
            case State.IDLE:
                //Debug.Log("IDLE state");
                nav.isStopped = true;
                anim.SetBool(hashRun, false);
                anim.SetBool(hashidle, true);
                //anim.SetBool(hashWalkingAttack, false);
                anim.SetBool(hashAttack, false);
                anim.SetBool(hashAttack2, false);
                anim.SetBool(hashAttack3, false);
                anim.SetBool(hashAttack4, false);
                break;

            // TRACE 상태 =======================================================
            case State.TRACE:
                //Debug.Log("TRACE state");
                nav.isStopped = false;
                nav.SetDestination(playerTr.position);
                anim.SetBool(hashRun, true);
                anim.SetBool(hashWalkingAttack, false);
                anim.SetBool(hashAttackRuning, true);
                anim.SetBool(hashAttackRuning2, true);
                anim.SetBool(hashAttackRuning3, true);
                break;

            // ATTACK 상태 =======================================================
            case State.ATTACK:
                    
                //Debug.Log("ATTACK state");

                switch (monsterType)
                {
                    case Type.HUMAN_ROBOT:

                        anim.SetBool(hashWalkingAttack, true);
                        anim.SetBool(hashAttack, true);
                        yield return new WaitForSeconds(0.5f);
                        anim.SetBool(hashidle, true);
                        anim.SetBool(hashAttack, false);
                        anim.SetBool(hashRun, false);
                        yield return new WaitForSeconds(0.3f);
                        break;

                    case Type.HUMAN_GOLEM:

                        int humanGolem = Random.Range(0, 3);

                        switch (humanGolem)
                        {
                            case 0:
                                anim.SetBool(hashWalkingAttack, true);
                                anim.SetBool(hashAttack, true);
                                yield return new WaitForSeconds(1.3f);
                                anim.SetBool(hashidle, true);
                                anim.SetBool(hashAttack, false);
                                anim.SetBool(hashWalkingAttack, false);
                                anim.SetBool(hashRun, false);
                                yield return new WaitForSeconds(0.3f);
                                break;

                            case 1:
                                //anim.SetBool(hashWalkingAttack, true);
                                anim.SetBool(hashAttack2, true);
                                    
                                yield return new WaitForSeconds(1.3f);
                                anim.SetBool(hashidle, true);
                                anim.SetBool(hashAttack2, false);
                                //anim.SetBool(hashWalkingAttack, false);
                                anim.SetBool(hashRun, false);
                                yield return new WaitForSeconds(0.3f);
                                break;

                            case 2:
                                //anim.SetBool(hashWalkingAttack, true);
                                anim.SetBool(hashAttack3, true);
                                yield return new WaitForSeconds(1.2f);
                                anim.SetBool(hashidle, true);
                                anim.SetBool(hashAttack3, false);
                                //anim.SetBool(hashWalkingAttack, false);
                                anim.SetBool(hashRun, false);
                                yield return new WaitForSeconds(0.3f);
                                break;

                        }
                        break;

                    case Type.BEAST_SPIDER:

                        int spider = Random.Range(0, 3);

                        switch (spider)
                        {
                            case 0:
                                anim.SetBool(hashWalkingAttack, true);
                                anim.SetBool(hashAttack, true);
                                yield return new WaitForSeconds(0.8f);
                                anim.SetBool(hashidle, true);
                                anim.SetBool(hashAttack, false);
                                //anim.SetBool(hashWalkingAttack, false);
                                anim.SetBool(hashRun, false);
                                yield return new WaitForSeconds(0.3f);
                                break;

                            case 1:
                                anim.SetBool(hashAttack2, true);
                                yield return new WaitForSeconds(0.8f);
                                anim.SetBool(hashidle, true);
                                anim.SetBool(hashAttack2, false);
                                //anim.SetBool(hashWalkingAttack, false);
                                anim.SetBool(hashRun, false);
                                yield return new WaitForSeconds(0.3f);
                                break;

                            case 2:
                                anim.SetBool(hashAttack3, true);
                                yield return new WaitForSeconds(0.8f);
                                anim.SetBool(hashidle, true);
                                anim.SetBool(hashAttack3, false);
                                //anim.SetBool(hashWalkingAttack, false);
                                anim.SetBool(hashRun, false);
                                yield return new WaitForSeconds(0.3f);
                                break;

                        }
                        break;

                    case Type.BEAST_STING:

                        int sting = Random.Range(0, 3);

                        switch (sting)
                        {
                            case 0:
                                anim.SetBool(hashWalkingAttack, true);
                                anim.SetBool(hashAttack, true);
                                yield return new WaitForSeconds(0.8f);
                                anim.SetBool(hashidle, true);
                                anim.SetBool(hashAttack, false);
                                //anim.SetBool(hashWalkingAttack, false);
                                anim.SetBool(hashRun, false);
                                yield return new WaitForSeconds(0.3f);
                                break;

                            case 1:
                                anim.SetBool(hashAttack2, true);
                                yield return new WaitForSeconds(0.8f);
                                anim.SetBool(hashidle, true);
                                anim.SetBool(hashAttack2, false);
                                //anim.SetBool(hashWalkingAttack, false);
                                anim.SetBool(hashRun, false);
                                yield return new WaitForSeconds(0.3f);
                                break;

                            case 2:
                                anim.SetBool(hashAttack3, true);
                                yield return new WaitForSeconds(0.8f);
                                anim.SetBool(hashidle, true);
                                anim.SetBool(hashAttack3, false);
                                //anim.SetBool(hashWalkingAttack, false);
                                anim.SetBool(hashRun, false);
                                yield return new WaitForSeconds(0.3f);
                                break;
                            //case 3:
                            //    anim.SetBool(hashAttack4, true);
                            //    yield return new WaitForSeconds(0.8f);
                            //    anim.SetBool(hashidle, true);
                            //    anim.SetBool(hashAttack4, false);
                            //    //anim.SetBool(hashWalkingAttack, false);
                            //    anim.SetBool(hashRun, false);
                            //    yield return new WaitForSeconds(0.3f);
                            //    break;
                        }
                        break;

                    case Type.SIMPLE_FUNGI:

                        int fungi = Random.Range(0, 2);

                        switch (fungi)
                        {
                            case 0:
                                anim.SetBool(hashWalkingAttack, true);
                                anim.SetBool(hashAttack, true);
                                yield return new WaitForSeconds(0.2f);
                                anim.SetBool(hashidle, true);
                                anim.SetBool(hashAttack, false);
                                //anim.SetBool(hashWalkingAttack, false);
                                anim.SetBool(hashRun, false);
                                yield return new WaitForSeconds(0.3f);
                                break;

                            case 1:
                                anim.SetBool(hashAttack2, true);
                                yield return new WaitForSeconds(0.2f);
                                anim.SetBool(hashidle, true);
                                anim.SetBool(hashAttack2, false);
                                //anim.SetBool(hashWalkingAttack, false);
                                anim.SetBool(hashRun, false);
                                yield return new WaitForSeconds(0.3f);
                                break;
                        }
                        break;

                    case Type.SIMPLE_SPOOK:

                        int spook = Random.Range(0, 3);

                        switch (spook)
                        {
                            case 0:
                                anim.SetBool(hashWalkingAttack, true);
                                anim.SetBool(hashAttack, true);
                                yield return new WaitForSeconds(0.7f);
                                anim.SetBool(hashidle, true);
                                anim.SetBool(hashAttack, false);
                                //anim.SetBool(hashWalkingAttack, false);
                                anim.SetBool(hashRun, false);
                                yield return new WaitForSeconds(0.3f);
                                break;

                            case 1:
                                    
                                anim.SetBool(hashAttack2, true);
                                yield return new WaitForSeconds(0.7f);
                                anim.SetBool(hashidle, true);
                                anim.SetBool(hashAttack2, false);
                                //anim.SetBool(hashWalkingAttack, false);
                                anim.SetBool(hashRun, false);
                                yield return new WaitForSeconds(0.3f);
                                break;

                            case 2:
                                anim.SetBool(hashAttack3, true);
                                yield return new WaitForSeconds(0.7f);
                                anim.SetBool(hashidle, true);
                                anim.SetBool(hashAttack3, false);
                                //anim.SetBool(hashWalkingAttack, false);
                                anim.SetBool(hashRun, false);
                                yield return new WaitForSeconds(0.3f);
                                break;
                        }
                        break;
                }
                break;

                    
            case State.DIE:
                isDie = true;
                nav.isStopped = true;
                //Debug.Log("nav.isStopped: " + nav.isStopped);
                anim.SetTrigger(hashDie);

                //Destroy(this.gameObject, 1.3f); //damageable 쪽에서 처리
                yield break;
            }
        yield return new WaitForSeconds(0.1f);
        }

        
    }

    public void OnDeal()
    {
        if (isStun)
            return;
        if (state != State.DIE && state != State.STUN)
        {
            if (damageable.Health >= 0)
            {
                // 만약에 스턴루틴에 이미 다른 코루틴이 실행중인 경우
                if(stunRoutine != null)
                {
                    StopCoroutine(stunRoutine);
                    stunRoutine = null;
                }

                stunRoutine = StunDelay();
                StartCoroutine(stunRoutine);

                Debug.Log($"state:{state}");
            }
        }
        Debug.Log($"hp:{damageable.Health}");
    }
    // 스턴 딜레이
    IEnumerator StunDelay()
    {
        isStun = true;
        anim.SetTrigger(hashHit);

        yield return new WaitForSeconds(stunDelay);
        isStun = false;
        yield break;
    }

    //IEnumerator OnStun()
    //{
    //    isStun = true;

    //    anim.SetTrigger(hashHit);
    //    yield return new WaitForSeconds(0.5f);
        
    //    isStun = false;
    //    Debug.Log($"isStun:{isStun}");

    //    //넘어가는 로직이 필요한데...
    //}

   



    void OnDrawGizmos()
    {
        if (state == State.TRACE)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, recRange);
        }

        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attRange);
        }

    }

    

    //int GetAttackHash(int attackIndex)
    //{
    //    switch (attackIndex)
    //    {
    //        case 0: return hashAttack;
    //        case 1: return hashAttack2;
    //        case 2: return hashAttack3;
    //        case 3: return hashAttack4;
    //        default: return hashAttack;
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.CompareTag("test"))
    //    {
    //        RaycastWeaponDrill weaponDrill = other.GetComponent<RaycastWeaponDrill>();
    //        OnDamage(weaponDrill.Damage);
    //    }
    //}

    //public void OnDamage(float Damage)
    //{
    //    if (!isDie)
    //    {
    //        RaycastWeaponDrill weaponDrill = GetComponent<RaycastWeaponDrill>();
    //        hp -= Damage;
    //        anim.SetTrigger(hashHit);


    //        if (hp <= 0)
    //        {          

    //            state = State.DIE;
    //            //Debug.Log($"state:{state}");

    //        }
    //    }
    //    Debug.Log($"hp:{hp}");
    //}

}



