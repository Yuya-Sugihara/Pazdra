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

	public class BallController : MonoBehaviour
	{
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
        }
        /// <summary>
        /// 待機ステート
        /// </summary>
        public class StayState: BallStateBase
        {
            public StayState(BallController controller):
                base(controller)
            { }

            ///ボールを選択されたらステート変更
            public override void start()
            {
                base.start();
            }

            public override void update()
            {
                base.update();

                ///　選択されているか判定
                ///　とりあえず左クリックで判定
                if(Input.GetMouseButtonDown(0))
                {
                    /// 範囲判定
                    if (ownerController.isTouched())
                    {
                        Debug.Log("[StayState] changeState to Move");
                        nextState = new MoveState(ownerController);
                        return;
                    }
                }
            }

            public override void end()
            {
                base.end();
            }
        }

        /// <summary>
        /// 移動ステート
        /// </summary>
        public class MoveState: BallStateBase
        {
            public MoveState(BallController controller):
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

                ownerController.onSelectBall();                
            }

            public override void update()
            {
                base.update();

                if(Input.GetMouseButtonUp(0))
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

        #endregion

        #region プロパティ
        public BallType ballType { get; set; }
        public Point boardPoint { get; set; }
        public Transform ownerTransform { get; private set; }
        public BallStateBase currentBallState { get; private set; }
        #endregion

        #region フィールド
        private SpriteRenderer SpriteRenderer;
		#endregion

		#region MonoBehaviorメソッド
		public void Awake()
		{
			SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            ownerTransform = gameObject.GetComponent<Transform>();
		}

		void Start()
		{
            currentBallState = new StayState(this);
            currentBallState.start();
        }


		void Update()
		{
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
        public void setSprite(Sprite sprite)		{
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
        private Vector3 adjustPos(Vector3 desirePos)
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
            Debug.Log("onSelectBall");
        }

        public void offSelectBall()
        {
            SpriteRenderer.sortingLayerName = "Ball";            
        }
        #endregion

        #region 非公開メソッド
        private void moveBall()
        {
            /// 入力処理
            /// ボールをタッチされているか
            /// タッチされていたら移動ステートへ
            /// ステート変更
            /// スプライト移動
            /// 範囲判定
            /// 置く
        }
        #endregion
    }
}
