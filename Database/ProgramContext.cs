using ConsoleAppListOfProducts.Models;
using ConsoleAppListOfProducts.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics.Metrics;
using System.Diagnostics;

namespace ConsoleAppListOfProducts.Database
{
    public static class ProgramContext
    {
        private static DatabaseContext context = new DatabaseContext();



        /// <summary>
        /// Поиск товара по названию.
        /// </summary>
        /// <param name="searchText">Название товара.</param>
        public static void SearchProducts(string searchText)
        {
            try
            {
                List<Product> products = context.Products.Where(p => p.ProductName.Contains(searchText)).ToList();
                if (products.Count > 0)
                { GetProducts(products); }
                else
                { Check.WriteStylishText(false, "Ничего не найдено.", AppColors.Info); }
            }
            catch { Check.WriteStylishText(false, "Ошибка! Не удалось загрузить.", AppColors.Error); }
        }



        /// <summary>
        /// Вывод списка товаров.
        /// </summary>
        /// <param name="products">Список товаров для вывода.</param>
        public static void GetProducts(List<Product> products)
        {
            try
            {
                foreach (var product in products)
                {
                    Check.WriteStylishText(false, $"\n\n{product.ProductId}: ", AppColors.Action);

                    Console.Write($"Название товара: {product.ProductName}, ");

                    if (product.ProductDescription == null)
                    { Console.WriteLine("Описание товара: Отсутствует, "); }
                    else
                    { Console.WriteLine($"Описание товара: {product.ProductDescription}, "); }

                    if (product.ProductImagePath == null)
                    { Console.WriteLine($"Изображение товара: Отсутствует, "); }
                    else
                    { Console.WriteLine($"Изображение товара: {product.ProductImagePath}, "); }

                    if (product.ProductQuantity == 0)
                    { Console.ForegroundColor = AppColors.Error; }
                    Console.WriteLine($"Количество товара: {product.ProductQuantity}, ");
                    Console.ResetColor();

                    if (product.ProductDiscount > 0)
                    {
                        var discount = (product.ProductPrice / 100) * product.ProductDiscount;
                        Console.WriteLine($"Цена товара со скидкой {product.ProductDiscount}% --> {product.ProductPrice - discount} рублей.");
                        Console.WriteLine($"Старая цена: {product.ProductPrice} рублей.");
                    }
                    else
                    { Console.WriteLine($"Цена товара: {product.ProductPrice} рублей."); }
                }
            }
            catch { Check.WriteStylishText(false, "Ошибка! Не удалось получить список товаров.", AppColors.Error); }
        }



        /// <summary>
        /// Получение списка товаров.
        /// </summary>
        /// <returns>null или Список категорий пуст.</returns>
        public static string ListOfProducts()
        {
            try
            {
                List<Product> products = context.Products.ToList();
                if (products.Count != 0)
                {
                    GetProducts(products);
                    return null;
                }
            }
            catch { Check.WriteStylishText(false, "Ошибка! Не удалось загрузить список товаров.", AppColors.Error); }

            string info = "Список товаров пуст.";
            Check.WriteStylishText(false, info, AppColors.Info);
            return info;
        }



        /// <summary>
        /// Получение первых трёх дорогих товаров.
        /// </summary>
        /// <returns>null или Список пуст.</returns>
        public static string GetThreeMostExpensiveProducts()
        {
            try
            {
                List<Product> products = context.Products.OrderByDescending(p => p.ProductPrice).Take(3).ToList();

                if (products.Count > 0)
                {
                    foreach (var product in products)
                    {
                        Console.Write($"\nНазвание: {product.ProductName}, ");
                        Console.Write($"Цена: {product.ProductPrice} рублей.");
                    }
                    return null;
                }
            }
            catch { Check.WriteStylishText(false, "Ошибка! Не удалось загрузить список первых трёх самых дорогих товаров.", AppColors.Error); }

            string info = "Список пуст.";
            Check.WriteStylishText(false, info, AppColors.Info);
            return info;
        }



        /// <summary>
        /// Выбор категории из списка по её идентификатору.
        /// </summary>
        /// <returns>Идентификатор выбранной категории.</returns>
        public static int SelectCategoryById()
        {
            try
            {
                if (ListOfCategories() == null)
                {
                    string result = Check.CorrectNumber("\nПожалуйста, введите номер выбранной категории: ");
                    if (Check.CheckExit(result) != null)
                    {
                        Category checkCategory = context.Categories.FirstOrDefault(c => c.CategoryId == byte.Parse(result));
                        if (checkCategory != null)
                        { return int.Parse(result); }
                        Check.WriteStylishText(false, "Категория с данным номером не найдена.", AppColors.Info);
                    }
                }
            }
            catch { Check.WriteStylishText(false, "Ошибка! Не удалось получить категорию с данным номером.", AppColors.Error); }

            return 0;
        }



