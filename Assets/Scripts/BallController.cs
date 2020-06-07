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
            { }

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
                
                var worldPos = Camera.main.ScreenToWorldPoint(inputPos);
                worldPos.z = 0.0f;
                ownerController.transform.position = worldPos;
                /// todo: 線形補間
                //ownerController.transform.position = inputPos;
            }
        }

        #endregion

        #region プロパティ
        public BallType ballType { get; set; }
        public Point boardPoint { get; set; }
        public Transform ownerTransform { get; private set; }
        #endregion

        #region フィールド
        private SpriteRenderer SpriteRenderer;
        private BallStateBase CurrentBallState;
		#endregion

		#region MonoBehaviorメソッド
		public void Awake()
		{
			SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            ownerTransform = gameObject.GetComponent<Transform>();
		}

		void Start()
		{
            CurrentBallState = new StayState(this);
            CurrentBallState.start();
        }


		void Update()
		{
            CurrentBallState.update();

            ///　ステートの変更
            if(CurrentBallState.nextState != null)
            {
                CurrentBallState.end();
                CurrentBallState = CurrentBallState.nextState;
                CurrentBallState.start();
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
            ///入力座標の確認
            var inputPos = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(inputPos);
            var result = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
            if(result)
            {
                Debug.Log("touched ball: " + result.transform.GetComponent<BallController>().ballType);
                return true;
            }

            return false;
        }

        /// <summary>
        /// ボールが選択された時の処理
        /// </summary>
        public void onSelectBall()
        {
            ///描画を優先する
            //SpriteRenderer.sortingLayerName = "SelectedBall";
            //SpriteRenderer.enabled = false;
            Debug.Log("onSelectBall");
        }

        public void offSelectBall()
        {
            //SpriteRenderer.sortingLayerName = "Ball";
            //SpriteRenderer.enabled = true;
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
