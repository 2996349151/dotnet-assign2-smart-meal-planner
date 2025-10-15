1) 领域与架构

统一命名空间为 SmartMealPlanner.*，便于分层与引用一致。

明确三层结构：Models / Repositories / Services，并保持最小可运行入口（若存在 Program.cs）。

2) 仓储层（Repositories）

定义并实现：

IRecipeRepository / JsonRecipeRepository：从 assets/recipes.json 读入食谱数据。

IPantryRepository / JsonPantryRepository：读写 assets/pantry.json，提供持久化。

容错 JSON 映射：对缺失字段、大小写差异、数值/字符串单位（如 “200g”/“200”）做宽松处理，提升数据兼容性。

3) 服务层（Services）

RecipeService：

关键词搜索（标题/标签/食材），按 ID 获取。

收藏功能：读写 assets/favorites.json，提供 Add/Remove/Get 收藏接口。

PantryService：

提供 GetAll、GetById、GetByTitle、Add/Update/Delete、HasIngredient 等常用能力。

采用反射型映射减少字段变动对代码的侵入性（字段新增/重命名时改动更小）。

4) 数据资产（assets）

更新与规范化本地数据：

assets/recipes.json：用最小可运行示例替换，三道菜（番茄意面、鸡肉炒饭、蒜香煎蛋），精简字段与步骤，ImageUrl 允许为空。

assets/pantry.json：统一键名与数量表示（如 pasta: 300、egg: 12、soy_sauce: 5），去除歧义与复数形式。


assets/favorites.json：作为收藏持久化文件（由 RecipeService 维护）。

键名与单位策略：倾向小写+下划线/无空格的键名；数量尽量使用纯数值，便于比较与运算。
