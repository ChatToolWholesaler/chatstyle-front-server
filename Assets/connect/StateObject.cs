using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System;
using Net;



public class StateObject : MonoBehaviour {

    public string ip = "vipgz1.idcfengye.com";
    public int pos_port = 10134;
    public int msg_port = 10135;
    //public ClientSocket pos_Socket;
    //public ClientSocket msg_Socket;


    /*void Awake()
    {
        pos_Socket = new ClientSocket();
        msg_Socket = new ClientSocket();
    }

    public IEnumerator login(string _url, WWWForm _wForm)
    {
        WWW postData = new WWW(_url, _wForm);
        yield return postData;
        if (postData.error != null)
        {
            Debug.Log(postData.error);
        }
        else
        {
            Debug.Log(postData.text);
            LoginModel obj = JsonUtility.FromJson<LoginModel> (postData.text);
            if (obj.result == 0)
            {
                GameObject.Find("HintMessage").GetComponent<hint>().Hint("登陆成功！");
                //GameObject.Find("GameManagement").GetComponent<GameManagement>().LoginOK(obj.username, obj.nickname);
            }
            else if (obj.result == 3)
            {
                GameObject.Find("ErrorText").GetComponent<Text>().text = "该账号已登录";
            }
            else if (obj.result == 1)
            {
                GameObject.Find("ErrorText").GetComponent<Text>().text = "账号不存在或者密码不匹配";
            }
            else {
                Debug.Log(obj.username);
                Debug.Log(obj.nickname);
                GameObject.Find("ErrorText").GetComponent<Text>().text = "登陆错误";
            }
        }
    }
    public IEnumerator register(string _url, WWWForm _wForm)
    {
        WWW postData = new WWW(_url, _wForm);
        yield return postData;
        if (postData.error != null)
        {
            Debug.Log(postData.error);
        }
        else
        {
            RegisterModel obj = JsonUtility.FromJson<RegisterModel>(postData.text);
            if (obj.result == 0)
            {
                GameObject.Find("HintMessage").GetComponent<hint>().Hint("注册成功！");
                GameObject.Find("Register UI").GetComponent<LoginControl>().Hide();
                GameObject.Find("Login UI").GetComponent<LoginControl>().Show();
            }
            else
            {
                if (obj.result == 1)
                {
                    GameObject.Find("ErrorText_r").GetComponent<Text>().text = "账号或者密码已存在";
                }
                else if (obj.result == 3)
                {
                    GameObject.Find("ErrorText_r").GetComponent<Text>().text = "昵称已存在";
                }
                else {
                    Debug.Log(obj);
                }
            }

            Debug.Log(postData.text);
        }
    }

    public IEnumerator go_online(string _url, WWWForm _wForm)
    {
        WWW postData = new WWW(_url, _wForm);
        yield return postData;
        if (postData.error != null)
        {
            Debug.Log(postData.error);
        }
        else
        {
            RegisterModel obj = JsonUtility.FromJson<RegisterModel>(postData.text);
            if (obj.result == 1)
            {
                GameObject.Find("HintMessage").GetComponent<hint>().Hint("欢迎来到聊吧");
            }
            else
            {
                if (obj.result == 0)
                {
                    GameObject.Find("HintMessage").GetComponent<hint>().Hint("连接服务器失败！");//插入数据库失败
                    GameObject.Find("quit room").GetComponent<Btn_Quitroom>().quitroom();
                }
                else
                {
                    Debug.Log(obj);
                }
            }

            Debug.Log(postData.text);
        }
    }

    public IEnumerator pull_player(string _url, WWWForm _wForm)
    {
        WWW postData = new WWW(_url, _wForm);
        yield return postData;
        if (postData.error != null)
        {
            Debug.Log(postData.error);
        }
        else
        {
            //PullModel obj = JsonUtility.FromJson<PullModel>(postData.text);
            //对拉回的本房间玩家信息各自克隆玩家
            first_paint(postData.text);
        }
    }

    public void socket(string id,string nickname, float position_x, float position_y, float position_z, float forward_x, float forward_z, int roomno)
    {
        SocketModel obj = new SocketModel();
        obj.id = id;
        obj.nickname = nickname;
        obj.position_x = position_x;
        obj.position_y = position_y;
        obj.position_z = position_z;
        obj.forward_x = forward_x;
        obj.forward_z = forward_z;
        obj.roomno = roomno;
        string json = JsonUtility.ToJson(obj);
        //Debug.Log(json);
        pos_Socket.AsyncSendData(json,1);
        //AsyncCallback callback = new AsyncCallback(paint);
    }

    public void first_paint(string data)//用于首次加载角色
    {
        PullModel obj = JsonUtility.FromJson<PullModel>(data);
        if (obj.socketmodel.Length > 0) {
            foreach (SocketModel item in obj.socketmodel)
            {
                GameObject roleInstance = Instantiate(GameObject.Find("role"), new Vector3(item.position_x, item.position_y, item.position_z), Quaternion.Euler(0f, 0f, 0f));
                roleInstance.transform.LookAt(roleInstance.transform.position + new Vector3(item.forward_x, 0, item.forward_z));
                roleInstance.GetComponent<movement>().setup(item.id, item.nickname);
            }
        }
    }

    public void paint(string data)//用于线上角色绘制回调
    {
        try
        {
            PullModel obj = JsonUtility.FromJson<PullModel>(data);
            if (obj.socketmodel.Length > 0)
            {
                foreach (SocketModel item in obj.socketmodel)
                {
                    GameObject.Find(item.id).GetComponent<Rigidbody>().MovePosition(new Vector3(item.position_x, item.position_y, item.position_z));
                    GameObject.Find(item.id).transform.LookAt(GameObject.Find(item.id).transform.position + new Vector3(item.forward_x, 0, item.forward_z));
                }
            }
        }
        catch { return; }
    }

    public void pullmsg(int no ,string id)//不发消息，只收上下线消息
    {
        MsgModel msgmodel = new MsgModel();
        msgmodel.type = 4;
        msgmodel.position_x = 0;
        msgmodel.position_y = 0;
        msgmodel.position_z = 0;
        msgmodel.roomno = no;
        msgmodel.username = id;
        msgmodel.nickname = null;
        msgmodel.channel = 0;
        msgmodel.content = null;
        string json = JsonUtility.ToJson(msgmodel);
        msg_Socket.AsyncSendData(json,2);
        //Callback_MsgModel result = JsonUtility.FromJson<Callback_MsgModel>(res);
    }
    public void pull_back(string data)//用于上下线信息回调
    {
        try
        {
            Callback_MsgModel obj = JsonUtility.FromJson<Callback_MsgModel>(data);
            if (obj.msgmodel.Length > 0)
            {
                foreach (MsgModel item in obj.msgmodel)
                {

                    if (item.type != 1 && item.type != 2 && item.type != 3) 
                    {
                        Debug.Log("信息类型错误！ 错误type："+ item.type);
                        continue;
                    }
                    if (item.type == 3)
                    {
                        GameObject.Find("Chat UI").GetComponent<ChatControl>().AddContent(item.type, item.position_x, item.position_y, item.position_z, item.roomno, item.username, item.nickname, item.channel, item.content);
                        continue;
                    }
                    if (item.username == GameObject.Find("GameManagement").GetComponent<GameManagement>().id)//避免绘制自己
                    {
                        continue;
                    }
                    if (item.type == 1 && item.roomno == GameObject.Find("GameManagement").GetComponent<GameManagement>().RoomNo)
                    {
                        GameObject roleInstance = Instantiate(GameObject.Find("role"), new Vector3(item.position_x, item.position_y, item.position_z), Quaternion.Euler(0f, 0f, 0f));
                        roleInstance.GetComponent<movement>().setup(item.username, item.nickname);
                        GameObject.Find("Chat UI").GetComponent<ChatControl>().AddContent(item.type, item.position_x, item.position_y, item.position_z, item.roomno, item.username, item.nickname, item.channel, item.content);
                    }
                    if (item.type == 2 && item.roomno == GameObject.Find("GameManagement").GetComponent<GameManagement>().RoomNo)
                    {
                        Destroy(GameObject.Find(item.username));
                        GameObject.Find("Chat UI").GetComponent<ChatControl>().AddContent(item.type, item.position_x, item.position_y, item.position_z, item.roomno, item.username, item.nickname, item.channel, item.content);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(data);
            return;
        }
    }

    public void sendmsg(int type, float position_x, float position_y, float position_z, int roomno, string id, string nickname, int channel, string content)//发各种消息，收玩家发的消息
    {
        MsgModel msgmodel = new MsgModel();
        msgmodel.type = type;
        msgmodel.position_x = position_x;
        msgmodel.position_y = position_y;
        msgmodel.position_z = position_z;
        msgmodel.roomno = roomno;
        msgmodel.username = id;
        msgmodel.nickname = nickname;
        msgmodel.channel = channel;
        msgmodel.content = content;
        string json = JsonUtility.ToJson(msgmodel);
        msg_Socket.AsyncSendData(json,3);
        //Callback_MsgModel result = JsonUtility.FromJson<Callback_MsgModel>(res);
        //将消息显示在聊天框中
    }
    public void send_back(string data)//用于玩家发送的消息回调
    {
        try
        {
            Callback_MsgModel obj = JsonUtility.FromJson<Callback_MsgModel>(data);
            if (obj.msgmodel.Length > 0)
            {
                foreach (MsgModel item in obj.msgmodel)
                {
                    if (item.type != 1 && item.type != 2 && item.type != 3)
                    {
                        Debug.Log("信息类型错误！ 错误type：" + item.type);
                        continue;
                    }
                    if (item.type == 3)
                    {
                        GameObject.Find("Chat UI").GetComponent<ChatControl>().AddContent(item.type, item.position_x, item.position_y, item.position_z, item.roomno, item.username, item.nickname, item.channel, item.content);
                        continue;
                    }
                    if (item.username == GameObject.Find("GameManagement").GetComponent<GameManagement>().id)//避免绘制自己
                    {
                        continue;
                    }
                    if (item.type == 1 && item.roomno == GameObject.Find("GameManagement").GetComponent<GameManagement>().RoomNo)
                    {
                        GameObject roleInstance = Instantiate(GameObject.Find("role"), new Vector3(item.position_x, item.position_y, item.position_z), Quaternion.Euler(0f, 0f, 0f));
                        roleInstance.GetComponent<movement>().setup(item.username, item.nickname);
                        GameObject.Find("Chat UI").GetComponent<ChatControl>().AddContent(item.type, item.position_x, item.position_y, item.position_z, item.roomno, item.username, item.nickname, item.channel, item.content);
                    }
                    if (item.type == 2 && item.roomno == GameObject.Find("GameManagement").GetComponent<GameManagement>().RoomNo)
                    {
                        Destroy(GameObject.Find(item.username));
                        GameObject.Find("Chat UI").GetComponent<ChatControl>().AddContent(item.type, item.position_x, item.position_y, item.position_z, item.roomno, item.username, item.nickname, item.channel, item.content);
                    }
                }
            }
            
            //将消息显示在聊天框中
        }
        catch(Exception e) {
            Debug.Log(e.Message);
            Debug.Log(data);
            return;
        }
    }

    public bool is_pos_connected()
    {
        return pos_Socket.is_connected();
    }

    public bool is_msg_connected()
    {
        return msg_Socket.is_connected();
    }

    public bool begin_connect()
    {
        //pos_Socket.ConnectServer(ip, pos_port);//消息系统与游戏系统使用不同的端口
       // msg_Socket.ConnectServer(ip, msg_port);
        return (pos_Socket.ConnectServer(ip, pos_port) && msg_Socket.ConnectServer(ip, msg_port));
    }
    public void close_connect()
    {
        pos_Socket.CloseServer();
        msg_Socket.CloseServer();
    }*/
    private float time = 0f;
    public PosSeverSocket pos_socket;
    public MsgSeverSocket msg_socket;
    public Hashtable[] clientsockets;//键为msgsocket，值为possocket
    public Dictionary<Socket,List<GameObject>> postoplayers;//键为possocket，值为GameObject泛型
    public int RoomCount;
    public GameObject prefabs = null;
    public GameObject GM;

