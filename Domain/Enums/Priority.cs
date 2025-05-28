

namespace Domain.Enums
{
    //Al usar byte se ahorra hasta un 75% de recursos

    /*Se usan valores potencia de dos para que al seleccionar
    //mas de un elemento del enum no de errores, porque al seleccionar
    //Estamos aplicando or o (bitwise)  los bytes seleccionados (0,1,2,4,8,16,...)*/

    /*ejemplo gráfico:
        0000 0001 OR
        0000 0010
    --------------------
        0000 0011    */



    public enum Priority : byte
    {
        Low,
        Medium,
        High
    }

    //[Flags] se usa para mostrar directamente el texto y no el valor
    public enum Status : byte
    {
        Pendiente,
        EnProceso,
        Completado,
        Cancelado = 4,
        Todos = Pendiente | EnProceso | Completado | Cancelado
        //Tambien se pueden crear grupos como por ejemplo:
        //Todos = Pendiente | EnProceso | Completado | Cancelado
    }
}
