using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Delegates
{
    public delegate bool EntityValidator<T>(T entity);


}
