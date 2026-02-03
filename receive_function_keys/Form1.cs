using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace receive_function_keys
{
    public partial class Form1 : Form
    {
        private GlobalKeyboardHook _globalKeyboardHook;
        private ConfigRoot _config;

        public Form1()
        {
            InitializeComponent();

            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;

            // Load Config
            _config = ConfigLoader.Load();

            // Initialize Hook
            try
            {
                _globalKeyboardHook = new GlobalKeyboardHook();
                _globalKeyboardHook.KeyDown += OnGlobalKeyDown;
                _globalKeyboardHook.Hook();
                Logger.Log("Application started. Hook initialized.", _config.Settings.LoggingEnabled);
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to initialize hook: {ex.Message}", _config.Settings.LoggingEnabled);
                MessageBox.Show("キーボードフックの初期化に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void OnGlobalKeyDown(object sender, GlobalKeyEventArgs e)
        {
            // Convert Keys to string (e.g., "F13")
            string keyName = e.Key.ToString();

            if (_config.Actions.ContainsKey(keyName))
            {
                // Mark as handled to block the key from reaching other applications
                e.Handled = true;

                // Execute actions asynchronously to avoid blocking the hook thread
                var actions = _config.Actions[keyName];
                Task.Run(() =>
                {
                    Logger.Log($"Trigger key detected: {keyName}", _config.Settings.LoggingEnabled);
                    ExecuteActions(actions);
                });
            }
        }

        private void ExecuteActions(List<ActionStep> steps)
        {
            foreach (var step in steps)
            {
                if (step.Type.Equals("key", StringComparison.OrdinalIgnoreCase))
                {
                    // Parse key
                    try
                    {
                        // Use KeysConverter to parse strings like "Alt", "Ctrl+C", "F13"
                        Keys k = (Keys)new KeysConverter().ConvertFromString(step.Value);
                        KeySender.SendKey(k, _config.Settings.KeyHoldTime);
                        Logger.Log($"Action executed: SendKey {step.Value}", _config.Settings.LoggingEnabled);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Error parsing key: {step.Value}. {ex.Message}", _config.Settings.LoggingEnabled);
                    }
                }
                else if (step.Type.Equals("wait", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(step.Value, out int waitTime))
                    {
                        System.Threading.Thread.Sleep(waitTime);
                        Logger.Log($"Action executed: Wait {waitTime}ms", _config.Settings.LoggingEnabled);
                    }
                }
            }
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
            // タスクトレイのアイコンを明示的に非表示にする
            notifyIcon1.Visible = false;

            _globalKeyboardHook?.Dispose();
            Logger.Log("Application exiting.", _config.Settings.LoggingEnabled);

            // プロセスを完全に終了させる
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _globalKeyboardHook?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
