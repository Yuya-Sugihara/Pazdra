using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace app
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None,
    }

    /// <summary>
    /// 多次元配列を扱い易くするクラス
    /// </summary>
    public class MatrixArray<T>
       where T:class
    {
        /*
        * (0,0) (1,0) (2,0) (3,0) (4,0) (5,0) ...
        * (0,1) (1,1) (2,1) (3,1) (4,1) (5,1) ...
        * (0,2) (1,2) (2,2) (3,2) (4,2) (5,2) ...
        * (0,3) (1,3) (2,3) (3,3) (4,3) (5,3) ...
        * (0,4) (1,4) (2,4) (3,4) (4,4) (5,4) ...
        * ...
        */
        private T[,] Matrix;
        private int XMAX = -1;
        private int YMAX = -1;

        public MatrixArray(int xMax, int yMax)
        {
            XMAX = xMax;
            YMAX = yMax;

            Matrix = new T[YMAX, XMAX];
        }

        /// <summary>
        /// 座標指定アクセサ
        /// </summary>
        public T getElement(Point point)
        {
            return getElement(point.x, point.y);
        }

        /// <summary>
        /// 座標指定アクセサ
        /// </summary>
        public T getElement(int x, int y)
        {
            ///　範囲外判定
            if(x < 0 || XMAX-1 < x)
            {
                Debug.LogError("MatrixArray: Xの値が範囲外の物をアクセスされました。");
                return null;
            }

            if (y < 0 || YMAX - 1 < y)
            {
                Debug.LogError("MatrixArray: Yの値が範囲外の物をアクセスされました。");
                return null;
            }

            return Matrix[y, x];
        }

        /// <summary>
        /// 方向指定アクセサ
        /// </summary>
        public T getElementByDirection(Point point, Direction dir,int length = 1)
        {
            Point offset = new Point(0,0);
            switch(dir)
            {
                case Direction.Up:
                    offset = new Point(0, -length);
                    break;
                case Direction.Down:
                    offset = new Point(0, length);
                    break;
                case Direction.Left:
                    offset = new Point(-length, 0);
                    break;
                case Direction.Right:
                    offset = new Point(length, 0);
                    break;
                default:
                    break;
            }

            return getElement(point + offset);
        }

        public void setElement(T element,Point point)
        {
            setElement(element, point.x, point.y);
        }

        /// <summary>
        /// 座標指定アクセサ
        /// </summary>
        public void setElement(T element,int x, int y)
        {
            ///　範囲外判定
            if (x < 0 || XMAX - 1 < x)
            {
                Debug.LogError("MatrixArray: Xの値が範囲外の物をアクセスされました。");
                return;
            }

            if (y < 0 || YMAX - 1 < y)
            {
                Debug.LogError("MatrixArray: Yの値が範囲外の物をアクセスされました。");
                return;
            }

            Matrix[y, x] = element;
        }
    }
}
