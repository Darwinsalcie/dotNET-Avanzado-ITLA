using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Delegates
{

    //Toma un objeto de tipo T y devuelve un valor booleano.
    public delegate bool EntityValidator<T>(T entity);


}
