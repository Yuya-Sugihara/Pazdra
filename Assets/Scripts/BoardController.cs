using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace app
{
    /// <summary>
    /// 盤面上の位置を保持するクラス
    /// </summary>
    public class Point
    {
        public int x { get; set; } = -1;
        public int y { get; set; } = -1;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point operator+ (Point lhs,Point rhs)
        {
            return new Point(lhs.x + rhs.x, lhs.y + rhs.y);
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
        private MatrixArray<BallController> Board;
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
            Board = new MatrixArray<BallController>(XPOINTMAX, YPOINTMAX);

            for(int y =0;y<YPOINTMAX;y++)
            {
                for(int x = 0;x<XPOINTMAX;x++)
                {
                    var boardPoint = new Point(x,y);
                    ///　ここで設置するボールの色を設定する
                    var ballType = selectGeneratedBallType(boardPoint);
                    Board.setElement(generateBall(ballType, boardPoint),boardPoint);
                }
            }
        }

        /// <summary>
        /// 盤面情報をみて、次に生成するボールの色を選択する
        /// </summary>
        private BallType selectGeneratedBallType(Point point)
        {
            var selectable = new List<BallType>()
            {
                BallType.Red,
                BallType.Blue,
                BallType.Green,
                BallType.Yellow,
                BallType.Purple,
                BallType.Pink
            };

            BallType upBallType = BallType.None;
            BallType upUpBallType = BallType.None;
            BallType downBallType = BallType.None;
            BallType downDownBallType = BallType.None;

            Debug.Log("x: " + point.x + " y: " + point.y);

            if (point.y > 0)
            {
                upBallType = getBoardBallType(point, Direction.Up);
                if (point.y > 1)
                {
                    upUpBallType = getBoardBallType(point, Direction.Up, 2);
                }
            }

            if (point.y < YPOINTMAX - 1)
            {
                downBallType = getBoardBallType(point, Direction.Down);
                if (point.y < YPOINTMAX - 2)
                {
                    downDownBallType = getBoardBallType(point, Direction.Down, 2);
                }
            }

            Func<BallType, BallType, bool> isEqual = (type1, type2) =>
            {
                if (type1 == BallType.None)
                    return false;
                else if (type2 == BallType.None)
                    return false;

                return type1 == type2;
            };


            /// 判定 
            if(isEqual(upBallType,upUpBallType) ||
                isEqual(upBallType,downBallType) ||
                isEqual(downBallType,downDownBallType))
            {
                selectable.Remove(upBallType);
            }

            BallType leftBallType = BallType.None;
            BallType leftLeftBallType = BallType.None;
            BallType rightBallType = BallType.None;
            BallType rightRightBallType = BallType.None;

            if (point.x > 0)
            {
                leftBallType = getBoardBallType(point, Direction.Left);
                if (point.x > 1)
                {
                    leftLeftBallType = getBoardBallType(point, Direction.Left, 2);
                }
            }

            if (point.x < XPOINTMAX - 1)
            {
                rightBallType = getBoardBallType(point, Direction.Right);
                if (point.x < XPOINTMAX - 2)
                {
                    rightRightBallType = getBoardBallType(point, Direction.Right, 2);
                }
            }

            /// 判定 左２個、右２個が同じになることがある
            if (isEqual(leftBallType, leftLeftBallType) ||
                isEqual(leftBallType, rightBallType) ||
                isEqual(rightBallType, rightRightBallType))
            {
                selectable.Remove(leftBallType);
            }


            var rand = new System.Random();
            return selectable[rand.Next(0,selectable.Count-1)];
        }

       
        /// <summary>
        /// ボールプレハブをインスタンス化し生成する
        /// </summary>     
        private BallController generateBall(BallType ballType, Point point)
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
        private Vector3 convertPointToPos(Point point)
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

        private BallType getBoardBallType(Point point,Direction dir,int length = 1)
        {
            var ballController = Board.getElementByDirection(point,dir,length);
            if (ballController == null)
                return BallType.None;

            return ballController.ballType;
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
                    log += Board.getElement(x,y).ballType.ToString() + " ";
                }

                log += "\n";
            }

            LogDrawer.drawLog(log);
        }

        #endregion
    }
}
