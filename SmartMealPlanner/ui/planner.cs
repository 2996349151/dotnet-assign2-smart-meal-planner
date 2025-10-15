using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartMealPlanner.Core.Models;
using SmartMealPlanner.Core.Services;
using SmartMealPlanner.Core.Interfaces;

namespace SmartMealPlanner.UI
{
    public class PlannerForm : Form
    {
        private Button btnWelcome;
        private Button btnGeneratePlan;
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

        public PlannerForm()
        {
            this.Text = "Planner";
            this.Width = 800;
            this.Height = 600;

            lblInstructions = new Label { Text = "Set your preferences and generate a meal plan!", Left = 20, Top = 20, Width = 600, Height = 30 };
            btnWelcome = new Button { Left = 650, Top = 20, Width = 100, Text = "Home" };
            btnWelcome.Click += (s, e) => {
                this.Hide();
                this.Owner?.Show();
                this.Close();
            };

            var lblDays = new Label { Text = "Days:", Left = 20, Top = 60, Width = 50 };
            numDays = new NumericUpDown { Left = 80, Top = 60, Width = 60, Minimum = 1, Maximum = 30, Value = 7 };
            var lblDietTags = new Label { Text = "Diet Tags (comma):", Left = 160, Top = 60, Width = 120 };
            txtDietTags = new TextBox { Left = 290, Top = 60, Width = 100 };
            var lblDisliked = new Label { Text = "Disliked (comma):", Left = 400, Top = 60, Width = 110 };
            txtDisliked = new TextBox { Left = 520, Top = 60, Width = 100 };
            var lblCuisineWeights = new Label { Text = "Cuisine Weights (tag:weight, ...):", Left = 20, Top = 100, Width = 200 };
            txtCuisineWeights = new TextBox { Left = 230, Top = 100, Width = 200 };
            var lblTolerance = new Label { Text = "Missing Tolerance:", Left = 450, Top = 100, Width = 120 };
            numTolerance = new NumericUpDown { Left = 580, Top = 100, Width = 60, Minimum = 0, Maximum = 10, Value = 2 };

            btnGeneratePlan = new Button { Left = 650, Top = 100, Width = 100, Text = "Generate Plan" };
            btnGeneratePlan.Click += async (s, e) => await GeneratePlanAsync();

            gridPlan = new DataGridView { Left = 20, Top = 150, Width = 740, Height = 380, ReadOnly = true, AllowUserToAddRows = false };
            gridPlan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridPlan.Columns.Add("Date", "Date");
            gridPlan.Columns.Add("MealType", "Meal Type");
            gridPlan.Columns.Add("Recipe", "Recipe");

            this.Controls.Add(lblInstructions);
            this.Controls.Add(btnWelcome);
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

            // Setup services (simple instantiation, adjust as needed)
            plannerService = new PlannerService();
            recipeService = new RecipeService(new Core.Data.Repositories.JsonRecipeRepository("assets/recipes.json"));
            pantryService = new PantryService(new Core.Data.Repositories.JsonPantryRepository("assets/pantry.json"));
        }

        private async Task GeneratePlanAsync()
        {
            gridPlan.Rows.Clear();
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
            var pantry = await pantryService.GetAsync();
            var recommendations = await recipeService.GetRecommendationsAsync(pantry, userPref, userPref.MissingTolerance);
            var plan = await plannerService.GenerateAsync(days);
            var mealTypes = Enum.GetValues(typeof(MealType)).Cast<MealType>().ToList();
            var recIdx = 0;
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
    }
}