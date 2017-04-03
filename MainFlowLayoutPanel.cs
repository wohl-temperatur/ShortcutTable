using System;
using System.Diagnostics;//Process
using System.Drawing;//Color;
using System.Drawing.Drawing2D;
using System.IO;//File
using System.Windows.Forms;//FlowLayoutPanel

namespace Jp.Co.Kensan.ShortcutTable
{

    public sealed class MainFlowLayoutPanel : FlowLayoutPanel
    {
        #region プロパティ
        public static int INT_DEFAULT_TAB { get; set; } = 0;
        #endregion

        #region データフィールド
        // private const string STR_FPATH_CATEGORY = @"category.tsv";
        private const string STR_FNAME = @"data";

        private static MainForm form;
        private static TabLabel[] lbs = new TabLabel[10];
        private static int intActiveTab;
        private static CategoryLabel lbCate;
        // private static string[] strCate = new String[10];
        private static LinkButton[] bts = new LinkButton[26];

        private static bool flgCreated;
        #endregion


        /// <summary>
        /// Formのコンストラクタからインスタンス生成するだけ。
        /// 2回目以降エラー返し。
        /// </summary>
        /// <param name="formParent">null不可</param>
        public MainFlowLayoutPanel(MainForm formParent)
        {
            if (flgCreated)
            {
                throw new Exception("一回しか呼べません");
            }
            else
            {
                flgCreated = true;
            }

            form = formParent;
            form.Controls.Add(this);
            FlowDirection = FlowDirection.TopDown;
            Width = 800;
            Height = 600;

            new TabFlowLayoutPanel(this);
            lbCate = new CategoryLabel(this);
            lbCate.DoubleClick += OnDoubleClick_Category;
            new MainPanel(this);
            form.Load += OnLoad_readTSV;
        }

        public int getActiveTab()
        {
            return intActiveTab;
        }

        //負を渡すとアクティブタブのを返す
        public string getCategory(int tab)
        {
            tab = tab < 0 ? intActiveTab : tab;
            return lbs[tab].strCategory;
        }

        //負を渡すとアクティブタブのを返す
        public string[] getLabels(int tab)
        {
            tab = tab < 0 ? intActiveTab : tab;
            string[] s = new String[bts.Length];
            for (int key = 0; key < bts.Length; key++)
            {
                s[key] = bts[key].getLabel(tab);
            }
            return s;
        }

        //負を渡すとアクティブタブのを返す
        public string[] getLinks(int tab)
        {
            tab = tab < 0 ? intActiveTab : tab;
            string[] s = new String[bts.Length];
            for (int key = 0; key < bts.Length; key++)
            {
                s[key] = bts[key].getLink(tab);
            }
            return s;
        }

        public void updateData(int tab, string category, string[] labels, string[] links)
        {
            lbs[tab].strCategory = category;
            try
            {
                for (int key = 0; key < bts.Length; key++)
                {
                    bts[key].setLabelAndLink(tab, labels[key], links[key]);
                }
            }
            catch
            {
                //IndexOutOfRangeException//配列エラー
                //NullReferenceExceptio//null参照エラー
                return;
            }
            using (StreamWriter sw = new StreamWriter(STR_FNAME + tab + ".tsv"))
            {
                sw.WriteLine(category);
                for (int key = 0; key < bts.Length; key++)
                {
                    sw.WriteLine(labels[key] + '\t' + links[key]);
                }
            }
            setTab(INT_DEFAULT_TAB);
        }

        public void setTab(int num)
        {
            lbs[num].setFocus2();
        }
        public void clickButton(int num)
        {
            bts[num].clickStart();
        }

        private void OnDoubleClick_Category(object sender, EventArgs e)
        {
            new SettingForm(this).Show();
        }

        private void OnLoad_readTSV(object sender, EventArgs e)
        {
            for (int tab = 0; tab < lbs.Length; tab++)
            {
                string fpath = STR_FNAME + tab + ".tsv";
                if (File.Exists(fpath))
                {
                    int key = -1;
                    foreach (string tsv in File.ReadAllLines(fpath))
                    {
                        if (key < 0)
                        {
                            lbs[tab].strCategory = tsv;
                        }
                        else if (key < bts.Length)
                        {
                            string[] s = tsv.Split('\t');
                            bts[key].setLabelAndLink(tab, s[0], s.Length > 1 ? s[1] : "");
                        }
                        else
                        {
                            break;
                        }
                        key++;
                    }
                }
                else
                {//ファイルが無かったら
                    lbs[tab].strCategory = tab.ToString();
                }
            }
        }

        private class TabFlowLayoutPanel : FlowLayoutPanel
        {
            public TabFlowLayoutPanel(Control parent)
            {
                parent.Controls.Add(this);

                FlowDirection = FlowDirection.LeftToRight;
                WrapContents = false;
                Width = 800;
                Height = 27;

                for (int i = 0; i < lbs.Length; i++)
                {
                    lbs[i] = new TabLabel(this);
                    lbs[i].Text = i.ToString();
                }
            }
        }

