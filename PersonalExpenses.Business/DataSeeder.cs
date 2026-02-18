using System.Text.Json;
using PersonalExpenses.Entities;

namespace PersonalExpenses.Business;

// DOCUMENTO HECHO CON IA PARA FINES DE RELLENO DE DATOS.
// Para usarlo, en Program.cs hay que descomentar la linea que llama a esta clase.
// Ojo: usarlo 1 vez y comentarlo posteriormente. Sino, los datos estaran cambiando en cada
//      ejecucion.

public static class DataSeeder
{
    public static void SeedExpenses(string filePath)
    {
        var random = new Random();
        var expenses = new List<Expense>();

        // Tus IDs exactos extraídos de tu categories.json
        string catSaludId = "686cb7ca-653c-47de-bd7b-9a6e1840beea";
        string catDeportesId = "1b2710d1-50bd-4b81-b41e-b51e2ced4309";
        string catComidaId = "3a2c6a9b-d1d1-4f52-97ed-9d1e58de5e43";

        // Datos para variar
        var descSalud = new[] { "Consulta General", "Antibióticos", "Vitaminas", "Dentista", "Analíticas Lab", "Jarabe para la tos", "Pastillas dolor de cabeza", "Consulta Dermatologo" };
        var descDeportes = new[] { "Mensualidad Gym", "Proteína Whey", "Creatina", "Ropa deportiva", "Gatorade", "Inscripción Torneo", "Tenis nuevos", "Vendas boxeo" };
        var descComida = new[] { "Almuerzo Cafetería", "Cena Tacos", "Supermercado Semanal", "Desayuno", "Pizza Viernes", "Botellita de agua", "Compra Carne", "Helado" };

        DateTime startDate = new DateTime(2024, 1, 1);
        int rangeDays = (DateTime.Today - startDate).Days;

        // Generamos 1200 gastos
        for (int i = 0; i < 60; i++)
        {
            string catId = "";
            string description = "";
            decimal amount = 0;

            // Selección aleatoria de categoría (Ponderada: Comemos más veces de las que vamos al médico)
            int chance = random.Next(1, 101); // 1 a 100

            if (chance <= 60) // 60% probabilidad de Comida
            {
                catId = catComidaId;
                description = descComida[random.Next(descComida.Length)];
                amount = random.Next(150, 1500); // Entre 150 y 1500 pesos
            }
            else if (chance <= 85) // 25% probabilidad de Deportes
            {
                catId = catDeportesId;
                description = descDeportes[random.Next(descDeportes.Length)];
                amount = random.Next(500, 3500);
            }
            else // 15% probabilidad de Salud
            {
                catId = catSaludId;
                description = descSalud[random.Next(descSalud.Length)];
                amount = random.Next(800, 5000);
            }

            // Fecha aleatoria dentro de los últimos 2 años y pico
            DateTime randomDate = startDate.AddDays(random.Next(rangeDays)).AddHours(random.Next(8, 22)).AddMinutes(random.Next(0, 60));

            var expense = new Expense
            {
                Id = Guid.NewGuid().ToString(),
                Amount = amount,
                Description = description,
                Date = randomDate,
                CategoryId = catId
            };
            // ShortId es calculado en tu clase, no hay que setearlo si es getter, 
            // pero si es setter lo dejamos que se genere solo o lo asignamos si tu constructor no lo hace.
            
            expenses.Add(expense);
        }

        // Ordenamos por fecha para que se vea bonito
        expenses = expenses.OrderBy(e => e.Date).ToList();

        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(expenses, options);
        File.WriteAllText(filePath, json);

        Console.WriteLine($"¡Generados {expenses.Count} gastos exitosamente en {filePath}!");
    }
}