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
        public static PuzzleManager instance
        {
            get
            {
                return Instance;
            }
        }

        private static PuzzleManager Instance;
        private BoardController BoardController;


        #region MonoBehaviorメソッド
        public void Awake()
        {
            if(Instance == null)
                Instance = this;
        }
        #endregion

        #region 公開メソッド

        #region 登録、解除
        public void registerBoardController(BoardController controller)
        {
            BoardController = controller;
        }

        public void registerCurrentOperationBall(BallController controller)
        {
            BoardController.registerOperatingBall(controller);
        }

        public void unregisterCurrentOperationBall()
        {
            BoardController.unregisterOperatingBall();
        }

        #region データアクセサ
        public Vector3 getBoardPos()
        {
            return BoardController.getBoardPos();
        }

        public Vector2 getBoardSize()
        {
            return BoardController.getBoardSize();
        }
        #endregion

        #endregion




    }
}
