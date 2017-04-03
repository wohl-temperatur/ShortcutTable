using System;
using System.Drawing;
using System.Windows.Forms;

namespace Jp.Co.Kensan.ShortcutTable
{

    public class SettingForm : Form
    {
        private int intActiveTab;
        private TextBox tbCate;
        private TextBox[] tbsLabel = new TextBox[26];
        private TextBox[] tbsLink = new TextBox[26];

        private MainFlowLayoutPanel mflp;
        public SettingForm(MainFlowLayoutPanel mflp)
        {
            this.mflp = mflp;
            intActiveTab = mflp.getActiveTab();
            Text = "ショートカット設定「" + intActiveTab + '」';
            Width = 687;
            Height = 396;
            Opacity = 0.5 + 0.25 + 0.125 + 0.0625;//+0.03125+0.015625;
            KeyPreview = true;

            setCategory();
            setLabelAndLink();
            setButton();
        }

        private void OnClick_OK(object sender, EventArgs e)
        {
            string[] labels = new string[tbsLabel.Length];
            string[] links = new string[tbsLink.Length];
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = tbsLabel[i].Text;
                links[i] = tbsLink[i].Text;
            }
            mflp.updateData(intActiveTab, tbCate.Text, labels, links);
            Dispose();
            //クリックしたと言うことはMainFormは非表示状態のはず
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            int key = (int)e.KeyChar;
            if (key == 0x1b) //ESCキー
            {
                Dispose();
            }
        }

        private void setButton()
        {
            Font font = new Font(Font.FontFamily, 10);

            Button btOK = new Button();
            Controls.Add(btOK);
            btOK.Text = "保 存";
            btOK.TextAlign = ContentAlignment.MiddleCenter;
            btOK.Font = font;
            btOK.Left = 511;
            btOK.Top = 5;
            btOK.Width = 75;
            btOK.Height = 30;
            btOK.Click += OnClick_OK;

            Button btCancel = new Button();
            Controls.Add(btCancel);
            btCancel.Text = "キャンセル";
            btCancel.TextAlign = ContentAlignment.MiddleCenter;
            btCancel.Font = font;
            btCancel.Left = btOK.Right + 5;
            btCancel.Top = btOK.Top;
            btCancel.Width = btOK.Width;
            btCancel.Height = btOK.Height;
            btCancel.Click += delegate (object sender, EventArgs e)
            {
                Dispose();
            };
        }

        private void setCategory()
        {
            Label l = new Label();
            Controls.Add(l);
            l.TextAlign = ContentAlignment.MiddleRight;
            l.Text = "カテゴリー名：";
            //l.Left = 30;
            l.Top = 5;
            l.Width = 75;

            Controls.Add(tbCate = new TextBox());
            tbCate.Text = mflp.getCategory(-1);
            tbCate.Left = l.Right;
            tbCate.Top = l.Top;
            tbCate.Width = 200;
        }

        private void setLabelAndLink()
        {
            Label lbKey = new Label();
            Controls.Add(lbKey);
            lbKey.TextAlign = ContentAlignment.MiddleRight;
            lbKey.Text = "キー";
            lbKey.Top = 35;
            lbKey.Width = 30;

            Label lbLabel = new Label();
            Controls.Add(lbLabel);
            lbLabel.TextAlign = ContentAlignment.MiddleLeft;
            lbLabel.Text = " 表示名";
            lbLabel.Left = lbKey.Right;
            lbLabel.Top = lbKey.Top;
            lbLabel.Width = 100;

            Label lbLink = new Label();
            Controls.Add(lbLink);
            lbLink.TextAlign = ContentAlignment.MiddleLeft;
            lbLink.Text = " リンクパス";
            lbLink.Left = lbLabel.Right;
            lbLink.Top = lbLabel.Top;
            lbLink.Width = 200;

            Label lbKey2 = new Label();
            Controls.Add(lbKey2);
            lbKey2.TextAlign = lbKey.TextAlign;
            lbKey2.Text = lbKey.Text;
            lbKey2.Left = lbLink.Right + 5;
            lbKey2.Top = lbLink.Top;
            lbKey2.Width = lbKey.Width;

            Label lbLabel2 = new Label();
            Controls.Add(lbLabel2);
            lbLabel2.TextAlign = lbLabel.TextAlign;
            lbLabel2.Text = lbLabel.Text;
            lbLabel2.Left = lbKey2.Right;
            lbLabel2.Top = lbKey2.Top;
            lbLabel2.Width = lbLabel.Width;

            Label lbLink2 = new Label();
            Controls.Add(lbLink2);
            lbLink2.TextAlign = lbLink.TextAlign;
            lbLink2.Text = lbLink.Text;
            lbLink2.Left = lbLabel2.Right;
            lbLink2.Top = lbLabel2.Top;
            lbLink2.Width = lbLink.Width;

            Label tmp = lbKey;
            string[] labels = mflp.getLabels(-1);
            string[] links = mflp.getLinks(-1);
            for (int i = 0; i < tbsLabel.Length; i++)
            {
                Label l = new Label();
                Controls.Add(l);
                l.TextAlign = ContentAlignment.MiddleRight;
                l.Text = char.ConvertFromUtf32('A' + i) + '：';
                l.Left = tmp.Left;
                l.Top = tmp.Bottom;
                l.Width = tmp.Width;

                Controls.Add(tbsLabel[i] = new TextBox());
                tbsLabel[i].Text = labels[i];
                tbsLabel[i].Left = l.Right;
                tbsLabel[i].Top = l.Top;
                tbsLabel[i].Width = lbLabel.Width;
                Controls.Add(tbsLink[i] = new TextBox());
                tbsLink[i].Text = links[i];
                tbsLink[i].Left = tbsLabel[i].Right;
                tbsLink[i].Top = l.Top;
                tbsLink[i].Width = lbLink.Width;

                tmp = (i == tbsLabel.Length / 2 - 1) ? lbKey2 : l;
            }
        }
    }
}
