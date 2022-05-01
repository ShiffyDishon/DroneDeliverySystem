using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DalApi;
using DO;
using System.Runtime.CompilerServices;


namespace Dal
{
    public partial class DalObject : DalApi.IDal
    {
        /// <summary>
        /// Get a Worker/s with a specific condition = predicate
        /// </summary>
        /// <param name="predicate">return a worker/s that meeets the condition</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Worker> getWorkerWithSpecificCondition(Predicate<Worker> predicate)
        {
            return (from worker in DataSource.Workers
                    where predicate(worker)
                    select worker);
        }
    }
}