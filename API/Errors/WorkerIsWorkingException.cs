using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Errors
{
    public class WorkerIsWorkingException : Exception
    {
        public WorkerIsWorkingException() : base("Worker is working on order or off now.")
        {

        }
    }
}