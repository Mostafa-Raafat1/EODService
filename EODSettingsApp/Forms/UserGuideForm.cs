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
                new("h2",     "System Architecture & Overview"),
                new("body",   "EOD Data Service consists of two complementary applications:\n1. EODSettingsApp (WinForms Admin Console) — GUI for system administration, provider setup, database configuration, historical queries, and schedule management.\n2. EODService.exe (Background Engine) — Lightweight C# console runner executed automatically by Windows Task Scheduler or instantly via the '⚡ Run Now' trigger."),
                new("h2",     "First-Time Deployment Checklist"),
                new("bullet", "Step 1 — Configure your Oracle DB Connection → Settings → Database Connection."),
                new("bullet", "Step 2 — Configure provider endpoints & API keys in PROVIDER table → Settings → Provider Settings."),
                new("bullet", "Step 3 — Register securities and mapping tickers in STOCK table → Settings → Symbol Management."),
                new("bullet", "Step 4 — Select active market data provider on the main dashboard dropdown."),
                new("bullet", "Step 5 — Enable automated execution or click '⚡ Run Now' to verify end-to-end price ingestion."),
                new("warning","EODService will fail if the active provider has no matching row in the PROVIDER table, or if the STOCK table has no securities with that provider's existence flag enabled. Verify database records prior to the first run."),
            ]),

            new("🗄️", "Database Setup & Engine",
            [
                new("h1",          "Oracle Database Integration & Architecture"),
                new("body",        "The service interfaces with Oracle Database via Entity Framework Core. Connection strings are encrypted and stored in C:\\EODConfig\\AppSettings.json."),
                new("h2",          "Database Configuration Screen"),
                new("db_screen",   ""),
                new("h2",          "Connection String Format"),
                new("code",        "User Id=YOUR_SCHEMA_USER; Password=YOUR_PASSWORD; Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=YOUR_HOST)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=YOUR_SERVICE)));"),
                new("tip",         "If using TNS names: Data Source=YOUR_TNS_ALIAS; — Oracle client will resolve the address automatically via tnsnames.ora."),
                new("h2",          "Database Schema & Tables"),
                new("bullet",      "PROVIDER — Stores market data provider configurations: ID, NAME, BASE_URL, END_POINT, API_KEY, and PARAMETERS (JSON payload)."),
                new("bullet",      "STOCK (EOD_STOCKS) — Master security master list: ID, SC_COMP_ID, STOCKNAME, ISIN, exchange code, provider ticker IDs, and existence flags (YahooFinanceExists, TwelveDataExists, ReuterExists)."),
                new("bullet",      "EOD_DAILY — Holds the latest End-Of-Day market quote per security. Atomic upsert updates existing quotes or inserts new daily snapshots."),
                new("bullet",      "EOD_HISTORY — Cumulative historical price ledger. Appends daily OHLCV trading records without overwriting past days."),
                new("h2",          "Oracle IN-Clause Safety (ORA-01795)"),
                new("body",        "Oracle Database limits SQL IN-clause expressions to a maximum of 1,000 literals (ORA-01795). EODService features a built-in batching engine that processes symbol queries in fixed chunks of 500 records."),
                new("warning",     "Never bypass EODService's repository layer when executing custom SQL scripts. Always query in chunks of 500 or fewer items to prevent ORA-01795 runtime exceptions."),
            ]),

            new("🔌", "Market Data Providers",
            [
                new("h1",              "Dynamic Provider Management"),
                new("body",            "Market data providers are managed dynamically from the PROVIDER table. The main dashboard populates available providers at runtime."),
                new("h2",              "Provider Configuration Screen"),
                new("provider_screen", ""),
                new("h2",              "Supported Providers"),
                new("bullet",          "1. Yahoo Finance (ID = 1) — HTTP REST API. Fetches OHLCV chart quotes via query1/query2 endpoints."),
                new("bullet",          "2. TwelveData (ID = 2) — HTTP REST API. Time-series daily quotes authenticated via API Key parameter."),
                new("bullet",          "3. Refinitiv / LSEG / Reuters (ID = 3) — Real-time WebSocket connection (tr_json2 subprotocol). Uses DACS authentication handshake."),
                new("h2",              "Refinitiv / LSEG (Reuters) Integration Deep-Dive"),
                new("body",            "Refinitiv connections interact over WebSockets requesting snapshot views (Streaming = false) with DACS authentication."),
                new("code",            "PARAMETERS JSON Format:\n{\n  \"DacsUser\": \"EODService\",\n  \"ApplicationId\": \"256\",\n  \"ServiceName\": \"ELEKTRON_DD\"\n}"),
                new("warning",         "London Stock Exchange (LSE) RIC Symbol Prefix Rule:\nFor Refinitiv/LSEG real-time & EOD data on London Stock Exchange symbols, delayed quote symbols require a leading slash '/' (e.g., '/VOD.L' instead of 'VOD.L'). Omitting the leading slash on un-entitled DACS accounts will result in entitlement errors."),
                new("h2",              "Symbol Management Screen"),
                new("symbols_screen",  ""),
                new("tip",             "To enable a symbol for ingestion, open Symbol Management, check the provider existence flag (e.g. LSEG Active), enter the ticker ID (e.g. /COMI.CA), and save changes."),
            ]),

            new("⏰", "Task Scheduler & Execution",
            [
                new("h1",             "Automated Scheduling & Instant Execution"),
                new("body",           "EODService is engineered for zero-maintenance background execution after market close. The management UI does not need to stay open."),
                new("h2",             "Historical Data & Query Screen"),
                new("history_screen", ""),
                new("h2",             "Automated Windows Task Scheduler"),
                new("bullet",         "Task Name — EODService_AutoImport (registered in Windows Task Scheduler root folder)."),
                new("bullet",         "Privilege Level — Standard User (TaskRunLevel.LUA). Administrator elevation is NOT required."),
                new("bullet",         "Configuring Days — Select working days matching your exchange (e.g. Sun–Thu for EGX, Mon–Fri for LSE/NYSE)."),
                new("bullet",         "Run Time — Set execution time 30–60 minutes after exchange close to allow providers to publish final settlement prices."),
                new("h2",             "⚡ Instant 'Run Now' Trigger"),
                new("body",           "Clicking '⚡ Run Now' on the main dashboard instantly launches EODService.exe as an independent background process. Ingestion logs stream live into the dashboard log viewer."),
                new("tip",            "You can monitor live execution from the main dashboard terminal panel or inspect daily rotating log files in C:\\EODConfig\\Logs."),
            ]),

            new("📊", "Reporting & Data Export",
            [
                new("h1",     "Multi-Format Data Export & Reporting"),
                new("body",   "The Historical Data screen allows searching, filtering, and exporting market records to Excel, CSV, and PDF."),
                new("h2",     "Supported Export Formats"),
                new("bullet", "Excel Workbook (.xlsx) — Native OpenXML Excel spreadsheet format with custom column formatting and sheet names."),
                new("bullet", "CSV File (.csv) — Standard comma-separated UTF-8 text for database import and Python/R analysis."),
                new("bullet", "PDF Document (.pdf) — Executive-grade financial report with dark navy banner, proportional auto-fitting columns, right-aligned numbers, zebra row striping, and page numbers."),
                new("tip",    "Filter by specific stock symbol or date range before exporting to generate targeted compliance and operational reports."),
            ]),

            new("🔍", "Troubleshooting & Diagnostics",
            [
                new("h1",     "Diagnostics, Logs & Common Error Matrix"),
                new("h2",     "Log File Location"),
                new("code",   "C:\\EODConfig\\Logs\\yyyy-MM\\yyyy-MM-dd.txt"),
                new("h2",     "Error Diagnostic Reference Matrix"),
                new("bullet", "Database Connection Failed — Check HOST, PORT, SERVICE_NAME, user credentials, and Oracle TNS listener status."),
                new("bullet", "Refinitiv DACS Handshake Error — Verify DacsUser, ApplicationId, and ServiceName in PROVIDER.PARAMETERS JSON. Confirm RTDS IP reachability."),
                new("bullet", "Refinitiv Symbol Entitlement Error — Ensure London Stock Exchange symbols have a leading '/' prefix (e.g. '/VOD.L')."),
                new("bullet", "No Symbols Processed — Verify STOCK table has securities with the active provider's existence flag enabled (e.g. YAHOOFINANCEEXISTS = 1)."),
                new("bullet", "ORA-01795 IN-Clause Error — An external query exceeded 1,000 items. Ensure all repository calls use EODService 500-chunk batching."),
                new("bullet", "Task Scheduler Access Denied — Verify Windows user account permissions or check C:\\EODConfig folder write permissions."),
            ]),

            new("ℹ️", "About & System Info",
            [
                new("h1",     "About EOD Data Service"),
                new("body",   "A high-performance financial market data management platform for capital markets operations teams requiring reliable, auditable daily equity price ingestion."),
                new("h2",     "Technology Stack"),
                new("bullet", "Runtime — .NET 10.0 (net10.0-windows), C# 13"),
                new("bullet", "UI Framework — Windows Forms with PerMonitorV2 High-DPI support"),
                new("bullet", "Database — Oracle Database via Entity Framework Core 10"),
                new("bullet", "Scheduling — Windows Task Scheduler (TaskScheduler 2.11)"),
                new("bullet", "Logging — Microsoft.Extensions.Logging with daily rotating file logger"),
                new("h2",     "Configuration Directory Structure"),
                new("code",   "C:\\EODConfig\\                   Runtime configuration root\nC:\\EODConfig\\AppSettings.json   Encrypted database connection & schedule settings\nC:\\EODConfig\\Logs\\              Rotating daily diagnostic log files"),
                new("warning", "Restrict C:\\EODConfig to authorized system administrators only. DACS credentials, database passwords, and API keys must never be shared."),
            ]),
        ];
    }
}
