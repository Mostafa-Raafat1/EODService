using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EODSettingsApp.AppSettingsConfig;

namespace EODSettingsApp.Forms
{
    /// <summary>
    /// Modal dialog for viewing, adding, editing, and removing stock ticker symbols stored in AppSettings.json.
    /// </summary>
    public partial class SymbolSettingsForm : Form
    {
        private int _editingIndex = -1;

        public SymbolSettingsForm()
        {
            InitializeComponent();
            LoadSymbolSettings();
        }

        // ── Load ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Reads AppSettings.json and populates the symbols listbox.
        /// </summary>
        private void LoadSymbolSettings()
        {
            try
            {
                var model = AppSettingsService.Load();
                lstSymbols.Items.Clear();
                ResetEditState();

                if (model.SymbolSettings?.Symbols != null)
                {
                    foreach (var symbol in model.SymbolSettings.Symbols)
                    {
                        if (!string.IsNullOrWhiteSpace(symbol))
                        {
                            lstSymbols.Items.Add(symbol.Trim().ToUpperInvariant());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘ Could not load symbols: {ex.Message}");
            }
        }

        // ── Add / Edit Symbol Logic ──────────────────────────────────────────────

        private void BtnAddSymbol_Click(object? sender, EventArgs e)
        {
            AddOrUpdateSymbolFromInput();
        }

        private void TxtNewSymbol_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                AddOrUpdateSymbolFromInput();
            }
        }

        private void BtnEditSymbol_Click(object? sender, EventArgs e)
        {
            if (lstSymbols.SelectedItem == null)
            {
                SetStatus(success: false, "✘ Please select a symbol from the list to edit.");
                return;
            }

            _editingIndex = lstSymbols.SelectedIndex;
            var currentSymbol = lstSymbols.SelectedItem.ToString()!;

            txtNewSymbol.Text = currentSymbol;
            txtNewSymbol.Focus();
            txtNewSymbol.SelectAll();

            btnAddSymbol.Text = "Update Symbol";
            btnAddSymbol.BackColor = Color.FromArgb(16, 185, 129); // Emerald green for update action

            SetStatus(success: true, $"Editing '{currentSymbol}'. Type changes and click 'Update Symbol'.");
        }

        private void AddOrUpdateSymbolFromInput()
        {
            var symbol = txtNewSymbol.Text.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(symbol))
            {
                SetStatus(success: false, "✘ Please enter a valid ticker symbol.");
                return;
            }

            // Check duplicate entries (excluding the symbol currently being edited)
            for (int i = 0; i < lstSymbols.Items.Count; i++)
            {
                if (i == _editingIndex) continue;

                if (string.Equals(lstSymbols.Items[i].ToString(), symbol, StringComparison.OrdinalIgnoreCase))
                {
                    SetStatus(success: false, $"✘ Symbol '{symbol}' is already in the list.");
                    return;
                }
            }

            if (_editingIndex >= 0 && _editingIndex < lstSymbols.Items.Count)
            {
                // Update existing item
                var oldSymbol = lstSymbols.Items[_editingIndex].ToString();
                lstSymbols.Items[_editingIndex] = symbol;
                lstSymbols.SelectedIndex = _editingIndex;
                SetStatus(success: true, $"✔ Updated '{oldSymbol}' → '{symbol}'.");
            }
            else
            {
                // Add new item
                lstSymbols.Items.Add(symbol);
                lstSymbols.SelectedIndex = lstSymbols.Items.Count - 1;
                SetStatus(success: true, $"✔ Added '{symbol}' to list.");
            }

            ResetEditState();
        }

        private void ResetEditState()
        {
            _editingIndex = -1;
            txtNewSymbol.Clear();
            btnAddSymbol.Text = "Add Symbol";
            btnAddSymbol.BackColor = Color.FromArgb(30, 58, 138); // Standard Navy Blue
        }

        // ── Remove Symbol ────────────────────────────────────────────────────────

        private void BtnRemoveSymbol_Click(object? sender, EventArgs e)
        {
            if (lstSymbols.SelectedItem == null)
            {
                SetStatus(success: false, "✘ Please select a symbol to remove.");
                return;
            }

            var removedSymbol = lstSymbols.SelectedItem.ToString();
            lstSymbols.Items.Remove(lstSymbols.SelectedItem);
            ResetEditState();

            SetStatus(success: true, $"✔ Removed '{removedSymbol}' from list.");
        }

        // ── Save ─────────────────────────────────────────────────────────────────

        private void BtnSaveSymbolSettings_Click(object? sender, EventArgs e)
        {
            if (lstSymbols.Items.Count == 0)
            {
                SetStatus(success: false, "✘ Symbol list cannot be empty. At least 1 symbol is required.");
                return;
            }

            try
            {
                var symbolsList = new List<string>();
                foreach (var item in lstSymbols.Items)
                {
                    symbolsList.Add(item.ToString()!);
                }

                var currentModel = AppSettingsService.Load();
                currentModel.SymbolSettings = new SymbolSettingsSection
                {
                    Symbols = symbolsList
                };

                AppSettingsService.Save(currentModel);
                SetStatus(success: true, "✔ Symbol settings saved successfully.");
            }
            catch (Exception ex)
            {
                SetStatus(success: false, $"✘ Save failed: {ex.Message}");
            }
        }

        // ── Status Helper ────────────────────────────────────────────────────────

        private void SetStatus(bool success, string message)
        {
            lblSymbolSettingsStatus.ForeColor = success
                ? Color.FromArgb(22, 163, 74)   // Green
                : Color.FromArgb(185, 28, 28);  // Red
            lblSymbolSettingsStatus.Text = message;
        }
    }
}
