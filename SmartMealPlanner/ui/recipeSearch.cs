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

        // Build the UI components
        private void BuildUI()
        {
            this.Text = "Recipe Search";
            this.Width = 700;
            this.Height = 500;

            // Three-line instruction label
            var lblInstructions = new Label
            {
                Left = 10,
                Top = 10,
                Width = 650,
                Height = 60,
                Text = "Welcome to the Recipe Search Page!\n" +
                       "1. Enter keywords or select a tag to filter recipes.\n" +
                       "2. Double-click a recipe to view its details."
            };

            // Search box, filter combo, and search button
            searchBox = new TextBox { Left = 10, Top = 80, Width = 300 };
            filterCombo = new ComboBox { Left = 320, Top = 80, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            searchBtn = new Button { Left = 480, Top = 80, Width = 80, Text = "Search" };
            searchBtn.Click += async (s, e) => await LoadRecipesAsync();

            // Data grid for displaying recipes
            grid = new DataGridView
            {
                Left = 10,
                Top = 120,
                Width = 650,
                Height = 320,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false
            };
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Id", DataPropertyName = "Id", Width = 100, Visible = false });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Title", DataPropertyName = "Title", Width = 250 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tags", DataPropertyName = "Tags", Width = 250 });
            grid.DoubleClick += Grid_DoubleClick;

            // Home button
            btnWelcome = new Button { Left = 570, Top = 80, Width = 80, Text = "Home" };
            btnWelcome.Click += (s, e) => {
                this.Hide();
                this.Owner?.Show();
                this.Close();
            };

            // Add controls to the form
            this.Controls.Add(lblInstructions);
            this.Controls.Add(searchBox);
            this.Controls.Add(filterCombo);
            this.Controls.Add(searchBtn);
            this.Controls.Add(grid);
            this.Controls.Add(btnWelcome);
        }

        // Load recipes based on search and filter
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

        // Load tags into the filter combo box
        private async void LoadTags()
        {
            var all = await _service.SearchAsync("");
            var tags = all.SelectMany(r => r.Tags).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(t => t).ToList();
            filterCombo.Items.Clear();
            filterCombo.Items.Add("All");
            filterCombo.Items.AddRange(tags.ToArray());
            filterCombo.SelectedIndex = 0;
        }

        // Handle double-click on a recipe to show details
        private void Grid_DoubleClick(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null) return;
            var id = grid.CurrentRow.Cells[0].Value?.ToString();
            var recipe = _recipes.FirstOrDefault(r => r.Id == id);
            if (recipe != null)
            {
                var detail = new RecipeDetailForm(recipe);
                detail.ShowDialog();
            }
        }
    }
    
}
