// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/05/2016 by Konstantin Sidorenko

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

using Aspose.JavaAttributes;

#if !NETSTANDARD
using System.Windows.Forms;
#endif

namespace Aspose.TestFx.Pal
{
    /// <summary>
    /// Code that is platform specific and used for tests only is located here.
    ///
    /// Note: Please do not make this class dependent on TestEnvironment.
    /// This class is supposed to be implemented manually for each platform so it is okay to have any platform specific paths hardcoded here.
    /// Otherwise it creates a cyclical dependency during execution of static constructors between TestEnvironment and TestUtilPal that is hard to resolve.
    /// </summary>
    [JavaManual("Platform abstraction for utility methods used by our unit tests.")]
    public static class TestUtilPal
    {
        static TestUtilPal()
        {
#if !NETSTANDARD
            string value = Environment.GetEnvironmentVariable("SettingsFormTriggerKeys");
            if (value == "2")
            {
                gSettingsFormTriggerKeys = (Keys.Control | Keys.Shift);
                gSettingsFormTriggerMouseButton = (MouseButtons.Left);
            }
#endif
        }

        /// <summary>
        /// This throws an exception that causes the testing framework to skip/ignore the current running test.
        /// </summary>
        public static void ThrowIgnoreTestException(string message)
        {
            throw new NUnit.Framework.IgnoreException(message);
        }

        public static int ExecuteProcess(string exe, string cmdLine, string tempFolder, out string output)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = exe;
            startInfo.Arguments = cmdLine;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            if (StringUtil.HasChars(tempFolder))
            {
                startInfo.EnvironmentVariables["TMP"] = tempFolder;
                startInfo.EnvironmentVariables["TEMP"] = tempFolder;
            }

            using (Process process = Process.Start(startInfo))
            {
                output = process.StandardOutput.ReadToEnd();
                output += process.StandardError.ReadToEnd();
                process.WaitForExit();
                return process.ExitCode;
            }
        }

        public static int ExecuteProcess(string exe, string cmdLine, out string output)
        {
            return ExecuteProcess(exe, cmdLine, string.Empty, out output);
        }

        public static int ExecuteProcess(string exe, string cmdLine)
        {
            string dummyOutput;
            return ExecuteProcess(exe, cmdLine, out dummyOutput);
        }

        public static bool IsScrollLockOn()
        {
#if !NETSTANDARD
            return Control.IsKeyLocked(Keys.Scroll);
#else
            if (PlatformUtilPal.IsWindows())
            {
                string output;
                // Run a simple console app that writes to console True or False depending on Scroll Lock status.
                ExecuteProcess(@"X:\awnet\Aspose.Foundation\Tools\ScrollLockIndicator\ScrollLockIndicator\bin\ScrollLockIndicator.exe", "", out output);
                return bool.Parse(output.Trim());
            }
            else
            {
                return false;
            }
#endif
        }

        /// <summary>
        /// On Java this method replaces TestGold\ in the path with TestGoldJava\ if the specified file in TestGoldJava\
        /// exists, otherwise just returns the name without changes.
        /// On .NET this method just returns the given parameter without changes.
        /// </summary>
        public static string GetMostActualExistingGold(string dotNetGold)
        {
#if NETSTANDARD
            string netstandardGold = GetMostActualGold(dotNetGold);
            return File.Exists(netstandardGold) ? netstandardGold : dotNetGold;
#elif CPLUSPLUS
            string awcppGold = GetMostActualGold(dotNetGold);
            return File.Exists(awcppGold) ? awcppGold : dotNetGold;
#else
            return dotNetGold;
#endif
        }

        /// <summary>
        /// On Java this method replaces TestGold\ in the path with TestGoldJava\.
        /// On .NET this method just returns the given parameter without changes.
        /// </summary>
        public static string GetMostActualGold(string dotNetGold)
        {
#if NETSTANDARD
            return dotNetGold.Replace("words-net-standard-golds", @"awnet")
                .Replace("awnet", @"words-net-standard-golds")
                .Replace("TestGoldNETStandard", "TestGold")
                .Replace("TestGold", "TestGoldNETStandard");
#elif CPLUSPLUS
            return dotNetGold.Replace("TestGold", TestEnvironment.GetGoldDirName());
#else
            return dotNetGold;
#endif
        }

