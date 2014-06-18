/*
 * http://www.buildinsider.net/small/leapmotionfirstimp/01
 * 
 * 
 * 
 * v2から？ empty -> isempty に変更になってる
 * 
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Leap;
using System.Diagnostics; // Controller
using net.tikomo;
using System.Windows.Ink;

namespace WpfLeapSample7
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        Controller leapController;
        LeapListener leapListener;
        // int cnt = 0;

        //
        // フォームに指を表示する
        //
        DrawingAttributes touchIndicator = new DrawingAttributes();
        StylusPoint touchPoint;

        float windowWidth = 1920;
        float windowHeight = 1080;
        double x;
        double y;
        double tx;
        double ty;

        int FingersCount;
        string Message;
        int Index;

        public MainWindow()
        {
            InitializeComponent();

            // サンプルのリスナーとコントローラーを作成
            leapListener = new LeapListener();
            leapController = new Controller();
            
            // サンプルのリスナーが、コントローラーからイベントを受け取るよう設定
            leapController.AddListener(leapListener);

            leapListener.OnFrameEvent += listener_FrameReady;
            leapListener.OnConnectEvent += listener_onConnect;
            leapListener.OnDisconnectEvent += listener_onDisconnect;
            leapListener.OnInitEvent += listener_onInit;
            leapListener.OnExitEvent += listener_onExit;

            //
            // 指を描画するため
            //
            //CompositionTarget.Rendering += Update;

            touchIndicator.Width = 20;
            touchIndicator.Height = 20;
            touchIndicator.StylusTip = StylusTip.Ellipse;
            //this.TextBlock1.Foreground = new SolidColorBrush(Colors.White);
            //this.TextBlock2.Foreground = new SolidColorBrush(Colors.White);

        }

        void listener_onInit(Controller leap)
        {
            // ここではWPFのUIオブジェクトも触ることができる

            Debug.WriteLine("onInit");
        }
        
        void listener_onExit(Controller leap)
        {
            // ここではWPFのUIオブジェクトも触ることができる

            Debug.WriteLine("onExit");
        }

        void listener_onConnect(Controller leap)
        {
            // ここではWPFのUIオブジェクトも触ることができる

            Debug.WriteLine("onConnect");

            //
            // サークル（TYPECIRCLE）： 指で円を描く動作
            // キー・タップ（TYPEKEYTAP）： 指で（あたかもキーを押しているように）「下方向」にタップする動作
            // スクリーン・タップ（TYPESCREENTAP）： 指で（あたかもスクリーンを押しているように）「前方向」にタップする動作
            // スワイプ（TYPESWIPE）： 指を伸ばした状態の手で直線を描く動作
            //
            leapController.EnableGesture(Gesture.GestureType.TYPECIRCLE);
            leapController.EnableGesture(Gesture.GestureType.TYPEKEYTAP);
            leapController.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
            leapController.EnableGesture(Gesture.GestureType.TYPESWIPE);
        }
        
        void listener_onDisconnect(Controller leap)
        {
            // ここではWPFのUIオブジェクトも触ることができる

            Debug.WriteLine("onDisconnect");
        }
        
        void listener_FrameReady(Controller leap)
        {
            paintCanvas.Strokes.Clear();



            // ここではWPFのUIオブジェクトも触ることができる
            // cnt++;
            // Debug.WriteLine("Hello World " + cnt.ToString("000"));

            //
            // フレーム情報を取得する
            //
            
            //
            // Handクラス： 検出された手の物理的な特徴をレポート
            // HandListクラス： Handオブジェクトのリスト。FrameオブジェクトのHandsプロパティで取得できる
            // Fingerクラス： トラッキングしている指を表現
            // FingerListクラス： Fingerオブジェクトのリスト。FrameオブジェクトのFingersプロパティで取得できる
            // Toolクラス： トラッキングしている道具（例えばペンなど）を表現
            // ToolListクラス： Toolオブジェクトのリスト。FrameオブジェクトのToolsプロパティで取得できる
            // Gestureクラス： 認識されたユーザーの動きを表現
            // GestureListクラス： Gestureオブジェクトのリスト。FrameオブジェクトのGesturesメソッドで取得できる
            // Vector構造体： 3次元ベクトル情報を表現
            //

            // 最新のフレームを取得して、基本情報をレポートする
            Leap.Frame frame = leapController.Frame();

            /*
            Debug.WriteLine("フレームID: " + frame.Id
                  + ", タイムスタンプ: " + frame.Timestamp
                  + ", 手の数: " + frame.Hands.Count
                  + ", 指の数: " + frame.Fingers.Count
                  + ", 道具の数: " + frame.Tools.Count
                  + ", ジェスチャーの数: " + frame.Gestures().Count);
            */
            /*
            if (!frame.Hands.IsEmpty)
            {
                // 1つ目の手を取得
                Hand hand = frame.Hands[0];

                // 手に指があるかチェック
                FingerList fingers = hand.Fingers;
                if (!fingers.IsEmpty)
                {
                    // 手の指先の平均的な位置を計算
                    Leap.Vector avgPos = Leap.Vector.Zero;
                    foreach (Finger finger in fingers)
                    {
                        avgPos += finger.TipPosition;
                    }
                    avgPos /= fingers.Count;
                    Debug.WriteLine("手には、" + fingers.Count
                      + "本の指があり、指先の平均的な位置は: " + avgPos);
                }

                // 手の球半径と手のひらの位置を取得
                Debug.WriteLine("手の球半径: " + hand.SphereRadius.ToString("n2")
                      + " mm, 手のひらの位置: " + hand.PalmPosition);

                // 手のひらの法線ベクトルと（手のひらから指までの）方向を取得
                Leap.Vector normal = hand.PalmNormal;
                Leap.Vector direction = hand.Direction;

                // 手のピッチとロールとヨー角を計算
                Debug.WriteLine("手のピッチ: " + direction.Pitch * 180.0f / (float)Math.PI + " 度, "
                      + "ロール: " + normal.Roll * 180.0f / (float)Math.PI + " 度, "
                      + "ヨー角: " + direction.Yaw * 180.0f / (float)Math.PI + " 度");
            }
            */
            //
            // ジェスチャの取得
            //
            // 全ジェスチャーを取得して、個別に処理する
            GestureList gestures = frame.Gestures();
            for (int i = 0; i < gestures.Count; i++)
            {
                Gesture gesture = gestures[i];

                switch (gesture.Type)
                {
                    case Gesture.GestureType.TYPECIRCLE:
                        CircleGesture circle = new CircleGesture(gesture);

                        // 回転方向を計算
                        String clockwiseness;
                        if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 4)
                        {
                            // 角度が90度以下なら、時計回り
                            clockwiseness = "時計回り";
                        }
                        else
                        {
                            clockwiseness = "反時計回り";
                        }

                        float sweptAngle = 0;

                        // 最後のフレームから回った角度を計算
                        if (circle.State != Gesture.GestureState.STATESTART)
                        {
                            CircleGesture previousUpdate = new CircleGesture(leapController.Frame(1).Gesture(circle.Id));
                            sweptAngle = (circle.Progress - previousUpdate.Progress) * 360;
                        }

                        this.TextBlock2.Text = "サークル";

                        Debug.WriteLine("サークルID: " + circle.Id
                                 + ", " + circle.State
                                 + ", 進度: " + circle.Progress
                                 + ", 半径: " + circle.Radius
                                 + ", 角度: " + sweptAngle
                                 + ", " + clockwiseness);
                        break;

                    case Gesture.GestureType.TYPESWIPE:
                        SwipeGesture swipe = new SwipeGesture(gesture);
                        Debug.WriteLine("スワイプID: " + swipe.Id
                                 + ", " + swipe.State
                                 + ", 位置: " + swipe.Position
                                 + ", 方向: " + swipe.Direction
                                 + ", スピード: " + swipe.Speed);

                        this.TextBlock2.Text = "スワイプ";

                        break;

                    case Gesture.GestureType.TYPEKEYTAP:
                        KeyTapGesture keytap = new KeyTapGesture(gesture);
                        Debug.WriteLine("キー・タップID: " + keytap.Id
                                 + ", " + keytap.State
                                 + ", 位置: " + keytap.Position
                                 + ", 方向: " + keytap.Direction);

                        this.TextBlock2.Text = "キー・タップ";

                        break;

                    case Gesture.GestureType.TYPESCREENTAP:
                        ScreenTapGesture screentap = new ScreenTapGesture(gesture);
                        Debug.WriteLine("スクリーン・タップID: " + screentap.Id
                                 + ", " + screentap.State
                                 + ", 位置: " + screentap.Position
                                 + ", 方向: " + screentap.Direction);

                        this.TextBlock2.Text = "スクリーン・タップ";

                        break;

                    default:
                        Debug.WriteLine("未知のジェスチャー・タイプです。");

                        this.TextBlock2.Text = "未知のジェスチャー・タイプです。";

                        break;
                }
            }

            if (!frame.Hands.IsEmpty || !frame.Gestures().IsEmpty)
            {
                Debug.WriteLine("");
            }


            //
            // 指の描画
            //
            // ここから、はじまり
            //

            // フレーム・データを取得する
            // このフレームでのLeap.FrameオブジェクトおよびLeap.InteractionBoxオブジェクトを取得する。
            // Leap.Frame frame = leap.Frame();

            // leap.Frame().InteractionBox は Leap Motionで認識できる可動範囲
            InteractionBox interactionBox = leap.Frame().InteractionBox;

            // PointableListオブジェクトをループで順に処理する
            foreach (Pointable pointable in leap.Frame().Pointables)
            {
                // ここで、伸ばしてない指をスルーする
                if (pointable.IsExtended == false)
                {
                    continue;
                }


                // Leap.InteractionBox.NormalizePoint()メソッドにLeap.Pointable.StabilizedTipPositionプロパティを渡してポインターの画面上の位置を取得する。
                Leap.Vector normalizedPosition = interactionBox.NormalizePoint(pointable.StabilizedTipPosition);

                // このサイトの説明よくわかる
                // http://www.buildinsider.net/small/leapmotioncs/05
                //
                // Intersect Point を利用した座標変換
                //Leap.Vector normalizedPosition = locatedScreen.Intersect( pointable, true );
                // Projection Point を利用した座標変換
                //Leap.Vector normalizedPosition = locatedScreen.Project( pointable.TipPosition, false );

                // 画面サイズ上でのポインターの位置が「0.0」～「1.0」の間で表される。
                // これを実際の画面サイズで割り出すが、Leap Motionのスクリーン座標系の原点が左下であるため
                // Y座標は高さの値から算出した値を引くことで、左上を原点とする座標にしている。
                float tx = normalizedPosition.x * windowWidth;
                float ty = windowHeight - normalizedPosition.y * windowHeight;

                StylusPoint touchPoint = new StylusPoint(tx, ty);
                StylusPointCollection tips = new StylusPointCollection(new StylusPoint[] { touchPoint });
                Stroke touchStroke = new Stroke(tips, touchIndicator);
                paintCanvas.Strokes.Add(touchStroke);

                touchIndicator.Color = Colors.Navy;
                x = touchPoint.X;
                y = touchPoint.Y;
                FingersCount = leap.Frame().Fingers.Count; // 指の数を取得する

                // 伸びている指の数が返る
                FingersCount = leap.Frame().Fingers.Count((f) => f.IsExtended);


                // TouchDistanceプロパティが「1」であれば、TouchZoneプロパティは「ZONENONE」
                // TouchDistanceプロパティが「1」以下、かつ「0」より大きい値であれば、TouchZoneプロパティは「ZONEHOVERING」
                // TouchDistanceプロパティが「0」以下、かつ「-1」より大きい値であれば、TouchZoneプロパティは「ZONETOUCHING」

                if (pointable.TouchDistance > 0 && pointable.TouchZone != Pointable.Zone.ZONENONE)
                {
                    touchIndicator.Color = Colors.Navy;
                    x = touchPoint.X;
                    y = touchPoint.Y;

                    FingersCount = leap.Frame().Fingers.Count; // 指の数を取得する

                    // 伸びている指の数が返る
                    FingersCount = leap.Frame().Fingers.Count((f) => f.IsExtended);

                    TextBlock1.Text = "伸ばしている指の数 = " + FingersCount.ToString();

                }
                // タッチ状態
                else if (pointable.TouchDistance <= 0)
                {
                    touchIndicator.Color = Colors.Red;

                    /*
                    if ((x > (double)redButton.GetValue(Canvas.LeftProperty)) && (x < (double)redButton.GetValue(Canvas.LeftProperty) + redButton.Width) && (y > (double)redButton.GetValue(Canvas.TopProperty)) && (y < (double)redButton.GetValue(Canvas.TopProperty) + redButton.Height))
                    {
                        Index = 1;
                    }

                    else if ((x > (double)blueButton.GetValue(Canvas.LeftProperty)) && (x < (double)blueButton.GetValue(Canvas.LeftProperty) + blueButton.Width) && (y > (double)blueButton.GetValue(Canvas.TopProperty)) && (y < (double)blueButton.GetValue(Canvas.TopProperty) + blueButton.Height))
                    {
                        Index = 2;
                    }

                    else if ((x > (double)greenButton.GetValue(Canvas.LeftProperty)) && (x < (double)greenButton.GetValue(Canvas.LeftProperty) + greenButton.Width) && (y > (double)greenButton.GetValue(Canvas.TopProperty)) && (y < (double)greenButton.GetValue(Canvas.TopProperty) + greenButton.Height))
                    {
                        Index = 3;
                    }

                    else if ((x > (double)yellowButton.GetValue(Canvas.LeftProperty)) && (x < (double)yellowButton.GetValue(Canvas.LeftProperty) + yellowButton.Width) && (y > (double)yellowButton.GetValue(Canvas.TopProperty)) && (y < (double)yellowButton.GetValue(Canvas.TopProperty) + yellowButton.Height))
                    {
                        Index = 4;
                    }

                    else if ((x > (double)blackButton.GetValue(Canvas.LeftProperty)) && (x < (double)blackButton.GetValue(Canvas.LeftProperty) + blackButton.Width) && (y > (double)blackButton.GetValue(Canvas.TopProperty)) && (y < (double)blackButton.GetValue(Canvas.TopProperty) + blackButton.Height))
                    {
                        Index = 5;
                    }
                    else
                    {
                        Index = 0; // ここでクリアする　でないと以前の状態を覚えている事になってしまう
                    }
                    if (FingersCount == 1)
                    {
                        switch (Index)
                        {
                            case 1:
                                //ShowArea.Background = new SolidColorBrush(Colors.Red);
                                //Message = "背景色は赤です。";

                                // クリックイベントを発生させても、ボタンを押した感じにならないな～
                                this.redButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));


                                break;
                            case 2:
                                ShowArea.Background = new SolidColorBrush(Colors.Blue);
                                Message = "背景色は青です。";
                                break;
                            case 3:
                                ShowArea.Background = new SolidColorBrush(Colors.Green);
                                Message = "背景色は緑です。";
                                break;
                            case 4:
                                ShowArea.Background = new SolidColorBrush(Colors.Gold);
                                Message = "背景色は黄です。";
                                break;
                            case 5:
                                ShowArea.Background = new SolidColorBrush(Colors.Black);
                                Message = "背景色は黒です。";
                                break;
                            default:
                                break;
                        }

                        TextBlock1.Text = Message;
                    }
                    else if (FingersCount == 5)
                    {

                        // ５本指で画面を白にする ... 無くてもいいと思う
                        ShowArea.Background = new SolidColorBrush(Colors.White);
                    }
                    */

                }
                // タッチ対象外
                else
                {
                    touchIndicator.Color = Colors.Gold;
                }
            }



        }
    }
}
