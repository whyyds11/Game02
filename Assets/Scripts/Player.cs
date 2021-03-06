using UnityEngine;
using UnityEngine.UI;
using System.Threading;


public class Player : MonoBehaviour
{
    #region 欄位
    [Header("移動速度"), Range(0, 1000)]
    public float speed = 5;
    [Header("跳越高度"), Range(0, 1000)]
    public int jump = 350;
    [Header("血量"), Range(0, 2000)]
    public float hp = 500;

    private bool isGround;
    private int coin;
    private float hpMax;
    private bool dead;

    [Header("音效區域")]
    public AudioClip soundHit;
    public AudioClip soundSlide;
    public AudioClip soundJump;
    public AudioClip soundCoin;

    [Header("金幣數量")]
    public Text textCoin;

    [Header("血條")]
    public Image imageHp;

    [Header("結束畫面")]
    public GameObject final;

    [Header("過關標題與金幣")]
    public Text textTitle;
    public Text textFinalCoin;

    public Animator ani;
    public Rigidbody2D rig;
    public CapsuleCollider2D cap;
    public AudioSource aud;

    [SerializeField]
    private Vector3[] rayOffset =
    {
        new Vector3(-0.30f, -1.05f),
        new Vector3(-0.05f, -1.10f),
        new Vector3(+0.20f, -1.05f)
    };
    [SerializeField]
    private float[] rayLength = { 0.15f, 0.12f, 0.15f };
    #endregion

    #region 方法
    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {
        // Time.deltaTime 一禎的時間
        // Update 內移動、旋轉、運動 * Time.deltaTime
        // 避免不同裝置執行速度不同
        transform.Translate(speed * Time.deltaTime, 0, 0);   // 變形.位移(x, y, z)
    }

    /// <summary>
    /// 跳躍
    /// </summary>
    private void Jump()
    {
        // 布林值 = 輸入.按下按鍵(按鍵列舉.空白鍵)
        bool space = Input.GetKeyDown(KeyCode.Space);

        // 2D 射線碰撞物件 = 2D 物理.射線碰撞(起點，方向，長度，圖層)
        // 圖層語法：1 << 圖層編號
        RaycastHit2D hitL = Physics2D.Raycast(transform.position + rayOffset[0], -transform.up, rayLength[0], 1 << 8);
        RaycastHit2D hitC = Physics2D.Raycast(transform.position + rayOffset[1], -transform.up, rayLength[1], 1 << 8);
        RaycastHit2D hitR = Physics2D.Raycast(transform.position + rayOffset[2], -transform.up, rayLength[2], 1 << 8);

        if (hitL || hitC || hitR)
        {
            isGround = true;                    // 如果 碰到地板 是否在地板上 = 是
            ani.SetBool("跳躍開關", false);
        }
        else
        {
            isGround = false;                   // 否則 是否在地板上 = 否
        }

        // 如果 在地板上
        if (isGround)
        {
            // 如果 按下空白鍵
            if (space)
            {
                // 剛體.添加推力(二維向量)
                rig.AddForce(new Vector2(0, jump));
                // 音效來源.播放一次(音效，音量)
                aud.PlayOneShot(soundJump, 0.3f);
                ani.SetBool("跳躍開關", true);       // 動畫控制器.設定布林值("參數名稱"，布林值)
            }
        }
    }

    /// <summary>
    /// 滑行
    /// </summary>
    private void Slide()
    {
        bool ctrl = Input.GetKey(KeyCode.LeftControl);
        ani.SetBool("滑行開關", ctrl);

        // 如果 按下 左邊 ctrl 播放一次音效
        // 判斷式如果只有一行程式可以省略大括號
        if (Input.GetKeyDown(KeyCode.LeftControl)) aud.PlayOneShot(soundSlide, 0.8f);

        // 如果 按下 ctrl
        if (ctrl)
        {
            // 滑行 位移 -0.1 -1.5 尺寸 1.35 1.35
            cap.offset = new Vector2(-0.1f, -1.5f);
            cap.size = new Vector2(1.35f, 1.35f);
        }
        // 否則
        else
        {
            // 站立 位移 -0.1 -0.4 尺寸 1.35 3.6
            cap.offset = new Vector2(-0.1f, -0.4f);
            cap.size = new Vector2(1.35f, 3.6f);
        }
    }

