using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SweepGameControl : MonoBehaviour {
    /// <summary>
    /// 游戏结束统计
    /// </summary>
    Text text;
    Text ResultText;
    bool Gameresult = false;
    /// <summary>
    /// 初始化数据
    /// </summary>
    public int width=8;
    public int height=8;
    public int sweapNum=10;
    public int winNum = 0;
    float timer = 0;
    public GameObject prefab;
    /// <summary>
    /// 游戏地图存储
    /// </summary>
    Dictionary<Vector2, int> map = new Dictionary<Vector2, int>();
    //int[][] map=new int[20][];
    Dictionary<Vector2, GameObject> gameobject = new Dictionary<Vector2, GameObject>();
    /// <summary>
    /// 射线检测
    /// </summary>
    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;

    // Use this for initialization
    void Start () {
        text = GameObject.Find("Score").GetComponent<Text>();
        ResultText = GameObject.Find("WinText").GetComponent<Text>();
        Init();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Moved)
            {
                ray = mainCamera.ScreenPointToRay(Input.touches[0].position);
                if (Physics.Raycast(ray, out hit, 100))
                {
                    string name = hit.collider.name;
                    ///对射线碰撞的物体名字进行分析从而决定游戏逻辑
                    int q = name.IndexOf("-");
                    int x = int.Parse(name.Substring(0, q));
                    int y = int.Parse(name.Substring(q + 1));
                    OnClick(new Vector2(x, y));
                }
            }
        }
        //PC模式下检测
        if (Input.GetMouseButtonDown(0))
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                int q = hit.collider.name.IndexOf("-");
                int x = int.Parse(hit.collider.name.Substring(0, q));
                int y = int.Parse(hit.collider.name.Substring(q + 1));
                OnClick(new Vector2(x, y));
            }
        }
        else if(Input.GetMouseButtonDown(1))
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                int q = hit.collider.name.IndexOf("-");
                int x = int.Parse(hit.collider.name.Substring(0, q));
                int y = int.Parse(hit.collider.name.Substring(q + 1));
                Sign(new Vector2(x, y));
            }
        }

        if (winNum <= 0 && !Gameresult)
        {
            Gameresult = true;
            GameResult(true);
        }

        if(!Gameresult)
        {//计时
            timer += Time.deltaTime;
            int i = (int)timer;
            int j = i / 60;
            i = i % 60;
            text.text = j.ToString() + ":" + i.ToString();
        }
    }
    void Init()
    {
        //重置摄像机的参数包括位置和视口大小
        mainCamera = Camera.main;
        mainCamera.transform.position = new Vector3(width / 2 * 1.4f, height / 2 * 1.4f, -10);
        mainCamera.orthographicSize = width * 2;

        //初始化地图，0表示没有东西
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                map[new Vector2(i, j)] = 0;
                InitPrefab(new Vector2(i, j));
            }
        }
        //随机位置出现雷
        for (int i = 0; i < sweapNum; )
        {
            Vector2 pos = new Vector2(Random.Range(0, width), Random.Range(0, height));
            if (map[pos] != -1)
            {
                map[pos] = -1;
                i++;
            }
        }
        //填充详细地图（包括周围雷数）
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 pos = new Vector2(i, j);
                if (map[pos] != -1)
                {
                    map[pos] = CheakSweap(pos);              
                }
            }
        }
        //胜利个数
        winNum = width * height - sweapNum;
    }
    /// <summary>
    /// 检测该位置周围的雷的个数
    /// </summary>
    /// <param name="Pos"></param>
    /// <returns>周围8个位置的雷个数</returns>
    int CheakSweap(Vector2 Pos)
    {
        int num = 0;
        //遍历周围8个位置
        for (int i = -1; i <=1; i++)
        {
            for (int j = -1; j <=1; j++)
            {
                Vector2 posNew = Pos + new Vector2(i, j);
                if (posNew.x>=0&& posNew.x <width && posNew.y >= 0 && posNew.y <height)
                { 
                    if (map[posNew] < 0)
                        num++;
                }
            }
        }
        return num;
    }
    /// <summary>
    /// 当点开的为0自动弹开周围
    /// </summary>
    /// <param name="Pos"></param>
    void Sweap(Vector2 Pos)
    {
        map[Pos] = 10;
        Change(Pos,0);
        for (int i = -1; i <=1; i++)
        {
            for (int j = -1; j <=1; j++)
            {
                Vector2 posNew = Pos + new Vector2(i, j);
                if (posNew.x >= 0 && posNew.x < width && posNew.y >= 0 && posNew.y < height)
                {
                    OnClick(posNew);
                }
            }
        }
    }
    /// <summary>
    /// 左键点击判断事件
    /// </summary>
    /// <param name="Pos"></param>
    void OnClick(Vector2 Pos)
    {
        //-1表示雷，0到8表示周围地雷的数据，10表示点开的（不处理）
        int value = map[Pos];
        if (value < 0 )
        {
            if (!Gameresult)
            {
                GameResult(false);
                Gameresult = true;
            }
        }
        else if (value == 0)//周围8个都没有地雷，自动点开
            Sweap(Pos);
        else if (value < 9)
        {
            Change(Pos, value);
        }
    }
    /// <summary>
    /// 右键标记
    /// </summary>
    /// <param name="Pos"></param>
    void Sign(Vector2 Pos)
    {
        if (map[Pos] == 10)
            return;
        if (map[Pos] > 15)
        { map[Pos] -= 20;
            gameobject[Pos].GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            map[Pos] += 20;
            gameobject[Pos].GetComponent<SpriteRenderer>().color = Color.red;
        }

    }
    GameObject InitPrefab(Vector2 pos)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        go.transform.SetParent(mainCamera.gameObject.transform);
        go.transform.localScale = new Vector3(10, 10, 10);
        go.transform.position = new Vector3(pos.x * 1.4f, pos.y * 1.4f, 0);
        go.name = pos.x.ToString() + "-" + pos.y.ToString();
        gameobject[pos] = go;
        return go;
    }
    void Change(Vector2 pos,int value)
    {
        gameobject[pos].GetComponent<SpriteRenderer>().color = Color.white;
        gameobject[pos].GetComponentInChildren<Text>().text = value.ToString();
        winNum--;
        map[pos] = 10;
    }
    void GameResult(bool result)
    {
        if(result)
        {
            ResultText.text = "游戏胜利！！";
        }
        else
        {
            ResultText.text = "游戏失败！！";
        }
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void Resume()
    {
        Gameresult = false;
        ResultText.text="";
        timer = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                gameobject[new Vector2(i, j)].GetComponent<SpriteRenderer>().color = Color.green;
                gameobject[new Vector2(i, j)].GetComponentInChildren<Text>().text = "";
                map[new Vector2(i, j)] = 0;
            }
        }
        //随机位置出现雷
        for (int i = 0; i < sweapNum;)
        {
            Vector2 pos = new Vector2(Random.Range(0, width), Random.Range(0, height));
            if (map[pos] != -1)
            {
                map[pos] = -1;
                i++;
            }
        }
        //填充详细地图（包括周围雷数）
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 pos = new Vector2(i, j);
                if (map[pos] != -1)
                {
                    map[pos] = CheakSweap(pos);
                }
            }
        }

        winNum = width * height - sweapNum;
    }
}