        public static string CorrectGoldFileNameIfNeeded(string sourceFileName)
        {
            return sourceFileName;
        }

        public static string CorrectOutFileNameIfNeeded(string sourceFileName)
        {
            return sourceFileName;
        }

        /// <summary>
        /// Gets an array of int constant values defined in a given class.
        /// </summary>
        [JavaThrows(true)]
        public static int[] GetIntConstantValues(Type type)
        {
            FieldInfo[] fieldAttrFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Static);
            int[] intValues = new int[fieldAttrFields.Length];
            int count = 0;

            for (int attrIdx = 0; attrIdx < fieldAttrFields.Length; ++attrIdx)
            {
                FieldInfo info = fieldAttrFields[attrIdx];

                if(info.IsLiteral && info.FieldType == typeof(int))
                {
                    // RK Don't use the GetRawConstantValue because it is not available in .NET 1.1.
                    intValues[count] = Int32.Parse(info.GetValue(info).ToString());
                    count++;
                }
            }

            int[] justIntValues = new int[count];
            Array.Copy(intValues, justIntValues, count);

            return justIntValues;
        }

        /// <summary>
        /// Gets all enum values defined in a given type as an array of integer constants.
        /// </summary>
        public static int[] GetEnumValues(Type type)
        {
            Array values = Enum.GetValues(type);

            int[] intValues = new int[values.Length];

            for (int i = 0; i < values.Length; i++)
                intValues[i] = (int)values.GetValue(i);

            return intValues;
        }

        /// <summary>
        /// Returns "awnet" on .NET and "words-java" for Java.
        /// </summary>
        public static string GetProjectName()
        {
            return "awnet";
        }

        /// <summary>
        /// Returns "words-net" on .NET and "words-java" for Java.
        /// </summary>
        public static string GetAlternativeProjectName()
        {
            return "words-net";
        }

        public static string GetExecutingAssemblyPath()
        {
            return GetAssemblyPath(Assembly.GetExecutingAssembly());
        }

        private static string GetAssemblyPath(Assembly assembly)
        {
            return Path.GetDirectoryName(new Uri(assembly.Location).LocalPath);
        }

        /// <summary>
        /// Returns true if the OS is 64 bit. At the moment this is only needed for Windows to determine which XPS validator exe to launch.
        /// </summary>
        public static bool Is64BitWindows()
        {
            return (Environment.GetEnvironmentVariable("ProgramFiles(x86)") != null);
        }

        /// <summary>
        /// Returns the current desktop resolution.
        ///
        /// All tests are really expected to be independent of the desktop resolution,
        /// but there is a couple that is quite hard to make independent. So this method
        /// is used in those tests.
        /// </summary>
        public static bool IsNonStandardDesktopResolution()
        {
#if NETSTANDARD
            return true; // not sure how to detect resulution on mac. return true for now.
#else
            // In .NET a new Bitmap resolution is same as the current desktop resolution.
            using (Bitmap bitmap = new Bitmap(1, 1))
                return bitmap.HorizontalResolution != ImageConstants.StandardResolution;
#endif
        }

        /// <summary>
        /// If IsNonStandardDesktopResolution() returns true - we can use this method
        /// to get the scale of current non standart desktop resolution.
        /// </summary>
        public static float GetNonStandardDesktopResolutionScale()
        {
            float currentResolution;

#if NETSTANDARD
            currentResolution = ImageConstants.StandardResolution;// not sure how to detect resolution on mac. Use standard resolution for now.
#else
            // In .NET a new Bitmap resolution is same as the current desktop resolution.
            using (Bitmap bitmap = new Bitmap(1, 1))
                currentResolution = bitmap.HorizontalResolution;
#endif
            if (currentResolution == ImageConstants.StandardResolution)
                return 1;
            return currentResolution / ImageConstants.StandardResolution;

        }

