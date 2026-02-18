using PersonalExpenses.Presentation.Enums;
using PersonalExpenses.Presentation.Extensions;

namespace PersonalExpenses.Presentation.Core;

public static class Menu
{
    private static bool showHelp = true;

    public static int Show(string title, string[] choices, string subtitle = "", bool error = false, string[]? tips = null)
    {
        int selectedIndex = 0;
        bool loop = true;

        while (loop)
        {
            // Utilizamos la función de Draw para dibujar el menú
            Draw(title, choices, selectedIndex, subtitle, error, tips);

            // Con esta parte manejamos las teclas que presione el usuario
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Escape:
                    return -1;

                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    selectedIndex = selectedIndex > 0 ? --selectedIndex : choices.Length - 1;
                    break;

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    selectedIndex = selectedIndex >= choices.Length - 1 ? 0 : ++selectedIndex;
                    break;

                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    return choices.Length > 0 ? selectedIndex : -1;

                case ConsoleKey.H:
                    showHelp = !showHelp;
                    break;
            }
        }

        return -1;
    }

    public static int Show(string title, string[][] pages, int page = 1, int rowsPerPage = 10, bool specialKeys = false, string subtitle = "", string[]? tips = null)
    {
        bool loop = true;
        int selectedIndex = 0,
            offset = (page - 1) * rowsPerPage;

        while (loop)
        {
            // Utilizamos la función de Draw para dibujar el menú
            Draw(
                title,
                pages.Length > 0 ? pages[page - 1] : [],
                selectedIndex,
                subtitle,
                false,
                tips,
                offset,
                page,
                pages.Length,
                rowsPerPage
            );

            // Con esta parte manejamos las teclas que presione el usuario
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Escape:
                    return -1;

                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    if (pages.Length > 0 && pages[page - 1].Length > 0)
                    {
                        selectedIndex = selectedIndex > 0 ? --selectedIndex : pages[page - 1].Length - 1;
                    }                    
                    break;

                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    if (pages.Length > 0)
                    {
                        page = pages.Length.NextPage(page);
                        offset = (page - 1) * rowsPerPage;
                        selectedIndex = 0;
                    }
                    return Show(title, pages, page, rowsPerPage, true, subtitle, tips);

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    if (pages.Length > 0 && pages[page - 1].Length > 0)
                    {
                        selectedIndex = selectedIndex >= pages[page - 1].Length - 1 ? 0 : ++selectedIndex;
                    }
                    break;

                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    if (pages.Length > 0)
                    {
                        page = pages.Length.PreviousPage(page);
                        offset = (page - 1) * rowsPerPage;
                        selectedIndex = 0; 
                    }

                    return Show(title, pages, page, rowsPerPage, true, subtitle, tips);

                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    return pages.Length > 0 ? offset + selectedIndex : -1;

                case ConsoleKey.H:
                    showHelp = !showHelp;
                    break;

                // Claves especiales...
                case ConsoleKey.I: // Insertar
                    if (!specialKeys)
                        break;
                    return (int)SpecialKeys.Insert;

                case ConsoleKey.E: // Exportar
                    if (!specialKeys)
                        break;
                    return (int)SpecialKeys.Export;

                case ConsoleKey.F: // Aplicar filtros
                    if (!specialKeys)
                        break;
                    return (int)SpecialKeys.SetFilters;

                case ConsoleKey.L: // Limpiar filtros
                    if (!specialKeys)
                        break;
                    return (int)SpecialKeys.ClearFilters;
            }
        }

        return -1;
    }

    private static void Draw(
        string title,
        string[] choices,
        int selectedIndex,
        string subtitle = "",
        bool error = false,
        string[]? tips = null,
        int offset = 0,
        int currentPage = 0,
        int totalPages = 0,
        int rowsPerPage = 10
    )
    {
        Console.Clear();
        Console.WriteLine();
        Console.WriteLine($"\t{title}");

        if (!string.IsNullOrWhiteSpace(subtitle))
        {
            Console.WriteLine();
            Console.WriteLine($"\t{subtitle}");
        }

        Console.WriteLine();

        if (choices.Length == 0)
        {
            Console.WriteLine("\tNo hay datos disponibles.");
        }
        else 
        {
            for (int i = 0; i < choices.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.WriteLine(error ? $"\t> {choices[i]}." : $"\t> {offset + i + 1}. {choices[i]}.");
                    continue;
                }

                Console.WriteLine(error ? $"\t{choices[i]}." : $"\t{offset + i + 1}. {choices[i]}.");
            }
        }

        // Verifica si hay paginacion
        if (currentPage > 0 && totalPages > 0)
        {
            if (currentPage > 1 && choices.Length < rowsPerPage)
            {
                for (int i = 0; i < rowsPerPage - choices.Length; i++)
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine();
            Console.WriteLine($"\tPágina: {currentPage}/{totalPages}");
        }

        if (tips != null && tips.Length > 0)
        {
            Console.WriteLine();
            for (int i = 0; i < tips.Length; i++)
            {
                Console.WriteLine($"\t{tips[i]}" + ((tips[i].Length > 0) ? "." : ""));
            }
        }

        if (showHelp)
        {
            Console.WriteLine();
            Console.WriteLine("\tPresiona [W,A,S,D o FLECHAS] para moverte.");
            Console.WriteLine("\tPresiona [ESPACIO o ENTER] para seleccionar una opción.");
            Console.WriteLine("\tPresiona [ESC] para salir.");
            Console.WriteLine("\tPresiona [H] para mostrar/ocultar este menú.");
        }
    }
}