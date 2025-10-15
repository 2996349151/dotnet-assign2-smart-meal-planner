using SmartMealPlanner.Core.Data.Repositories;
using SmartMealPlanner.Core.Interfaces;
using SmartMealPlanner.Core.Models;
using SmartMealPlanner.Core.Services;
using System.ComponentModel;

namespace SmartMealPlanner.UI
{

    public class IngredientManagementForm : Form
    {
        private IPantryService _pantryService;
        private Pantry _pantry;
        private BindingList<Ingredient> _ingredientsBindingList;

        // Add missing controls
        private DataGridView dataGridViewIngredients;
        private Button buttonSave;
        private Button buttonAdd;
        private DataGridViewTextBoxColumn titleColumn;
        private DataGridViewTextBoxColumn quantityColumn;
        private Button buttonDelete;
        private Label labelInstructions; // Instructions label
        private Button btnWelcome;

        public IngredientManagementForm()
        {
            var jsonPath = "assets/pantry.json";
            var repo = new JsonPantryRepository(jsonPath);
            _pantryService = new PantryService(repo);
            InitializeComponent();
            this.Load += IngredientManagementForm_Load;
            this.Shown += (s, e) => { };
            AddNavigationButtons();
        }

        // Add missing InitializeComponent method
        private void InitializeComponent()
        {
            labelInstructions = new Label();
            dataGridViewIngredients = new DataGridView();
            titleColumn = new DataGridViewTextBoxColumn();
            quantityColumn = new DataGridViewTextBoxColumn();
            buttonSave = new Button();
            buttonAdd = new Button();
            buttonDelete = new Button();
            ((ISupportInitialize)dataGridViewIngredients).BeginInit();
            SuspendLayout();
            // 
            // labelInstructions
            // 
            labelInstructions.AutoSize = true;
            labelInstructions.Location = new Point(12, 8);
            labelInstructions.Name = "labelInstructions";
            labelInstructions.Size = new Size(800, 60);
            labelInstructions.Text = "Welcome to the ingredient management page!\n" +
                "You can add, remove, edit available ingredients here.\n" +
                "Remember to save any changes!";
            // 
            // dataGridViewIngredients
            // 
            dataGridViewIngredients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewIngredients.ColumnHeadersHeight = 46;
            dataGridViewIngredients.AutoGenerateColumns = false;
            // Only show Title and Quantity columns
            titleColumn.DataPropertyName = "Title";
            titleColumn.HeaderText = "Ingredient (Unit)";
            titleColumn.Name = "Title";
            quantityColumn.DataPropertyName = "Quantity";
            quantityColumn.HeaderText = "Quantity";
            quantityColumn.Name = "Quantity";
            dataGridViewIngredients.Columns.Clear();
            dataGridViewIngredients.Columns.AddRange(new DataGridViewColumn[] { titleColumn, quantityColumn });
            dataGridViewIngredients.Location = new Point(12, 60); // Move table down to make space for instructions
            dataGridViewIngredients.MultiSelect = false;
            dataGridViewIngredients.Name = "dataGridViewIngredients";
            dataGridViewIngredients.RowHeadersWidth = 82;
            dataGridViewIngredients.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewIngredients.Size = new Size(1228, 480); // Reduced height to make room for buttons
            dataGridViewIngredients.TabIndex = 0;
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(12, 560);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(75, 30);
            buttonSave.TabIndex = 1;
            buttonSave.Text = "Save";
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonAdd
            // 
            buttonAdd.Location = new Point(93, 560);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Size = new Size(75, 30);
            buttonAdd.TabIndex = 2;
            buttonAdd.Text = "Add";
            buttonAdd.Click += buttonAdd_Click;
            // 
            // buttonDelete
            // 
            buttonDelete.Location = new Point(174, 560);
            buttonDelete.Name = "buttonDelete";
            buttonDelete.Size = new Size(75, 30);
            buttonDelete.TabIndex = 3;
            buttonDelete.Text = "Delete";
            buttonDelete.Click += buttonDelete_Click;
            // 
            // IngredientManagementForm
            // 
            ClientSize = new Size(1282, 610); // Reduced height to fit everything
            Controls.Add(labelInstructions);
            Controls.Add(dataGridViewIngredients);
            Controls.Add(buttonSave);
            Controls.Add(buttonAdd);
            Controls.Add(buttonDelete);
            Name = "IngredientManagementForm";
            Text = "Ingredient Management";
            ((ISupportInitialize)dataGridViewIngredients).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private async void IngredientManagementForm_Load(object sender, EventArgs e)
        {
            await LoadPantryAsync();
        }

        private async Task LoadPantryAsync()
        {
            try
            {
                _pantry = await _pantryService.GetAsync();
                _ingredientsBindingList = new BindingList<Ingredient>(
                    _pantry.Items
                        .Select(kvp => new Ingredient(0, kvp.Key, kvp.Value, ""))
                        .ToList()
                );
                dataGridViewIngredients.AutoGenerateColumns = false;
                dataGridViewIngredients.DataSource = null;
                dataGridViewIngredients.DataSource = _ingredientsBindingList;
            }
            catch (Exception ex)
            {
                // Optionally log error, but do not show message box
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        private void SaveChanges()
        {
            _pantry.Items.Clear();
            foreach (var ingredient in _ingredientsBindingList)
            {
                if (!string.IsNullOrWhiteSpace(ingredient.Title))
                {
                    _pantry.Items[ingredient.Title] = ingredient.Quantity;
                }
            }

            _pantryService.SaveAsync(_pantry).GetAwaiter().GetResult();
            MessageBox.Show("Pantry saved successfully.");
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            _ingredientsBindingList.Add(new Ingredient(0, "New Ingredient", 0, ""));
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewIngredients.CurrentRow != null)
            {
                var ingredient = dataGridViewIngredients.CurrentRow.DataBoundItem as Ingredient;
                if (ingredient != null)
                {
                    _ingredientsBindingList.Remove(ingredient);
                }
            }
        }

        private void AddNavigationButtons()
        {
            btnWelcome = new Button { Left = 1100, Top = 10, Width = 80, Text = "Home" };
            btnWelcome.Click += (s, e) => {
                this.Hide();
                this.Owner?.Show();
                this.Close();
            };
            this.Controls.Add(btnWelcome);
        }
    }
}