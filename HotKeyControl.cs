using System;//IntPtr
using System.ComponentModel;//CancelEventArgs
using System.Runtime.InteropServices;//DllImport
using System.Windows.Forms;//Keys//Form

namespace Jp.Co.Kensan.ShortcutTable
{
    public class HotKeyControl
    {
        [DllImport("user32")]
        static extern int RegisterHotKey(IntPtr hwnd, int id, int fsModifiers, Keys vKey);
        [DllImport("user32")]
        static extern int UnregisterHotKey(IntPtr hwnd, int id);
        [DllImport("kernel32", EntryPoint = "GlobalAddAtomA")]
        static extern short GlobalAddAtom(string lpString);
        [DllImport("kernel32")]
        static extern short GlobalDeleteAtom(short nAtom);

        /// <summary> OSからのホットキーメッセージ(Message#Msg) </summary>
        public static readonly int INT_MSG_HOTKEY = 0x312;
        /// <summary> OSからのホットキーLParam(Message#LParam.ToInt32())の値、取得専用 </summary>
        public readonly int INT_LPARAM_HOTKEY;

        //コンストラクタで受け取る
        protected Form form;
        private int fsModifiers;
        private Keys vKey;

        /// <summary>
        /// ホットキーの登録・解除を行う。
        /// ホットキー入力時の処理は、Form#WndProc(ref Message m)をオーバーライドして記述して下さい。
        /// </summary>
        /// <param name="form">フォームインスタンス</param>
        /// <param name="fsModifiers">
        /// MODキー番号。
        /// 0x01:Altキー,0x02:Ctrlキー,0x04:Shiftキー,0x08:Winキー
        /// </param>
        /// <param name="vKey">Keys.キー</param>
        public HotKeyControl(Form form, int fsModifiers, Keys vKey)
        {
            this.form = form;
            this.fsModifiers = fsModifiers;
            this.vKey = vKey;
            INT_LPARAM_HOTKEY = ((int)vKey << 16) + fsModifiers;
            controlRegisterHotKey();
        }

        //ホットキーの登録・解除
        private void controlRegisterHotKey()
        {
            short shortHotKeyID = GlobalAddAtom("GlobalHotKey" + GetHashCode().ToString());
            RegisterHotKey(form.Handle, shortHotKeyID, fsModifiers, vKey);
            form.Closing += delegate (object sender, CancelEventArgs e)
            {
                UnregisterHotKey(form.Handle, shortHotKeyID);
                GlobalDeleteAtom(shortHotKeyID);
            };
        }
    }
}
