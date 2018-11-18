﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagement : MonoBehaviour {

    public Texture2D cursorTexture_green;
    public Texture2D cursorTexture_red;

    public string id;
    public string nickname;
    public int RoomNo;
    public float scdis;//可选中玩家的范围约束
    [HideInInspector] public Transform m_Targets;
    [HideInInspector] public GameObject con_obj=null;
    public Vector3[] SpawnPosition;
    private int deltaframe = 0;//计帧数
    private GameObject maincamera;
    private GameObject manu;
    private GameObject chatitem;
    private GameObject role;
    private GameObject sight;
    private GameObject settings;
    private GameObject association;
    private GameObject information;
    private RaycastHit hit;
    private string q = "q";
    // Use this for initialization
    void Start () {
        maincamera = GameObject.Find("Main Camera");
        manu = GameObject.Find("Manu");
        chatitem = GameObject.Find("ChatItem");
        deltaframe = 0;
        con_obj = GameObject.Find("StateObject");
        //m_Targets = GameObject.Find("role").transform;
        //role = GameObject.Find("role");
        settings = GameObject.Find("Setting_Call");
        //association = GameObject.Find("Association_Call");
        information = GameObject.Find("Information UI");
        sight = GameObject.Find("Sight_bead_UI");
        sight.GetComponent<Sightcontrol>().SetSight(0);
        //GameObject.Find("Login UI").GetComponent<LoginControl>().Show();
        //GameObject.Find("Register UI").GetComponent<LoginControl>().Hide();
        //GameObject.Find("Main Camera").GetComponent<Camera>().cullingMask = 1 << 5;
        //显示登录界面（选择房间号成功之后锁定鼠标，enablemousemove，enablemanucontrol，map.show(),enablemovecontrol）还需要做的准备：隐藏room1(调整摄像机显示层)
        EnterRoom(1,"起始之城");
    }
    
	// Update is called once per frame
	void Update () {
        if (Cursor.lockState == CursorLockMode.Locked && Cursor.visible == false)
        {
            sight.GetComponent<Sightcontrol>().Show();
        }
        else
        {
            sight.GetComponent<Sightcontrol>().Hide();
        }
        if (con_obj != null /*&& con_obj.GetComponent<StateObject>().is_pos_connected()*/)//性能跟不上可考虑在这里每隔2帧加载一次玩家
        {
            if (Input.GetButtonDown(q))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    maincamera.GetComponent<CameraControl>().DisableAngleControl();
                    maincamera.GetComponent<CameraControl>().DisableTurnControl();
                    //role.GetComponent<movement>().DisableTurnControl();
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    GameObject gameObject = GameObject.Find("Main Camera");
                    gameObject.GetComponent<CameraControl>().EnableAngleControl();
                    gameObject.GetComponent<CameraControl>().EnableTurnControl();
                    //role.GetComponent<movement>().EnableTurnControl();
                }
            }
            //接下来屏幕中心发射线，一定距离内击中玩家则可与玩家交互
            //记得将屏幕中心稍微偏离本玩家，方便与其他玩家交互
            Ray rayOrigin = maincamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            scdis = maincamera.GetComponent<CameraControl>().m_CurDis + 30f;
            if (!manu.GetComponent<CanvasGroup>().interactable && !chatitem.GetComponent<CanvasGroup>().interactable && !settings.GetComponent<CanvasGroup>().interactable /*&& !association.GetComponent<CanvasGroup>().interactable && !information.GetComponent<CanvasGroup>().interactable*/)//当菜单，聊天框，设置，好友列表，个人资料页未唤出时方可选中玩家，记得菜单唤回同时关闭由菜单打开的UI
            {
                int layerMask = 1 << 1;//只检测与第一层物体（玩家层）的碰撞
                if (Physics.Raycast(rayOrigin, out hit, scdis, layerMask)) 
                {
                    if (hit.collider.tag == "Player" && hit.collider.name != "role")
                    {
                        if (Cursor.lockState == CursorLockMode.Locked && Cursor.visible == false)//准星变型
                        {
                            sight.GetComponent<Sightcontrol>().SetSight(1);
                        }
                        Cursor.SetCursor(cursorTexture_green, Vector2.zero, CursorMode.Auto);//设置鼠标样式//恢复默认样式;
                        //显示交互选项UI,并将选中玩家id传入方便交互,考虑做玩家信息UI，当玩家远离的时候自动关闭交互UI

                    }
                    else
                    {
                        if ( Cursor.visible)
                        {
                            if (Cursor.lockState == CursorLockMode.Locked)
                            {
                                Cursor.visible = false;
                            }
                            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);//设回默认鼠标样式
                        }
                        if (Cursor.lockState == CursorLockMode.Locked && Cursor.visible == false)//准星恢复
                        {
                            sight.GetComponent<Sightcontrol>().SetSight(0);
                        }
                    }
                }
                else
                {
                    if (Cursor.visible)
                    {
                        if (Cursor.lockState == CursorLockMode.Locked)
                        {
                            Cursor.visible = false;
                        }
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);//设回默认鼠标样式
                    }
                    if (Cursor.lockState == CursorLockMode.Locked && Cursor.visible == false)//准星恢复
                    {
                        sight.GetComponent<Sightcontrol>().SetSight(0);
                    }
                }
            }
            
            //con_obj.GetComponent<StateObject>().socket(id, nickname, m_Targets.position.x, m_Targets.position.y, m_Targets.position.z, m_Targets.forward.x, m_Targets.forward.z, RoomNo);
            //con_obj.GetComponent<StateObject>().pullmsg(RoomNo,id);//根据玩家当前位置拉取可接收到的信息
        }
        /*if (con_obj != null && con_obj.GetComponent<StateObject>().is_msg_connected()) //每隔10帧拉一次消息
        {
            //con_obj.GetComponent<StateObject>().pullmsg(RoomNo, id);
            deltaframe++;
            if (deltaframe >= 10) {
                deltaframe = 0;
                con_obj.GetComponent<StateObject>().sendmsg(5, m_Targets.position.x, m_Targets.position.y, m_Targets.position.z, RoomNo, id, nickname, 1, null);
            }
            
        }*/
        //只要处在连接中就持续显示其他玩家，同时在此接收消息以及上下线信息,有人上线则创建一个新的对象，有人下线（退出房间/登出）则销毁该对象
        
    }

    /*public void LoginOK(string username,string nickname)
    {
        GameObject.Find("Login UI").GetComponent<LoginControl>().Hide();
        id = username;
        this.nickname = nickname;
        GameObject.Find("Selectroom UI").GetComponent<SelectroomControl>().Show();
    }*/

    public void EnterRoom(int number , string RoomName)
    {
        RoomNo = number;
        //GameObject.Find("Selectroom UI").GetComponent<SelectroomControl>().Hide();
        GameObject.Find("RoomNo").GetComponent<Text>().text = RoomName;
        LoadScene();
    }
    public void LoadScene()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (RoomNo <= SpawnPosition.Length)
        {
            GameObject.Find("Main Camera").transform.position = SpawnPosition[RoomNo - 1];
        }
        //m_Targets.position = new Vector3(0f, 0.5f, 0f);
        GameObject.Find("BGM").GetComponent<BGMControl>().LoadBGM(RoomNo-1);
        GameObject.Find("BGM").GetComponent<AudioSource>().Play();
        GameObject.Find("Main Camera").GetComponent<Camera>().cullingMask = -1;
        GameObject.Find("Main Camera").GetComponent<Camera>().cullingMask &= ~(1 << 10);//关闭圆点层
        GameObject.Find("Manu").GetComponent<ManuControl>().EnableMouseMove();
        GameObject.Find("Manu").GetComponent<ManuControl>().EnableManuControl();
        GameObject.Find("Map").GetComponent<MapControl>().Show();
        //GameObject.Find("role").GetComponent<movement>().EnableMoveControl();
        //GameObject.Find("role").GetComponent<movement>().setup("role", nickname);
        //GameObject.Find("Chat UI").GetComponent<ChatControl>().HalfShow();
        //启动连接并第一次加载目前的其他玩家
        //first_pull();
        con_obj = GameObject.Find("StateObject");
    }

    /*public void first_pull()//首次拉取当前房间所有玩家信息
    {
        WWWForm form = new WWWForm();
        form.AddField("roomno", RoomNo);
        form.AddField("id", id);
        StartCoroutine(GameObject.Find("StateObject").GetComponent<StateObject>().pull_player("http://yangyuqing.vipgz1.idcfengye.com/chat_room/pull.php", form));
        if (GameObject.Find("StateObject").GetComponent<StateObject>().begin_connect())
        {
            WWWForm form1 = new WWWForm();
            form1.AddField("roomno", RoomNo);
            form1.AddField("username", id);
            form1.AddField("nickname", nickname);
            form1.AddField("channel", 2);
            form1.AddField("content", nickname + "上线了");
            StartCoroutine(GameObject.Find("StateObject").GetComponent<StateObject>().go_online("http://yangyuqing.vipgz1.idcfengye.com/chat_room/go_online.php", form1));
        }
        else {
            GameObject.Find("quit room").GetComponent<Btn_Quitroom>().quitroom();
        }
        //向后台发送数据
        //下线信息交给后台自动产生,上线信息在前台发送，通过http协议保证上线信息发送到服务器，若上线信息发送失败则强制下线
    }*/
}
