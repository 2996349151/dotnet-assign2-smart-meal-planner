# Smart Meal Planner

## 1) Domain & Architecture

Unified namespace as `SmartMealPlanner.*` for consistent layering and references.

Clear three-layer structure: Models / Repositories / Services, maintaining a minimal runnable entry point (if Program.cs exists).

## 2) Repository Layer (Repositories)

Define and implement:

**IRecipeRepository / JsonRecipeRepository**: Read recipe data from `assets/recipes.json`.

**IPantryRepository / JsonPantryRepository**: Read/write `assets/pantry.json`, providing persistence.

**Fault-tolerant JSON mapping**: Handle missing fields, case differences, and numeric/string units (e.g., "200g"/"200") with lenient processing to improve data compatibility.

## 3) Service Layer (Services)

**RecipeService**:
- Keyword search (title/tags/ingredients), retrieval by ID
- Favorites functionality: Read/write `assets/favorites.json`, providing Add/Remove/Get favorites interface

**PantryService**:
- Provide GetAll, GetById, GetByTitle, Add/Update/Delete, HasIngredient and other common capabilities
- Use reflection-based mapping to reduce code intrusion from field changes (smaller changes when fields are added/renamed)

## 4) Data Assets (assets)

Update and standardize local data:

**assets/recipes.json**: Replace with minimal runnable examples - three dishes (Tomato Pasta, Chicken Fried Rice, Garlic Fried Eggs), simplified fields and steps, ImageUrl can be null.

**assets/pantry.json**: Unified key names and quantity representation (e.g., pasta: 300, egg: 12, soy_sauce: 5), removing ambiguity and plural forms.

**assets/favorites.json**: Favorites persistence file (maintained by RecipeService).

**Key naming and unit strategy**: Prefer lowercase + underscore/no spaces for key names; quantities should use pure numeric values when possible for easier comparison and calculation.
