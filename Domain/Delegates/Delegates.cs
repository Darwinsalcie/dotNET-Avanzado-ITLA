using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Delegates
{
    public delegate bool TodoValidator<T>(T entity);
    public delegate void TodoCreatedNotification<T>(Todo<T> todo);
    public delegate void TodoDeletedNotification<T>(Todo<T> todo);

}
