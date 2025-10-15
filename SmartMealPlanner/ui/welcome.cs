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

            // Three-line instruction label
            var lblInstructions = new Label
            {
                Left = 20,
                Top = 10,
                Width = 360,
                Height = 60,
                Text = "Welcome to Smart Meal Planner!\n" +
                       "1. Use the buttons below to navigate.\n" +
                       "2. Here you can search recipes, manage ingredients, or plan your meals."
            };

            // Buttons for navigation
            var btnRecipe = new Button { Text = "Recipe Search", Left = 100, Top = 70, Width = 200, Height = 40 };
            var btnIngredients = new Button { Text = "Ingredient Management", Left = 100, Top = 130, Width = 200, Height = 40 };
            var btnPlanner = new Button { Text = "Planner", Left = 100, Top = 190, Width = 200, Height = 40 };
            var btnExit = new Button { Text = "Exit", Left = 100, Top = 250, Width = 200, Height = 40 };

            // Button click events
            btnRecipe.Click += (s, e) => { NavigateTo(new RecipeSearchForm()); };
            btnIngredients.Click += (s, e) => { NavigateTo(new IngredientManagementForm()); };
            btnPlanner.Click += (s, e) => { NavigateTo(new PlannerForm()); };
            btnExit.Click += (s, e) => { this.Close(); };

            // Add controls to the form
            this.Controls.Add(lblInstructions);
            this.Controls.Add(btnRecipe);
            this.Controls.Add(btnIngredients);
            this.Controls.Add(btnPlanner);
            this.Controls.Add(btnExit);
        }

        private void NavigateTo(Form form)
        {
            // Set this form as the owner of the new form
            form.Owner = this; 
            this.Hide();
            form.FormClosed += (s, e) => { this.Show(); };
            form.Show();
        }
    }
}
