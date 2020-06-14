using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace app
{
    /// <summary>
    /// 盤面上の位置を保持するクラス
    /// </summary>
    public struct Point
    {
        public int x { get; set; }
        public int y { get; set; }

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

    public struct Cell
    {
        public Vector3 cellPos { get; set; }
        public Vector2 cellSize { get; set; }

        /// <summary>
        /// セルからの位置からどの方向に存在するか返す
        /// </summary>
        public Direction[] getDirectionFromCell(Vector3 pos)
        {
            /// 斜め判定をするためにサイズを2に設定する
            /// 要素１は縦方向の判定結果
            /// 要素２は横方向の判定結果
            var result = new Direction[2]
                {
                    Direction.None,
                    Direction.None
                };

            /// 左判定
            var left = cellPos.x - cellSize.x * 0.5f;
            if (pos.x < left)
                result[0] = Direction.Left;

            /// 右判定
            var right = cellPos.x + cellSize.x * 0.5f;
            if (right < pos.x)
                result[0] = Direction.Right;

            /// 上判定
            var up = cellPos.y + cellSize.y * 0.5f;
            if (up < pos.y)
                result[1] = Direction.Up;

            /// 下判定
            var bottom = cellPos.y - cellSize.y * 0.5f;
            if (pos.y < bottom)
                result[1] = Direction.Down;

            DebugEx.drawRect(up, bottom, left, right);
            return result;
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

        #region プロパティ
        public BallController currentOperatingBall { get; private set; }
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
        private Transform Transform;
        #endregion

        public void Awake()
        {
            Renderer = gameObject.GetComponent<SpriteRenderer>();
            Transform = gameObject.GetComponent<Transform>();
        }

        public void Start()
        {
            /// マネージャクラスに登録をリクエストする
            PuzzleManager.instance.registerBoardController(this);

            /// 盤面生成
            generateBoard();
        }

        public void Update()
        {
            /////////////////////////////////////////
            /// 操作中のボールの位置を監視する
            /////////////////////////////////////////
            updateBoard();
        }

        public void LateUpdate()
        {
            #region DEBUG
            drawDebugLog();
            #endregion
        }

        #region 公開メソッド

        #region データ取得
        /// <summary>
        /// 盤面座標取得
        /// </summary>
        public Vector3 getBoardPos()
        {
            return Transform.position;
        }

        /// <summary>
        /// 盤面サイズ取得
        /// </summary>
        public Vector2 getBoardSize()
        {
            return Renderer.size;
        }

        /// <summary>
        /// ボールタイプ取得
        /// </summary>
        public BallType getBoardBallType(Point point, Direction dir, int length = 1)
        {
            var ballController = Board.getElementByDirection(point, dir, length);
            if (ballController == null)
                return BallType.None;

            return ballController.ballType;
        }
        #endregion

        #region 操作ボール登録、解除
        public void registerOperatingBall(BallController controller)
        {
            currentOperatingBall = controller;
        }

        public void unregisterOperatingBall()
        {
            currentOperatingBall = null;
        }
        #endregion

        public Vector3 getBoardPos(Point point)
        {
            return convertPointToPos(point);
        }
        #endregion

        #region 非公開メソッド

        #region ボール生成
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
            newBall.name = ballType.ToString();
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

        #endregion

        private void updateBoard()
        {
            var moveDirection = getCurrentBallMovedDirection();
            if(moveDirection[0] == Direction.None &&
                moveDirection[1] == Direction.None)
            {
                /// 移動しなかった
                return;
            }

            Debug.Log("ball moved");

            /// 移動先のボール情報を取得する
            BallController BallMoveTo = null;
            var horizontalDirection = moveDirection[0];
            var verticalDirection = moveDirection[1];
            
            if (horizontalDirection != Direction.None) /// 横方向のボールを取得
            {
                Debug.Log("moved horizontal");
                BallMoveTo = Board.getElementByDirection(
                    currentOperatingBall.boardPoint,
                    horizontalDirection);
            }
            else if (verticalDirection != Direction.None)     ///縦方向のボールを取得
            {
                Debug.Log("moved horizontal");
                BallMoveTo = Board.getElementByDirection(
                    currentOperatingBall.boardPoint,
                    verticalDirection);
            }

            /// 斜め方向のボールを取得
            if(horizontalDirection != Direction.None &&
                verticalDirection != Direction.None)
            {
                Debug.Log("moved horizontal and vertical");
                ///斜め方向に移動する時は、必ず水平方向の処理を行うので、引数は垂直方向で良い
                BallMoveTo = Board.getElementByDirection(
                    BallMoveTo.boardPoint,
                    verticalDirection);
            }

            swapBall(currentOperatingBall,BallMoveTo);
        }

        private void swapBall(BallController origin,BallController moveTo)
        {
            /// 入れ替え中などは入れ替えできない 
            if (!origin.isOperating || !moveTo.canSwap)
                return;

            /// ボードインデックスの入れ替え
            var originPoint = new Point(origin.boardPoint.x, origin.boardPoint.y);
            var moveToPoint = new Point(moveTo.boardPoint.x, moveTo.boardPoint.y);

            Debug.Log("swap start");
            Debug.Log("originPoint.x: " + origin.boardPoint.x + "originPoint.y" + origin.boardPoint.y);
            Debug.Log("moveToPoint.x: " + moveTo.boardPoint.x + "moveToPoint.y" + moveTo.boardPoint.y); 
            ///入れ替え
            origin.boardPoint = moveToPoint;
            moveTo.boardPoint = originPoint;
            Debug.Log("swap end");
            Debug.Log("originPoint.x: " + origin.boardPoint.x + "originPoint.y" + origin.boardPoint.y);
            Debug.Log("moveToPoint.x: " + moveTo.boardPoint.x + "moveToPoint.y" + moveTo.boardPoint.y);

            /// ボード上での入れ替え
            Board.setElement(origin, origin.boardPoint);
            Board.setElement(moveTo, moveTo.boardPoint);

            onSwapBall(origin,moveTo);
        }

        private void onSwapBall(BallController origin, BallController moveTo)
        {
            ///moveTo側を指定ポイントへ移動するステートへ変更する
            moveTo.currentBallState.requestSwapState();
        }

        /// <summary>
        /// 操作中のボールがセル外に移動したか判定する
        /// </summary>
        private Direction[] getCurrentBallMovedDirection()
        {
            var result = new Direction[2]
            {
                Direction.None,
                Direction.None
            };

            if (currentOperatingBall == null)
                return result;

            var existingCell = convertPointToCell(currentOperatingBall.boardPoint);
            var operatingBallPos = currentOperatingBall.ownerTransform.position;
            result = existingCell.getDirectionFromCell(operatingBallPos);

            return result;
        }

        #region データ変換
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

        private Cell convertPointToCell(Point point)
        {
            var centerPos = gameObject.GetComponent<Transform>().position;

            var cell = new Cell();
            cell.cellPos = convertPointToPos(point);
            cell.cellSize = new Vector2(
                Renderer.size.x / XPOINTMAX,
                Renderer.size.y / YPOINTMAX);

            return cell;
        }
        #endregion
  
        #region DEBUG
        private void drawDebugLog()
        {
            ///盤面デバッグ表示
            string log = "";
            for (int y = 0; y < YPOINTMAX; y++)
            {
                for (int x = 0; x < XPOINTMAX; x++)
                {
                    log += Board.getElement(x,y).currentBallState.GetType() + " ";
                }

                log += "\n";
            }

            LogDrawer.drawLog(log);
        }

        #endregion

        #endregion

    }
}