    /// <summary>
    /// 吃金幣
    /// </summary>
    /// <param name="obj">金幣的遊戲物件</param>
    private void EatCoin(GameObject obj)
    {
        coin++;                                 // 遞增 1
        aud.PlayOneShot(soundCoin, 1.2f);       // 播放音效
        textCoin.text = "金币数量：" + coin;     // 文字介面.文字 = 字串 + 整數
        Destroy(obj, 0);                        // 刪除(金幣物件，延遲時間)
    }

    /// <summary>
    /// 受傷
    /// </summary>
    private void Hit(GameObject obj)
    {
        hp -= 99999;                           // 扣血 hp -= 10
        aud.PlayOneShot(soundHit);          // 播放音效
        imageHp.fillAmount = hp / hpMax;    // 更新血條
        Destroy(obj);                       // 刪除障礙物

        if (hp <= 0) Dead();                // 如果 血量 <= 0 死亡
    }

    /// <summary>
    /// 死亡
    /// </summary>
    private void Dead()
    {
        ani.SetTrigger("死亡觸發");     // 死亡動畫
        final.SetActive(true);          // 顯示結束畫面
        speed = 0;                      // 速度 = 0 (停止移動)
        dead = true;                    // 死亡 = 打勾
        textTitle.text = "游戏结束了呜...";

    }

    /// <summary>
    /// 過關
    /// </summary>
    private void Pass()
    {
        speed = 0;                          // 速度 = 0
        final.SetActive(true);              // 顯示結束畫面
        textTitle.text = "恭喜你过关啦!";
        textFinalCoin.text = "本次金币数量：" + coin;
    }
    #endregion

    #region 事件
    private void Start()
    {
        hpMax = hp;     // 最大血量 = 血量
    }

    private void Update()
    {
        if (dead)
        {

            return;
        }// 如果 死亡 跳出

        if (transform.position.y <= -5) Dead();    // 第二種死法

        Jump();
        Slide();
        int number = GetComponent<SampleUserPolling_ReadWrite>().Len;
        //Debug.Log("OnMessageArrived", number.ToString());
        //number = 100;
        // print(number);
        if (number<15) {
            Move();
            //print(number);
        }

    }

    // 碰撞 (觸發) 事件：
    // 兩個物件必須有一個勾選 Is Trigger
    // Enter 進入時執行一次
    // Stay 碰撞時執行一秒約 60 次
    // Exit 離開時執行一次
    // 參數：紀錄碰撞到的碰撞資訊
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果 碰撞資訊.標籤 等於 金幣 吃掉金幣(碰撞資訊.遊戲物件)
        if (collision.tag == "金幣") EatCoin(collision.gameObject);

        // 如果 碰到障礙物 受傷
        if (collision.tag == "障礙物") Hit(collision.gameObject);

        // 如果 碰撞資訊.名稱 等於 傳送門 過關
        if (collision.name == "傳送門") Pass();
    }

    // 繪製圖示事件：繪製輔助線條，僅在 Scene 看得到
    private void OnDrawGizmos()
    {
        // 圖示.顏色 = 顏色.紅色
        Gizmos.color = Color.red;
        // 圖示.繪製射線(起點，方向)
        // transform 此物件的變形元件
        // transform.position 此物件的座標
        // transform.up 此物件上方      Y 預設為 1
        // transform.right 此物件右方   X 預設為 1
        // transform.forward 此物件前方 Z 預設為 1
        Gizmos.DrawRay(transform.position + rayOffset[0], -transform.up * rayLength[0]);
        Gizmos.DrawRay(transform.position + rayOffset[1], -transform.up * rayLength[1]);
        Gizmos.DrawRay(transform.position + rayOffset[2], -transform.up * rayLength[2]);
    }
    #endregion
}
