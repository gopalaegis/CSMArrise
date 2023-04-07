using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCInsight.DAL;
using BCInsight.BAL.Repository;

namespace BCInsight.BAL.Services
{
    public class DailyTaskStatusRepository : GenericRepository<admin_csmariseEntities, DailyTaskStatus>, IDailytaskstatus
    {
    }
}