    public IEnumerator go_offline(string _url, WWWForm _wForm, string userid)
    {
        WWW postData = new WWW(_url, _wForm);
        yield return postData;
        if (postData.error != null)
        {
            Debug.Log(postData.error);
        }
        else
        {
            GoOnlineModel obj = JsonUtility.FromJson<GoOnlineModel>(postData.text);
            if (obj.code == 200)
            {
                Debug.Log(userid + "下线成功");
            }
            else
            {
                if (obj.code == 400)
                {
                    Debug.Log(userid + "下线失败！");//插入数据库失败
                }
                else
                {
                    Debug.Log(obj);
                }
            }

            Debug.Log(postData.text);
        }
    }

    public void Pos_Receive(string data, Socket posclientsocket)
    {
        //如果此人已上线，即该线哈希表存在此人的msgsocket，则修改表项键值
        //计划接到nickname不为空(第一次必不为空)则加载一次周围人，客户端刷新周围人列表
        try
        {
            SocketModel obj = JsonUtility.FromJson<SocketModel>(data);
            movement Player;
            PullModel NearPlayers = null;
            bool temp = false;
            GameObject playerobj = null;
            foreach (GameObject P in GameObject.FindGameObjectsWithTag("Player"))//判断是否存在这个玩家
            {
                if (P.name == obj.id)
                {
                    temp = true;
                    playerobj = P;
                    break;
                }
            }
            if (temp)
            {
                int roomno = (int)(obj.r >> 30);
                float position_x = ((float)(obj.p >> 16)) / 32 - 1024 + GM.GetComponent<GameManagement>().SpawnPosition[roomno-1].x;
                float position_y = ((float)((obj.p << 16) >> 16)) / 32 - 1024 + GM.GetComponent<GameManagement>().SpawnPosition[roomno-1].y;
                float position_z = ((float)((obj.r << 2) >> 16)) / 32 - 1024 + GM.GetComponent<GameManagement>().SpawnPosition[roomno-1].z;
                float velocity_x = ((float)(obj.v >> 20)) / 8 - 64;
                float velocity_y = ((float)((obj.v << 12) >> 22)) / 8 - 64;
                float velocity_z = ((float)((obj.v << 22) >> 22)) / 8 - 64; ;
                float forward_x = ((float)((obj.r << 18) >> 25)) / 64 - 1;
                float forward_z = ((float)((obj.r << 25) >> 25)) / 64 - 1;
                Player = playerobj.GetComponent<movement>();
                if (Player.posSocket == null)
                {
                    Player.setpos(posclientsocket);
                    clientsockets[roomno - 1][Player.msgSocket] = posclientsocket;//哈希表赋值
                    postoplayers.Add(posclientsocket,new List<GameObject>());//初始化对应周围玩家列表
                }
                Player.GetComponent<Rigidbody>().velocity = new Vector3(velocity_x, velocity_y, velocity_z);
                Player.GetComponent<Rigidbody>().MovePosition(new Vector3(position_x, position_y, position_z));
                Player.transform.forward = new Vector3(forward_x, 0f, forward_z);
                NearPlayers = new PullModel();

                ArrayList socketmodel = new ArrayList();
                if (obj.nickname != null&& obj.nickname != "")//此时需要加载该玩家周围玩家
                {
                    postoplayers[posclientsocket].Clear();
                    foreach (GameObject P in GameObject.FindGameObjectsWithTag("Player"))
                    {
                        if ((P.transform.position - Player.transform.position).magnitude < 30)//局部加载附近的玩家
                        {
                            Callback_SocketModel tmp = new Callback_SocketModel();
                            tmp.id = P.name;
                            tmp.nickname = P.GetComponent<movement>().nickname;
                            /*tmp.forward_x = P.transform.forward.x;
                            tmp.forward_z = P.transform.forward.z;
                            tmp.position_x = P.transform.position.x;
                            tmp.position_y = P.transform.position.y;
                            tmp.position_z = P.transform.position.z;
                            tmp.velocity_x = P.GetComponent<Rigidbody>().velocity.x;
                            tmp.velocity_y = P.GetComponent<Rigidbody>().velocity.y;
                            tmp.velocity_z = P.GetComponent<Rigidbody>().velocity.z;*/
                            tmp.p = ((uint)((P.transform.position.x - GM.GetComponent<GameManagement>().SpawnPosition[roomno - 1].x + 1024) * 32) << 16) | ((uint)((P.transform.position.y - GM.GetComponent<GameManagement>().SpawnPosition[roomno - 1].y + 1024) * 32));
                            tmp.v = ((uint)((P.GetComponent<Rigidbody>().velocity.x + 64) * 8) << 20) | ((uint)((P.GetComponent<Rigidbody>().velocity.y + 64) * 8) << 10) | ((uint)((P.GetComponent<Rigidbody>().velocity.z + 64) * 8));
                            tmp.r = ((uint)(roomno) << 30) | ((uint)((P.transform.position.z - GM.GetComponent<GameManagement>().SpawnPosition[roomno - 1].z + 1024) * 32) << 14) | ((uint)((P.transform.forward.x + 1) * 64) << 7) | ((uint)((P.transform.forward.z + 1) * 64));
                            socketmodel.Add(tmp);
                            postoplayers[posclientsocket].Add(P);
                        }
                    }
                }
                else//此时不需要加载，只需要提供位置数据
                {
                    foreach (GameObject P in postoplayers[posclientsocket])
                    {
                        if (P!=null)
                        {
                            Callback_SocketModel tmp = new Callback_SocketModel();
                            tmp.id = null;
                            tmp.nickname = null;
                            /*tmp.forward_x = P.transform.forward.x;
                            tmp.forward_z = P.transform.forward.z;
                            tmp.position_x = P.transform.position.x;
                            tmp.position_y = P.transform.position.y;
                            tmp.position_z = P.transform.position.z;
                            tmp.velocity_x = P.GetComponent<Rigidbody>().velocity.x;
                            tmp.velocity_y = P.GetComponent<Rigidbody>().velocity.y;
                            tmp.velocity_z = P.GetComponent<Rigidbody>().velocity.z;*/
                            tmp.p = ((uint)((P.transform.position.x - GM.GetComponent<GameManagement>().SpawnPosition[roomno - 1].x + 1024) * 32) << 16) | ((uint)((P.transform.position.y - GM.GetComponent<GameManagement>().SpawnPosition[roomno - 1].y + 1024) * 32));
                            tmp.v = ((uint)((P.GetComponent<Rigidbody>().velocity.x + 64) * 8) << 20) | ((uint)((P.GetComponent<Rigidbody>().velocity.y + 64) * 8) << 10) | ((uint)((P.GetComponent<Rigidbody>().velocity.z + 64) * 8));
                            tmp.r = ((uint)(roomno) << 30) | ((uint)((P.transform.position.z - GM.GetComponent<GameManagement>().SpawnPosition[roomno - 1].z + 1024) * 32) << 14) | ((uint)((P.transform.forward.x + 1) * 64) << 7) | ((uint)((P.transform.forward.z + 1) * 64));
                            socketmodel.Add(tmp);
                        }
                    }
                }
                NearPlayers.socketmodel = (Callback_SocketModel[])socketmodel.ToArray(typeof(Callback_SocketModel));

                    /*NearPlayers.socketmodel = new Callback_SocketModel[5];
                    Callback_SocketModel tempPlayers = new Callback_SocketModel();
                    int i = -1;
                    foreach (GameObject P in GameObject.FindGameObjectsWithTag("Player"))
                    {
                        if ((P.transform.position - Player.transform.position).magnitude < 30)//局部加载附近的玩家
                        {
                            i++;
                            if (i >= NearPlayers.socketmodel.Length)//以下手写动态放缩数组
                            {
                                Callback_SocketModel[] socketmodel = new Callback_SocketModel[NearPlayers.socketmodel.Length * 5];
                                for (int ii = 0; ii < NearPlayers.socketmodel.Length; ii++)
                                {
                                    socketmodel[i] = NearPlayers.socketmodel[i];
                                }
                                NearPlayers.socketmodel = socketmodel;
                            }
                            NearPlayers.socketmodel[i] = new Callback_SocketModel();
                            NearPlayers.socketmodel[i].id = P.name;
                            NearPlayers.socketmodel[i].nickname = P.GetComponent<movement>().nickname;
                            NearPlayers.socketmodel[i].forward_x = P.transform.forward.x;
                            NearPlayers.socketmodel[i].forward_z = P.transform.forward.z;
                            NearPlayers.socketmodel[i].position_x = P.transform.position.x;
                            NearPlayers.socketmodel[i].position_y = P.transform.position.y;
                            NearPlayers.socketmodel[i].position_z = P.transform.position.z;
                            NearPlayers.socketmodel[i].velocity_x = P.GetComponent<Rigidbody>().velocity.x;
                            NearPlayers.socketmodel[i].velocity_y = P.GetComponent<Rigidbody>().velocity.y;
                            NearPlayers.socketmodel[i].velocity_z = P.GetComponent<Rigidbody>().velocity.z;
                        }
                    }
                    Callback_SocketModel[] socketmodel2 = new Callback_SocketModel[i+1];
                    for (int ii = 0; ii < i+1; ii++)
                    {
                        socketmodel2[ii] = NearPlayers.socketmodel[ii];
                    }
                    NearPlayers.socketmodel = socketmodel2;*/
                }
            string json = JsonUtility.ToJson(NearPlayers);
            //Debug.Log(json);
            pos_socket.AsyncSendData(posclientsocket, json);
            
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(data);
            return;
        }
    }
    public void Msg_Receive(string data, Socket msgclientsocket)
    {
        //若为上线消息则创建玩家，并添加表项
        try
        {
            MsgModel obj = JsonUtility.FromJson<MsgModel>(data);
            GameObject.Find("Chat UI").GetComponent<ChatControl>().AddContent(obj.type, obj.position_x, obj.position_y, obj.position_z, obj.roomno, obj.username, obj.nickname, obj.channel, obj.content);
            MsgModel msgmodel = new MsgModel();
            //msgmodel.type = 3;
            msgmodel.type = obj.type;
            msgmodel.position_x = obj.position_x;
            msgmodel.position_y = obj.position_y;
            msgmodel.position_z = obj.position_z;
            msgmodel.roomno = obj.roomno;
            msgmodel.username = obj.username;
            msgmodel.nickname = obj.nickname;
            msgmodel.channel = obj.channel;
            msgmodel.content = obj.content;
            Callback_MsgModel callback = new Callback_MsgModel();
            callback.msgmodel = new MsgModel[1];
            callback.msgmodel[0] = msgmodel;
            string json = JsonUtility.ToJson(callback);
            if (obj != null) 
            {
                if (obj.type > 5 || obj.type < 1) 
                {
                    Debug.Log("信息类型错误！ 错误type：" + obj.type);
                }
                if (obj.type == 3)
                {
                    
                    switch (obj.channel)//根据范围发送消息
                    {
                        case 1:
                            foreach (Hashtable ht in clientsockets)
                            {
                                foreach (DictionaryEntry de in ht)
                                {
                                    msg_socket.AsyncSendData((Socket)de.Key, json);
                                }
                            }
                            break;
                        case 2:
                            foreach (DictionaryEntry de in clientsockets[obj.roomno - 1]) 
                            {
                                msg_socket.AsyncSendData((Socket)de.Key, json);
                            }
                            break;
                        case 3:
                            foreach (GameObject Player in GameObject.FindGameObjectsWithTag("Player"))
                            {
                                if ((Player.transform.position - new Vector3(obj.position_x, obj.position_y, obj.position_z)).magnitude < 30)
                                {
                                    msg_socket.AsyncSendData(Player.GetComponent<movement>().msgSocket, json);
                                }
                            }
                            break;
                        case 4://建议添加系统频道 查询双方昵称并分别对两个客户端发送通道4消息，注意玩家不存在的情况
                            bool tempj = false;//判断是否找到对应玩家
                            Debug.Log(obj.username);
                            foreach (GameObject Player in GameObject.FindGameObjectsWithTag("Player"))
                            {
                                if (Player.GetComponent<movement>().id == obj.username)
                                {
                                    tempj = true;
                                    msg_socket.AsyncSendData(Player.GetComponent<movement>().msgSocket, json);//客户端记得判断收到的悄悄话消息中的id是否为自己，是的话就是from...,否则to...
                                    callback.msgmodel[0].nickname = Player.GetComponent<movement>().nickname;
                                    json = JsonUtility.ToJson(callback);
                                    msg_socket.AsyncSendData(msgclientsocket, json);
                                }
                            }
                            if (!tempj)
                            {
                                callback.msgmodel[0].content = "该玩家不存在或者未上线";
                                json = JsonUtility.ToJson(callback);
                                msg_socket.AsyncSendData(msgclientsocket, json);
                            }
                            break;
                        default:
                            break;
                    }
                }
                if (obj.type == 1 /*&& obj.roomno == GameObject.Find("GameManagement").GetComponent<GameManagement>().RoomNo*/)
                {
                    if (clientsockets[obj.roomno - 1].Count > 0)
                    {
                        foreach (DictionaryEntry de in clientsockets[obj.roomno - 1])
                        {
                            msg_socket.AsyncSendData((Socket)de.Key, json);
                        }
                    }
                    clientsockets[obj.roomno - 1].Add(msgclientsocket, null);
                    GameObject tmpObj = Instantiate(prefabs/*, pos, Quaternion.identity*/)as GameObject;
                    tmpObj.transform.position = GameObject.Find("GameManagement").GetComponent<GameManagement>().SpawnPosition[obj.roomno - 1];
                    tmpObj.GetComponent<movement>().setup(obj.username, obj.nickname, msgclientsocket);
                    //创建玩家
                    //GameObject roleInstance = Instantiate(GameObject.Find("role"), new Vector3(item.position_x, item.position_y, item.position_z), Quaternion.Euler(0f, 0f, 0f));
                    //roleInstance.GetComponent<movement>().setup(item.username, item.nickname);
                }
                if (obj.type == 2 /*&& obj.roomno == GameObject.Find("GameManagement").GetComponent<GameManagement>().RoomNo*/)
                {
                    Debug.Log(obj.roomno - 1);
                    if (clientsockets[obj.roomno - 1].Count > 0)
                    {
                        
                        foreach (DictionaryEntry de in clientsockets[obj.roomno - 1])
                        {
                            msg_socket.AsyncSendData((Socket)de.Key, json);
                        }
                    }
                    WWWForm form1 = new WWWForm();
                    form1.AddField("roomno", obj.roomno);
                    form1.AddField("userId", int.Parse(obj.username));
                    form1.AddField("type", 0);
                    StartCoroutine(go_offline("http://localhost:3000/api/v1/user/setOnline", form1, obj.username));
                    //Destroy(GameObject.Find(item.username));
                }
            }

            //将消息显示在聊天框中
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
            Debug.Log(data);
            return;
        }
    }
    public void Kick_Off(Socket clientSocket)
    {
        MsgModel msgmodel = new MsgModel();
        msgmodel.type = 2;
        Debug.Log("aaa");
        // msgmodel.position_x = obj.position_x;
        // msgmodel.position_y = obj.position_y;
        // msgmodel.position_z = obj.position_z;
        // msgmodel.roomno = obj.roomno;
        // msgmodel.username = obj.username;
        // msgmodel.nickname = obj.nickname;
        // msgmodel.channel = obj.channel;
        // msgmodel.content = obj.content;
        bool is_online = false;
        foreach (GameObject Player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (Player.GetComponent<movement>().msgSocket == clientSocket)//两个socket一者断线则另者掐断
            {
                for (int i = 0; i < RoomCount; i++)
                {
                    if (clientsockets[i].Contains(Player.GetComponent<movement>().msgSocket))
                    {
                        msgmodel.roomno = i + 1;
                        msgmodel.username = Player.GetComponent<movement>().id;
                        msgmodel.nickname = Player.GetComponent<movement>().nickname;
                        msgmodel.channel = 2;
                        msgmodel.content = msgmodel.nickname + "下线了";
                        postoplayers.Remove((Socket)clientsockets[i][Player.GetComponent<movement>().msgSocket]);
                        clientsockets[i].Remove(Player.GetComponent<movement>().msgSocket);
                        break;
                    }
                }
                pos_socket.sessionTable.Remove(Player.GetComponent<movement>().posSocket);
                msg_socket.sessionTable.Remove(Player.GetComponent<movement>().msgSocket);
                if (Player.GetComponent<movement>().msgSocket.Connected)
                {
                    Player.GetComponent<movement>().msgSocket.Shutdown(SocketShutdown.Both);
                    Player.GetComponent<movement>().msgSocket.Close();
                }
                if (Player.GetComponent<movement>().posSocket.Connected)
                {
                    Player.GetComponent<movement>().posSocket.Shutdown(SocketShutdown.Both);
                    Player.GetComponent<movement>().posSocket.Close();
                }
                Destroy(Player);
                is_online = true;
                break;
            }
            else if (Player.GetComponent<movement>().posSocket == clientSocket)
            {
                for (int i = 0; i < RoomCount; i++)
                {
                    if (clientsockets[i].Contains(Player.GetComponent<movement>().msgSocket))
                    {
                        msgmodel.roomno = i + 1;
                        msgmodel.username = Player.GetComponent<movement>().id;
                        msgmodel.nickname = Player.GetComponent<movement>().nickname;
                        msgmodel.channel = 2;
                        msgmodel.content = msgmodel.nickname + "下线了";
                        postoplayers.Remove((Socket)clientsockets[i][Player.GetComponent<movement>().msgSocket]);
                        clientsockets[i].Remove(Player.GetComponent<movement>().msgSocket);
                        break;
                    }
                }
                pos_socket.sessionTable.Remove(Player.GetComponent<movement>().posSocket);
                msg_socket.sessionTable.Remove(Player.GetComponent<movement>().msgSocket);
                if (Player.GetComponent<movement>().msgSocket.Connected)
                {
                    Player.GetComponent<movement>().msgSocket.Shutdown(SocketShutdown.Both);
                    Player.GetComponent<movement>().msgSocket.Close();
                }
                if (Player.GetComponent<movement>().posSocket.Connected)
                {
                    Player.GetComponent<movement>().posSocket.Shutdown(SocketShutdown.Both);
                    Player.GetComponent<movement>().posSocket.Close();
                }
                Destroy(Player);
                is_online = true;
                break;
            }
        }
        if (is_online && msgmodel.roomno > 0) //如果存在这个玩家被踢下线则发送离线消息
        {
            Debug.Log("bbb");
            string json = JsonUtility.ToJson(msgmodel);
            Msg_Receive(json, null);
        }

        //发送下线消息,销毁对象
    }
    public void Process()//处理玩家信息，控制指令
    {
        pos_socket.Process();
        msg_socket.Process();
        //运行两个服务端套接字的process程序，下同
    }
    public void CheckOffLine()//检测客户端是否断开连接
    {
        pos_socket.checkoffline();
        msg_socket.checkoffline();
    }
    void Start()
    {
        time = 0f;
        clientsockets = new Hashtable[RoomCount];
        for (int i = 0; i < RoomCount; i++)
        {
            clientsockets[i] = new Hashtable();
        }
        pos_socket = new PosSeverSocket();
        msg_socket = new MsgSeverSocket();
        GM = GameObject.Find("GameManagement");
        postoplayers = new Dictionary<Socket, List<GameObject>>();
        Thread thread = new Thread(new ThreadStart(posthread));
        thread.Start();
        Thread thread2 = new Thread(new ThreadStart(msgthread));
        thread2.Start();
    }
    private void posthread()
    {
        pos_socket.ListenServer(ip, pos_port);
    }
    private void msgthread()
    {
        msg_socket.ListenServer(ip, msg_port);
    }
    void Update()
    {
        Process();
        time += Time.deltaTime;
        if (time > 55f)
        {
            time = 0f;
            CheckOffLine();
        }
    }
}

