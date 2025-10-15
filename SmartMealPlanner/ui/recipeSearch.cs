using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartMealPlanner.Core.Services;
using SmartMealPlanner.Core.Models;
using SmartMealPlanner.Core.Data.Repositories;
using WinFormsAppFinal.UI;

namespace SmartMealPlanner.UI
{
    public partial class RecipeSearchForm : Form
    {
        private readonly RecipeService _service;
        private List<Recipe> _recipes = new();
        private ComboBox filterCombo;
        private TextBox searchBox;
        private DataGridView grid;
        private Button searchBtn;
        private Button btnWelcome;

        public RecipeSearchForm()
        {
            // Debug: Output recipes.json content
            var jsonPath = "assets/recipes.json";
            var repo = new JsonRecipeRepository(jsonPath);
            _service = new RecipeService(repo);
            BuildUI();
            LoadTags();
            LoadRecipesAsync();
        }

        private void BuildUI()
        {
            this.Text = "Recipe Search";
            this.Width = 700;
            this.Height = 500;

            searchBox = new TextBox { Left = 10, Top = 10, Width = 300 };
            filterCombo = new ComboBox { Left = 320, Top = 10, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            searchBtn = new Button { Left = 480, Top = 10, Width = 80, Text = "Search" };
            searchBtn.Click += async (s, e) => await LoadRecipesAsync();

            grid = new DataGridView
            {
                Left = 10,
                Top = 40,
                Width = 650,
                Height = 400,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false
            };
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Id", DataPropertyName = "Id", Width = 100, Visible = false });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Title", DataPropertyName = "Title", Width = 250 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tags", DataPropertyName = "Tags", Width = 250 });
            grid.DoubleClick += Grid_DoubleClick;

            btnWelcome = new Button { Left = 570, Top = 10, Width = 80, Text = "Home" };
            btnWelcome.Click += (s, e) => {
                this.Hide();
                this.Owner?.Show();
                this.Close();
            };

            this.Controls.Add(searchBox);
            this.Controls.Add(filterCombo);
            this.Controls.Add(searchBtn);
            this.Controls.Add(grid);
            this.Controls.Add(btnWelcome);
        }

        private async Task LoadRecipesAsync()
        {
            string keyword = searchBox.Text;
            string tag = filterCombo.SelectedItem as string;
            var results = await _service.SearchAsync(keyword);
            if (!string.IsNullOrEmpty(tag) && tag != "All")
                results = results.Where(r => r.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase)).ToList();
            _recipes = results.ToList();
            grid.DataSource = _recipes.Select(r => new
            {
                r.Title,
                r.CookTimeMins,
                Tags = string.Join(", ", r.Tags),
                r.Id
            }).ToList();
        }

        private async void LoadTags()
        {
            var all = await _service.SearchAsync("");
            var tags = all.SelectMany(r => r.Tags).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(t => t).ToList();
            filterCombo.Items.Clear();
            filterCombo.Items.Add("All");
            filterCombo.Items.AddRange(tags.ToArray());
            filterCombo.SelectedIndex = 0;
        }

        private void Grid_DoubleClick(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null) return;
            var id = grid.CurrentRow.Cells[0].Value?.ToString(); // Id is now column 0
            var recipe = _recipes.FirstOrDefault(r => r.Id == id);
            if (recipe != null)
            {
                var detail = new RecipeDetailForm(recipe);
                detail.ShowDialog();
            }
        }
    }
    
}
