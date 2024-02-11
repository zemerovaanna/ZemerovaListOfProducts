using ConsoleAppListOfProducts.Database;
using ConsoleAppListOfProducts.Models;

namespace ConsoleAppListOfProducts
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Check.WriteStylishText(false, " ——> Добро пожаловать! <——", AppColors.Title);
            int step = 1;
            while (step < int.MaxValue)
            {
                Check.WriteStylishText(false, "\n\n Шаг " + step + ".", AppColors.Info);
                Check.WriteStylishText(false, "\n ——> Выбор действия.", AppColors.Action);
                Check.GetActions();

                string answer = "0";
                do
                {
                    Check.WriteStylishText(true, "\n Пожалуйста, введите ", AppColors.Default);
                    Check.WriteStylishText(true, "код ", AppColors.Action);
                    Check.WriteStylishText(true, "(номер) действия: ", AppColors.Default);
                    answer = Console.ReadLine();
                }
                while (byte.TryParse(answer, out byte number) == false);

                string enter = "";
                switch (byte.Parse(answer))
                {
                    case 0:
                        {
                            Check.WriteStylishText(false, "\n ——> До свидания! <——", AppColors.Title);
                            Environment.Exit(0);
                            break;
                        }

                    case 1:
                        {
                            step++;
                            Check.WriteStylishText(false, "\n\n ==> Поиск товара.\n", AppColors.Title);
                            enter = Check.CheckString("название товара", 100);
                            if (Check.CheckExit(enter) == null) { break; }
                            ProgramContext.SearchProducts(enter);
                            break;
                        }

                    case 2:
                        {
                            step++;
                            Check.WriteStylishText(false, "\n\n ==> Список товаров:\n", AppColors.Title);
                            ProgramContext.ListOfProducts();
                            break;
                        }

                    case 3:
                        {
                            step++;
                            Check.WriteStylishText(false, "\n\n ==> Первые три дорогих товара:\n", AppColors.Title);
                            ProgramContext.GetThreeMostExpensiveProducts();
                            break;
                        }

                    case 4:
                        {
                            step++;
                            Check.WriteStylishText(false, "\n\n ==> Список товаров из определённой категории.\n", AppColors.Title);
                            int categoryId = ProgramContext.SelectCategoryById();
                            if (categoryId != 0) { ProgramContext.GetProductsByCategory(categoryId); }
                            break;
                        }

                    case 5:
                        {
                            step++;
                            ProgramContext.ListOfCategories();
                            break;
                        }

                    case 6:
                        {
                            step++;
                            Check.WriteStylishText(false, "\n\n ==> Добавление категории.\n", AppColors.Title);
                            enter = Check.CheckString("название категории", 50);
                            if (Check.CheckExit(enter) == null) { break; }
                            ProgramContext.AddCategory(enter);
                            break;
                        }

                    case 7:
                        {
                            step++;
                            Check.WriteStylishText(false, "\n\n ==> Изменение категории.\n", AppColors.Title);
                            int categoryId = ProgramContext.SelectCategoryById();
                            if (categoryId != 0)
                            {
                                enter = Check.CheckString("новое название категории", 50);
                                if (Check.CheckExit(enter) == null) { break; }
                                ProgramContext.UpdateCategory(categoryId, enter);
                            }
                            break;
                        }

                    case 8:
                        {
                            step++;
                            Check.WriteStylishText(false, "\n\n ==> Удаление категории.\n", AppColors.Title);
                            int categoryId = ProgramContext.SelectCategoryById();
                            if (categoryId != 0) { ProgramContext.DeleteCategory(categoryId); }
                            break;
                        }

                    case 9:
                        {
                            step++;
                            Check.WriteStylishText(false, "\n\n ==> Добавление товара.\n", AppColors.Title);
                            Check.WriteProduct(0);
                            break;
                        }

                    case 10:
                        {
                            step++;
                            Check.WriteStylishText(false, "\n\n ==> Изменение товара (без изменения количества).\n", AppColors.Title);
                            int productId = ProgramContext.SelectProductById();
                            if (productId != 0) { Check.WriteProduct(productId); }
                            break;
                        }

                    case 11:
                        {
                            step++;
                            Check.WriteStylishText(false, "\n\n ==> Изменение количества товара.\n", AppColors.Title);
                            int productId = ProgramContext.SelectProductById();
                            if (productId != 0)
                            {
                                string quantity = Check.CorrectNumber("\nПожалуйста, введите количество товара:");
                                if (Check.CheckExit(quantity) == null) { break; }
                                ProgramContext.UpdateProductQuantity(productId, Convert.ToInt32(quantity));
                            }
                            break;
                        }

                    case 12:
                        {
                            step++;
                            Check.WriteStylishText(false, "\n\n ==> Удаление товара.\n", AppColors.Title);
                            int productId = ProgramContext.SelectProductById();
                            if (productId != 0) { ProgramContext.DeleteProduct(productId); }
                            break;
                        }

                    default:
                        {
                            step++;
                            Check.WriteStylishText(false, " Действие не найдено.", AppColors.Info);
                            break;
                        }
                }
            }
            Check.WriteStylishText(false, "\n ——> Работа завершена. <——", AppColors.Title);
        }
    }
}