        /// <summary>
        /// Получение списка товаров из выбранной категории.
        /// </summary>
        /// <param name="categoryId">Идентификатор категории.</param>
        public static void GetProductsByCategory(int categoryId)
        {
            try
            {
                Category category = context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
                if (category != null)
                {
                    List<Product> products = context.Products.Where(p => p.CategoryId == categoryId).ToList();
                    if (products.Count > 0)
                    {
                        GetProducts(products);
                    }
                    else
                    { Check.WriteStylishText(false, "Список пуст.", AppColors.Info); }
                }
                else
                { Check.WriteStylishText(false, "Категория не найдена.", AppColors.Info); }
            }
            catch { Check.WriteStylishText(false, $"Ошибка! Не удалось получить список продуктов из категории {categoryId}.", AppColors.Error); }
        }



        /// <summary>
        /// Получение списка категорий.
        /// </summary>
        /// <returns>null или Список категорий пуст.</returns>
        public static string ListOfCategories()
        {
            try
            {
                Check.WriteStylishText(false, "Список категорий:", ConsoleColor.White);
                List<Category> categories = context.Categories.ToList();
                if (categories.Count != 0)
                {
                    foreach (var category in categories)
                    { Console.WriteLine($"{category.CategoryId}. {category.CategoryName}"); }
                    return null;
                }
            }
            catch { Check.WriteStylishText(false, "Ошибка! Не далось загрузить список категорий.", AppColors.Error); }

            string info = "Список категорий пуст.";
            Check.WriteStylishText(false, info, AppColors.Info);
            return info;
        }



        /// <summary>
        /// Добавление новой категории.
        /// </summary>
        /// <param name="name">Название категории.</param>
        public static void AddCategory(string name)
        {
            try
            {
                Category checkCaregory = context.Categories.FirstOrDefault(c => c.CategoryName == name);
                if (checkCaregory == null)
                {
                    Category category = new Category();
                    category.CategoryName = name;
                    context.Categories.Add(category);
                    context.SaveChanges();
                    Check.WriteStylishText(false, "Категория успешно добавлена.", AppColors.Success);
                }
                else
                { Check.WriteStylishText(false, "Категория с таким названием уже существует.", AppColors.Warning); }
            }
            catch { Check.WriteStylishText(false, "Ошибка! Не удалось добавить категорию.", AppColors.Error); }
        }



        /// <summary>
        /// Обновление данных существующей категории из списка.
        /// </summary>
        /// <param name="categoryId">Идентификатор категории.</param>
        /// <param name="name">Название категории.</param>
        public static void UpdateCategory(int categoryId, string name)
        {
            try
            {
                Category checkCaregory = context.Categories.FirstOrDefault(c => c.CategoryName == name);
                if (checkCaregory == null)
                {
                    Category category = context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);

                    if (Check.WaitConfirm("Вы уверены, что хотите обновить категорию под номером " + category.CategoryId + "?") == true)
                    {
                        category.CategoryName = name;
                        context.SaveChanges();
                        Check.WriteStylishText(false, "Категория успешно обнавлена.", AppColors.Success);
                    }
                }
                else
                { Check.WriteStylishText(false, "Категория с таким названием уже существует.", AppColors.Warning); }
            }
            catch { Check.WriteStylishText(false, "Ошибка! Не удалось обновить категорию.", AppColors.Error); }
        }



        /// <summary>
        /// Удаление категории.
        /// </summary>
        /// <param name="categotyId">Идентификатор категории.</param>
        public static void DeleteCategory(int categoryId)
        {
            try
            {
                Category category = context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
                if (category != null)
                {
                    if (Check.WaitConfirm("Вы уверены, что хотите удалить категорию " + category.CategoryName + "?") == true)
                    {
                        bool flag = false;
                        List<Product> products = context.Products.Where(p => p.CategoryId == categoryId).ToList();
                        if (products.Count > 0)
                        {
                            if (Check.WaitConfirm("В данной категории есть товары. Удалить категорию вместе с товарами?") == true)
                            {
                                List<Product> checkProducts = context.Products.ToList();
                                for (int i = 0; i < checkProducts.Count - 1; i++)
                                {
                                    if (products[i].CategoryId == categoryId)
                                    {
                                        context.Products.Remove(products[i]);
                                    }
                                }
                                flag = true;
                            }
                        }
                        else { flag = true; }

                        if (flag == true)
                        {
                            context.Categories.Remove(category);
                            Check.WriteStylishText(false, "Удаление прошло успешно.", AppColors.Success);
                        }
                        context.SaveChanges();
                    }
                }
                else
                { Check.WriteStylishText(false, "Категория не найдена.", AppColors.Info); }
            }
            catch { Check.WriteStylishText(false, "Ошибка! Не удалось удалить категорию.", AppColors.Error); }
        }



