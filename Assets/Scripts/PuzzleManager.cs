using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace app
{
    /// <summary>
    /// 盤面全体を管理するマネージャクラス
    /// </summary>
    public class PuzzleManager : MonoBehaviour
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

        #region MonoBehaviorメソッド
        public void Awake()
        {

        }

        public void Start()
        {
            /// ボールスプライトを読み込む
            /// 盤面コントローラーを作成
            /// ボール生成
            generateBall(BallType.Red);
            /// 生成したボールをコントローラーに設定
        }

        public void Update()
        {

        }

        #endregion

        private void generateBall(BallType ballType)
        {
            var newBall = Instantiate(BallPrefab, Vector3.zero, Quaternion.identity);
            var ballContorller = newBall.GetComponent<BallController>();
            if (ballContorller != null)
            {
                ballContorller.setSprite(selectBallSprite(ballType));
            }
        }

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


    }
}
