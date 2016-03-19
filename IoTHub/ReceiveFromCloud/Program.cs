using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiveFromCloud
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbContext = new IvaDatabase2Entities();
            HeartRateInfo last = dbContext.HeartRateInfoes.OrderByDescending(hr => hr.Id).FirstOrDefault(); 
        
            
            foreach (var hrModel in dbContext.HeartRateInfoes)
            {
                 if(hrModel.Id > 80)
                    Console.WriteLine(hrModel.heartRate);
            }

            while (true)
            {
                if(!last.Equals(dbContext.HeartRateInfoes.OrderByDescending(hr => hr.Id).FirstOrDefault()))
                {
                    last = dbContext.HeartRateInfoes.OrderByDescending(hr => hr.Id).FirstOrDefault();
                    Console.WriteLine(last.heartRate);
                }
            }
            
            Console.ReadLine();
        }
    }
}
