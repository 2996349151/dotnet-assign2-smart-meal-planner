// show detailed information about a single recipe
// allow user to select this recipe for planner
using SmartMealPlanner.Core.Models;
namespace WinFormsAppFinal.UI
{
    public class RecipeDetailForm : Form
    {
        public RecipeDetailForm(Recipe recipe)
        {
            this.Text = recipe.Title;
            this.Width = 400;
            this.Height = 500;
            var lbl = new Label { Left = 10, Top = 10, Width = 360, Height = 30, Text = recipe.Title, Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold) };
            var img = new PictureBox { Left = 10, Top = 50, Width = 360, Height = 180, SizeMode = PictureBoxSizeMode.Zoom };
            if (!string.IsNullOrEmpty(recipe.ImageUrl))
            {
                try { img.Load(recipe.ImageUrl); } catch { }
            }
            var ing = new Label { Left = 10, Top = 240, Width = 360, Height = 80, Text = "Ingredients:\n" + string.Join("\n", recipe.Ingredients.Select(kv => $"{kv.Key}: {kv.Value}")) };
            var steps = new Label { Left = 10, Top = 330, Width = 360, Height = 120, Text = "Steps:\n" + string.Join("\n", recipe.Steps) };
            this.Controls.Add(lbl);
            this.Controls.Add(img);
            this.Controls.Add(ing);
            this.Controls.Add(steps);
        }
    }
}