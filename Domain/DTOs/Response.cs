

namespace Domain.DTOs
{
    public class Response
    {
        /* Las lambdas normales son (parametros) => expresión
         *  como por ejemplo: x => x * 2
         *  Esta de abajo es una expression-bodied property
         *  
         *  Un ejemplo de una expression-bodied property sería:
         *  
         *  public int Doble => numero * 2;
         *  
         *  Acá Doble siempre devolverá numero * 2
         *  En el caso de abajo ThereIsError siempre devuelve Errors.Any()
         *  
         *  Erros.Any() devuelve true or false dependiendo si la lista Errors tiene algún elemento o no.
         */

        public bool ThereIsError => Errors.Any();
        public long EntityId { get; set; }
        public bool Successful { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>(0);
    }

    // Se separan las clases para usar Response cuando solo se necesita información básica (como errores),
    // evitando el uso de genéricos y recursos extra si no se van a devolver datos.

    public class Response<T> : Response
    {
        //Ienumerable <T> es una interfaz que representa una colección de elementos que se pueden enumerar (iterar).
        //Se usa si queremos devolver una lista de elementos del tipo T.
        public IEnumerable<T> DataList { get; set; }

        //T es un tipo genérico, lo que significa que puede ser cualquier tipo de dato.
        //En este caso, se usa para devolver un único elemento del tipo T.
        public T SingleData { get; set; }
    }
}