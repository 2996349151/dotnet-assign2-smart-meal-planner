using SmartMealPlanner.Core.Models;
using SmartMealPlanner.Core.Services;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace SmartMealPlanner.UI
{
    public class PlannerForm : Form
    {
        private Button btnWelcome;
        private Button btnGeneratePlan;
        private Button btnApplyPlan;
        private NumericUpDown numDays;
        private TextBox txtDietTags;
        private TextBox txtDisliked;
        private TextBox txtCuisineWeights;
        private NumericUpDown numTolerance;
        private DataGridView gridPlan;
        private Label lblInstructions;
        private PlannerService plannerService;
        private RecipeService recipeService;
        private PantryService pantryService;

        // Add this field to store the last recommendations for lookup
        private List<Recipe> lastRecommendations = new();

        public PlannerForm()
        {
            this.Text = "Planner";
            this.Width = 800;
            this.Height = 600;

            // Three-line instruction label
            lblInstructions = new Label
            {
                Text = "Welcome to the Meal Planner!\n" +
                       "1. Set your preferences below and click 'Generate Plan'.\n" +
                       "2. Double-click a recipe in the plan to view its details.",
                Left = 20,
                Top = 20,
                Width = 740,
                Height = 60
            };

            // Input controls
            // Days
            var lblDays = new Label { Text = "Days:", Left = 20, Top = 80, Width = 50 };
            numDays = new NumericUpDown { Left = 80, Top = 80, Width = 60, Minimum = 1, Maximum = 30, Value = 7 };

            // Diet Tags
            var lblDietTags = new Label { Text = "Diet Tags (comma):", Left = 160, Top = 80, Width = 120 };
            txtDietTags = new TextBox { Left = 290, Top = 80, Width = 100 };

            // Disliked Ingredients
            var lblDisliked = new Label { Text = "Disliked (comma):", Left = 400, Top = 80, Width = 110 };
            txtDisliked = new TextBox { Left = 520, Top = 80, Width = 100 };

            // Cuisine Weights
            var lblCuisineWeights = new Label { Text = "Cuisine Weights (tag:weight, ...):", Left = 20, Top = 120, Width = 200 };
            txtCuisineWeights = new TextBox { Left = 230, Top = 120, Width = 200 };

            // Tolerance
            var lblTolerance = new Label { Text = "Missing Tolerance:", Left = 450, Top = 120, Width = 120 };
            numTolerance = new NumericUpDown { Left = 580, Top = 120, Width = 60, Minimum = 0, Maximum = 10, Value = 2 };

            // Generate Plan button
            btnGeneratePlan = new Button { Left = 650, Top = 120, Width = 100, Text = "Generate Plan" };
            btnGeneratePlan.Click += async (s, e) => await GeneratePlanAsync();

            // Data grid for plan
            gridPlan = new DataGridView { Left = 20, Top = 170, Width = 740, Height = 320, ReadOnly = true, AllowUserToAddRows = false };
            gridPlan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridPlan.Columns.Add("Date", "Date");
            gridPlan.Columns.Add("MealType", "Meal Type");
            gridPlan.Columns.Add("Recipe", "Recipe");
            gridPlan.DoubleClick += GridPlan_DoubleClick;

            // Apply Plan button at the bottom
            btnApplyPlan = new Button { Left = 20, Top = 500, Width = 120, Text = "Apply Plan" };
            btnApplyPlan.Click += async (s, e) => await ApplyPlanAsync();

            // Home button at the bottom
            btnWelcome = new Button { Left = 160, Top = 500, Width = 120, Text = "Home" };
            btnWelcome.Click += (s, e) => {
                this.Hide();
                this.Owner?.Show();
                this.Close();
            };

            // Add controls to the form
            this.Controls.Add(lblInstructions);
            this.Controls.Add(lblDays);
            this.Controls.Add(numDays);
            this.Controls.Add(lblDietTags);
            this.Controls.Add(txtDietTags);
            this.Controls.Add(lblDisliked);
            this.Controls.Add(txtDisliked);
            this.Controls.Add(lblCuisineWeights);
            this.Controls.Add(txtCuisineWeights);
            this.Controls.Add(lblTolerance);
            this.Controls.Add(numTolerance);
            this.Controls.Add(btnGeneratePlan);
            this.Controls.Add(gridPlan);
            this.Controls.Add(btnApplyPlan);
            this.Controls.Add(btnWelcome);

            // Initialize services
            plannerService = new PlannerService();
            recipeService = new RecipeService(new Core.Data.Repositories.JsonRecipeRepository("assets/recipes.json"));
            pantryService = new PantryService(new Core.Data.Repositories.JsonPantryRepository("assets/pantry.json"));
        }

        // Method to generate the meal plan
        private async Task GeneratePlanAsync()
        {
            gridPlan.Rows.Clear();
            // Gather user preferences
            int days = (int)numDays.Value;
            var userPref = new UserPreference
            {
                DietTags = txtDietTags.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
                Disliked = txtDisliked.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
                CuisineWeights = txtCuisineWeights.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(pair => pair.Split(':'))
                    .Where(arr => arr.Length == 2 && double.TryParse(arr[1], out _))
                    .ToDictionary(arr => arr[0].Trim().ToLowerInvariant(), arr => double.Parse(arr[1])),
                MissingTolerance = (int)numTolerance.Value
            };
            // Get pantry and recommendations
            var pantry = await pantryService.GetAsync();
            var recommendations = await recipeService.GetRecommendationsAsync(pantry, userPref, userPref.MissingTolerance);
            lastRecommendations = recommendations.ToList();

            // Generate and display plan
            var plan = await plannerService.GenerateAsync(days);
            var mealTypes = Enum.GetValues(typeof(MealType)).Cast<MealType>().ToList();
            var recIdx = 0;

            // Assign recipes to the plan
            foreach (var kv in plan)
            {
                var date = kv.Key;
                foreach (var meal in mealTypes)
                {
                    Recipe recipe = null;
                    if (recIdx < recommendations.Count)
                        recipe = recommendations[recIdx++];
                    await plannerService.AssignAsync(date, meal, recipe);
                    gridPlan.Rows.Add(date.ToShortDateString(), meal.ToString(), recipe?.Title ?? "(none)");
                }
            }
        }

        // Method to handle double-click on a recipe in the plan
        private void GridPlan_DoubleClick(object sender, EventArgs e)
        {
            if (gridPlan.CurrentRow == null) return;
            var recipeTitle = gridPlan.CurrentRow.Cells["Recipe"].Value?.ToString();
            if (string.IsNullOrEmpty(recipeTitle) || recipeTitle == "(none)") return;

            var recipe = lastRecommendations.FirstOrDefault(r => r.Title == recipeTitle);
            if (recipe != null)
            {
                var detailForm = new WinFormsAppFinal.UI.RecipeDetailForm(recipe);
                detailForm.ShowDialog();
            }
        }

        // Method to apply the meal plan
        private async Task ApplyPlanAsync()
        {
            // Ensure a plan has been generated
            if (gridPlan.Rows.Count == 0)
            {
                MessageBox.Show("No plan has been generated. Please generate a plan before applying.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var plan = plannerService.GetCurrentPlan();

            try
            {
                // Update pantry based on recipes in the plan
                foreach (var day in plan.Values)
                {
                    foreach (var recipe in day.Values)
                    {
                        if (recipe != null)
                        {
                            await pantryService.ApplyCookingAsync(recipe);
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                // Show error if ingredients are insufficient
                MessageBox.Show(ex.Message, "Insufficient Ingredients", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Save the updated pantry
            using var sfd = new SaveFileDialog
            {
                Title = "Save Meal Plan",
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                FileName = "mealplan.json"
            };

            // Show save file dialog
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var output = plan.ToDictionary(
                    kv => kv.Key.ToShortDateString(),
                    kv => kv.Value.ToDictionary(
                        mt => mt.Key.ToString(),
                        mt => mt.Value?.Title ?? "(none)"
                    )
                );

                await File.WriteAllTextAsync(sfd.FileName, JsonSerializer.Serialize(output, new JsonSerializerOptions { WriteIndented = true }));

                MessageBox.Show("Meal plan applied and saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}