[System.Serializable]
public class LoginModel
{
    public int result;
    public string username;
    public string nickname;
}
[System.Serializable]
public class RegisterModel
{
    public int result;
}
[System.Serializable]
public class GoOnlineModel
{
    public int code;
}
[System.Serializable]
public class Callback_SocketModel
{
    public string id;
    public string nickname;
    /*public float position_x;
    public float position_y;
    public float position_z;
    public float velocity_x;
    public float velocity_y;
    public float velocity_z;
    public float forward_x;
    public float forward_z;
    public int roomno;*/
    public uint p;//前16位存储position_x,后16位存储position_y
    public uint v;//分10位存储velocity_xyz
    public uint r;//前2位存储场景号，后数16位存储position_z,后数14位分7位存储forward
}
[System.Serializable]
public class SocketModel//简化变量名并浓缩为三个int型数据，想办法取得服务器接收到至少一次id与nickname的信号(例如回传id为自身id的同时nickname为null)，之后不再传输id与nickname（null）。
{
    public string id;
    public string nickname;
    /*public float position_x;
    public float position_y;
    public float position_z;
    public float velocity_x;
    public float velocity_y;
    public float velocity_z;
    public float forward_x;
    public float forward_z;
    public int roomno;*/
    public uint p;//前16位存储position_x,后16位存储position_y
    public uint v;//分10位存储velocity_xyz
    public uint r;//前2位存储场景号，后数16位存储position_z,后数14位分7位存储forward
}
[System.Serializable]
public class PullModel
{
    public Callback_SocketModel[] socketmodel;
}
[System.Serializable]
public class MsgModel
{
    public int type;
    /*类型说明:
    1:(发送)上线信息
    2:(发送)下线信息
    3:(发送)玩家发的消息
    4:取得上下线信息
    5:取得玩家发的消息
    */
    public float position_x;
    public float position_y;
    public float position_z;//信息位置
    public int roomno;//信息房间
    public string username;
    public string nickname;//发送者(悄悄话接受者)信息
    public int channel;//消息范围 1世界2频道3附近4私聊
    public string content;//消息内容
}
[System.Serializable]
public class Callback_MsgModel
{
    public MsgModel[] msgmodel;
}