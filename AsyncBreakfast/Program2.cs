//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace AsyncBreakfast
//{
//    class Program
//    {
//        private static void Main(string[] args)
//        {
//            var t1 = GetTaskTestSum();      //同步阻塞
//            Console.WriteLine(t1.Result);

//            var t2 = ToDoWithTimeOutAsync();    //同步阻塞
//            Console.WriteLine(t2.Result);

//            Console.WriteLine("Press [Enter] to exit ...");
//            Console.ReadLine();
//        }

//        /// <summary>
//        /// 测试
//        /// </summary>
//        /// <returns></returns>
//        private static async Task<int> GetTaskTestSum()
//        {
//            int sum = 0;
//            for (int i = 0; i <= 10; i++)
//            {
//                sum += 1;
//                Console.WriteLine("sum += " + i);
//                await Task.Delay(50);
//            }

//            return sum;
//        }

//        private static async Task<string> ToDoAsync()
//        {
//            await Task.Delay(TimeSpan.FromSeconds(5));
//            return "To Do Success";
//        }

//        private static async Task<string> ToTimeAsync()
//        {
//            await Task.Delay(2000);
//            return "Throw TimeoutExceptions.";
//        }

//        /// <summary>
//        /// Task.WhenAny任意一个任务完成返回
//        /// </summary>
//        /// <returns>返回String类型</returns>
//        private static async Task<string> ToDoWithTimeOutAsync()
//        {
//            var toDoTaskAsync = ToDoAsync();
//            var timeOutTaskAsync = ToTimeAsync();
            
//            var completedTask = await Task.WhenAny(toDoTaskAsync, timeOutTaskAsync);
//            return completedTask.Result;
//        }
//    }
//}