        /// <summary>
        /// Добавление нового товара.
        /// </summary>
        /// <param name="name">Название товара.</param>
        /// <param name="description">Описание товара.</param>
        /// <param name="image">Изображение товара.</param>
        /// <param name="quantity">Количество товара.</param>
        /// <param name="price">Цена товара.</param>
        /// <param name="discount">Скидка товара.</param>
        /// <param name="category">Идентификатор категории, к которой относится товар.</param>
        public static void AddProduct(string name, string? description, string? image, int quantity, long price, byte discount, int category)
        {
            try
            {
                Product product = new Product()
                {
                    ProductName = name,
                    ProductDescription = description,
                    ProductImagePath = image,
                    ProductQuantity = quantity,
                    ProductPrice = price,
                    ProductDiscount = discount,
                    CategoryId = category
                };
                context.Products.Add(product);
                context.SaveChanges();
                Check.WriteStylishText(false, "Товар успешно добавлен.", AppColors.Success);
            }
            catch { Check.WriteStylishText(false, "Ошибка! Не удалось добавить товар.", AppColors.Error); }
        }



        /// <summary>
        /// Выбор продукта из списка по его идентификатору.
        /// </summary>
        /// <returns>Идентификатор выбранного товара.</returns>
        public static int SelectProductById()
        {
            try
            {
                Check.WriteStylishText(false, "Список товаров:", AppColors.Title);
                List<Product> products = context.Products.ToList();
                if (products.Count != 0)
                {
                    foreach (var product in products)
                    { Console.WriteLine($"{product.ProductId}. {product.ProductName}"); }
                    string result = Check.CorrectNumber("\n\nПожалуйста, введите номер выбранного товара: ");
                    if (Check.CheckExit(result) != null)
                    {
                        Product checkProduct = context.Products.FirstOrDefault(p => p.ProductId == int.Parse(result));
                        if (checkProduct != null)
                        { return int.Parse(result); }
                        Check.WriteStylishText(false, "Товар с данным номером не найден.", AppColors.Info);
                    }

                }
                else
                { Check.WriteStylishText(false, "Список товаров пуст.", AppColors.Info); }
            }
            catch { Check.WriteStylishText(false, "Ошибка! Не удалось получить товар с данным номером.", AppColors.Error); }

            return 0;
        }



        /// <summary>
        /// Удаление товара.
        /// </summary>
        /// <param name="productId">Идентификатор продекта.</param>
        public static void DeleteProduct(int productId)
        {
            try
            {
                Product product = context.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    if (Check.WaitConfirm("Вы уверены, что хотите удалить товар " + product.ProductName + "?") == true)
                    {
                        context.Products.Remove(product);
                        context.SaveChanges();
                        Check.WriteStylishText(false, "Товар успешно удален.", AppColors.Success);
                    }
                    else
                    { Check.WriteStylishText(false, "Товар не найден.", AppColors.Info); }
                }
            }
            catch { Check.WriteStylishText(false, "Ошибка! Не удалось удалить товар.", AppColors.Error); }
        }



        /// <summary>
        /// Обновление данных выбранного товара из списка (не изменяется количество).
        /// </summary>
        /// <param name="productId">Идентификатор продукта.</param>
        /// <param name="name">Название продукта.</param>
        /// <param name="description">Описание товара.</param>
        /// <param name="image">Изображение товара.</param>
        /// <param name="price">Цена товара.</param>
        /// <param name="discount">Скидка товара.</param>
        /// <param name="category">Идентификатор категории, к которой относится товар.</param>
        public static void UpdateProduct(int productId, string name, string? description, string? image, string price, string discount, string category)
        {
            try
            {
                Product product = context.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    if (Check.WaitConfirm("Вы уверены, что хотите обновить товар под номером " + product.ProductId + "?") == true)
                    {
                        if (!string.IsNullOrEmpty(name))
                        { product.ProductName = name; }
                        if (!string.IsNullOrEmpty(image))
                        { product.ProductImagePath = image; }
                        if (!string.IsNullOrEmpty(description))
                        { product.ProductDescription = description; }
                        if (!string.IsNullOrEmpty(price))
                        { product.ProductPrice = long.Parse(price); }
                        if (!string.IsNullOrEmpty(discount))
                        { product.ProductDiscount = byte.Parse(discount); }
                        if (!string.IsNullOrEmpty(category))
                        { product.CategoryId = int.Parse(category); }
                        context.SaveChanges();
                        Check.WriteStylishText(false, "Товар обновлён.", AppColors.Success);
                    }
                }
                else
                { Check.WriteStylishText(false, "Товар не найден.", AppColors.Info); }
            }
            catch { Check.WriteStylishText(false, "Ошибка! Не удалось обновить товар.", AppColors.Error); }
        }



        /// <summary>
        /// Обновление количества выбранного товара из списка.
        /// </summary>
        /// <param name="productId">Идентификатор товара.</param>
        /// <param name="quantity">Количество товара.</param>
        public static void UpdateProductQuantity(int productId, int quantity)
        {
            try
            {
                Product product = context.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    if (Check.WaitConfirm("Вы уверены, что хотите обновить количество товара под номером " + product.ProductId + "?") == true)
                    {
                        product.ProductQuantity = quantity;
                        context.SaveChanges();
                        Console.WriteLine("Количество товара обновлено.");
                    }
                }
                else
                { Check.WriteStylishText(false, "Товар не найден.", AppColors.Info); }
            }
            catch { Check.WriteStylishText(false, "Ошибка! Не удалось обновить количество товара.", AppColors.Error); }
        }
    }
}