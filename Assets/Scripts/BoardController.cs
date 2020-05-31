using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace app
{
    /// <summary>
    /// 盤面上の位置を保持するクラス
    /// </summary>
    public class BoardPoint
    {
        public int x { get; set; } = -1;
        public int y { get; set; } = -1;

        public BoardPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    /// <summary>
    /// 盤面の情報を管理するクラス
    /// </summary>
    public class BoardController : MonoBehaviour
    {
        #region インスペクタ表示
        [SerializeField, Tooltip("ボールプレハブ")]
        private GameObject BallPrefab = null;

        [SerializeField, Tooltip("赤ボールスプライト")]
        private Sprite RedBallSprite = null;
        [SerializeField, Tooltip("青ボールスプライト")]
        private Sprite BlueBallSprite = null;
        [SerializeField, Tooltip("緑ボールスプライト")]
        private Sprite GreenBallSprite = null;
        [SerializeField, Tooltip("黄ボールスプライト")]
        private Sprite YellowBallSprite = null;
        [SerializeField, Tooltip("紫ボールスプライト")]
        private Sprite PurpleBallSprite = null;
        [SerializeField, Tooltip("ピンクボールスプライト")]
        private Sprite PinkBallSprite = null;
        #endregion

        #region 定数プロパティ
        public int XPOINTMAX { get; } = 6; 
        public int YPOINTMAX { get; } = 5;
        #endregion

        #region フィールド
        /*
         * (0,0) (1,0) (2,0) (3,0) (4,0) (5,0)
         * (0,1) (1,1) (2,1) (3,1) (4,1) (5,1)
         * (0,2) (1,2) (2,2) (3,2) (4,2) (5,2)
         * (0,3) (1,3) (2,3) (3,3) (4,3) (5,3) 
         * (0,4) (1,4) (2,4) (3,4) (4,4) (5,4) 
         */
        private BallController[,] Board;
        private SpriteRenderer Renderer;
        #endregion

        public void Awake()
        {
            Renderer = gameObject.GetComponent<SpriteRenderer>();    
        }

        public void Start()
        {
            /// 盤面生成
            generateBoard();
        }

        public void Update()
        {
            
        }

        public void LateUpdate()
        {
            #region DEBUG
            drawDebugLog();
            #endregion

        }

        /// <summary>
        /// 盤面上のボールを生成する
        /// </summary>
        private void generateBoard()
        {
            ///盤面データテーブル作成
            Board = new BallController[YPOINTMAX,XPOINTMAX];

            for(int y =0;y<YPOINTMAX;y++)
            {
                for(int x = 0;x<XPOINTMAX;x++)
                {
                    Board[y,x] = generateBall(BallType.Green, new BoardPoint(x, y));
                }
            }
        }

        /// <summary>
        /// ボールプレハブをインスタンス化し生成する
        /// </summary>     
        private BallController generateBall(BallType ballType, BoardPoint point)
        {
            /// BoardPointの数値から、具体的な座標を計算し、生成する
            var newBallPos = convertPointToPos(point);

            var newBall = Instantiate(BallPrefab, newBallPos, Quaternion.identity);

            /// コントローラーセットアップ
            var ballContorller = newBall.GetComponent<BallController>();
            if (ballContorller != null)
            {
                ballContorller.setSprite(selectBallSprite(ballType));
                ballContorller.ballType = ballType;
                ballContorller.boardPoint = point;
            }

            return ballContorller;
        }
        
        /// <summary>
        /// ボード位置のインデックスを座標に変換する
        /// </summary>
        private Vector3 convertPointToPos(BoardPoint point)
        {
            var centerPos = gameObject.GetComponent<Transform>().position;
            ///一つのセルは正方形なので、一辺の長さはxとyで同じ
            var cellSize = Renderer.size.x / XPOINTMAX;

            var originPos = centerPos + new Vector3(
                -Renderer.size.x/2 + cellSize/2,
                Renderer.size.y/2 - cellSize/2 );

            return originPos +
                new Vector3(point.x * cellSize, -point.y * cellSize, 0.0f);
        }

        /// <summary>
        /// ボールタイプに合わせてスプライトを選択する
        /// </summary>
        private Sprite selectBallSprite(BallType type)
        {
            switch (type)
            {
                case BallType.Red:
                    return RedBallSprite;
                case BallType.Blue:
                    return BlueBallSprite;
                case BallType.Green:
                    return GreenBallSprite;
                case BallType.Yellow:
                    return YellowBallSprite;
                case BallType.Purple:
                    return PurpleBallSprite;
                case BallType.Pink:
                    return PinkBallSprite;
                default:
                    Debug.LogError("ボールのスプライトを選ぶことができませんでした。");
                    break;
            }

            return null;
        }

        #region DEBUG
        private void drawDebugLog()
        {
            ///盤面デバッグ表示
            string log = "";
            for (int y = 0; y < YPOINTMAX; y++)
            {
                for (int x = 0; x < XPOINTMAX; x++)
                {
                    log += Board[y, x].ballType.ToString() + " ";
                }

                log += "\n";
            }

            LogDrawer.drawLog(log);
        }

        #endregion
    }
}
