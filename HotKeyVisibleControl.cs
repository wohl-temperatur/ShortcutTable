using System;//EventArgs
using System.ComponentModel;//CancelEventArgs//ComponentResourceManager//IContainer//Container
using System.Drawing;//Icon
using System.Windows.Forms;//Keys//Form

namespace Jp.Co.Kensan.ShortcutTable
{
    /// <summary>
    /// プロセスを常駐させ、ホットキーにより表示をコントロールできるように拡張したクラス。
    /// </summary>
    public class HotKeyVisibleControl : HotKeyControl
    {
        public HotKeyVisibleControl(Form form, int fsModifiers, Keys vKey) : base(form, fsModifiers, vKey)
        {
            ComponentResourceManager res = new ComponentResourceManager(this.GetType());
            IContainer container = new Container();
            NotifyIcon icon = new NotifyIcon(container);
            icon.Icon = (Icon)res.GetObject("NotifyIconName");
            icon.Text = "ショートカットテーブル";
            icon.Visible = true;
            setContextMenu(icon, form);
            form.Disposed += delegate (object sender, EventArgs e)
            {
                icon.Visible = false;
                container.Dispose();
            };
        }

        private void setContextMenu(NotifyIcon icon, Form form)
        {
            ContextMenu cmenu = icon.ContextMenu = new ContextMenu();
            cmenu.MenuItems.Add(new MyMenuItem(form));
            cmenu.MenuItems.Add(new MyMenuItemFinish(icon, form));
        }

        private class MyMenuItem : MenuItem
        {
            private Form form;
            public MyMenuItem(Form form)
            {
                this.form = form;
                Index = 0;
                Text = "表示(Alt+Q)";
            }
            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                form.Visible = true;
            }
        }
        private class MyMenuItemFinish : MenuItem
        {
            public MyMenuItemFinish(NotifyIcon icon, Form form)
            {
                Index = 1;
                Text = "終了";
                Click += delegate (object sender, EventArgs e)
                {
                    form.Dispose();
                };
            }
        }
    }
}