        private class TabLabel : Label
        {
            public string strCategory;
            public TabLabel(Panel parent)
            {
                parent.Controls.Add(this);

                Width = 57;
                Height = 27;
                TextAlign = ContentAlignment.TopCenter;
                Font = new Font(Font.FontFamily, 20, FontStyle.Bold);//サイズ変更
            }
            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);

                setFocus2();
            }
            public void setFocus2()
            {
                int i = 0;
                foreach (TabLabel l in lbs)
                {
                    if (l == this)
                    {
                        BackColor = Color.LightYellow;
                        ForeColor = Color.DarkGreen;
                        lbCate.Text = strCategory;
                        updateButtons(i);
                        intActiveTab = i;
                    }
                    else
                    {
                        l.BackColor = Color.DarkGray;
                        l.ForeColor = Color.Black;
                    }
                    i++;
                }
            }
            private void updateButtons(int tab)
            {
                foreach (LinkButton bt in bts)
                {
                    bt.setText(tab);
                }
            }
        }

        private class CategoryLabel : Label
        {
            public CategoryLabel(Control parent)
            {
                parent.Controls.Add(this);
                Margin = new Padding(6, 0, 0, 0);
                Width = 624;
                Height = 30;
                BackColor = Color.Gray;
                ForeColor = Color.White;
                TextAlign = ContentAlignment.MiddleCenter;
                Font = new Font(Font.FontFamily, 20, FontStyle.Italic);//サイズ変更
            }
        }

        private class MainPanel : Panel
        {
            private const int height = 30;
            private const int widthLabel = 30;
            private const int widthButton = 280;

            public MainPanel(Control parent)
            {
                parent.Controls.Add(this);
                Width = 800;
                Height = 510;


                Bitmap bmp = getBMP();
                Font font = new Font(Font.FontFamily, 12, FontStyle.Bold);
                for (int i = 0; i < bts.Length; i++)
                {
                    bts[i] = new LinkButton(this);
                    bts[i].Image = bmp;
                    bts[i].TextAlign = ContentAlignment.MiddleLeft;
                    bts[i].Font = font;
                    if (i < bts.Length / 2)
                    {
                        new CharLabel(this, i).setRect(5, i * height, widthLabel, height);
                        bts[i].setRect(widthLabel, i * height, widthButton, height);
                    }
                    else
                    {
                        new CharLabel(this, i).setRect(5 + widthLabel + widthButton, (i - bts.Length / 2) * height, widthLabel, height);
                        bts[i].setRect(widthLabel + widthLabel + widthButton, (i - bts.Length / 2) * height, widthButton, height);
                    }
                }
            }

            private Bitmap getBMP()
            {
                // ビットマップとGraphicsオブジェクトの作成
                Bitmap bmp = new Bitmap(widthButton, height);
                Graphics g = Graphics.FromImage(bmp);

                // グラデーション・ブラシの作成
                LinearGradientBrush gradBrush = new LinearGradientBrush(
                    g.VisibleClipBounds, // ビットマップの領域サイズ
                    Color.White, // 開始色
                    Color.Gray, // 終了色
                    LinearGradientMode.Vertical); // 縦方向にグラデーション

                // ビットマップをグラデーション・ブラシで塗る
                g.FillRectangle(gradBrush, g.VisibleClipBounds);

                gradBrush.Dispose();
                g.Dispose();

                return bmp;
            }

            private class CharLabel : Label
            {
                public CharLabel(Control parent, int c)
                {
                    parent.Controls.Add(this);
                    Text = (char)('A' + c) + "：";
                    TextAlign = ContentAlignment.MiddleCenter;
                    ForeColor = Color.LightBlue;
                    Font = new Font(Font.FontFamily, 10);
                }
                public void setRect(int left, int top, int width, int height)
                {
                    this.Left = left;
                    this.Top = top;
                    this.Width = width;
                    this.Height = height;
                }
            }
        }

        private class LinkButton : Button
        {
            private string[] strLabel = new string[10];
            private string[] strLink = new string[10];

            public LinkButton(Control parent)
            {
                parent.Controls.Add(this);
            }

            public void clickStart()
            {
                string s = strLink[intActiveTab];
                form.Visible = false;
                try
                {
                    if (s.StartsWith("http"))
                    {
                        Process.Start("IExplore", s);
                    }
                    else
                    {
                        Process.Start(s);
                    }
                }
                catch/*(Exception e)*/
                {
                    //MessageBox.Show(s + "\n" + e.ToString());
                }
            }

            public string getLabel(int tab)
            {
                return strLabel[tab];
            }

            public string getLink(int tab)
            {
                return strLink[tab];
            }

            public void setText(int num)
            {
                Text = strLabel[num];
            }

            public void setLabelAndLink(int tab, string label, string link)
            {
                strLabel[tab] = label;
                strLink[tab] = link;
            }
            public void setRect(int left, int top, int width, int height)
            {
                this.Left = left;
                this.Top = top;
                this.Width = width;
                this.Height = height;
            }
            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                clickStart();
            }
        }
    }
}
