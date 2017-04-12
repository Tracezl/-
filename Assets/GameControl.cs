using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameControl : MonoBehaviour {
    /// <summary>
    /// 游戏结束统计
    /// </summary>
    Text text;
    GameObject WinText;
    //GameObject quit;
    // GameObject next;
    public GameObject super;
    bool isWin = false;
    int step=0;
    /// <summary>
    /// 初始化数据
    /// </summary>
    public int width;
    public int height;
    public int timer = 3;
    public GameObject prefab;
    /// <summary>
    /// 游戏地图存储
    /// </summary>
    List<Vector2> white = new List<Vector2>();
    List<Vector2> black = new List<Vector2>();
    Dictionary<Vector2, GameObject> gameobject = new Dictionary<Vector2, GameObject>();
    /// <summary>
    /// 射线检测
    /// </summary>
    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
    /// <summary>
    /// 单例模式
    /// </summary>
    static GameControl instance;

    public static GameControl Instance
    {

        get
        {
            return instance;
        }
    }
    void Awake()
    {
        if (instance == null)
            instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start () {
        text = GameObject.Find("Score").GetComponent<Text>();
        WinText = GameObject.Find("WinText");
       // quit= GameObject.Find("Quit");
        //next= GameObject.Find("Next");
        Init();
    }
	
	// Update is called once per frame
	void Update () {
        //if (Input.touchCount == 1) //单点触碰移动摄像机
        //{
        //    if (Input.touches[0].phase == TouchPhase.Began)
        //        m_screenPos = Input.touches[0].position;   //记录手指刚触碰的位置
        //    if (Input.touches[0].phase == TouchPhase.Moved) //手指在屏幕上移动，移动摄像机
        //    {
        //        transform.Translate(new Vector3(Input.touches[0].deltaPosition.x * Time.deltaTime, Input.touches[0].deltaPosition.y * Time.deltaTime, 0));
        //    }
        //}
        if (Input.touchCount == 1 && isWin == false)
        {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Moved)
            {
                ray = mainCamera.ScreenPointToRay(Input.touches[0].position);
                if (Physics.Raycast(ray, out hit, 100))
                {
                    int q = hit.collider.name.IndexOf("-");
                    int x = int.Parse(hit.collider.name.Substring(0, q));
                    int y = int.Parse(hit.collider.name.Substring(q + 1));
                    step += 1;
                    OnClick(x, y);
                }
            }
        }
        if (Input.GetMouseButtonDown(0) && isWin == false)
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                int q = hit.collider.name.IndexOf("-");
                int x = int.Parse(hit.collider.name.Substring(0, q));
                int y = int.Parse(hit.collider.name.Substring(q + 1));
                step += 1;
                OnClick(x, y);
            }
        }

        if (black.Count == 0&&isWin==false)
        {
            text.gameObject.SetActive(true);
            text.text = "步数：" + step.ToString()+"      分数:"+(100-(step-timer)*5);
            WinText.SetActive(true);
            //quit.SetActive(true);
           // next.SetActive(true);
            isWin = true;
        }
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void Resume()
    {
        isWin = true;
        WinText.SetActive(false);
        //quit.SetActive(false);
        // next.SetActive(false);
        //foreach (var item in black)
        //{
        //    gameobject[item].GetComponent<SpriteRenderer>().color = Color.green;
        //    black.Remove(item);
        //    white.Add(item);
        //    Debug.Log(black.Count);
        //}
        while(black.Count!=0)
        {
            gameobject[black[0]].GetComponent<SpriteRenderer>().color = Color.green;
            white.Add(black[0]);
            black.Remove(black[0]);
            
            //Debug.Log(black.Count);
        }
        text.gameObject.SetActive(false);
        step = 0;

        for (int i = 0; i < timer; i++)
           OnClick(Random.Range(0, width), Random.Range(0, height));
        isWin = false;
    }
    public void SuperGame()
    {
        timer -= 3;
        Resume();
        if (timer < 4)
            super.gameObject.SetActive(false);
    }
    public void NextGame()
    {
        if (timer < 3)
            super.gameObject.SetActive(true);
        timer += 3;
        Resume();
    }
    void Init()
    {
        WinText.SetActive(false);
        //quit.SetActive(false);
        //next.SetActive(false);
        //重置摄像机的参数包括位置和视口大小
        mainCamera = Camera.main;
        mainCamera.transform.position = new Vector3(width / 2 * 1.4f, height / 2 * 1.4f, -10);
        mainCamera.orthographicSize = width*2;
        //填充胜利数组
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                white.Add(new Vector2(i, j));
            }
        }
        //根据数组实例图片
        foreach (var item in white)
        {
            InitPrefab(item);
        }
        //模拟点击，次数越多难度越大
        for (int i = 0; i < timer; i++)
            OnClick(Random.Range(0, width), Random.Range(0, height));
    }
    GameObject InitPrefab(Vector2 pos)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        go.transform.SetParent(mainCamera.gameObject.transform);
        go.transform.localScale = new Vector3(10, 10, 10);
        go.transform.position = new Vector3(pos.x*1.4f, pos.y * 1.4f,0);
        go.name = pos.x.ToString() + "-" + pos.y.ToString();
        gameobject[pos] = go;
        return go;
    }
    void OnClick(int x,int y)
    {
        Change(new Vector2(x, y));
        Change(new Vector2(x - 1, y));
        Change(new Vector2(x + 1, y));
        Change(new Vector2(x, y - 1));
        Change(new Vector2(x, y + 1));
    }
    void Change(Vector2 pos)
    {
        if (white.Contains(pos))
        {
            gameobject[pos].GetComponent<SpriteRenderer>().color = Color.white;
            white.Remove(pos);
            black.Add(pos);
        }
        else if(black.Contains(pos))
        {
            gameobject[pos].GetComponent<SpriteRenderer>().color = Color.green;
            white.Add(pos);
            black.Remove(pos);
        }
    }

}
