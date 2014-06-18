/*
 * http://www.buildinsider.net/small/leapmotioncs/01
 * 
 * Natural Software　中村 薫 さんの記事
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Leap;

namespace net.tikomo
{
    class LeapListener : Listener
    {
        // UIスレッドに戻すためのコンテキスト
        SynchronizationContext context = SynchronizationContext.Current;

        public delegate void LeapListenerEvent(Controller leap);

        // イベント・ハンドラー
        public event LeapListenerEvent OnConnectEvent;
        public event LeapListenerEvent OnDisconnectEvent;
        public event LeapListenerEvent OnInitEvent;
        public event LeapListenerEvent OnExitEvent;
        public event LeapListenerEvent OnFocusGainedEvent;
        public event LeapListenerEvent OnFocusLostEvent;
        public event LeapListenerEvent OnFrameEvent;

        // OnConnectメソッド： コントローラーがLeapに接続して、モーション・トラッキング・データ
        //（motion tracking data）のフレーム（frames。詳細後述）を送信開始する準備が整ったとき
        public override void OnConnect(Controller leap)
        {
            Invoke(leap, OnConnectEvent);

        }

        // OnDisconnectメソッド： コントローラーがLeapから接続解除した場合（例えば、Leapデバイス
        // が取り外されたり、Leapソフトウェアが強制終了したりした場合）
        public override void OnDisconnect(Controller leap)
        {
            Invoke(leap, OnDisconnectEvent);
        }

        // OnInitメソッド： リスナーの登録先のコントローラーが初期化されたときに、1回だけ
        public override void OnInit(Controller leap)
        {
            Invoke(leap, OnInitEvent);
        }

        // OnExitメソッド： リスナーがコントローラーから削除されたとき
        //（もしくは、コントローラーが破棄されたとき）
        public override void OnExit(Controller leap)
        {
            Invoke(leap, OnExitEvent);
        }

        // OnFocusGainedメソッド（＝アプリが前面になったときに発生）
        public override void OnFocusGained(Controller leap)
        {
            Invoke(leap, OnFocusGainedEvent);
        }

        // OnFocusLostメソッド（＝アプリが前面ではなくなったときに発生）
        public override void OnFocusLost(Controller leap)
        {
            Invoke(leap, OnFocusLostEvent);
        }

        // OnFrameメソッド： モーション・トラッキング・データの新しいフレーム
        // が利用可能になったとき
        public override void OnFrame(Controller leap)
        {
            Invoke(leap, OnFrameEvent);
        }

        // イベントを発行する
        private void Invoke(Controller leap, LeapListenerEvent handler)
        {
            // UIスレッドに同期的に処理を戻す
            context.Post(state =>
            {
                if (handler != null)
                {
                    handler(leap);
                }
            }, null);
        }
    }
}



    
    