#if !NETSTANDARD

        private static readonly Keys gSettingsFormTriggerKeys = (Keys.Alt | Keys.Shift);

        private static readonly MouseButtons gSettingsFormTriggerMouseButton = MouseButtons.None;
        private static CheckBox gGlobalConfigFile;

        /// <summary>
        /// Shows the settings UI if Alt+Shift is depressed.
        /// This lets the user to change (and save) the test settings before running the tests.
        /// </summary>
        public static void ShowTestSettingsFormIfNeeded()
        {
            // NOTE For some reason Control.ModifierKeys will constantly return None after the UI is first shown.
            // Otherwise it could be possible to show UI later and change settings on the go.
            if (Control.ModifierKeys == Keys.None)
                return;

            if (Control.ModifierKeys != gSettingsFormTriggerKeys)
                return;

            if (Control.MouseButtons != gSettingsFormTriggerMouseButton)
                return;

            Debug.WriteLine("WARNING: Use test settings configuration dialog to proceed.");

            Form form = CreateTestSettingsForm();
            if (form != null)
                form.ShowDialog();
        }

        private const int FORM_WIDTH = 600;
        private const int ITEM_HEIGHT = 24;

        public static Form CreateTestSettingsForm()
        {
            IDictionary<string, TestSetting> values = TestSettings.GetAllSettings();
            if (values.Count == 0)
                return null;

            Form form = new Form();
            form.Font = new Font("Tahoma",9f);

            form.AutoSize = false;
            form.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            form.Text = "AW Test Settings";
            form.ControlBox = false;
            form.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            form.TopLevel = true;
            form.Size = new Size(FORM_WIDTH, 800);
            form.MinimumSize = new Size(300, 200);
            form.Padding = new Padding(PAD, PAD, PAD, PAD);

#if false
            // Enable movement with middle mouse button.
            new ControlMover(form);
#endif

            Panel bottomPanel = new Panel();
            bottomPanel.Size = new Size(FORM_WIDTH, ITEM_HEIGHT);
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.AutoSize = true;

            gGlobalConfigFile = new CheckBox();
            gGlobalConfigFile.Location = new Point(0, 0);
            gGlobalConfigFile.AutoSize = true;
            gGlobalConfigFile.Checked = TestSettings.IsGlobalConfigFile;
            gGlobalConfigFile.Text = "Global config file at %USERPROFILE%\\Aspose folder";
            gGlobalConfigFile.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            gGlobalConfigFile.MinimumSize = new Size(4*ITEM_HEIGHT, ITEM_HEIGHT);

            Button closeButton = new Button();
            closeButton.AutoSize = false;
            closeButton.Text = "Close";
            closeButton.Size = new Size(3*ITEM_HEIGHT, ITEM_HEIGHT);
            closeButton.Location = new Point(FORM_WIDTH - closeButton.Width - 2*PAD, 0);
            closeButton.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            closeButton.DialogResult = DialogResult.OK;

            Button saveButton = new Button();
            saveButton.AutoSize = false;
            saveButton.Text = "Save and Close";
            saveButton.Size = new Size(4*ITEM_HEIGHT, ITEM_HEIGHT);
            saveButton.Location = new Point(closeButton.Left - saveButton.Width - 4*PAD, 0);
            saveButton.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            saveButton.DialogResult = closeButton.DialogResult;
            saveButton.Click += new EventHandler(save_Click);

            form.AcceptButton = closeButton;
            form.CancelButton = closeButton;

            bottomPanel.Controls.Add(saveButton);
            bottomPanel.Controls.Add(closeButton);
            bottomPanel.Controls.Add(gGlobalConfigFile);

            Panel topPanel = new Panel();
            topPanel.Width = FORM_WIDTH - 2*PAD;
            topPanel.Dock = DockStyle.Fill;
            topPanel.AutoScroll = true;
            topPanel.HorizontalScroll.Visible = false;
            //topPanel.WrapContents = false;
            //topPanel.FlowDirection = FlowDirection.TopDown;

            List<Control> controls = new List<Control>();
            foreach (KeyValuePair<string, TestSetting> value in values)
                controls.AddRange(CreateControlsForSetting(value));

            controls.Reverse();
            topPanel.Controls.AddRange(controls.ToArray());

            form.Controls.Add(topPanel);
            form.Controls.Add(bottomPanel);

            // TODO There should be a better way to center form on the screen, but this is good enough.
            Rectangle workingArea = Screen.GetWorkingArea(form);
            form.Left = (workingArea.Width - form.Width) / 2;
            form.Top = (workingArea.Height - form.Height) / 2;

            // It's a hack to display the form on top, thanks to http://stackoverflow.com/a/1463479/505893
            form.WindowState = FormWindowState.Minimized;
            form.Shown += FormOnShown;

            return form;
        }

        private const int PAD = 3;

        private static Control[] CreateControlsForSetting(KeyValuePair<string, TestSetting> entry)
        {
            Control spacer = null;
            if (entry.Value.Leader)
                spacer = CreateControl(typeof(Label), entry);

            Control input = null;
            switch (entry.Value.Kind)
            {
                case TestSetting.KindEnum.Bool: input = CreateControl(typeof(CheckBox), entry); break;
                case TestSetting.KindEnum.String:
                case TestSetting.KindEnum.Float: input = CreateControl(typeof(TextBox), entry); break;
                case TestSetting.KindEnum.Array: input = CreateControl(typeof(ComboBox), entry); break;
                default:
                    throw new Exception("unexpected '" + entry.Value.Kind + "' kind");
            }

            input.Width = FORM_WIDTH - 4*PAD;

            if (spacer != null)
            {
                spacer.Width = FORM_WIDTH - 4*PAD;
                return new Control[] { spacer, input };
            }

            return new Control[] { input };
        }

        private static Control CreateControl(Type controlType, KeyValuePair<string, TestSetting> entry)
        {
            string text = GetTestSettingDescription(entry);
            object value = null;
            if (entry.Value != null)
            {
                switch (entry.Value.Kind)
                {
                    case TestSetting.KindEnum.Bool:
                        value = entry.Value.AsBool ? CheckState.Checked : CheckState.Unchecked;
                        break;
                    case TestSetting.KindEnum.Float:
                        value = entry.Value.AsFloat.ToString(); // textbox gets string value only
                        break;
                    case TestSetting.KindEnum.String:
                        value = entry.Value.AsString;
                        break;
                    case TestSetting.KindEnum.Array:
                        value = entry.Value.AsArray;
                        break;
                    default:
                        throw new Exception("unexpected entry type");
                }
            }

            Control result;

            if (controlType == typeof(CheckBox))
            {
                CheckBox cb = new CheckBox();
                cb.Size = new Size(FORM_WIDTH - 4*PAD, ITEM_HEIGHT);
                cb.TextAlign = ContentAlignment.MiddleLeft;
                cb.FlatStyle = FlatStyle.System;
                cb.Appearance = Appearance.Normal;
                cb.ThreeState = false;
                cb.Text = text;
                cb.CheckState = (CheckState)value;
                cb.CheckedChanged += cb_CheckedChanged;
                cb.Tag = entry;
                result = cb;
            }
            else if (controlType == typeof(TextBox))
            {
                Label label = new Label();
                label.Dock = DockStyle.Top;
                label.AutoSize = true;
                label.Text = text;

                TextBox tb = new TextBox();
                tb.Size = new Size(FORM_WIDTH - 4*PAD, ITEM_HEIGHT);
                tb.Dock = DockStyle.Top;
                tb.ReadOnly = false;
                tb.Text = (string)value;
                tb.TextChanged += tb_CheckedChanged;
                tb.Tag = entry;

                Panel panel = new Panel();
                panel.Controls.Add(tb);
                panel.Controls.Add(label);
                result = panel;
            }
            else if (controlType == typeof(ComboBox))
            {
                Panel panel = new Panel();
                result = panel;

#if false
                ComboBox lb = new ComboBox();
                string[] values = (string[])value;
                lb.Items.AddRange(values);
                lb.Text = ((string[])value)[0];
                lb.Left = 0;
                lb.Width = FORM_WIDTH - 3*ITEM_HEIGHT;
                lb.IntegralHeight = true;
                //lb.ItemHeight = ITEM_HEIGHT;
                lb.FlatStyle = FlatStyle.Standard;
                lb.DropDownStyle = ComboBoxStyle.DropDownList;
                lb.TextChanged += lb_CheckedChanged;
                panel.Controls.Add(lb);
#else
                string[] rbs = new string[((string[])value).Length];
                ((string[])value).CopyTo(rbs, 0);
                Array.Reverse(rbs);
                foreach (var v in rbs)
                {
                    RadioButton rb = new RadioButton();
                    rb.Dock = DockStyle.Top;
                    rb.AutoSize = true;
                    rb.Text = v;
                    rb.Tag = entry;
                    rb.CheckedChanged += rb_CheckedChanged;
                    panel.Controls.Add(rb);
                }

                ((RadioButton)panel.Controls[panel.Controls.Count-1]).Checked = true;
#endif

                Label label = new Label();
                label.Dock = DockStyle.Top;
                label.AutoSize = true;
                label.Text = text;
                panel.Controls.Add(label);

                panel.Controls.Add(CreateControl(typeof(Label), new KeyValuePair<string, TestSetting>()));
            }
            else if (controlType == typeof(Label))
            {
                Label label = new Label();
                label.Dock = DockStyle.Top;
                label.Text = string.Empty;
                label.Size = new Size(FORM_WIDTH, 1);
                label.BorderStyle = BorderStyle.Fixed3D;
                return label;
            }
            else
            {
                throw new Exception("unexpected '" + controlType + "' control type");
            }

            result.Dock = DockStyle.Top;
            result.Width = FORM_WIDTH - 4*PAD;
            result.AutoSize = true;
            result.Padding = new Padding(0, PAD, 0, PAD);

            return result;
        }

        private static void FormOnShown(object sender, EventArgs eventArgs)
        {
            ((Form)sender).WindowState = FormWindowState.Normal;
        }

        private static string GetTestSettingDescription(KeyValuePair<string, TestSetting> entry)
        {
            if (entry.Key == null || entry.Value == null)
                return string.Empty;
            string description = entry.Value.Description;
            return StringUtil.HasChars(description) ? description : entry.Key;
        }

        private static void save_Click(object sender, EventArgs e)
        {
            TestSettings.IsGlobalConfigFile = gGlobalConfigFile.Checked;
            TestSettings.SaveConfig();
        }

        private static void cb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            bool value = cb.CheckState == CheckState.Checked;
            string key = ((KeyValuePair<string, TestSetting>)cb.Tag).Key;
            TestSettings.Set(key, value);
        }

        private static void tb_CheckedChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            KeyValuePair<string, TestSetting> kvp = (KeyValuePair<string, TestSetting>)tb.Tag;
            object value = tb.Text;
            if (kvp.Value.Kind == TestSetting.KindEnum.Float)
            {
                float f;
                if (float.TryParse(tb.Text, out f))
                    TestSettings.Set(kvp.Key, f);
            }
            else
            {
                TestSettings.Set(kvp.Key, tb.Text);
            }
        }

        private static void lb_CheckedChanged(object sender, EventArgs e)
        {
            ListBox lb = (ListBox)sender;
            string key = ((KeyValuePair<string, TestSetting>)lb.Tag).Key;
            TestSettings.Set(key, new string[] { lb.Text });
        }

        private static void rb_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            string key = ((KeyValuePair<string, TestSetting>)rb.Tag).Key;
            TestSettings.Set(key, new string[] { rb.Text });
        }
#endif
    }
}
