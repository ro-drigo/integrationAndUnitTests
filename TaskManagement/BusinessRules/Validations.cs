using TaskManagement.Models;

namespace TaskManagement.BusinessRules
{
    public class Validations
    {
        //método que verifica se a tarefa está atrasada
        public static bool IsTaskLate(Tasks task)
        {
            return !task.IsCompleted && task.DueDate < DateTime.Now;
        }
    }
}
