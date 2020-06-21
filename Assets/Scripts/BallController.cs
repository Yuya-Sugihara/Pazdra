using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace app
{
	#region 定義
	public enum BallType
	{
		Red,
		Blue,
		Green,
		Yellow,
		Purple,
		Pink,
        None
	};
    #endregion

    #region 定義
    public class BallStateBase
    {
        public BallStateBase nextState { get; set; }
        protected BallController ownerController { get; private set; }

        public BallStateBase(BallController controller)
        {
            nextState = null;

            ownerController = controller;
        }

        public virtual void start()
        {
            nextState = null;
        }

        public virtual void update()
        { }

        public virtual void end()
        { }

        /// <summary>
        /// 複数のステート移動リクエストが必要な場合はenumで指定できるようにする
        /// </summary>
        public void requestSwapState()
        {
            nextState = new SwapState(ownerController);
        }
    }

    /// <summary>
    /// 待機ステート
    /// </summary>
    public class StayState : BallStateBase
    {
        private Vector3 TargetPos;

        public StayState(BallController controller) :
            base(controller)
        { }

        ///ボールを選択されたらステート変更
        public override void start()
        {
            base.start();

            TargetPos = ownerController.getBoardPos();
        }

        public override void update()
        {
            base.update();

            ///　選択されているか判定
            ///　とりあえず左クリックで判定
            if (Input.GetMouseButtonDown(0))
            {
                /// 範囲判定
                if (ownerController.isTouched())
                {
                    Debug.Log("[StayState] changeState to Move");
                    nextState = new MoveState(ownerController);
                    return;
                }
            }

            move();
        }

        public override void end()
        {
            base.end();
        }

        public void move()
        {
            var currentPos = ownerController.ownerTransform.position;
            var nextPos = Vector3.Lerp(currentPos, TargetPos, 0.3f);

            ownerController.ownerTransform.position = nextPos;
        }
    }

    /// <summary>
    /// 移動ステート
    /// </summary>
    public class MoveState : BallStateBase
    {
        public MoveState(BallController controller) :
            base(controller)
        { }

        /// <summary>
        /// 入力が続いている間入力されている場所へ移動する
        /// 入力が終わるとステート変更
        /// 開始時に操作時間の初期化→更新していく
        /// </summary>
        public override void start()
        {
            base.start();

            ///　操作中ボールの登録
            PuzzleManager.instance.registerCurrentOperationBall(ownerController);
            ownerController.onSelectBall();
        }

        public override void update()
        {
            base.update();

            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("[MoveState] changeState to Stay");
                nextState = new StayState(ownerController);
                return;
            }

            move();
        }

        public override void end()
        {
            base.end();

            PuzzleManager.instance.unregisterCurrentOperationBall();
            ownerController.offSelectBall();
        }

        private void move()
        {
            var inputPos = Input.mousePosition;

            var desirePos = Camera.main.ScreenToWorldPoint(inputPos);
            desirePos.z = 0.0f;
            ///ここで範囲盤面範囲から座標調整を行う
            var adjustedPos = ownerController.adjustPos(desirePos);

            var currentPos = ownerController.transform.position;
            var targetPos = Vector3.Lerp(currentPos, adjustedPos, 0.2f);
            ownerController.transform.position = targetPos;
        }
    }

    /// <summary>
    /// 入れ替わりステート
    /// </summary>
    public class SwapState : BallStateBase
    {
        private float SwapTime = 0.0f;
        private float SwapTimeMax = 0.1f;
        private Vector3 PrevPos = Vector3.zero;
        private Vector3 TargetPos = Vector3.zero; 

        public SwapState(BallController controller) :
            base(controller)
        { }

        /// <summary>
        /// 入力が続いている間入力されている場所へ移動する
        /// 入力が終わるとステート変更
        /// 開始時に操作時間の初期化→更新していく
        /// </summary>
        public override void start()
        {
            base.start();

            /// タイマー初期化
            SwapTime = 0.0f;
            PrevPos = ownerController.transform.position;
            TargetPos = ownerController.getBoardPos();
        }

        public override void update()
        {
            base.update();

            ///終了
            if(SwapTime >= SwapTimeMax)
            {
                nextState = new StayState(ownerController);
                return;
            }

            move();
        }

        public override void end()
        {
            base.end();
        }

        private void move()
        {
            SwapTime += Time.deltaTime;
            var nextPos = Vector3.Lerp(PrevPos, TargetPos, SwapTime/SwapTimeMax);

            ownerController.ownerTransform.position = nextPos;
        }
    }

    #endregion

    public class BallController : MonoBehaviour
	{
        public enum SwapReasonType
        {
            Move,   /// 移動した
            Swap,   /// 入れ替えを要求された
            None,
        }

        #region プロパティ
        public BallType ballType { get; set; }
        public Point boardPoint { get; set; }
        public Transform ownerTransform { get; private set; }
        public BallStateBase currentBallState { get; private set; }
        public bool canSwap
        {
            get { return currentBallState.GetType() != typeof(SwapState); }
        }
        public bool isOperating
        {
            get { return currentBallState.GetType() == typeof(MoveState); }
        }
        #endregion

        #region フィールド
        private SpriteRenderer SpriteRenderer;
        ///時間制御に使用する
        private bool isMoved;
        #endregion

		#region MonoBehaviorメソッド
		public void Awake()
		{
			SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            ownerTransform = gameObject.GetComponent<Transform>();
            isMoved = false;
        }

		void Start()
		{
            currentBallState = new StayState(this);
            currentBallState.start();
        }


		void Update()
		{
            if (currentBallState == null)
                Debug.Log("CurrentBallState is null");

            currentBallState.update();

            ///　ステートの変更
            if(currentBallState.nextState != null)
            {
                currentBallState.end();
                currentBallState = currentBallState.nextState;
                currentBallState.start();
            }
        } 
        #endregion

        #region 公開メソッド
        public void setSprite(Sprite sprite)
        {
			if (sprite == null)
			{
				Debug.LogError("[BallController] setSprite(): 引数がnullです。");
				return;
			}

			if (SpriteRenderer == null)
			{
				SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
			}

            SpriteRenderer.sprite = sprite;

		}

        public bool isTouched()
        {
            var inputPos = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(inputPos);
            var result = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);

            if (result && result.transform.gameObject == gameObject)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 盤面にはみ出さないように移動座標を調整する
        /// </summary>
        public Vector3 adjustPos(Vector3 desirePos)
        {
            var result = desirePos;

            var boardPos = PuzzleManager.instance.getBoardPos();
            var boardSize = PuzzleManager.instance.getBoardSize();
            var ballRadius = SpriteRenderer.size * 0.5f;

            ///縦方向のクランプ ボールの半径を考慮
            var boardUp = boardPos.y + boardSize.y * 0.5f;
            var boardBottom = boardPos.y - boardSize.y * 0.5f;
            result.y = Mathf.Clamp(result.y, boardBottom + ballRadius.y, boardUp - ballRadius.y);

            ///横方向のクランプ　ボールの半径を考慮
            var boardLeft = boardPos.x - boardSize.x * 0.5f;
            var boardRight = boardPos.x + boardSize.x * 0.5f;
            result.x = Mathf.Clamp(result.x, boardLeft + ballRadius.x, boardRight - ballRadius.x);

            return result;
        }

        /// <summary>
        /// ボールが選択された時の処理
        /// </summary>
        public void onSelectBall()
        {
            ///描画を優先する
            SpriteRenderer.sortingLayerName = "SelectedBall";

            ///操作対象ボールに設定
            PuzzleManager.instance.registerCurrentOperationBall(this);
        }

        public void offSelectBall()
        {
            /// 描画優先を解除する
            SpriteRenderer.sortingLayerName = "Ball";

            /// 操作対象ボールを解除する
            PuzzleManager.instance.unregisterCurrentOperationBall();
            PuzzleManager.instance.notifyOperationEnd();
        }

        public Vector3 getBoardPos()
        {
            return PuzzleManager.instance.getBoardPos(boardPoint);
        }

        public void onSwap(SwapReasonType reason)
        {
            if(reason == SwapReasonType.Move)
            {
                ///　移動したよ
                isMoved = true;
            }
            else if(reason == SwapReasonType.Swap)
            {
                /// 入れ替えステートに入るよ
                currentBallState.requestSwapState();
            }

        }
        #endregion

        #region 非公開メソッド
       
        #endregion
    }
}
