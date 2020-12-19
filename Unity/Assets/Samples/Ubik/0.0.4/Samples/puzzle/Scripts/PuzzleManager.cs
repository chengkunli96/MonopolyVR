using System.Collections;
using System.Collections.Generic;
//using System.Windows.Forms;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Ubik.Messaging;
using Ubik.Samples;

// Log: 1. 改了PiecesDrag和PuzzleMagger之间的交互关系
// 2. 给PuzzleManager加了网络属性，让其交互时间! 所以不要忘记在对应地GameObject上加上Network Component

public class PuzzleManager : MonoBehaviour, INetworkComponent
{
    public static PuzzleManager Instance;
    public GameObject GridsGameObject;
    public GameObject FinishPanel;
    // JM
    public Button StartButton;
    public Button CloseButton;
    public GameObject ClosePanel;
    public Text Timer;
    bool startTime = false;
    float spendTime = 0.0f;
    int minute = 0, second = 0, millisecond = 0;

    List<GameObject> targets = new List<GameObject>();
    List<string> gridsName = new List<string>();
    // TZY
    List<Vector3> gridTrans = new List<Vector3>();
    private NetworkContext content;
    bool isMePressStartButton = false;
    bool distroyflag = false;
    bool finishedflag = false;
    //bool isgetposition = false;

    public bool[] correctPiece;
    int total_pieces = 0;
    [SerializeField]
    float threshold = 0.125f;
    public GameController gameController;
    public PuzzleSpanwer puzzlespanwer;

    private struct Message
    {
        //public RectTransform trans;
        public float time;
        public bool distroyflag;
        //public Vector3 position;

        public Message(float speedtime, bool distroy)//, Vector3 position)
        {
            this.time = speedtime;
            this.distroyflag = distroy;
            //this.position = position;
        }
    }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        content = NetworkScene.Register(this);
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        puzzlespanwer = GameObject.Find("Generator").GetComponent<PuzzleSpanwer>();
    }
    void Start()
    {
        InitGrids();
        //JM
        StartButton.GetComponent<Button>().onClick.AddListener(OnStartClick);
        CloseButton.GetComponent<Button>().onClick.AddListener(OnCloseClick);
        Timer.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // if the start button is pressed by me, I need to send the time to other all the time
        // Otherwise, just parse the time received in Update()
        if (isMePressStartButton)
        {
            content.SendJson(new Message(spendTime, distroyflag));//, puzzlespanwer.transform.position + new Vector3(0.0f, 1.2f, 0.0f)));
            if (startTime && !finishedflag) UpdateTime();
        }
        else
        {
            ParseTime(spendTime);
        }
        if (distroyflag)
        {
            Destroy(ClosePanel);
        }
    }

    //JM
    private void OnCloseClick()
    {
        //ClosePanel.SetActive(false);
        distroyflag = true;
        puzzlespanwer.already_spawn = false;
        isMePressStartButton = false;
        //isgetposition = false;
    }
    //JM
    private void OnStartClick()
    {
        // close the start panel and start timing
        isMePressStartButton = true;
        startTime = true;
        gameController.get_puzzle_time = false;
        //puzzlespanwer.transform.position += new Vector3(0.0f, 1.2f, 0.0f);
    }
    //JM
    private void UpdateTime()
    {
        spendTime += Time.deltaTime;
        ParseTime(spendTime);
    }

    // parse time float and show
    private void ParseTime(float time)
    {
        minute = (int)time / 60;
        second = (int)(time - 60 * minute);
        millisecond = (int)((time - (int)time) * 100);
        Timer.text = string.Format("{0:D2}:{1:D2}:{2:D2}", minute, second, millisecond);
    }

    private void InitGrids()
    {
        foreach (Transform child in GridsGameObject.transform)
        {
            targets.Add(child.gameObject);
            gridsName.Add(child.gameObject.name);
            gridTrans.Add(new Vector3(0, 0, 0));
            total_pieces++;
        }
        correctPiece = new bool[targets.Count];
        int i = 0;
        foreach (GameObject child in targets)
        {
            correctPiece[i] = false;
            i++;
        }
    }

    public GameObject SetPiece(GameObject piece, RectTransform rect)
    {
        int i = 0;
        float x = piece.transform.position.x;
        float y = piece.transform.position.y;

        // this function just return a nearest distance but don't judge wther it is right or not
        foreach (GameObject target in targets)
        {
            if (Mathf.Abs(x - target.transform.position.x) < threshold && Mathf.Abs(y - target.transform.position.y) < threshold)
            {
                return target;
            }
            i++;
        }

        return null;
    }

    public void UpdatePosition(Vector3 pos, string name)
    {
        int i = 0;
        foreach (GameObject target in targets)
        {
            // write the position to the corresponding place
            if (target.name == name) gridTrans[i] = pos;
            i++;
        }
    }

    public void UpdateGridCorrection()
    {
        int i = 0;
        foreach (GameObject target in targets)
        {
            // The Corresponding piece coordinate
            Vector3 piecePosition = gridTrans[i];

            // if the gridTrans have the same coordinate with current target, which means the position is right
            if (target.transform.position == piecePosition)
                correctPiece[i] = true;
            else
                correctPiece[i] = false;
            i++;
        }
    }
    public bool IsFinish()
    {
        int correct_num = 0;
        for (int i = 0; i < total_pieces; i++)
        {
            if (correctPiece[i])
                correct_num++;
        }
        if (correct_num == total_pieces)
        {
            // JM Stop timing
            startTime = false;
            finishedflag = true;
            FinishPanel.SetActive(true);
            gameController.finishtime = spendTime;
            return true;
        }
        else
            return false;
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        if (!isMePressStartButton)
        {
            var msg = message.FromJson<Message>();
            spendTime = msg.time;
            distroyflag = msg.distroyflag;
            /*if (!isgetposition)
            {
                ClosePanel.transform.position = msg.position;
                isgetposition = true;
            }*/
        }
    }
}
