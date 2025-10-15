using System;
using System.Windows.Forms;
using SmartMealPlanner.Core.Services;
using SmartMealPlanner.Core.Data.Repositories;
using SmartMealPlanner.Core.Interfaces;

namespace SmartMealPlanner.UI
{
    public class WelcomeForm : Form
    {
        private IPantryService _pantryService;
        public WelcomeForm()
        {
            this.Text = "Welcome";
            this.Width = 400;
            this.Height = 350;

            // Create the pantry service using the JSON repository

            var btnRecipe = new Button { Text = "Recipe Search", Left = 100, Top = 40, Width = 200, Height = 40 };
            var btnIngredients = new Button { Text = "Ingredient Management", Left = 100, Top = 100, Width = 200, Height = 40 };
            var btnPlanner = new Button { Text = "Planner", Left = 100, Top = 160, Width = 200, Height = 40 };
            var btnExit = new Button { Text = "Exit", Left = 100, Top = 220, Width = 200, Height = 40 };

            btnRecipe.Click += (s, e) => { NavigateTo(new RecipeSearchForm()); };
            btnIngredients.Click += (s, e) => { NavigateTo(new IngredientManagementForm()); };
            btnPlanner.Click += (s, e) => { NavigateTo(new PlannerForm()); };
            btnExit.Click += (s, e) => { this.Close(); };

            this.Controls.Add(btnRecipe);
            this.Controls.Add(btnIngredients);
            this.Controls.Add(btnPlanner);
            this.Controls.Add(btnExit);
        }

        private void NavigateTo(Form form)
        {
            form.Owner = this; // Set WelcomeForm as owner
            this.Hide();
            form.FormClosed += (s, e) => { this.Show(); };
            form.Show();
        }
    }
}
