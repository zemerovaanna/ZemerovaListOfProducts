using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using ConsoleAppListOfProducts.Database;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleAppListOfProducts.Models
{
    public static class Check
    {
        /// <summary>
        /// Добавление цвета и сокращения кода для вывода.
        /// </summary>
        /// <param name="line">true - есть ввод, false - нет ввода.</param>
        /// <param name="text">Текст вывода.</param>
        /// <param name="consoleColore">Цвет текста.</param>
        public static void WriteStylishText (bool line, string text, ConsoleColor consoleColore)
        {
            if (line == true)
            {
                Console.ForegroundColor = consoleColore;
                Console.Write(text);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = consoleColore;
                Console.WriteLine(text);
                Console.ResetColor();
            }
        }



        /// <summary>
        /// Получение таблицы действий для выбора действия.
        /// </summary>
        public static void GetActions ()
        {
            Console.WriteLine(" _______________________________________________________________________________________________________________" +
                "\n|  Список:" + "\t\t\t\t| Категория:" + "\t\t\t| Товар:\t\t\t\t|" +
                "\n|_______________________________________________________________________________________________________________|" +
                "\n|  1 —> Поиск товара.\t\t\t| 6 —> Добавить категорию.\t| 9 —> Добавить товар.\t\t\t|" +
                "\n|  2 —> Посмотреть список товаров.\t| 7 —> Изменить категорию.\t| 10 —> Изменить товар.\t\t\t|" +
                "\n|  3 —> Первые три дорогих товара.\t| 8 —> Удалить категорию.\t| 11 —> Изменить количество товара.\t|" +
                "\n|  4 —> Посмотреть товары по категории.\t|\t\t\t\t| 12 —> Удалить товар.\t\t\t|" +
                "\n|  5 —> Посмотреть список категорий.\t|\t\t\t\t|\t\t\t\t\t|" +
                "\n|_______________________________________________________________________________________________________________|" +
                "\n|  Выход (Команды для выхода в поле ввода).\t\t\t\t\t\t\t\t\t|" +
                "\n|  00000 —> При вводе в любое поле.\t\t\t\t\t\t\t\t\t\t|" +
                "\n|  Назад —> При вводе в поле, где принимается строка (например: можно ввести буквы в этой строке).\t\t|" +
                "\n|_______________________________________________________________________________________________________________|");
        }



        /// <summary>
        /// Получение корректных строковых данных.
        /// </summary>
        /// <param name="enter">Текст сообщения.</param>
        /// <returns>Корректная строка.</returns>
        public static string CheckString(string enter, byte length)
        {
            string result = null;
            while (string.IsNullOrEmpty(result))
            {
                WriteStylishText(true, "\nПожалуйста, введите " + enter + ": ", AppColors.Default);
                result = Console.ReadLine();
                if (result.StartsWith(' ') || result.EndsWith(' '))
                { result = null; }
                else if (result.Length > length)
                {
                    WriteStylishText(false, "Превышено допустимое количество символов " + result.Length + "/" + length + " символов.\n", AppColors.Warning);
                    result = null;
                }
            }
            return result;
        }



        /// <summary>
        /// Получение названия файла изображения.
        /// </summary>
        /// <returns>Название файла изображения.</returns>
        public static string CorrectImageName ()
        {
            string result = null;

            while (string.IsNullOrEmpty(result))
            {
                WriteStylishText(true, "\nПожалуйста, введите название изображения (без указания формата): ", AppColors.Default);
                result = Console.ReadLine();

                string pattern = "^[а-яА-Яa-zA-Z0-9]+$";
                Regex symbols = new Regex(pattern, RegexOptions.IgnoreCase);

                if (result.StartsWith(' ') || result.EndsWith(' '))
                { result = null; }
                else if (result.Length > 45)
                {
                    WriteStylishText(false, "Превышено допустимое количество символов " + result.Length + "/50 символов.\n", AppColors.Warning);
                    result = null;
                }
                else if (CheckExit(result) == null)
                { return null; }
                else if (symbols.IsMatch(result) == false)
                {
                    WriteStylishText(false, "Содержит недопустимые символы.", AppColors.Warning);
                    result = null;
                }
            }

            WriteStylishText(false, "\nСписок форматов изображения:", AppColors.Default);
            string[] formats = new string[]
            {
                ".bmp",
                ".jpg",
                ".jpeg",
                ".png",
                ".ico",
                ".tif",
                ".tiff"
            };

            for (int i = 0; i < formats.Length; i++)
            {
                WriteStylishText(true, $"{i}", AppColors.Action);
                Console.Write($" -> {formats[i]} \n");
            }

            bool success = false;
            string enter;
            do
            {
                WriteStylishText(true, "\nПожалуйста, введите номер формата: ", AppColors.Default);
                enter = Console.ReadLine();
                if (CheckExit(enter) == null)
                { return null; }
                else
                {
                    success = byte.TryParse(enter, out byte number);
                    if (success == true)
                    { if (byte.Parse(enter) > formats.Length) { enter = null; } }
                }
            }
            while (!success);

            result = result + formats[byte.Parse(enter)];

            return result;
        }



        /// <summary>
        /// Корректный ввод числа.
        /// </summary>
        /// <param name="enter">Текст вывода, что вводить пользователю.</param>
        /// <returns>Корректное число.</returns>
        public static string CorrectNumber (string enter)
        {
            bool success = false;
            string result;
            do
            {
                WriteStylishText(true, enter, AppColors.Default);
                result = Console.ReadLine();
                success = uint.TryParse(result, out uint number);
            }
            while (!success);
            return result;
        }



        /// <summary>
        /// Корректный ввод скидки на товар.
        /// </summary>
        /// <returns>Корректная скидка.</returns>
        public static string CorrrectDiscount ()
        {
            bool success = false;
            string result;
            do
            {
                WriteStylishText(true, "\nПожалуйста, введите процент скидки на данный товар: ", AppColors.Default);
                result = Console.ReadLine();
                success = ushort.TryParse(result, out ushort number);
                if (success == true && long.Parse(result) > 100)
                { success = false; }
            }
            while (!success);
            return result;
        }



        /// <summary>
        /// Добавление или обновление товара.
        /// </summary>
        /// <param name="action">Идентификатор товара или если action = 0, то добавление нового товара.</param>
        public static void WriteProduct (int action)
        {
            if (action == 0)
            {
                int quantity, category, c;
                string q = "0", price = "0", discount, name = null, description = null, image = null;

                name = CheckString("название товара", 100);
                if (Check.CheckExit(name) == null)
                { return; }
                else
                {
                    bool change = WaitConfirm("Хотите добавить описание товара?");
                    if (change == true)
                    {
                        description = Convert.ToString(CheckString("описание товара", 250));
                        if (Check.CheckExit(description) == null)
                        { return; }
                    }

                    change = WaitConfirm("Хотите добавить название файла изображения товара?");
                    if (change == true)
                    {
                        image = CorrectImageName();
                        if (image == null)
                        { return; }
                    }

                    q = CorrectNumber("Пожалуйста, введите количество товара: ");
                    if (Check.CheckExit(Convert.ToString(q)) == null)
                    { return; }
                    else
                    {
                        quantity = int.Parse(q);
                        bool correctPrice = false;
                        while (!correctPrice)
                        {
                            price = CheckString("цену товара", 6);
                            if (Check.CheckExit(price) == null)
                            { return; }
                            correctPrice = ulong.TryParse(price, out ulong number);
                            if (!correctPrice)
                            { WriteStylishText(true, "Пожалуйста, введите цену числом.", AppColors.Warning); }
                        }

                        discount = CorrrectDiscount();
                        if (Check.CheckExit(Convert.ToString(discount)) == null)
                        { return; }
                        else
                        {
                            c = ProgramContext.SelectCategoryById();
                            if (c == 0)
                            { return; }
                            category = c;
                            ProgramContext.AddProduct(name, description, image, quantity, Convert.ToInt64(price), Convert.ToByte(discount), category);
                        }
                    }
                }
            }
            else
            {
                int c;
                string name = null, description = null, image = null, quantity = null, price = null, category = null, discount = null;

                bool change = WaitConfirm("Желаете изменить название товара?");
                if (change == true)
                {
                    name = Convert.ToString(CheckString("новое название товара", 100));
                    if (Check.CheckExit(name) == null)
                    { return; }
                }

                change = WaitConfirm("Желаете изменить описание товара?");
                if (change == true)
                {
                    description = Convert.ToString(CheckString("новое описание товара", 250));
                    if (Check.CheckExit(description) == null)
                    { return; }
                }

                change = WaitConfirm("Желаете изменить название файла изображение товара?");
                if (change == true)
                {
                    image = CorrectImageName();
                    if (image == null)
                    { return; }
                }

                change = WaitConfirm("Желаете изменить цену товара?");
                if (change == true)
                {
                    bool correctPrice = false;
                    while (!correctPrice)
                    {
                        price = CheckString("новую цену товара", 6);
                        if (Check.CheckExit(price) == null)
                        { return; }
                        correctPrice = ulong.TryParse(price, out ulong number);
                        if (!correctPrice)
                        { WriteStylishText(true, "Пожалуйста, введите новую цену числом.", AppColors.Warning); }
                    }
                }

                change = WaitConfirm("Желаете изменить скидку товара?");
                if (change == true)
                {
                    discount = Convert.ToString(CorrrectDiscount());
                    if (Check.CheckExit(discount) == null)
                    { return; }
                }

                change = WaitConfirm("Желаете сменить категорию товара?");
                if (change == true)
                {
                    c = ProgramContext.SelectCategoryById();
                    if (c == 0)
                    { return; }
                    category = Convert.ToString(c);
                }

                ProgramContext.UpdateProduct(action, name, description, image, price, discount, category);
            }
        }



        /// <summary>
        /// Для подтверждения действия.
        /// </summary>
        /// <returns>Логический ответ пользователя (true или false).</returns>
        public static bool WaitConfirm (string enter)
        {
            string answer = "null";
            Regex regex = new Regex("^(Да|Нет)$", RegexOptions.IgnoreCase);
            while (!regex.IsMatch(answer) || string.IsNullOrEmpty(answer))
            {
                Check.WriteStylishText(true, "\n ——" + enter + "\nПожалуйста, введите \"Да\" или \"Нет\": ", AppColors.Confirm);
                answer = Console.ReadLine();
            }
            if (answer.Length == 2)
            { return true; }
            return false;
        }



        /// <summary>
        /// Проверка, желает ли пользователь вернуться назад.
        /// </summary>
        /// <param name="text">Ввод от пользователя.</param>
        /// <returns>Была ли команда возвращения назад (true или false).</returns>
        public static string CheckExit (string text)
        {
            try
            {
                Regex regex = new Regex("^(00000|Назад)$", RegexOptions.IgnoreCase);
                if (regex.IsMatch(text) == false)
                { return text; }
                else
                { return null; }
            }
            catch
            {
                WriteStylishText(false, "Ошибка...", AppColors.Error);
                return null;
            }
        }
    }
}