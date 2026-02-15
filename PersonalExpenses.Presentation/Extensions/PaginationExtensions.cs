namespace PersonalExpenses.Presentation.Extensions;

// Migrado desde PaginationUtils, aprovechando el concepto de extender clases nativas
public static class PaginationExtensions
{
    // Extensión para obtener la paginación de una lista
    public static List<T> GetPagination<T>(this List<T> items, int page = 1, int rowsPerPage = 15)
    {
        // Validaciones básicas para evitar errores de índice
        if (items == null || items.Count == 0 || page < 1)
            return new List<T>();

        int totalPages = (int)Math.Ceiling((double)items.Count / rowsPerPage);
        
        if (page > totalPages)
            return new List<T>();

        int offset = rowsPerPage * (page - 1);

        return items.TakeFirstN(offset, offset + rowsPerPage);
    }

    // Extensión para calcular el total de páginas desde la lista
    public static int GetTotalPages<T>(this List<T> items, int rowsPerPage)
    {
        if (items == null || items.Count == 0) return 0;
        
        int totalPages = items.Count / rowsPerPage;
        if (items.Count % rowsPerPage != 0) totalPages++;
        
        return totalPages;
    }

    public static string[][] ToPages(this List<string> items, int rowsPerPage)
    {
        // Calculamos cuántas páginas habrá en total
        int totalPages = items.GetTotalPages(rowsPerPage);
        
        // 2. Creamos la matriz (array de arrays) con el tamaño exacto
        string[][] matrix = new string[totalPages][];

        // 3. Llenamos cada posición de la matriz con los datos de esa página
        for (int i = 1; i <= totalPages; i++)
        {
            // Usamos GetPagination para obtener la lista de esa página específica
            List<string> pageData = items.GetPagination(i, rowsPerPage);
            
            // Convertimos la lista de la página a un array nativo para la matriz
            matrix[i - 1] = pageData.ToArray();
        }

        return matrix;
    }

    // Extensión para enteros: permite calcular la siguiente página desde el total
    // Uso: totalPages.NextPage(currentPage)
    public static int NextPage(this int totalPages, int currentPage)
    {
        if (totalPages <= 0) return 1;
        return currentPage >= totalPages ? 1 : currentPage + 1;
    }

    // Extensión para enteros: permite calcular la página anterior desde el total
    // Uso: totalPages.PreviousPage(currentPage)
    public static int PreviousPage(this int totalPages, int currentPage)
    {
        if (totalPages <= 0) return 1;
        return currentPage - 1 <= 0 ? totalPages : currentPage - 1;
    }
}