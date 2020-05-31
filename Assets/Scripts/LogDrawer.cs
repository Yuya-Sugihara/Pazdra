using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace app
{
    public class LogDrawer : MonoBehaviour
    {
        [SerializeField]
        private Text LogText = null;
        private static List<string> LogList = new List<string>();

        public void Awake()
        {
            LogText = gameObject.GetComponent<Text>();
        }

        public void LateUpdate()
        {
            clearLog();
            updateLog();
        }

        private void clearLog()
        {
            LogText.text = "";
        }

        private void updateLog()
        {
            LogList.ForEach(log => LogText.text += log);
            LogList.Clear();
        }

        ///  変数版
        public static void drawLog(object message)
        {
            LogList.Add(message.ToString());
        }

        /// 1次元配列版
        public static void drawLog(object[] logObjects)
        {
            var log = "";

            foreach(var logObject in logObjects)
            {
                log += logObject.ToString();
            }

            LogList.Add(log);
        }

        /// 2次元配列版
        public static void drawLog(object[,] logObjects)
        {
            string log = "";
            for (int y = 0; y < logObjects.GetLength(0); y++)
            {

                for (int x = 0; x < logObjects.GetLength(1); x++)
                {
                    log += logObjects.ToString();
                }

                log += "\n";
            }

            LogDrawer.drawLog(log);
        }
    }
}
