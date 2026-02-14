namespace PersonalExpenses.Entities;

public abstract class BaseModel
{
    // UUID para el manejo interno del id de los modelos
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Tomamos los primeros 8 caracteres del UUID para el ID de la factura de forma visual.
    public string ShortId => Id.Substring(0, 8).ToUpper();
}