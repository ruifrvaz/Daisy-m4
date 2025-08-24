using System;
using System.Collections.Generic;

namespace Daisy.Resources.Interfaces
{
    public interface IPool<T>
    {
        List<T> Pool { get; set; }

    }
}