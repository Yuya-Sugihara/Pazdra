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
		Pink
	};
	#endregion

	public class BallController : MonoBehaviour
	{
        #region プロパティ
        public BallType ballType { get; set; }
        public BoardPoint boardPoint { get; set; }
        #endregion

        #region フィールド
        private SpriteRenderer SpriteRenderer;
        
		#endregion

		#region MonoBehaviorメソッド
		public void Awake()
		{
			SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

		}

		void Start()
		{

		}


		void Update()
		{
            
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
		#endregion

	}
}