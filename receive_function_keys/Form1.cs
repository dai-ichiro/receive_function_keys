using System;
using System.Windows.Forms;

namespace receive_function_keys
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // 1. プロパティでも設定可能ですが、念のためコードでも指定
            this.ShowInTaskbar = false;      // タスクバーに表示しない
            this.WindowState = FormWindowState.Minimized; // 最小化状態で起動
        }
        // 起動時にフォームを一瞬も表示させないための核心部分
        protected override void SetVisibleCore(bool value)
        {
            if (!this.IsHandleCreated)
            {
                value = false;
                CreateHandle();
            }
            base.SetVisibleCore(value);
        }
        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // タスクトレイのアイコンを明示的に非表示にする（これをしないと終了後もアイコンが残ることがあります）
            notifyIcon1.Visible = false;

            // プロセスを完全に終了させる
            Application.Exit();
        }
    }
}
