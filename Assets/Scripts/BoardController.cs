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

        public void Start()
        {
            generateBall(BallType.Blue, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// ボールプレハブをインスタンス化し生成する
        /// </summary>     
        private void generateBall(BallType ballType, Vector3 pos, Quaternion rotation)
        {
            var newBall = Instantiate(BallPrefab, pos, rotation);
            var ballContorller = newBall.GetComponent<BallController>();
            if (ballContorller != null)
            {
                ballContorller.setSprite(selectBallSprite(ballType));
            }
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
    }
}