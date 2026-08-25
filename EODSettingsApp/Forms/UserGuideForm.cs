using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EODSettingsApp.Forms
{
    public partial class UserGuideForm : Form
    {
        // ── Content Model ─────────────────────────────────────────────────────────

        private readonly record struct Block(string Type, string Text);
        private readonly record struct Section(string Icon, string Title, Block[] Blocks);

        private static readonly Section[] Sections = BuildSections();

        // ── State ─────────────────────────────────────────────────────────────────

        private int  _selectedIndex = 0;
        private readonly List<Button> _navButtons = new();
        private FlowLayoutPanel? _contentFlow;
        private readonly System.Windows.Forms.Timer _resizeDebounce = new() { Interval = 120 };
        private readonly Dictionary<string, Bitmap?> _screenshots = new();  // cached per session

        // ── Construction ──────────────────────────────────────────────────────────

        public UserGuideForm()
        {
            InitializeComponent();
            BuildSidebarNav();
            SelectSection(0);

            _resizeDebounce.Tick += (_, _) =>
            {
                _resizeDebounce.Stop();
                RenderSection(Sections[_selectedIndex]);
            };
            pnlContentArea.Resize += (_, _) =>
            {
                _resizeDebounce.Stop();
                _resizeDebounce.Start();
            };
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _resizeDebounce.Dispose();
            foreach (var bmp in _screenshots.Values)
                bmp?.Dispose();
            base.OnFormClosed(e);
        }

        // ── Sidebar Navigation ────────────────────────────────────────────────────

        private void BuildSidebarNav()
        {
            pnlSidebar.Controls.Add(new Label
            {
                AutoSize    = false,
                Dock        = DockStyle.Top,
                Height      = 36,
                Font        = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor   = Color.FromArgb(95, 140, 210),
                Text        = "   DOCUMENTATION",
                TextAlign   = ContentAlignment.MiddleLeft,
                BackColor   = Color.Transparent,
            });

            int y = 40;
            for (int i = 0; i < Sections.Length; i++)
            {
                var s   = Sections[i];
                var idx = i;

                var btn = new Button
                {
                    Text      = $"   {s.Icon}  {s.Title}",
                    TextAlign = ContentAlignment.MiddleLeft,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Transparent,
                    ForeColor = Color.FromArgb(170, 205, 255),
                    Font      = new Font("Segoe UI", 9.5F),
                    Size      = new Size(215, 44),
                    Location  = new Point(0, y),
                    Cursor    = Cursors.Hand,
                    Name      = $"btnNav{i}",
                };
                btn.FlatAppearance.BorderSize         = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(35, 62, 140);
                btn.Click += (_, _) => SelectSection(idx);

                pnlSidebar.Controls.Add(btn);
                _navButtons.Add(btn);
                y += 46;
            }
        }

        private void SelectSection(int index)
        {
            _selectedIndex = index;
            var s = Sections[index];

            for (int i = 0; i < _navButtons.Count; i++)
            {
                bool active = i == index;
                _navButtons[i].BackColor = active ? Color.FromArgb(59, 130, 246) : Color.Transparent;
                _navButtons[i].ForeColor = active ? Color.White : Color.FromArgb(170, 205, 255);
                _navButtons[i].Font      = new Font("Segoe UI", 9.5F, active ? FontStyle.Bold : FontStyle.Regular);
            }

            lblContentIcon.Text  = s.Icon;
            lblContentTitle.Text = s.Title;

            RenderSection(s);
        }

        // ── Content Rendering ─────────────────────────────────────────────────────

        private void RenderSection(Section section)
        {
            int scrollBarW = SystemInformation.VerticalScrollBarWidth;
            int areaW      = pnlContentArea.ClientSize.Width;
            int contentW   = Math.Max(areaW - scrollBarW - 4, 320);
            int innerW     = contentW - 64;

            pnlContentArea.SuspendLayout();
            pnlContentArea.Controls.Clear();
            pnlContentArea.AutoScrollPosition = new Point(0, 0);

            _contentFlow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents  = false,
                AutoSize      = true,
                AutoSizeMode  = AutoSizeMode.GrowAndShrink,
                Width         = contentW,
                Location      = new Point(0, 0),
                BackColor     = Color.White,
                Padding       = new Padding(32, 20, 32, 48),
            };

            foreach (var block in section.Blocks)
            {
                Control ctrl = block.Type switch
                {
                    "h1"              => MakeH1(block.Text, innerW),
                    "h2"              => MakeH2(block.Text, innerW),
                    "body"            => MakeBody(block.Text, innerW),
                    "bullet"          => MakeBullet(block.Text, innerW),
                    "code"            => MakeCode(block.Text, innerW),
                    "tip"             => MakeCallout(block.Text, innerW, Color.FromArgb(240, 253, 244), Color.FromArgb(22, 163, 74),  "✓  Tip"),
                    "warning"         => MakeCallout(block.Text, innerW, Color.FromArgb(255, 251, 235), Color.FromArgb(234, 88, 12),  "⚠  Note"),
                    "divider"         => MakeDivider(innerW),
                    // Live screenshots from the actual app forms
                    "db_screen"       => MakeScreenCard("db",       () => new DatabaseSettingsForm(), new Size(850, 560), innerW, () => new DatabaseSettingsForm()),
                    "provider_screen" => MakeScreenCard("provider", () => new ProviderSettingsForm(), new Size(800, 520), innerW, () => new ProviderSettingsForm()),
                    "symbols_screen"  => MakeScreenCard("symbols",  () => new SymbolSettingsForm(),   new Size(980, 640), innerW, () => new SymbolSettingsForm()),
                    "history_screen"  => MakeScreenCard("history",  () => new HistoricalDataForm(),   new Size(900, 600), innerW, () => new HistoricalDataForm()),
                    _                 => MakeBody(block.Text, innerW),
                };
                _contentFlow.Controls.Add(ctrl);
            }

            pnlContentArea.Controls.Add(_contentFlow);
            pnlContentArea.ResumeLayout();
        }

        // ── Screenshot Capture ────────────────────────────────────────────────────

        /// <summary>
        /// Instantiates a form off-screen, captures it with DrawToBitmap, and caches the result.
        /// </summary>
        private Bitmap? GetOrCapture(string key, Func<Form> factory, Size size)
        {
            if (_screenshots.TryGetValue(key, out var cached)) return cached;

            Bitmap? bmp  = null;
            Form?   form = null;
            try
            {
                form = factory();
                form.StartPosition = FormStartPosition.Manual;
                form.Location      = new Point(-9000, -9000);
                form.Size          = size;
                form.Show();
                Application.DoEvents(); // flush paint so child controls render

                bmp = new Bitmap(size.Width, size.Height);
                form.DrawToBitmap(bmp, new Rectangle(0, 0, size.Width, size.Height));
            }
            catch
            {
                bmp = null;
            }
            finally
            {
                form?.Hide();
                form?.Dispose();
            }

            _screenshots[key] = bmp;
            return bmp;
        }

        /// <summary>
        /// Builds a card: screenshot on top, "Open This Screen" button below.
        /// </summary>
        private Control MakeScreenCard(string key, Func<Form> captureFactory, Size captureSize, int width, Func<Form> openFactory)
        {
            int imgH  = (int)(width * 0.56); // ~16:9
            int cardH = imgH + 50;

            var card = new Panel
            {
                Width     = width,
                Height    = cardH,
                BackColor = Color.White,
                Margin    = new Padding(0, 8, 0, 24),
            };

            var bmp = GetOrCapture(key, captureFactory, captureSize);

            if (bmp != null)
            {
                card.Controls.Add(new PictureBox
                {
                    Image       = bmp,
                    SizeMode    = PictureBoxSizeMode.Zoom,
                    Size        = new Size(width, imgH),
                    Location    = new Point(0, 0),
                    BackColor   = Color.FromArgb(248, 250, 252),
                    BorderStyle = BorderStyle.FixedSingle,
                });
            }
            else
            {
                card.Controls.Add(new Label
                {
                    Text      = "[ Screenshot not available ]",
                    Font      = new Font("Segoe UI", 9F),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    Size      = new Size(width, imgH),
                    Location  = new Point(0, 0),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.FromArgb(248, 250, 252),
                });
            }

            var btn = new Button
            {
                Text                    = "↗   Open This Screen",
                Font                    = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor               = Color.White,
                BackColor               = Color.FromArgb(30, 58, 138),
                FlatStyle               = FlatStyle.Flat,
                Size                    = new Size(180, 30),
                Location                = new Point(0, imgH + 10),
                Cursor                  = Cursors.Hand,
                UseVisualStyleBackColor = false,
            };
            btn.FlatAppearance.BorderSize         = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(59, 130, 246);
            btn.Click += (_, _) => OpenForm(openFactory());
            card.Controls.Add(btn);

            return card;
        }

        // ── Control Factories ─────────────────────────────────────────────────────

        private static Label MakeH1(string text, int width) => new()
        {
            Text        = text,
            Font        = new Font("Segoe UI", 18F, FontStyle.Bold),
            ForeColor   = Color.FromArgb(23, 48, 107),
            MaximumSize = new Size(width, 0),
            AutoSize    = true,
            Margin      = new Padding(0, 2, 0, 14),
        };

        private static Control MakeH2(string text, int width)
        {
            var pnl = new Panel
            {
                Width     = width,
                Height    = 36,
                BackColor = Color.White,
                Margin    = new Padding(0, 20, 0, 8),
            };
            pnl.Controls.Add(new Panel
            {
                BackColor = Color.FromArgb(59, 130, 246),
                Size      = new Size(4, 24),
                Location  = new Point(0, 6),
            });
            pnl.Controls.Add(new Label
            {
                Text      = text,
                Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(23, 48, 107),
                AutoSize  = true,
                Location  = new Point(14, 7),
            });
            return pnl;
        }

        private static Label MakeBody(string text, int width) => new()
        {
            Text        = text,
            Font        = new Font("Segoe UI", 10F),
            ForeColor   = Color.FromArgb(55, 65, 81),
            MaximumSize = new Size(width, 0),
            AutoSize    = true,
            Margin      = new Padding(0, 2, 0, 10),
        };

        private static Label MakeBullet(string text, int width) => new()
        {
            Text        = $"  ●   {text}",
            Font        = new Font("Segoe UI", 10F),
            ForeColor   = Color.FromArgb(55, 65, 81),
            MaximumSize = new Size(width, 0),
            AutoSize    = true,
            Margin      = new Padding(6, 1, 0, 6),
        };

        private static Control MakeCode(string text, int width)
        {
            var lbl = new Label
            {
                Text        = text,
                Font        = new Font("Consolas", 9F),
                ForeColor   = Color.FromArgb(30, 58, 138),
                MaximumSize = new Size(width - 32, 0),
                AutoSize    = true,
            };
            int h = lbl.PreferredSize.Height + 28;
            var pnl = new Panel
            {
                Width     = width,
                Height    = h,
                BackColor = Color.FromArgb(241, 245, 249),
                Margin    = new Padding(0, 6, 0, 14),
            };
            pnl.Controls.Add(new Panel
            {
                BackColor = Color.FromArgb(59, 130, 246),
                Size      = new Size(3, h),
                Location  = new Point(0, 0),
            });
            lbl.Location = new Point(16, 14);
            pnl.Controls.Add(lbl);
            return pnl;
        }

        private static Control MakeCallout(string text, int width, Color bgColor, Color accentColor, string label)
        {
            var bodyLbl = new Label
            {
                Text        = text,
                Font        = new Font("Segoe UI", 9.5F),
                ForeColor   = Color.FromArgb(55, 65, 81),
                MaximumSize = new Size(width - 36, 0),
                AutoSize    = true,
            };
            int headerH = 22;
            int pnlH    = headerH + bodyLbl.PreferredSize.Height + 24;

            var pnl = new Panel
            {
                Width     = width,
                Height    = pnlH,
                BackColor = bgColor,
                Margin    = new Padding(0, 8, 0, 14),
            };
            pnl.Controls.Add(new Panel
            {
                BackColor = accentColor,
                Size      = new Size(4, pnlH),
                Location  = new Point(0, 0),
            });
            pnl.Controls.Add(new Label
            {
                Text      = label,
                Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = accentColor,
                AutoSize  = true,
                Location  = new Point(16, 10),
            });
            bodyLbl.Location = new Point(16, 10 + headerH);
            pnl.Controls.Add(bodyLbl);
            return pnl;
        }

        private static Control MakeDivider(int width) => new Panel
        {
            Width     = width,
            Height    = 1,
            BackColor = Color.FromArgb(226, 232, 240),
            Margin    = new Padding(0, 14, 0, 14),
        };

        private static void OpenForm(Form targetForm)
        {
            try
            {
                targetForm.StartPosition = FormStartPosition.CenterScreen;
                targetForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                targetForm.Dispose();
            }
        }

        // ── Section Content ───────────────────────────────────────────────────────

        private static Section[] BuildSections() =>
        [
            new("🚀", "Getting Started",
            [
                new("h1",     "Welcome to EOD Data Service Manager"),
                new("tip",    "This application is fully dynamic — no database credentials, provider endpoints, API keys, or security symbols are hardcoded. Every deployment is configured by the administrator for their own specific infrastructure."),
                new("h2",     "What This Application Does"),
                new("body",   "EOD Data Service Manager automates the retrieval, validation, and atomic persistence of End-Of-Day (EOD) financial market data. It connects to your Oracle database, reads your configured market data providers, fetches daily OHLCV quotes for your configured security universe, and stores them reliably for downstream consumption."),
                new("h2",     "First-Time Setup"),
                new("bullet", "1. Configure your Oracle connection → Settings → Database Configuration."),
                new("bullet", "2. Verify or add provider records in your PROVIDER table → Settings → Provider Settings."),
                new("bullet", "3. Add securities into the STOCK table with provider-specific ticker mappings → Settings → Symbol Management."),
                new("bullet", "4. Select your active provider from the main dashboard dropdown."),
                new("bullet", "5. Enable automated scheduling or run a manual import to validate the pipeline."),
                new("warning","EODService will fail if the PROVIDER table has no matching row for the active provider, or if STOCK has no symbols mapped to that provider. Verify your database records before the first run."),
            ]),

            new("🗄️", "Database Setup",
            [
                new("h1",          "Database Connection & Schema"),
                new("body",        "The application connects to your own Oracle Database instance. Credentials are stored locally and never hardcoded in source code."),
                new("h2",          "Database Configuration Screen"),
                new("db_screen",   ""),
                new("h2",          "Connection String Format"),
                new("code",        "User Id=YOUR_SCHEMA_USER;\nPassword=YOUR_PASSWORD;\nData Source=YOUR_HOST:PORT/YOUR_SERVICE_NAME;"),
                new("tip",         "If using TNS aliases: Data Source=YOUR_TNS_ALIAS; — the Oracle client resolves the address via tnsnames.ora."),
                new("h2",          "Required Tables"),
                new("bullet",      "PROVIDER — Market data provider definitions: ID, NAME, BASE_URL, END_POINT, API_KEY, PARAMETERS (JSON)."),
                new("bullet",      "STOCK — Security master: ticker IDs, exchange codes, per-provider existence flags."),
                new("bullet",      "EOD_DAILY — Latest EOD snapshot per security. Upserted on each run."),
                new("bullet",      "EOD_HISTORY — Cumulative daily OHLCV history. New records are appended only."),
                new("warning",     "Oracle limits IN-clause expressions to 1,000 items (ORA-01795). The application automatically batches queries into chunks of 500, regardless of your portfolio size."),
            ]),

            new("🔌", "Market Data Providers",
            [
                new("h1",              "Dynamic Market Data Providers"),
                new("body",            "Providers are defined entirely in your PROVIDER table. The active provider dropdown on the main dashboard populates at runtime from whatever rows exist in your database."),
                new("h2",              "Provider Configuration Screen"),
                new("provider_screen", ""),
                new("h2",              "Symbol Management Screen"),
                new("symbols_screen",  ""),
                new("h2",              "Symbol Mapping"),
                new("body",            "Each row in STOCK can be mapped to multiple providers. Set the provider's existence flag (e.g. YAHOOFINANCEEXISTS = 1) and provide the ticker symbol (e.g. YAHOOFINANCEID = 'COMI.CA') for each security."),
                new("tip",             "Securities with no provider mapping enabled are silently skipped during import. Check the existence flag in STOCK if symbols are missing from results."),
            ]),

            new("⏰", "Task Scheduler",
            [
                new("h1",             "Automated Background Scheduling"),
                new("body",           "EODService runs automatically after each trading session via Windows Task Scheduler. The UI does not need to remain open."),
                new("h2",             "Historical Data & Manual Import Screen"),
                new("history_screen", ""),
                new("h2",             "Configuring Automation"),
                new("bullet",         "Step 1 — Enable 'Automated Execution' on the main dashboard."),
                new("bullet",         "Step 2 — Select your market's active trading days (e.g. Sun–Thu for EGX, Mon–Fri for most global markets)."),
                new("bullet",         "Step 3 — Set execution time 30–60 minutes after market close to ensure final prices are published."),
                new("bullet",         "Step 4 — Click Save & Apply Schedule."),
                new("tip",            "The scheduled task runs as the current Windows user. Ensure that account has network access to your Oracle instance and market data providers."),
                new("warning",        "Do not schedule runs during or immediately after market close. Most providers publish final EOD prices 30–60 minutes after the session ends."),
            ]),

            new("🔍", "Troubleshooting & Logs",
            [
                new("h1",     "Diagnostics, Logs & Common Issues"),
                new("h2",     "Log File Location"),
                new("body",   "Logs are stored inside your application folder under the EODConfig directory:"),
                new("code",   "EODConfig\\Logs\\yyyy-MM\\yyyy-MM-dd.txt"),
                new("tip",    "Each day gets its own folder (yyyy-MM) and log file. Start every investigation at the ERROR lines. Most issues are fully described by a single ERROR entry and its stack trace."),
                new("h2",     "Common Issues"),
                new("bullet", "Database Connection Failed — Verify HOST, PORT, SERVICE_NAME, and user privileges (CONNECT + RESOURCE)."),
                new("bullet", "Provider Dropdown Empty — PROVIDER table has no rows, or the DB connection failed before the UI loaded."),
                new("bullet", "No Symbols Processed — STOCK has no rows with the active provider's existence flag enabled."),
                new("bullet", "No Data from Provider — Verify BASE_URL, END_POINT, and API_KEY. For Refinitiv, check DacsUser, ApplicationId, and ServiceName in PARAMETERS."),
                new("bullet", "Refinitiv Login Failed — State.Data was not 'Ok'. Confirm DACS credentials and RTDS server reachability."),
                new("bullet", "ORA-01795 — A query exceeded Oracle's 1,000 IN-clause limit. Report this — a custom query may have bypassed batching."),
            ]),

            new("ℹ️", "About",
            [
                new("h1",     "About EOD Data Service"),
                new("body",   "A professional financial market data management platform for capital markets operations teams requiring reliable, configurable, and auditable daily equity price ingestion."),
                new("h2",     "Technology Stack"),
                new("bullet", "Runtime — .NET 10.0 (net10.0-windows), C# 13"),
                new("bullet", "UI Framework — Windows Forms, PerMonitorV2 High-DPI"),
                new("bullet", "Database — Oracle via Entity Framework Core 10"),
                new("bullet", "Scheduling — Windows Task Scheduler (TaskScheduler 2.11)"),
                new("bullet", "Logging — Microsoft.Extensions.Logging with rotating file logger"),
                new("h2",     "Configuration Root"),
                new("code",   "C:\\EODConfig\\                   Runtime config root\nC:\\EODConfig\\AppSettings.json   Provider & schedule settings\nC:\\EODConfig\\Logs\\              Rotating daily log files"),
                new("warning", "Restrict C:\\EODConfig to authorized administrators only. API keys, passwords, and DACS credentials must never appear in logs or source control."),
            ]),
        ];
    }
}
