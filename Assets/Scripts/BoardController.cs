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
        private BallController[,] Board;
        #endregion

        public void Start()
        {
            /// 盤面生成
            generateBoard();
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
                    generateBall(BallType.Red, new BoardPoint(x, y));
                }
            }
        }
        /// <summary>
        /// ボールプレハブをインスタンス化し生成する
        /// </summary>     
        private void generateBall(BallType ballType, BoardPoint point)
        {
            /// BoardPointの数値から、具体的な座標を計算し、生成する
            var newBallPos = convertPointToPos(point);
            var newBall = Instantiate(BallPrefab, newBallPos, Quaternion.identity);
            var ballContorller = newBall.GetComponent<BallController>();
            if (ballContorller != null)
            {
                ballContorller.setSprite(selectBallSprite(ballType));
            }
        }
        
        /// <summary>
        /// ボード位置のインデックスを座標に変換する
        /// </summary>
        private Vector3 convertPointToPos(BoardPoint point)
        {
            ///とりあえずマージン50で変換
            var margin = 1.88f;
            return new Vector3(point.x * margin, point.y * margin, 0.0f);
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
