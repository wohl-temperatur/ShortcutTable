using System;//EventArgs
using System.Drawing;//Color
using System.Windows.Forms;//KeyPressEventArgs//Form//Message

namespace Jp.Co.Kensan.ShortcutTable
{

    public class MainForm : Form
    {
        private HotKeyControl hkc;
        private MainFlowLayoutPanel panel;

        public MainForm()
        {
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Opacity = 0;
            BackColor = Color.Black;
            Width = 652;
            Height = 476;
            ControlBox = false;
            ShowInTaskbar = false;
            KeyPreview = true;

            StartPosition = FormStartPosition.CenterScreen;
            hkc = new HotKeyVisibleControl(this, 0x01, Keys.Q);
            
            panel = new MainFlowLayoutPanel(this);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            Visible = false;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            int key = (int)e.KeyChar;
            if (key == 0x1b) //ESCキー
            {
                Visible = false;
            }
            else if (0x30 <= key && key <= 0x39) //0～9
            {
                panel.setTab(key - 0x30);
            }
            else if (0x41 <= key && key <= 0x5a) //A～Z
            {
                panel.clickButton(key - 0x41);
            }
            else if (0x61 <= key && key <= 0x7a) //a～z
            {
                panel.clickButton(key - 0x61);
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            Visible = false;
            panel.setTab(MainFlowLayoutPanel.INT_DEFAULT_TAB);
            Opacity = 0.875;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
            {
                TopMost = true;
            }
            else
            {
                panel.setTab(MainFlowLayoutPanel.INT_DEFAULT_TAB);
                Update();//非表示中に描画を更新
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == HotKeyControl.INT_MSG_HOTKEY)
            {
                int param = m.LParam.ToInt32();
                if (param == hkc.INT_LPARAM_HOTKEY)
                {
                    Visible = !Visible;
                }
            }
        }
    }
}
